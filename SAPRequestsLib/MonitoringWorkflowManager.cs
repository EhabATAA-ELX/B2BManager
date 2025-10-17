using Microsoft.Web.Administration;
using SAPRequestsLib.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SAPRequestsLib
{
    public static class MonitoringWorkflowManager
    {
        public enum MonitoringActionType
        {
            Workflow = 0,
            WindowService = 1,
            ApplicationPool = 2,
            PingURL = 3,
            SQLQuery = 4,
            CustomAction = 5
        }

        public enum MonitoringLogType
        {
            MainWorkflow = 0,
            Action = 1
        }

        public class MonitoringActionInfo
        {
            public string Comments { get; set; }
            public Guid processID { get; set; }
            public int WorkflowActionID { get; set; }
            public Exception error { get; set; }

            public string OutputValue { get; set; }
            public bool Passed { get; set; }

            public decimal? WarningRatio { get; set; }
            public decimal? ResultRatio { get; set; }

            public bool InWarning
            {
                get
                {
                    if(monitoringActionType == MonitoringActionType.SQLQuery)
                    {
                        return (ResultRatio ?? 100) < WarningRatio;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            public MonitoringActionType monitoringActionType { get; set; }
            public int MainWorkflowID { get; set; }
        }

        public const int SECURE_WORKFLOW_PARENT_VALUE = 10000;

        private static volatile List<MonitoringActionInfo> MonitoringActionList = new List<MonitoringActionInfo>();
        public static MonitoringWorkflow ExecuteWorkflow(int WorkflowID, string ConnectionString, string emailFrom, string emailTo, string emailSMTPServer,int? ExecutedBy = null)
        {
            MonitoringWorkflow workflow = GetWorkflowByID(WorkflowID, ConnectionString, emailFrom, emailTo, emailSMTPServer);
            workflow.ProcessID = Guid.NewGuid();
            Thread thread = new Thread(t => ProcessWorkflowActions(workflow, ConnectionString, emailFrom, emailTo, emailSMTPServer, ExecutedBy));
            thread.Start();
            return workflow;
        }

        private static List<MonitoringActionInfo> ProcessWorkflowActions(MonitoringWorkflow workflow, string ConnectionString, string emailFrom, string emailTo, string emailSMTPServer, int? ExecutedBy = null)
        {
            List<MonitoringActionInfo> currentProcessMonitoringActionList = new List<MonitoringActionInfo>();
            UpdateLog(MonitoringLogType.MainWorkflow, workflow.ID, workflow.ProcessID, ConnectionString, emailFrom, emailTo, emailSMTPServer, ExecutedBy);
            List<MonitoringActionInfo> currentProcessMonitoringActionInErrorList = new List<MonitoringActionInfo>();
            if (workflow.actions != null && workflow.actions.Count > 0)
            {
                RunActions(workflow.GetTree(workflow.ID + SECURE_WORKFLOW_PARENT_VALUE), workflow.ID, workflow.ProcessID, ConnectionString, emailFrom, emailTo, emailSMTPServer, ExecutedBy);
                if (MonitoringActionList.Where(a => ((a.processID == workflow.ProcessID) && !a.Passed)) != null)
                {
                    currentProcessMonitoringActionInErrorList = MonitoringActionList.Where(a => ((a.processID == workflow.ProcessID) && !a.Passed)).ToList();
                }
                currentProcessMonitoringActionList = MonitoringActionList.Where(a => (a.processID == workflow.ProcessID)).ToList();
            }
            else
            {
                currentProcessMonitoringActionInErrorList.Add(new MonitoringActionInfo() { processID = workflow.ProcessID, error = new Exception("Failed to get workflow actions"), WorkflowActionID = workflow.ID });
                currentProcessMonitoringActionList = currentProcessMonitoringActionInErrorList;
            }
            if (MonitoringActionList.Where(a => a.processID == workflow.ProcessID) != null)
            {
                CheckRemainingActions(workflow.ProcessID, MonitoringActionList.Where(a => a.processID == workflow.ProcessID).ToList(), ConnectionString, emailFrom, emailTo, emailSMTPServer, ExecutedBy);
                MonitoringActionList.RemoveAll(a => a.processID == workflow.ProcessID);
            }

            UpdateLog(MonitoringLogType.MainWorkflow, workflow.ID, workflow.ProcessID, ConnectionString, emailFrom, emailTo, emailSMTPServer, ExecutedBy, (currentProcessMonitoringActionInErrorList.Count > 0) ? currentProcessMonitoringActionInErrorList.Take(1).Single().error : null, currentProcessMonitoringActionInErrorList.Count > 0 ? "Error" : "Success", currentProcessMonitoringActionInErrorList.Count == 0);
            return currentProcessMonitoringActionList;
        }

        public static List<MonitoringWorkflow> FillWorkflowsToMonitor(string ConnectionString, string emailFrom, string emailTo, string emailSMTPServer)
        {
            List<MonitoringWorkflow> monitoringWorkflowList = new List<MonitoringWorkflow>();

            try
            {
                using (SqlConnection cnx = new SqlConnection(ConnectionString))
                {
                    DataTable dataTable = new DataTable();
                    SqlCommand cmd = new SqlCommand("Monitoring.GetWorkflowsToMonitor", cnx);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 3600;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
                    sqlDataAdapter.Fill(dataTable);
                    if (dataTable.Rows.Count > 0)
                    {
                        foreach (DataRow row in dataTable.Rows)
                        {
                            MonitoringWorkflow monitoringWorkflow = new MonitoringWorkflow();
                            monitoringWorkflow.ID = (int)row["WorkflowID"];
                            FillWorfklowFromRow(row, ref monitoringWorkflow);
                            monitoringWorkflowList.Add(monitoringWorkflow);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SAPRequester.SendErrorEmail(ex, emailFrom, emailTo, emailSMTPServer, "FillWorkflowsToMonitor catched an error: ");
            }

            return monitoringWorkflowList;
        }

        public static void MonitorWorkflow(MonitoringWorkflow workflowToMonitor, string ConnectionString, string emailFrom, string emailTo, string emailSMTPServer)
        {
            if (workflowToMonitor != null)
            {
                bool isInError = false;
                bool wasInError = false;

                DateTime? startErrorOccurenceOn = null;
                DateTime? endErrorOccurenceOn = null;
                
                int NUMBER_OF_OCCURENCES = 0;
                int NUMBER_OF_FAILED_OCCURENCES = 0;
                int NUMBER_OF_WARNING_OCCURENCES = 0;
                double TOTAL_ERROR_TIME = 0;
                DateTime START_MONITORING = DateTime.Now;
                DateTime? END_MONITORING = null;

                while (true && workflowToMonitor != null)
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    workflowToMonitor = GetWorkflowByID(workflowToMonitor.ID, ConnectionString, emailFrom, emailTo, emailSMTPServer);
                    if (workflowToMonitor.MonitoringEnabled)
                    {
                        bool IsPaused = false;
                        List<MonitoringMessagePauseInterval> pauseIntervals = MonitoringManager.GetMonitoringMessagePauseIntervals(ConnectionString, emailFrom, emailTo, emailSMTPServer, workflowToMonitor.WorkflowUID);
                        if (pauseIntervals != null)
                        {
                            foreach (MonitoringMessagePauseInterval messagePauseInterval in pauseIntervals)
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
                        
                        wasInError = isInError;

                        if (!IsPaused)
                        {
                            workflowToMonitor.ProcessID = Guid.NewGuid();
                            List<MonitoringActionInfo> currentProcessMonitoringActionList = ProcessWorkflowActions(workflowToMonitor, ConnectionString, emailFrom, emailTo, emailSMTPServer);
                            stopwatch.Stop();
                            NUMBER_OF_OCCURENCES++;

                            if (currentProcessMonitoringActionList.Where(a => !a.Passed).ToList().Count > 0)
                            {
                                NUMBER_OF_FAILED_OCCURENCES++;
                                isInError = true;
                                if (!wasInError)
                                {
                                    startErrorOccurenceOn = DateTime.Now;
                                }
                            }
                            else
                            {
                                isInError = false;
                            }

                            DateTime? LastReportSentOn = null;
                            bool InitCounters = false;
                            string CheckExecutionsLog = string.Format("You can check the full workflow execution <a href='https://b2bstag.electrolux.net/B2BManager/MonitoringWorkflowProfile.aspx?WorkflowID={0}&ProcessID={1}'>here</a>.", workflowToMonitor.ID, workflowToMonitor.ProcessID);
                            if (!isInError)
                            {                                
                                if (workflowToMonitor.ActivateWarningNotifications)
                                {
                                    if (currentProcessMonitoringActionList.Where(a => a.InWarning).ToList().Count > 0)
                                    {
                                        string subject = "[WARNING] at least one action in the workflow {0} needs your attention";
                                        string body = "";
                                        subject = string.Format(subject, workflowToMonitor.Name.ToString());
                                        body += subject + " on <b>" + (DateTime.Now).ToString("dd/MM/yyyy HH:mm") + "</b></br>";
                                        foreach(MonitoringActionInfo info in currentProcessMonitoringActionList.Where(a => a.InWarning).ToList())
                                        {
                                            body += string.Format("Action <b>{0}</b> returned a ratio of {1} % which is lower than the warning ratio {2} %.</br>",info.Comments,info.ResultRatio,info.WarningRatio);
                                        }
                                        body += CheckExecutionsLog;
                                        SAPRequester.SendEmail(subject, body, emailFrom, workflowToMonitor.SendWarningNotificationsEmailTo, emailSMTPServer);
                                        NUMBER_OF_WARNING_OCCURENCES++;
                                    }
                                }
                            }

                            if (wasInError != isInError)
                            {
                                string subject = "[{0}] The workflow {1} {2}";
                                string body = "";

                                if (wasInError && !isInError) // Back up again - update variables and send email to inform support team
                                {
                                    endErrorOccurenceOn = DateTime.Now;
                                    TOTAL_ERROR_TIME = TOTAL_ERROR_TIME + (endErrorOccurenceOn - startErrorOccurenceOn).Value.TotalSeconds;
                                    subject = string.Format(subject,"INFO",workflowToMonitor.Name, "was executed with success");
                                    body += subject + " on <b>" + ((DateTime)endErrorOccurenceOn).ToString("dd/MM/yyyy HH:mm") + "</b></br>";
                                    body += "<b>Total estimated downtime:</b> " + MonitoringManager.ToReadableString((endErrorOccurenceOn - startErrorOccurenceOn).Value) + "</br>";
                                    body += "<b>First error occurred on:</b> " + ((DateTime)startErrorOccurenceOn).ToString("dd/MM/yyyy HH:mm") + "</br>";
                                    body += "<b>Last error occured on:</b> " + ((DateTime)endErrorOccurenceOn).ToString("dd/MM/yyyy HH:mm") + "</br></br>";
                                }

                                if (!wasInError && isInError) // Site is down - send email to inform support team
                                {
                                    subject = string.Format(subject, "ERROR", workflowToMonitor.Name, "failed to execute");
                                    body += subject + " since <b>" + ((DateTime)startErrorOccurenceOn).ToString("dd/MM/yyyy HH:mm") + "</b> due to the below:</br>";
                                    foreach (MonitoringActionInfo info in currentProcessMonitoringActionList.Where(a => !a.Passed).ToList())
                                    {
                                        if (info.error != null)
                                        {
                                            body += string.Format("Action <b>{0}</b> returned error <span style='color:red'>{1}</span>.</br>", info.Comments, info.error.Message??"");
                                        }
                                    }
                                }
                                body += CheckExecutionsLog;
                                SAPRequester.SendEmail(subject, body, emailFrom, workflowToMonitor.SendAlertsEmailTo , emailSMTPServer);
                            }
                            else
                            {
                                bool RunReport = false;
                                if (workflowToMonitor.LastReportSentOn == null || (DateTime.Now - (DateTime)workflowToMonitor.LastReportSentOn).TotalMinutes >= 24 * 60)
                                {
                                    if (DateTime.Now.Hour == workflowToMonitor.DailyReportHour && DateTime.Now.Minute >= workflowToMonitor.DailyReportMinute)
                                    {
                                        RunReport = true;
                                    }
                                }

                                if (RunReport)
                                {
                                    END_MONITORING = DateTime.Now;
                                    if (workflowToMonitor.SendDailyReport)
                                    {
                                        string subject = "Daily monitoring report";
                                        TimeSpan interval = ((DateTime)END_MONITORING - START_MONITORING);
                                        string body = "Below you can find the daily stats of the latest monitoring interval of the workflow " + workflowToMonitor.Name.ToString() + ":</br></br>";
                                        body += "<table>";
                                        body += "<tr><td><b>Start Monitoring Interval:</b></td><td>" + START_MONITORING.ToString("dd/MM/yyyy HH:mm") + "</td></tr>";
                                        body += "<tr><td><b>End Monitoring Interval:</b></td><td>" + ((DateTime)END_MONITORING).ToString("dd/MM/yyyy HH:mm") + "</td></tr>";
                                        body += "<tr><td><b>Number of requests:</b></td><td>" + NUMBER_OF_OCCURENCES + "</td></tr>";
                                        body += "<tr><td><b>Number of failed requests:</b></td><td>" + NUMBER_OF_FAILED_OCCURENCES + "</td></tr>";
                                        body += "<tr><td><b>Number of occurrences with warnings:</b></td><td>" + NUMBER_OF_WARNING_OCCURENCES + "</td></tr>";
                                        body += "<tr><td><b>Total error:</b></td><td>" + MonitoringManager.ToReadableString(TimeSpan.FromSeconds(TOTAL_ERROR_TIME)) + "</td></tr>";
                                        body += "<tr><td><b>Success percentage:</b></td><td>" + (Math.Round((((interval.TotalSeconds - TOTAL_ERROR_TIME) / interval.TotalSeconds) * 100), 2)) + " %</td></tr>";
                                        body += "<tr><td>You can see the workflow details from <a href='"+ string.Format("https://b2bstag.electrolux.net/B2BManager/MonitoringWorkflowProfile.aspx?WorkflowID={0}", workflowToMonitor.ID) + "'>here</a></td></tr>";
                                        body += "</table>";
                                        SAPRequester.SendEmail(subject, body, emailFrom, workflowToMonitor.DailyReportEmailTo, emailSMTPServer);
                                        InitCounters = true;
                                        UpdateReportSendDate(workflowToMonitor.ID, ConnectionString, emailFrom, emailTo, emailSMTPServer, LastReportSentOn);
                                    }
                                }
                            }

                            if (InitCounters)
                            {
                                NUMBER_OF_OCCURENCES = 0;
                                NUMBER_OF_FAILED_OCCURENCES = 0;
                                TOTAL_ERROR_TIME = 0;
                                START_MONITORING = DateTime.Now;
                                END_MONITORING = null;
                                LastReportSentOn = DateTime.Now;
                                NUMBER_OF_WARNING_OCCURENCES = 0;
                            }
                        }
                        else
                        {
                            stopwatch.Stop();
                        }
                    }
                    else
                    {
                        stopwatch.Stop();
                    }

                    if (stopwatch.ElapsedMilliseconds < workflowToMonitor.MonitoringIntervalInSeconds * 1000)
                    {
                        Thread.Sleep(workflowToMonitor.MonitoringIntervalInSeconds * 1000 - (int)stopwatch.ElapsedMilliseconds);
                    }
                }
            }
        }

        private static void UpdateReportSendDate(int WorkflowID, string ConnectionString, string emailFrom, string emailTo, string emailSMTPServer,DateTime? LastReportSentOn)
        {
            try
            {
                using (SqlConnection cnx = new SqlConnection(ConnectionString))
                {
                    DataSet dataSet = new DataSet();
                    SqlCommand cmd = new SqlCommand("Monitoring.UpdateWorkflowReportSendDate", cnx);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 3600;
                    cmd.Parameters.Add(new SqlParameter("@WorkflowID", WorkflowID));
                    cmd.Parameters.Add(new SqlParameter("@LastReportSentOn", LastReportSentOn));
                    cnx.Open();
                    cmd.ExecuteNonQuery();
                    cnx.Close();
                }
            }
            catch (Exception ex)
            {
                SAPRequester.SendErrorEmail(ex, emailFrom, emailTo, emailSMTPServer, "B2B Messages Monitoring Service catched an error: ");
            }
        }

        private static void CheckRemainingActions(Guid processID, List<MonitoringActionInfo> list, string ConnectionString, string emailFrom, string emailTo, string emailSMTPServer,int? ExecutedBy = null)
        {
            try
            {
                using (SqlConnection cnx = new SqlConnection(ConnectionString))
                {
                    DataTable dataTable = new DataTable();
                    SqlCommand cmd = new SqlCommand("Monitoring.CheckRemainingActions", cnx);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 3600;
                    cmd.Parameters.Add(new SqlParameter("@ProcessID", processID));
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
                    sqlDataAdapter.Fill(dataTable);
                    foreach (DataRow row in dataTable.Rows)
                    {
                        if (list != null)
                        {
                            MonitoringActionInfo monitoringActionInfo = list.Where(a => a.processID == processID && a.WorkflowActionID == (int)row["WorkflowActionID"]).SingleOrDefault();
                            if(monitoringActionInfo != null)
                            {
                                if (monitoringActionInfo.monitoringActionType != MonitoringActionType.Workflow)
                                {
                                    UpdateLog(MonitoringLogType.Action, monitoringActionInfo.WorkflowActionID, processID, ConnectionString, emailFrom, emailTo, emailSMTPServer, null, monitoringActionInfo.error,monitoringActionInfo.OutputValue,monitoringActionInfo.Passed);
                                }
                                else
                                {
                                    UpdateLog(MonitoringLogType.MainWorkflow, monitoringActionInfo.WorkflowActionID, processID, ConnectionString, emailFrom, emailTo, emailSMTPServer, ExecutedBy, monitoringActionInfo.error, monitoringActionInfo.OutputValue, monitoringActionInfo.Passed);
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception error)
            {
                SAPRequester.SendErrorEmail(error, emailFrom, emailTo, emailSMTPServer, "B2B Messages Monitoring Service catched an error: ");
            }
        }

        static void RunActions(IEnumerable<TreeItem<MonitoringWorkflowAction>> workflowActions,int workflowID, Guid processID, string ConnectionString, string emailFrom, string emailTo, string emailSMTPServer, int? ExecutedBy = null)
        {
            List<Thread> childrenThreads = new List<Thread>();
            foreach (TreeItem<MonitoringWorkflowAction> c in workflowActions)
            {
                Thread actionThread = new Thread(p => RunSingleAction(c, processID, ConnectionString, emailFrom, emailTo, emailSMTPServer, workflowID, ExecutedBy));
                actionThread.Start();
                childrenThreads.Add(actionThread);
            }
            foreach (Thread thread in childrenThreads)
            {
                thread.Join();
            }
        }

        static void RunSingleAction(TreeItem<MonitoringWorkflowAction> monitoringWorkflowActionTreeItem, Guid ProcessID, string ConnectionString, string emailFrom, string emailTo, string emailSMTPServer,int MainWorkflowID, int? ExecutedBy = null, ApplicationPoolCollection applicationPoolCollection = null)
        {
            if (monitoringWorkflowActionTreeItem.Item.monitoringActionType != MonitoringActionType.Workflow)
            {
                UpdateLog(MonitoringLogType.Action, monitoringWorkflowActionTreeItem.Item.ID, ProcessID, ConnectionString, emailFrom, emailTo, emailSMTPServer);
            }
            else
            {
                UpdateLog(MonitoringLogType.MainWorkflow, (int)monitoringWorkflowActionTreeItem.Item.LinkedActionID, ProcessID, ConnectionString, emailFrom, emailTo, emailSMTPServer, ExecutedBy);
            }
            Exception error = null;
            string OutputValue = null;
            decimal? ResultRatio = null;
            RunActionManually(ref OutputValue, ref error, monitoringWorkflowActionTreeItem.Item, ConnectionString, emailFrom, emailTo, emailSMTPServer, ref ResultRatio, applicationPoolCollection);


            MonitoringActionList.Add(new MonitoringActionInfo()
            {
                Comments = monitoringWorkflowActionTreeItem.Item.Comments,
                Passed = error == null,
                OutputValue = OutputValue,
                processID = ProcessID,
                error = error,
                monitoringActionType = monitoringWorkflowActionTreeItem.Item.monitoringActionType,
                WorkflowActionID = ((monitoringWorkflowActionTreeItem.Item.monitoringActionType == MonitoringActionType.Workflow) ? (int)monitoringWorkflowActionTreeItem.Item.LinkedActionID : monitoringWorkflowActionTreeItem.Item.ID),
                MainWorkflowID = MainWorkflowID,
                WarningRatio = monitoringWorkflowActionTreeItem.Item.WarningRatio,
                ResultRatio = ResultRatio                
            });

            if (monitoringWorkflowActionTreeItem.Item.monitoringActionType != MonitoringActionType.Workflow)
            {
                UpdateLog(MonitoringLogType.Action, monitoringWorkflowActionTreeItem.Item.ID, ProcessID, ConnectionString, emailFrom, emailTo, emailSMTPServer, null, error, OutputValue, error == null, ResultRatio);
            }
            else
            {
                MainWorkflowID = (int)monitoringWorkflowActionTreeItem.Item.LinkedActionID;
            }
            

            if (error == null || monitoringWorkflowActionTreeItem.Item.ExecuteChildrenActionOnFailure == true)
            {
                List<Thread> childrenThreads = new List<Thread>();
                foreach (TreeItem<MonitoringWorkflowAction> c in monitoringWorkflowActionTreeItem.Children)
                {
                    if (monitoringWorkflowActionTreeItem.Item.GroupChildren == true)
                    {

                        if (c.Item.monitoringActionType == MonitoringActionType.ApplicationPool)
                        {
                            if (applicationPoolCollection == null)
                            {
                                ServerManager serverManager = null;
                                try
                                {
                                    serverManager = ServerManager.OpenRemote(c.Item.ServerName);
                                    applicationPoolCollection = serverManager.ApplicationPools;
                                }
                                catch { }
                                finally
                                {
                                    serverManager = null;
                                }
                            }
                        }
                    }
                    Thread actionThread = new Thread(p => RunSingleAction(c, ProcessID, ConnectionString, emailFrom, emailTo, emailSMTPServer, MainWorkflowID, ExecutedBy, applicationPoolCollection));
                    actionThread.Start();
                    childrenThreads.Add(actionThread);
                }

                foreach(Thread thread in childrenThreads)
                {
                    thread.Join();
                }
            }

            if (monitoringWorkflowActionTreeItem.Item.monitoringActionType == MonitoringActionType.Workflow)
            {
                List<MonitoringActionInfo> currentProcessMonitoringActionInErrorList = new List<MonitoringActionInfo>();
                try
                {
                    var currentProcessMonitoringActionInErrorListEnum = MonitoringActionList.Where(a => ((a.processID == ProcessID) && !a.Passed && a.MainWorkflowID == (monitoringWorkflowActionTreeItem.Item.LinkedActionID ?? 0)));
                    if (currentProcessMonitoringActionInErrorListEnum != null)
                    {
                        currentProcessMonitoringActionInErrorList = currentProcessMonitoringActionInErrorListEnum.ToList();
                    }
                    var currentProcessActionsEnum = MonitoringActionList.Where(a => a.processID == ProcessID && a.MainWorkflowID == (monitoringWorkflowActionTreeItem.Item.LinkedActionID ?? 0));
                    if (currentProcessActionsEnum != null)
                    {
                        CheckRemainingActions(ProcessID, currentProcessActionsEnum.ToList(), ConnectionString, emailFrom, emailTo, emailSMTPServer, ExecutedBy);
                    }
                }
                catch(Exception ex)
                {
                    SAPRequester.SendErrorEmail(ex, emailFrom, emailTo, emailSMTPServer, "B2B Messages Monitoring Service catched an error: ");
                }
                UpdateLog(MonitoringLogType.MainWorkflow, (int)monitoringWorkflowActionTreeItem.Item.LinkedActionID, ProcessID, ConnectionString, emailFrom, emailTo, emailSMTPServer, ExecutedBy, (currentProcessMonitoringActionInErrorList.Count > 0) ? currentProcessMonitoringActionInErrorList.Take(1).Single().error : null, currentProcessMonitoringActionInErrorList.Count > 0 ? "Error" : "Success", currentProcessMonitoringActionInErrorList.Count == 0);
            }
        }

        public static void RunActionManually(ref string OutputValue,ref Exception error,MonitoringWorkflowAction monitoringWorkflowAction, string ConnectionString, string emailFrom, string emailTo, string emailSMTPServer,ref decimal? resultRatio, ApplicationPoolCollection applicationPoolCollection = null)
        {
            switch (monitoringWorkflowAction.monitoringActionType)
            {
                case MonitoringActionType.ApplicationPool: error = CheckApplicationPool(monitoringWorkflowAction.ServerName, monitoringWorkflowAction.InputParameter, ref OutputValue, applicationPoolCollection); break;
                case MonitoringActionType.WindowService: error = CheckWindowsService(monitoringWorkflowAction.ServerName, monitoringWorkflowAction.InputParameter, ref OutputValue); break;
                case MonitoringActionType.PingURL: error = PingUrl(monitoringWorkflowAction.InputParameter, ref OutputValue); break;
                case MonitoringActionType.SQLQuery: error = RunSQLQuery(monitoringWorkflowAction.InputParameter, monitoringWorkflowAction.AlertRatio, ConnectionString, emailFrom, emailTo, emailSMTPServer, ref OutputValue,ref resultRatio); break;
            }
        }

        private static Exception RunSQLQuery(string inputParameter, decimal? alertRatio, string connectionString, string emailFrom, string emailTo, string emailSMTPServer, ref string outputValue, ref decimal? resultRatio)
        {
            try
            {
                Regex Reg = new Regex(@"\sDROP\s|^DROP\s|\sUPDATE\s|^UPDATE\s|\sUSE\s|^USE\s|\sINSERT\s|^INSERT\s|\sTRUNCATE\s|^TRUNCATE\s|^DELETE\s|\sDELETE\s", RegexOptions.None);
                bool isValid = true;
                if (
                (Reg.IsMatch(inputParameter.ToUpper())) ||
                (inputParameter.ToUpper().IndexOf("--") > 0))
                {
                    isValid = false;
                }
                if (isValid)
                {
                    using (SqlConnection cnx = new SqlConnection(connectionString))
                    {
                        DataTable dataTable = new DataTable();
                        SqlCommand cmd = new SqlCommand(inputParameter, cnx);
                        cmd.CommandTimeout = 3600;
                        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
                        sqlDataAdapter.Fill(dataTable);
                        if (dataTable.Rows.Count == 1)
                        {
                            if (dataTable.Columns.Contains("Ratio"))
                            {
                                decimal ratio = 0;
                                outputValue = "";
                                decimal.TryParse(dataTable.Rows[0]["Ratio"].ToString(), out ratio);
                                resultRatio = ratio;
                                foreach (DataColumn col in dataTable.Columns)
                                {
                                    outputValue += (string.IsNullOrEmpty(outputValue) ? "" : "</br>") + col.ColumnName.Replace("_", " ") + ":&nbsp;<b>" + dataTable.Rows[0][col.ColumnName].ToString() + (col.ColumnName.ToLower().Contains("ratio") ? " %" : "") + "</b>";
                                }
                                outputValue = "<div style='text-align:left !important'>" + outputValue + "</div>";

                                if (ratio < alertRatio)
                                {
                                    return new Exception(string.Format("Your ratio {0} is lower than the alert ratio {1} ", ratio, alertRatio));
                                }
                            }
                            else
                            {
                                outputValue = "Error";
                                return new Exception("The column name Ratio doesn't exist, please ensure to add it in your query");
                            }
                        }
                        else
                        {
                            if (dataTable.Rows.Count == 0)
                            {
                                outputValue = "Query returned 0 line";
                                return new Exception("Your query didn't return any result");
                            }
                            else
                            {
                                outputValue = string.Format("Query returned {0} line", dataTable.Rows.Count);
                                return new Exception("Your query returned more than one ligne, please amend it");
                            }
                        }
                    }
                }
                else
                {
                    outputValue = "Dangerous query rejected by the system";
                    return new Exception("Your query has possible dangerous action operations, please amend it");
                }
            }
            catch (Exception ex)
            {
                return ex;
            }
            return null; // return null if success
        }

        private static Exception PingUrl(string inputParameter, ref string outputValue)
        {
            try
            {
                Uri uri = new Uri(inputParameter);

                Ping ping = new Ping();
                PingReply pingReply = ping.Send(uri.Host);
                outputValue = pingReply.Status.ToString();
                if (pingReply.Status != IPStatus.Success)
                {
                    // Website is not available.
                    return new Exception(string.Format("{0} is not available", inputParameter));
                }
            }
            catch (Exception ex)
            {
                outputValue = outputValue ?? "Error";
                return ex;
            }
            return null; // return null if success
        }

        private static Exception CheckWindowsService(string serverName, string inputParameter,ref string outputValue)
        {
            Exception error = null;
            try
            {
                ServiceController service = new ServiceController(inputParameter, serverName);
                outputValue = service.Status.ToString();
                if (service.Status.ToString() != "Running")
                {
                    error = new Exception("Windows Service " + inputParameter + " is " + service.Status.ToString() + " on server " + serverName);
                }
            }
            catch (Exception ex)
            {
                outputValue = outputValue ?? "Error";
                error = ex;
            }
            return error;
        }

        private static Exception CheckApplicationPool(string serverName, string inputParameter, ref string outputValue, ApplicationPoolCollection applicationPools = null)
        {
            Exception error = null;
            try
            {
                ApplicationPool pool = null;
                if (applicationPools != null)
                {
                    pool = applicationPools.Where(f => f.Name == inputParameter).SingleOrDefault();
                    applicationPools = null;
                }
                else
                {
                    using (ServerManager serverManagerLocal = ServerManager.OpenRemote(serverName))
                    {
                        pool = serverManagerLocal.ApplicationPools.Where(f => f.Name == inputParameter).SingleOrDefault();
                    }
                }

                if (pool == null)
                {
                    error = new Exception("Application pool " + inputParameter + " doesn't exist in server " + serverName);
                }
                else
                {
                    outputValue = pool.State.ToString();
                    if (pool.State != ObjectState.Started)
                    {
                        error = new Exception("Application pool " + inputParameter + " has the state [" + pool.State.ToString() + "] in server " + serverName);
                    }
                }

            }
            catch (Exception ex)
            {
                outputValue = outputValue??"Error";
                error = ex;
            }
            return error;
        }

        static void UpdateLog(MonitoringLogType logType, int Id, Guid processId, string ConnectionString, string emailFrom, string emailTo, string emailSMTPServer, int? ExecutedBy = null, Exception ex = null, string value = null, bool? passed = null,decimal? resultRatio = null)
        {
            try
            {
                using (SqlConnection cnx = new SqlConnection(ConnectionString))
                {
                    DataSet dataSet = new DataSet();
                    SqlCommand cmd = new SqlCommand(string.Format("Monitoring.{0}", logType == MonitoringLogType.Action ? "LogWorkflowExecutionAction" : "LogWorkflowExecution"), cnx);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 3600;
                    cmd.Parameters.Add(new SqlParameter(logType == MonitoringLogType.Action ? "@WorkflowActionID" : "@WorkflowID", Id));
                    cmd.Parameters.Add(new SqlParameter("@ProcessID", processId));
                    if (logType == MonitoringLogType.MainWorkflow)
                    {
                        cmd.Parameters.Add(new SqlParameter("@ExecutedBy", ExecutedBy));
                    }
                    else
                    {
                        cmd.Parameters.Add(new SqlParameter("@ResultRatio", resultRatio));
                    }
                    if (passed != null)
                    {
                        cmd.Parameters.Add(new SqlParameter("@ErrorMessage", ex != null ? (ex.Message ?? "An unhadled error was detected") + " " + (ex.StackTrace ?? "") : null));
                        cmd.Parameters.Add(new SqlParameter(logType == MonitoringLogType.MainWorkflow ? "@OverallStatus" : "@OutputValue", value));
                        cmd.Parameters.Add(new SqlParameter("@Passed", passed));
                    }
                    cnx.Open();
                    cmd.ExecuteNonQuery();
                    cnx.Close();
                }
            }
            catch (Exception error)
            {
                SAPRequester.SendErrorEmail(error, emailFrom, emailTo, emailSMTPServer, "B2B Messages Monitoring Service catched an error: ");
            }
        }

        public static List<MonitoringWorkflow> GetWorkflowsByEnivronmentID(string ConnectionString, string emailFrom, string emailTo, string emailSMTPServer, int EnivronmentID)
        {
            List<MonitoringWorkflow> monitoringWorkflowList = new List<MonitoringWorkflow>();

            try
            {
                using (SqlConnection cnx = new SqlConnection(ConnectionString))
                {
                    DataTable dataTable = new DataTable();
                    SqlCommand cmd = new SqlCommand("Monitoring.GetWorkflowsByEnivronmentID", cnx);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 3600;
                    cmd.Parameters.Add(new SqlParameter("@EnvironmentID", EnivronmentID));
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
                    sqlDataAdapter.Fill(dataTable);
                    if (dataTable.Rows.Count > 0)
                    {   
                        foreach(DataRow row in dataTable.Rows)
                        {
                            MonitoringWorkflow monitoringWorkflow = new MonitoringWorkflow();
                            monitoringWorkflow.ID = (int)row["WorkflowID"];
                            FillWorfklowFromRow(row, ref monitoringWorkflow);
                            monitoringWorkflow.StartedOn = row.GetValue<DateTime>("StartedOn");
                            monitoringWorkflow.FinishedOn = row.GetValue<DateTime>("FinishedOn");
                            monitoringWorkflow.Passed = row.GetValue<bool>("Passed");
                            monitoringWorkflow.OverallStatus = row.GetText("OverallStatus");
                            monitoringWorkflow.ErrorMessage = row.GetText("ErrorMessage");
                            monitoringWorkflowList.Add(monitoringWorkflow);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SAPRequester.SendErrorEmail(ex, emailFrom, emailTo, emailSMTPServer, "GetWorkflowByEnivronmentID catched an error: ");
            }

            return monitoringWorkflowList;
        }

        public static List<MonitoringActionTypeInfo> GetMonitoringActionTypes(string ConnectionString, string emailFrom, string emailTo, string emailSMTPServer)
        {
            List<MonitoringActionTypeInfo> monitoringActionTypes = new List<MonitoringActionTypeInfo>();

            try
            {
                using (SqlConnection cnx = new SqlConnection(ConnectionString))
                {
                    DataTable dataTable = new DataTable();
                    SqlCommand cmd = new SqlCommand("Monitoring.GetActionTypes", cnx);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 3600;
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
                    sqlDataAdapter.Fill(dataTable);
                    if (dataTable.Rows.Count > 0)
                    {
                        foreach (DataRow row in dataTable.Rows)
                        {
                            MonitoringActionTypeInfo monitoringActionType = new MonitoringActionTypeInfo();
                            monitoringActionType.ActionTypeID = (int)row["ActionTypeID"];
                            monitoringActionType.Name = row["Name"].ToString();
                            monitoringActionType.Description = row["Description"].ToString();
                            monitoringActionType.DefaultAlertRatio = row.GetValue<decimal>("DefaultAlertRatio");
                            monitoringActionType.DefaultWarningRatio = row.GetValue<decimal>("DefaultWarningRatio");
                            monitoringActionType.DefaultImageName = row["DefaultImageName"].ToString();
                            monitoringActionType.RequiresConfigurationRatio = (bool)row["RequiresConfigurationRatio"];
                            monitoringActionTypes.Add(monitoringActionType);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SAPRequester.SendErrorEmail(ex, emailFrom, emailTo, emailSMTPServer, "GetMonitoringActionTypes catched an error: ");
            }

            return monitoringActionTypes;
        }

        public static MonitoringWorkflow GetWorkflowByID(int WorkflowID, string ConnectionString, string emailFrom, string emailTo, string emailSMTPServer, int? WorkflowActionID = null,Guid? ProcessID = null,int? ParentID = null)
        {
            MonitoringWorkflow monitoringWorkflow = new MonitoringWorkflow();

            try
            {
                using (SqlConnection cnx = new SqlConnection(ConnectionString))
                {
                    DataSet dataSet = new DataSet();
                    SqlCommand cmd = new SqlCommand("Monitoring.GetWorkflowDetailsByID", cnx);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 3600;
                    cmd.Parameters.Add(new SqlParameter("@WorkflowID", WorkflowID));
                    cmd.Parameters.Add(new SqlParameter("@ProcessID", ProcessID));
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
                    sqlDataAdapter.Fill(dataSet);
                    if (dataSet.Tables.Count == 2)
                    {
                        monitoringWorkflow.ID = WorkflowID;
                        if (dataSet.Tables[0].Rows.Count == 1)
                        {
                            DataRow row = dataSet.Tables[0].Rows[0];
                            monitoringWorkflow.LastModifiedBy = row.GetText("LastModifiedBy");
                            monitoringWorkflow.CreatedBy = row.GetText("CreatedBy");
                            FillWorfklowFromRow(row,ref monitoringWorkflow);

                            if (ProcessID != null)
                            {
                                // Fill execution info if applicable
                                monitoringWorkflow.StartedOn = row.GetValue<DateTime>("StartedOn");
                                monitoringWorkflow.FinishedOn = row.GetValue<DateTime>("FinishedOn");
                                monitoringWorkflow.Passed = row.GetValue<bool>("Passed");
                                monitoringWorkflow.OverallStatus = row.GetText("OverallStatus");
                                monitoringWorkflow.ErrorMessage = row.GetText("ErrorMessage");
                            }
                        }

                        monitoringWorkflow.actions = GetWorkflowActionsFromDataRows(dataSet.Tables[1].Rows, monitoringWorkflow, ConnectionString, emailFrom, emailTo, emailSMTPServer, WorkflowActionID, ProcessID,ParentID);
                    }
                }
            }
            catch (Exception ex)
            {
                SAPRequester.SendErrorEmail(ex, emailFrom, emailTo, emailSMTPServer, "GetWorkflowByID catched an error: ");
            }

            return monitoringWorkflow;
        }

        private static void FillWorfklowFromRow(DataRow row,ref MonitoringWorkflow monitoringWorkflow)
        {
            monitoringWorkflow.Name = row.GetText("Name");
            monitoringWorkflow.EnvironmentID = (int)row["EnvironmentID"];
            monitoringWorkflow.CreatedOn = (DateTime)row["CreatedOn"];
            monitoringWorkflow.LastModifiedOn = row.GetValue<DateTime>("LastModifiedOn");                        
            monitoringWorkflow.WorkflowUID = (Guid)row["WorkflowUID"];
            monitoringWorkflow.MonitoringEnabled = (bool)row["MonitoringEnabled"];
            monitoringWorkflow.MonitoringIntervalInSeconds = (int)row["MonitoringIntervalInSeconds"];
            monitoringWorkflow.ActivateAlerts = (bool)row["ActivateAlerts"];
            monitoringWorkflow.SendAlertsEmailTo = row.GetText("SendAlertsEmailTo");
            monitoringWorkflow.ActivateWarningNotifications = (bool)row["ActivateWarningNotifications"];
            monitoringWorkflow.SendWarningNotificationsEmailTo = row.GetText("SendWarningNotificationsEmailTo");
            monitoringWorkflow.SendDailyReport = (bool)row["SendDailyReport"];
            monitoringWorkflow.DailyReportHour = (int)row["DailyReportHour"];
            monitoringWorkflow.DailyReportMinute = (int)row["DailyReportMinute"];
            monitoringWorkflow.DailyReportEmailTo = row.GetText("DailyReportEmailTo");
            monitoringWorkflow.LastReportSentOn = row.GetValue<DateTime>("LastReportSentOn");
        }

        private static List<MonitoringWorkflowAction> GetWorkflowActionsFromDataRows(DataRowCollection rows, MonitoringWorkflow monitoringWorkflow, string ConnectionString, string emailFrom, string emailTo, string emailSMTPServer, int? WorkflowActionID = null, Guid? ProcessID = null,int? ParentID = null)
        {
            List<MonitoringWorkflowAction> list = new List<MonitoringWorkflowAction>();            
            bool GroupChildren = true;
            foreach (DataRow row in rows)
            {
                MonitoringActionType currentRowType = (MonitoringActionType)((int)row["ActionTypeID"]);
                if (currentRowType != MonitoringActionType.Workflow)
                {
                    MonitoringWorkflowAction monitoringWorkflowAction = new MonitoringWorkflowAction();
                    monitoringWorkflowAction.ID = (int)row["WorkflowActionID"];
                    monitoringWorkflowAction.ParentID = (row["ParentWorkflowActionID"] == DBNull.Value) ? WorkflowActionID ?? monitoringWorkflow.ID + SECURE_WORKFLOW_PARENT_VALUE : (int)row["ParentWorkflowActionID"];
                    monitoringWorkflowAction.Name = row.GetText("Name");
                    monitoringWorkflowAction.Description = row.GetText("Description");
                    monitoringWorkflowAction.GroupChildren = row.GetValue<bool>("GroupChildren");
                    monitoringWorkflowAction.ExecuteChildrenActionOnFailure = row.GetValue<bool>("ExecuteChildrenActionOnFailure");
                    FillActionDetailsFromDataRow(row, ref monitoringWorkflowAction);

                    if (ProcessID != null)
                    {
                        // Fill execution info if applicable
                        monitoringWorkflowAction.StartedOn = row.GetValue<DateTime>("StartedOn");
                        monitoringWorkflowAction.FinishedOn = row.GetValue<DateTime>("FinishedOn");
                        monitoringWorkflowAction.Passed = row.GetValue<bool>("Passed");
                        monitoringWorkflowAction.OverallStatus = row.GetText("OverallStatus");
                        monitoringWorkflowAction.ErrorMessage = row.GetText("ErrorMessage");
                        monitoringWorkflowAction.OutputValue = row.GetText("OutputValue");
                        monitoringWorkflowAction.ResultRatio = row.GetValue<decimal>("ResultRatio");
                    }

                    if(row["ParentWorkflowActionID"] != DBNull.Value)
                    {
                        GroupChildren = false;
                    }

                    list.Add(monitoringWorkflowAction);
                }
                else
                {
                    GroupChildren = false;
                    list.AddRange(GetWorkflowByID((int)row["ActionID"], ConnectionString, emailFrom, emailTo, emailSMTPServer, (int)row["WorkflowActionID"],ProcessID, (row["ParentWorkflowActionID"] == DBNull.Value) ? WorkflowActionID ?? monitoringWorkflow.ID + SECURE_WORKFLOW_PARENT_VALUE : (int)row["ParentWorkflowActionID"]).actions);
                }
            }

            list.Add(new MonitoringWorkflowAction()
            {
                ID = WorkflowActionID ?? monitoringWorkflow.ID + SECURE_WORKFLOW_PARENT_VALUE,
                LinkedActionID = ParentID == null ? WorkflowActionID ?? monitoringWorkflow.ID : monitoringWorkflow.ID,
                Comments = monitoringWorkflow.Name,
                GroupChildren = GroupChildren,
                monitoringActionType = MonitoringActionType.Workflow,
                StartedOn = monitoringWorkflow.StartedOn,
                FinishedOn = monitoringWorkflow.FinishedOn,
                Passed = monitoringWorkflow.Passed,
                ErrorMessage = monitoringWorkflow.ErrorMessage,
                OverallStatus = monitoringWorkflow.OverallStatus,
                ParentID = ParentID
            });
            return list;
        }

        public static MonitoringWorkflowAction GetActionByID(int ActionID, string ConnectionString, string emailFrom, string emailTo, string emailSMTPServer)
        {
            MonitoringWorkflowAction monitoringWorkflowAction = new MonitoringWorkflowAction();

            try
            {
                using (SqlConnection cnx = new SqlConnection(ConnectionString))
                {
                    DataTable datatable = new DataTable();
                    SqlCommand cmd = new SqlCommand("Monitoring.GetActionByID", cnx);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 3600;
                    cmd.Parameters.Add(new SqlParameter("@ActionID", ActionID));
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
                    sqlDataAdapter.Fill(datatable);
                    if (datatable.Rows.Count == 1)
                    {
                        FillActionDetailsFromDataRow(datatable.Rows[0], ref monitoringWorkflowAction, true);
                    }
                }
            }
            catch (Exception ex)
            {
                SAPRequester.SendErrorEmail(ex, emailFrom, emailTo, emailSMTPServer, "GetActionByID catched an error: ");
            }

            return monitoringWorkflowAction;
        }

        private static void FillActionDetailsFromDataRow(DataRow row,ref MonitoringWorkflowAction monitoringWorkflowAction,bool UseLinkedActionIDAsID = true)
        {
            if (UseLinkedActionIDAsID)
            {
                monitoringWorkflowAction.LinkedActionID = row.GetValue<int>("ActionID");
            }
            else
            {
                monitoringWorkflowAction.ID = (int)row["ActionID"];
            }
            monitoringWorkflowAction.monitoringActionType = (MonitoringActionType)((int)row["ActionTypeID"]);
            monitoringWorkflowAction.InputParameter = row.GetText("InputParameter");
            monitoringWorkflowAction.ServerName = row.GetText("ServerName");
            monitoringWorkflowAction.Comments = row.GetText("Comments");
            monitoringWorkflowAction.CustomImageUrl = row.GetText("CustomImageUrl");
            monitoringWorkflowAction.AlertRatio = row.GetValue<decimal>("AlertRatio");
            monitoringWorkflowAction.WarningRatio = row.GetValue<decimal>("WarningRatio");
            if (row.Table.Columns.Contains("EnvironmentID"))
            {
                monitoringWorkflowAction.EnvironmentID = (int)row["EnvironmentID"];
            }
        }

        public static string GetText(this DataRow row, string columnName)
        {
            if (row.IsNull(columnName))
            {
                return string.Empty;
            }

            return row[columnName] as string ?? string.Empty;
        }

        public static T? GetValue<T>(this DataRow row, string columnName) where T : struct
        {
            if (row.IsNull(columnName))
            {
                return null;
            }

            return row[columnName] as T?;
        }

        public class TreeItem<T>
        {
            public T Item { get; set; }
            public IEnumerable<TreeItem<T>> Children { get; set; }
        }

        public static IEnumerable<TreeItem<T>> GenerateTree<T, K>(IEnumerable<T> collection, Func<T, K> id_selector, Func<T, K> parent_id_selector, K root_id = default(K))
        {
            foreach (var c in collection.Where(fc => parent_id_selector(fc).Equals(root_id)))
            {
                yield return new TreeItem<T>()
                {
                    Item = c,
                    Children = GenerateTree(collection, id_selector, parent_id_selector, id_selector(c))
                };
            }
        }
    }
}
