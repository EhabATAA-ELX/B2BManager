using System;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading;

namespace SAPRequestsLib
{
    public static class B2BMessagesMonitoringManager
    {
        private static long MAX_REPLY_TIME;
        private static long MIN_REPLY_TIME;
        private static long TOTAL_REPLY_TIME;
        private static int NUMBER_OF_REQUESTS;
        private static int NUMBER_OF_FAILED_REQUESTS;
        private static bool IS_DOWN;
        private static double TOTAL_DOWN_TIME;
        private static DateTime START_MONITORING;
        private static DateTime? END_MONITORING;

        private static DateTime LastReportSentOn;
        private static string EmailFrom;
        private static string EmailTo;
        private static string EmailSMTPServer;
        private static string XmlFilePath;

        public static void RunMonitoring(string xmlFilePath, string WcfB2BWebServiceURL, string WcfMethodName, string WcfUserName, string WcfPassword, string GlobalId, string SessionId, string emailFrom, string emailTo, string emailSMTPServer, int MessageIntervalInSeconds, bool SendReport = false, int ReportHour = 0, int ReportMinute = 0, string ReportEmailTo = "hamdi.kharrat@electrolux.com")
        {
            EmailFrom = emailFrom;
            EmailTo = emailTo;
            EmailSMTPServer = emailSMTPServer;
            XmlFilePath = xmlFilePath;
            string Message = GetMessage();
            LastReportSentOn = DateTime.Now.AddDays(-1);
            if (!string.IsNullOrEmpty(Message))
            {
                IS_DOWN = false;
                bool wasDown = false;
                DateTime? startDownTime = null;
                DateTime? endDownTime = null;
                InitCounters();

                while (true)
                {
                    wasDown = IS_DOWN;
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    Exception wcfB2BWebServiceURLException = Ping(WcfB2BWebServiceURL); // Ping the WCF webservice URL
                    Exception wcfMethodNameException = null;
                    SAPReplyResult sapReplyResult = new SAPReplyResult
                    {
                        HasError = wcfB2BWebServiceURLException == null,
                        error = wcfB2BWebServiceURLException
                    };
                    if (wcfB2BWebServiceURLException == null)
                    {
                        wcfMethodNameException = Ping(WcfMethodName); // Ping WMB webservice URL                
                        if (wcfMethodNameException == null)
                        {
                            sapReplyResult = SAPRequester.SendSAPMessage(Message,
                                                                   WcfB2BWebServiceURL,
                                                                   WcfMethodName,
                                                                   WcfUserName,
                                                                   WcfPassword,
                                                                   GlobalId,
                                                                   SessionId
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
                        IS_DOWN = true;
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
                        IS_DOWN = false;
                    }

                    if (wasDown != IS_DOWN)
                    {
                        string subject = "Communication between SAP and B2B via B2B Hybris webservices is {0}";
                        string body = "";
                        if (wcfB2BWebServiceURLException != null)
                        {
                            subject = "WCF B2B Web Service is {0}";
                        }
                        if (wcfMethodNameException != null)
                        {
                            subject = "Websphere message broker is {0}";
                        }

                        if (wasDown && !IS_DOWN) // Back up again - update variables and send email to inform support team
                        {
                            endDownTime = DateTime.Now;
                            TOTAL_DOWN_TIME = TOTAL_DOWN_TIME + (endDownTime - startDownTime).Value.TotalSeconds;
                            subject = string.Format(subject, "back up again");
                            body += subject + " on <b>" + ((DateTime)endDownTime).ToString("dd/MM/yyyy HH:mm") + "</b></br>";
                            body += "<b>Total estimated downtime:</b> " + ToReadableString((endDownTime - startDownTime).Value) + "</br>";
                            body += "<b>Begin downtime:</b> " + ((DateTime)startDownTime).ToString("dd/MM/yyyy HH:mm") + "</br>";
                            body += "<b>End downtime:</b> " + ((DateTime)endDownTime).ToString("dd/MM/yyyy HH:mm") + "</br>";
                        }

                        if (!wasDown && IS_DOWN) // Site is down - send email to inform support team
                        {
                            subject = string.Format(subject, "down");
                            body += subject + " since <b>" + ((DateTime)startDownTime).ToString("dd/MM/yyyy HH:mm") + "</b> due to the below error:</br>";
                            body += "<b>Error Message:</b> " + sapReplyResult.error.Message + "</br>";
                            if (!String.IsNullOrEmpty(sapReplyResult.error.StackTrace))
                            {
                                body += "<b>Error StackTrace:</b> " + sapReplyResult.error.StackTrace + "</br>";
                            }
                        }

                        SendEmail(subject, body);
                    }
                    else
                    {
                        bool RunReport = false;
                        if ((DateTime.Now - LastReportSentOn).TotalMinutes >= 24 * 60)
                        {
                            if (DateTime.Now.Hour == ReportHour && DateTime.Now.Minute >= ReportMinute)
                            {
                                RunReport = true;
                            }
                        }

                        if (RunReport)
                        {
                            END_MONITORING = DateTime.Now;
                            if (SendReport)
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
                                body += "<tr><td><b>Min reply time:</b></td><td>" + MIN_REPLY_TIME + " ms</td></tr>";
                                body += "<tr><td><b>Max reply time:</b></td><td>" + MAX_REPLY_TIME + " ms</td></tr>";
                                body += "<tr><td><b>Average reply time:</b></td><td>" + Math.Round((double)(TOTAL_REPLY_TIME / NUMBER_OF_REQUESTS), 2) + " ms</td></tr>";
                                body += "<tr><td><b>Total downtime:</b></td><td>" + ToReadableString(TimeSpan.FromSeconds(TOTAL_DOWN_TIME)) + "</td></tr>";
                                body += "<tr><td><b>Availability percentage:</b></td><td>" + (Math.Round((((interval.TotalSeconds - TOTAL_DOWN_TIME) / interval.TotalSeconds) * 100), 2)) + " %</td></tr>";
                                body += "</table>";
                                SendEmail(subject, body, ReportEmailTo);
                            }
                            InitCounters();
                            LastReportSentOn = DateTime.Now;
                        }

                    }

                    if (stopwatch.ElapsedMilliseconds < MessageIntervalInSeconds * 1000)
                    {
                        Thread.Sleep(MessageIntervalInSeconds * 1000 - (int)stopwatch.ElapsedMilliseconds);
                    }
                }
            }
        }


        private static void InitCounters()
        {
            MAX_REPLY_TIME = 0;
            MIN_REPLY_TIME = 10000;
            TOTAL_REPLY_TIME = 0;
            NUMBER_OF_REQUESTS = 0;
            NUMBER_OF_FAILED_REQUESTS = 0;
            TOTAL_DOWN_TIME = 0;
            START_MONITORING = DateTime.Now;
            END_MONITORING = null;
        }

        public static void SendEmail(string subject, string body, string emailTo = "")
        {
            try
            {
                System.Net.Mail.MailMessage oMail;
                oMail = new System.Net.Mail.MailMessage(EmailFrom, (String.IsNullOrEmpty(emailTo) ? EmailTo : emailTo));
                oMail.BodyEncoding = System.Text.Encoding.UTF8;
                oMail.IsBodyHtml = true;
                oMail.Body = body;
                oMail.Priority = System.Net.Mail.MailPriority.High;
                oMail.Subject = "B2B Messages Monitoring Service: " + subject;
                System.Net.Mail.SmtpClient objSmtp = new System.Net.Mail.SmtpClient(EmailSMTPServer);
                objSmtp.Send(oMail);
            }
            catch { }
        }

        private static string ToReadableString(this TimeSpan span)
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

        private static string GetMessage()
        {
            string path = XmlFilePath;
            string readText = string.Empty;

            if (File.Exists(path))
            {
                readText = File.ReadAllText(path);
            }

            return readText;
        }
    }
}
