using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading;
using System.Xml;
using SAPRequestsLib.Models;
using System.Xml.Linq;
using System.Linq;

namespace SAPRequestsLib
{
    public static class MonitoringManager
    {

        public static void MonitorMessage(MonitoringMessage message,string ConnectionString, string emailFrom,string emailTo,string emailSMTPServer)
        {
            if (!string.IsNullOrEmpty(message.MessageXML))
            {
                bool isDown = false;
                bool wasDown = false;

                bool affectedByPerformanceDegradation = false;
                bool wasAffectedByPerformanceDegradation = false;

                DateTime? startDownTime = null;
                DateTime? endDownTime = null;

                long MAX_REPLY_TIME = 0;
                long MIN_REPLY_TIME = 10000;
                long TOTAL_REPLY_TIME = 0;
                int NUMBER_OF_REQUESTS = 0;
                int NUMBER_OF_FAILED_REQUESTS = 0;
                int NUMBER_OF_CONSICUTIVE_DEGRADATION_MESSAGES = 0;
                int NUMBER_OF_DEGRADATION_MESSAGES = 0;
                double TOTAL_DOWN_TIME = 0;
                DateTime START_MONITORING = DateTime.Now;
                DateTime? END_MONITORING = null;
                int performanceDegradationMessages = message.PerformanceDegradationAlertMessagesCount ?? 99999;

                while (true && message != null)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    if (message.IsActive)
                    {
                        bool IsPaused = false;
                        if (message.pauseIntervals != null)
                        {
                            foreach (MonitoringMessagePauseInterval messagePauseInterval in message.pauseIntervals)
                            {
                                DateTime startTimeOfOccurrence = messagePauseInterval.OccursEveryDay ? DateTime.Now : (DateTime)messagePauseInterval.StartDayOfOccurence;
                                DateTime endTimeOfOccurence = messagePauseInterval.OccursEveryDay ? ((messagePauseInterval.StartPauseTimeHour > messagePauseInterval.EndPauseTimeHour) ? DateTime.Now.AddDays(1) : DateTime.Now) : (DateTime)messagePauseInterval.EndDayOfOccurence;

                                startTimeOfOccurrence = startTimeOfOccurrence.Date.AddHours(messagePauseInterval.StartPauseTimeHour).AddMinutes(messagePauseInterval.StartPauseTimeMinute);
                                endTimeOfOccurence = endTimeOfOccurence.Date.AddHours(messagePauseInterval.EndPauseTimeHour).AddMinutes(messagePauseInterval.EndPauseTimeMinute);
                                if (DateTime.Now >= startTimeOfOccurrence && DateTime.Now < endTimeOfOccurence)
                                {
                                    IsPaused = true;
                                    break;
                                }
                            }
                        }

                        wasDown = isDown;
                        wasAffectedByPerformanceDegradation = affectedByPerformanceDegradation;

                        if (!IsPaused)
                        {
                            Exception wcfB2BWebServiceURLException = Ping(message.WcfB2BWebServiceURL); // Ping the WCF webservice URL
                            Exception wcfMethodNameException = null;
                            SAPReplyResult sapReplyResult = new SAPReplyResult
                            {
                                HasError = wcfB2BWebServiceURLException == null,
                                error = wcfB2BWebServiceURLException,
                                sAPMessageType = SAPMessageType.NotSpecified
                            };
                            if (wcfB2BWebServiceURLException == null)
                            {
                                wcfMethodNameException = Ping(message.WcfMethodName); // Ping WMB webservice URL                
                                if (wcfMethodNameException == null)
                                {
                                    sapReplyResult = SAPRequester.SendSAPMessage(message.MessageXML,
                                                                           message.WcfB2BWebServiceURL,
                                                                           message.WcfMethodName,
                                                                           message.WcfUserName,
                                                                           message.WcfPassword,
                                                                           message.C_GLOBALID.ToString(),
                                                                           message.SessionID.ToString()
                                                                           );
                                }
                                else
                                {
                                    sapReplyResult.HasError = true;
                                    sapReplyResult.error = wcfMethodNameException;
                                }
                            }
                            stopwatch.Stop();
                            NUMBER_OF_REQUESTS++;

                            if (sapReplyResult.HasError)
                            {
                                NUMBER_OF_FAILED_REQUESTS++;
                                isDown = true;
                                if (!wasDown)
                                {
                                    startDownTime = DateTime.Now;
                                }
                            }
                            else
                            {
                                MIN_REPLY_TIME = Math.Min(stopwatch.ElapsedMilliseconds, MIN_REPLY_TIME);
                                MAX_REPLY_TIME = Math.Max(stopwatch.ElapsedMilliseconds, MAX_REPLY_TIME);
                                TOTAL_REPLY_TIME = TOTAL_REPLY_TIME + stopwatch.ElapsedMilliseconds;
                                isDown = false;
                            }

                            DateTime? LastReportSentOn = null;
                            bool InitCounters = false;

                            if (!isDown)
                            {
                                if (message.ActivatePerformanceDegradationAlerts)
                                {
                                    if(message.WorstAcceptableResponseTimeInMilliseconds <= stopwatch.ElapsedMilliseconds)
                                    {
                                        performanceDegradationMessages--;
                                        NUMBER_OF_DEGRADATION_MESSAGES++;
                                        if(wasAffectedByPerformanceDegradation && affectedByPerformanceDegradation)
                                        {
                                            NUMBER_OF_CONSICUTIVE_DEGRADATION_MESSAGES++; 
                                        }
                                    }
                                    else
                                    {
                                        performanceDegradationMessages = message.PerformanceDegradationAlertMessagesCount ?? 99999;
                                        affectedByPerformanceDegradation = false;
                                    }

                                    if(performanceDegradationMessages <= 0)
                                    {
                                        affectedByPerformanceDegradation = true;
                                        NUMBER_OF_CONSICUTIVE_DEGRADATION_MESSAGES = message.PerformanceDegradationAlertMessagesCount ?? 99999;
                                    }

                                    if (wasAffectedByPerformanceDegradation != affectedByPerformanceDegradation)
                                    {
                                        string subject = "[{0}] the message {1} ";
                                        string body = "";

                                        if (wasAffectedByPerformanceDegradation && !affectedByPerformanceDegradation) // Back to normal performance
                                        {
                                            subject = string.Format(subject,"INFO", sapReplyResult.sAPMessageType.ToString()) + " is back to normal performance ";
                                            body += subject + " on <b>" + (DateTime.Now).ToString("dd/MM/yyyy HH:mm") + "</b></br>";
                                            body += "<b>Total consucutive messages affected by the performance degradation:</b> " + NUMBER_OF_CONSICUTIVE_DEGRADATION_MESSAGES.ToString() + "</br>";
                                            body += "<b>The latest reply was received within:</b> " + stopwatch.ElapsedMilliseconds.ToString()+" ms</br>";
                                        }

                                        if (!wasAffectedByPerformanceDegradation && affectedByPerformanceDegradation) // Message is affected by a performance degradation - send email to inform support team
                                        {
                                            subject = string.Format(subject, "WARNING", sapReplyResult.sAPMessageType.ToString()) + " is affected by a performance degradation";
                                            body += subject + " on <b>" + (DateTime.Now).ToString("dd/MM/yyyy HH:mm") + "</b></br>";
                                            body += "You should receive this alert because the latest " 
                                                 + (message.PerformanceDegradationAlertMessagesCount ?? 99999) + " were affected by a performance degradation and took more than " 
                                                 + message.WorstAcceptableResponseTimeInMilliseconds.ToString() + " ms to give a reply from SAP</br>";
                                            body += "<b>The latest reply was received within:</b> " + stopwatch.ElapsedMilliseconds.ToString() + " ms</br>";
                                        }

                                        SAPRequester.SendEmail(subject, body, emailFrom, message.DailyReportEmailTo, emailSMTPServer);
                                    }
                                }
                            }

                            if (wasDown != isDown)
                            {
                                string subject = "Communication between SAP and B2B via B2B Hybris webservices is {0}";
                                string body = "";
                                if (wcfB2BWebServiceURLException != null)
                                {
                                    subject = "WCF B2B webservice is {0}";
                                }
                                if (wcfMethodNameException != null)
                                {
                                    subject = "Websphere message broker is {0}";
                                }

                                if (wasDown && !isDown) // Back up again - update variables and send email to inform support team
                                {
                                    endDownTime = DateTime.Now;
                                    TOTAL_DOWN_TIME = TOTAL_DOWN_TIME + (endDownTime - startDownTime).Value.TotalSeconds;
                                    subject = string.Format(subject, "back up again");
                                    body += subject + " on <b>" + ((DateTime)endDownTime).ToString("dd/MM/yyyy HH:mm") + "</b></br>";
                                    body += "<b>Total estimated downtime:</b> " + ToReadableString((endDownTime - startDownTime).Value) + "</br>";
                                    body += "<b>Begin downtime:</b> " + ((DateTime)startDownTime).ToString("dd/MM/yyyy HH:mm") + "</br>";
                                    body += "<b>End downtime:</b> " + ((DateTime)endDownTime).ToString("dd/MM/yyyy HH:mm") + "</br>";
                                }

                                if (!wasDown && isDown) // Site is down - send email to inform support team
                                {
                                    subject = string.Format(subject, "down");
                                    body += subject + " since <b>" + ((DateTime)startDownTime).ToString("dd/MM/yyyy HH:mm") + "</b> due to the below error:</br>";
                                    body += "<b>Error Message:</b> " + sapReplyResult.error.Message + "</br>";
                                    if (!String.IsNullOrEmpty(sapReplyResult.error.StackTrace))
                                    {
                                        body += "<b>Error StackTrace:</b> " + sapReplyResult.error.StackTrace + "</br>";
                                    }
                                }

                                SAPRequester.SendEmail(subject, body, emailFrom, message.DailyReportEmailTo, emailSMTPServer);
                            }
                            else
                            {
                                bool RunReport = false;
                                if (message.LastReportSentOn == null || (DateTime.Now - (DateTime)message.LastReportSentOn).TotalMinutes >= 24 * 60)
                                {
                                    if (DateTime.Now.Hour == message.DailyReportHour && DateTime.Now.Minute >= message.DailyReportMinute)
                                    {
                                        RunReport = true;
                                    }
                                }

                                if (RunReport)
                                {
                                    END_MONITORING = DateTime.Now;
                                    if (message.SendDailyReport)
                                    {
                                        string subject = "Daily monitoring report";
                                        TimeSpan interval = ((DateTime)END_MONITORING - START_MONITORING);
                                        string body = "Below you can find the daily stats of the latest monitoring interval of a " + sapReplyResult.sAPMessageType.ToString() + " message:</br></br>";
                                        body += "<table>";
                                        body += "<tr><td><b>Start Monitoring Interval:</b></td><td>" + START_MONITORING.ToString("dd/MM/yyyy HH:mm") + "</td></tr>";
                                        body += "<tr><td><b>End Monitoring Interval:</b></td><td>" + ((DateTime)END_MONITORING).ToString("dd/MM/yyyy HH:mm") + "</td></tr>";
                                        body += "<tr><td><b>Monitoring interval duration:</b></td><td>" + ToReadableString(interval) + "</td></tr>";
                                        body += "<tr><td><b>Number of requests:</b></td><td>" + NUMBER_OF_REQUESTS + "</td></tr>";
                                        body += "<tr><td><b>Number of failed requests:</b></td><td>" + NUMBER_OF_FAILED_REQUESTS + "</td></tr>";
                                        body += "<tr><td><b>Number of messages affected by a performance degradation:</b></td><td>" + NUMBER_OF_DEGRADATION_MESSAGES + "</td></tr>";
                                        body += "<tr><td><b>Min reply time:</b></td><td>" + MIN_REPLY_TIME + " ms</td></tr>";
                                        body += "<tr><td><b>Max reply time:</b></td><td>" + MAX_REPLY_TIME + " ms</td></tr>";
                                        body += "<tr><td><b>Average reply time:</b></td><td>" + Math.Round((double)(TOTAL_REPLY_TIME / NUMBER_OF_REQUESTS), 2) + " ms</td></tr>";
                                        body += "<tr><td><b>Total downtime:</b></td><td>" + ToReadableString(TimeSpan.FromSeconds(TOTAL_DOWN_TIME)) + "</td></tr>";
                                        body += "<tr><td><b>Availability percentage:</b></td><td>" + (Math.Round((((interval.TotalSeconds - TOTAL_DOWN_TIME) / interval.TotalSeconds) * 100), 2)) + " %</td></tr>";
                                        body += "</table>";
                                        SAPRequester.SendEmail(subject, body, emailFrom, message.DailyReportEmailTo, emailSMTPServer);
                                        InitCounters = true;
                                    }
                                }
                            }

                            string ErrorMessage = (sapReplyResult.HasError) ? sapReplyResult.error.Message ?? "" + sapReplyResult.error.StackTrace ?? "" : null;
                            if (InitCounters)
                            {
                                MAX_REPLY_TIME = 0;
                                MIN_REPLY_TIME = 10000;
                                TOTAL_REPLY_TIME = 0;
                                NUMBER_OF_REQUESTS = 0;
                                NUMBER_OF_FAILED_REQUESTS = 0;
                                TOTAL_DOWN_TIME = 0;
                                START_MONITORING = DateTime.Now;
                                END_MONITORING = null;
                                LastReportSentOn = DateTime.Now;
                                NUMBER_OF_DEGRADATION_MESSAGES = 0;
                            }
                            XDocument reply = GetXMlReplyByCorrelid(message.WcfB2BWebServiceURL, sapReplyResult.CorrelID, ConnectionString, emailFrom, emailTo, emailSMTPServer);

                            string DateIN = string.Empty;
                            string DateOUt = string.Empty;

                            if (reply.Root!= null)
                            {
                                var IN_API = reply.Descendants("IN_API").Select(x => new { IN_APIDate = (string)x.Element("DATE_TIME") });

                                var OUT_API = reply.Descendants("OUT_API").Select(x => new { OUT_APIDate = (string)x.Element("DATE_TIME") });

                                 DateIN = IN_API.First().IN_APIDate.ToString();

                                 DateOUt = OUT_API.First().OUT_APIDate.ToString();
                            }

                          
                        SaveMessagLog(ref message, ConnectionString, emailFrom, emailTo, emailSMTPServer, message.MessageID, sapReplyResult.CorrelID, stopwatch.ElapsedMilliseconds, sapReplyResult.RequestedDate, sapReplyResult.HasError, ErrorMessage, LastReportSentOn, ConvertToDateTime( DateIN), ConvertToDateTime(DateOUt));
                        }
                        else
                        {
                            message = GetMonitoringMessage(ConnectionString, emailFrom, emailTo, emailSMTPServer, message.MessageID);
                            stopwatch.Stop();
                        }
                    }
                    else
                    {
                        message = GetMonitoringMessage(ConnectionString, emailFrom, emailTo, emailSMTPServer, message.MessageID);
                        stopwatch.Stop();
                    }

                    if (stopwatch.ElapsedMilliseconds < message.MessageIntervalInSeconds * 1000)
                    {
                        Thread.Sleep(message.MessageIntervalInSeconds * 1000 - (int)stopwatch.ElapsedMilliseconds);
                    }
                }
            }
        }

        private static void SaveMessagLog(ref MonitoringMessage monitoringMessage, string ConnectionString, string emailFrom, string emailTo, string emailSMTPServer, Guid MessageID,string CorrelID,long ExecutionTime,DateTime sentOn,bool? HasError,string ErrorMessage,DateTime? LastReportSentOn,DateTime? InAPIDate,DateTime? OutAPIDate)
        {
            try
            {
                using (SqlConnection cnx = new SqlConnection(ConnectionString))
                {
                    DataSet dataSet = new DataSet();
                    SqlCommand cmd = new SqlCommand("Monitoring.SaveMessageLog", cnx);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 3600;
                    cmd.Parameters.Add(new SqlParameter("@MessageID", MessageID));
                    cmd.Parameters.Add(new SqlParameter("@CorrelID", CorrelID));
                    cmd.Parameters.Add(new SqlParameter("@ExecutionTime", ExecutionTime));
                    cmd.Parameters.Add(new SqlParameter("@SentOn", sentOn));
                    cmd.Parameters.Add(new SqlParameter("@HasError", HasError));
                    if (HasError == true)
                    {
                        cmd.Parameters.Add(new SqlParameter("@ErrorMessage", ErrorMessage));
                    }
                    cmd.Parameters.Add(new SqlParameter("@LastReportSentOn", LastReportSentOn));
                    cmd.Parameters.Add(new SqlParameter("@InAPIDate", InAPIDate));
                    cmd.Parameters.Add(new SqlParameter("@OutAPIDate", OutAPIDate));
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
                    sqlDataAdapter.Fill(dataSet);
                    if (dataSet.Tables.Count == 2)
                    {
                        DataTable dtMessages = dataSet.Tables[0];
                        DataTable dtPauseIntervals = dataSet.Tables[1];

                        if (dtMessages.Rows.Count == 1)
                        {
                            DataRow row = dtMessages.Rows[0];
                            monitoringMessage = GetMonitoringMessageFromRow(dtMessages.Rows[0], dataSet.Tables[1].Select("MessageID='" + MessageID.ToString() + "'"));
                        }
                        else
                        {
                            monitoringMessage = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SAPRequester.SendErrorEmail(ex, emailFrom, emailTo, emailSMTPServer, "B2B Messages Monitoring Service catched an error: ");
            }
        }

        private static XDocument GetXMlReplyByCorrelid(string wcfB2BWebServiceURL,string Correlid, string ConnectionString, string emailFrom, string emailTo, string emailSMTPServer)
        {
            XDocument res = new XDocument();
            try
            {
                using (SqlConnection cnx = new SqlConnection(ConnectionString))
                {
                    DataTable dataTable = new DataTable();
                    SqlCommand cmd = new SqlCommand("Monitoring.GetReplyByCorrelid", cnx);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 3600;
                    cmd.Parameters.Add(new SqlParameter("@wcfB2BWebServiceURL", wcfB2BWebServiceURL));
                    cmd.Parameters.Add(new SqlParameter("@Correlid", Correlid));
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
                    sqlDataAdapter.Fill(dataTable);
                    if( dataTable.Rows.Count == 1)
                    {
                        string str = dataTable.Rows[0][0].ToString();
                        res= XDocument.Parse(str);
                    }

                }
                 
            }
            catch (Exception ex)
            {
                SAPRequester.SendErrorEmail(ex, emailFrom, emailTo, emailSMTPServer, "B2B Messages Monitoring Service catched an error: ");
            }
            return res;

        }

        private static MonitoringMessage GetMonitoringMessageFromRow(DataRow row,DataRow[] pauseIntervals)
        {
            MonitoringMessage message = new MonitoringMessage(new Guid(row["MessageID"].ToString()),
                                                            new Guid(row["C_GLOBALID"].ToString()),
                                                            (bool)row["ActivatePerformanceDegradationAlerts"],
                                                            row["DailyReportEmailTo"].ToString(),
                                                            (row["DailyReportHour"] == DBNull.Value ? (int?)null : (int)row["DailyReportHour"]),
                                                            (row["DailyReportMinute"] == DBNull.Value ? (int?)null : (int)row["DailyReportMinute"]),
                                                            (int)row["ExpectedResponseTimeInMilliseconds"],
                                                            (bool)row["IsActive"],
                                                            (int)row["MessageIntervalInSeconds"],
                                                            row["MessageXML"].ToString(),
                                                            (bool)row["SendDailyReport"],
                                                            (row["PerformanceDegradationAlertMessagesCount"] == DBNull.Value ? (int?)null : (int)row["PerformanceDegradationAlertMessagesCount"]),
                                                            new Guid(row["SessionID"].ToString()),
                                                            row["WcfB2BWebServiceURL"].ToString(),
                                                            row["WcfMethodName"].ToString(),
                                                            row["WcfPassword"].ToString(),
                                                            row["WcfUserName"].ToString(),
                                                            (int)row["WorstAcceptableResponseTimeInMilliseconds"],
                                                            (row["LastReportSentOn"] == DBNull.Value ? (DateTime?)null : (DateTime)row["LastReportSentOn"])
                                                            );


            foreach (DataRow pauseInterval in pauseIntervals)
            {
                MonitoringMessagePauseInterval monitoringMessagePauseInterval = GetMonitoringMessagePauseIntervalFromDataRow(pauseInterval);
                if (message.pauseIntervals == null)
                {
                    message.pauseIntervals = new List<MonitoringMessagePauseInterval>();
                }

                message.pauseIntervals.Add(monitoringMessagePauseInterval);
            }
            return message;
        }

        public static MonitoringMessagePauseInterval GetMonitoringMessagePauseInterval(int pauseIntervalID,string ConnectionString, string emailFrom, string emailTo, string emailSMTPServer)
        {
            MonitoringMessagePauseInterval pauseInterval = null;

            try
            {
                using (SqlConnection cnx = new SqlConnection(ConnectionString))
                {
                    DataTable dataTable = new DataTable();
                    SqlCommand cmd = new SqlCommand("Monitoring.GetMonitoringMessagePauseInterval", cnx);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 3600;
                    cmd.Parameters.Add(new SqlParameter("@PauseIntervalID", pauseIntervalID));
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
                    sqlDataAdapter.Fill(dataTable);
                    if(dataTable.Rows.Count == 1)
                    {
                        pauseInterval = GetMonitoringMessagePauseIntervalFromDataRow(dataTable.Rows[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                SAPRequester.SendErrorEmail(ex, emailFrom, emailTo, emailSMTPServer, "B2B Messages Monitoring Service catched an error: ");
            }

            return pauseInterval;
        }

        public static MonitoringMessagePauseInterval GetMonitoringMessagePauseIntervalFromDataRow(DataRow dataRow)
        {
            MonitoringMessagePauseInterval pauseInterval = null;

            if(dataRow != null)
            {
                pauseInterval = new MonitoringMessagePauseInterval((int)dataRow["PauseIntervalID"],
                                                                            (int)dataRow["StartPauseTimeHour"],
                                                                            (int)dataRow["StartPauseTimeMinute"],
                                                                            (int)dataRow["EndPauseTimeHour"],
                                                                            (int)dataRow["EndPauseTimeMinute"],
                                                                            (bool)dataRow["OccursEveryDay"],
                                                                            (dataRow["StartDayOfOccurence"] == DBNull.Value ? (DateTime?)null : (DateTime)dataRow["StartDayOfOccurence"]),
                                                                            (dataRow["EndDayOfOccurence"] == DBNull.Value ? (DateTime?)null : (DateTime)dataRow["EndDayOfOccurence"]));
            }

            return pauseInterval;
        }



        public static MonitoringMessage GetMonitoringMessage(string ConnectionString, string emailFrom, string emailTo, string emailSMTPServer, Guid MessageID)
        {
            MonitoringMessage message = null;
            try
            {
                using (SqlConnection cnx = new SqlConnection(ConnectionString))
                {
                    DataSet dataSet = new DataSet();
                    SqlCommand cmd = new SqlCommand("Monitoring.GetMessageToMonitor", cnx);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 3600;
                    cmd.Parameters.Add(new SqlParameter("@MessageID", MessageID));
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
                    sqlDataAdapter.Fill(dataSet);
                    if (dataSet.Tables.Count == 2)
                    {
                        DataTable dtMessages = dataSet.Tables[0];

                        if (dtMessages.Rows.Count == 1)
                        {
                            DataRow row = dtMessages.Rows[0];
                            message = GetMonitoringMessageFromRow(dtMessages.Rows[0], dataSet.Tables[1].Select("MessageID='" + MessageID.ToString() + "'"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SAPRequester.SendErrorEmail(ex, emailFrom, emailTo, emailSMTPServer, "B2B Messages Monitoring Service catched an error: ");
            }

            return message;
        }

        public static List<MonitoringMessagePauseInterval> GetMonitoringMessagePauseIntervals(string ConnectionString, string emailFrom, string emailTo, string emailSMTPServer, Guid MessageID)
        {
            List<MonitoringMessagePauseInterval> pauseIntervalsList = null;
            try
            {
                using (SqlConnection cnx = new SqlConnection(ConnectionString))
                {
                    DataTable dataTable = new DataTable();
                    SqlCommand cmd = new SqlCommand("Monitoring.[GetMonitoringMessagePauseIntervals]", cnx);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 3600;
                    cmd.Parameters.Add(new SqlParameter("@MessageID", MessageID));
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
                    sqlDataAdapter.Fill(dataTable);
                    foreach(DataRow row in dataTable.Rows)
                    {
                        if(pauseIntervalsList == null)
                        {
                            pauseIntervalsList = new List<MonitoringMessagePauseInterval>();
                        }
                        pauseIntervalsList.Add(GetMonitoringMessagePauseIntervalFromDataRow(row));
                    }
                }
            }
            catch (Exception ex)
            {
                SAPRequester.SendErrorEmail(ex, emailFrom, emailTo, emailSMTPServer, "B2B Messages Monitoring Service catched an error: ");
            }

            return pauseIntervalsList;
        }

        public static List<MonitoringMessage> FillMessageToMonitor(string ConnectionString, string emailFrom, string emailTo, string emailSMTPServer, Guid? MessageID = null)
        {
            List<MonitoringMessage> monitoringMessagesList = new List<MonitoringMessage>();
            try
            {
                using (SqlConnection cnx = new SqlConnection(ConnectionString))
                {
                    DataSet dataSet = new DataSet();
                    SqlCommand cmd = new SqlCommand("Monitoring.GetMessageToMonitor", cnx);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 3600;
                    if (MessageID != null)
                    {
                        cmd.Parameters.Add(new SqlParameter("@MessageID", MessageID));
                    }
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
                    sqlDataAdapter.Fill(dataSet);
                    if (dataSet.Tables.Count == 2)
                    {
                        DataTable dtMessages = dataSet.Tables[0];
                        DataTable dtPauseIntervals = dataSet.Tables[1];

                        foreach (DataRow row in dtMessages.Rows)
                        {
                            Guid messageID = new Guid(row["MessageID"].ToString());
                            MonitoringMessage message = GetMonitoringMessageFromRow(row, dataSet.Tables[1].Select("MessageID='" + messageID.ToString() + "'"));
                            monitoringMessagesList.Add(message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SAPRequester.SendErrorEmail(ex,emailFrom,emailTo,emailSMTPServer, "B2B Messages Monitoring Service catched an error: ");
            }

            return monitoringMessagesList;
        }

        public static string ToReadableString(this TimeSpan span)
        {
            string formatted = string.Format("{0}{1}{2}{3}",
                span.Duration().Days > 0 ? string.Format("{0:0} day{1}, ", span.Days, span.Days == 1 ? String.Empty : "s") : string.Empty,
                span.Duration().Hours > 0 ? string.Format("{0:0} hour{1}, ", span.Hours, span.Hours == 1 ? String.Empty : "s") : string.Empty,
                span.Duration().Minutes > 0 ? string.Format("{0:0} minute{1}, ", span.Minutes, span.Minutes == 1 ? String.Empty : "s") : string.Empty,
                span.Duration().Seconds > 0 ? string.Format("{0:0} second{1}", span.Seconds, span.Seconds == 1 ? String.Empty : "s") : string.Empty);

            if (formatted.EndsWith(", "))
            {
                formatted = formatted.Substring(0, formatted.Length - 2);
            }

            if (string.IsNullOrEmpty(formatted))
            {
                formatted = "0 seconds";
            }

            return formatted;
        }

        public static DateTime? ConvertToDateTime(string date)
        {
            if (date != string.Empty)
            {
                string[] Array = date.Split('-');
                var DateArray = Array[0].Split('.').ToList();
                var TimeArray = Array[1].Split(':').ToList();
                if (TimeArray.Count == 3)
                {
                    DateTime res = new DateTime(int.Parse(DateArray[0]), int.Parse(DateArray[1]), int.Parse(DateArray[2]), int.Parse(TimeArray[0]), int.Parse(TimeArray[1]), int.Parse(TimeArray[2]));
                    return res;
                }
                else
                {
                   string millisecond  = TimeArray[3] ;
                    if (millisecond.Length==2)
                    {
                        millisecond  += "0";
                    }

                    DateTime res = new DateTime(int.Parse(DateArray[0]), int.Parse(DateArray[1]), int.Parse(DateArray[2]), int.Parse(TimeArray[0]), int.Parse(TimeArray[1]), int.Parse(TimeArray[2]),int.Parse(millisecond));
                    return res;
                }


            }
            else
                return null;
         
        }

        private static Exception Ping(string url)
        {
            try
            {
                Uri uri = new Uri(url);

                Ping ping = new Ping();
                PingReply pingReply = ping.Send(uri.Host);
                if (pingReply.Status != IPStatus.Success)
                {
                    // Website is not available.
                    return new Exception(string.Format("{0} is not available", url));
                }
            }
            catch (Exception ex)
            {
                return ex;
            }
            return null; // return null if success
        }
    }
}
