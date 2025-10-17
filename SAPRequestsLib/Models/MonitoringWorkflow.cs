using System;
using System.Collections.Generic;
using static SAPRequestsLib.MonitoringWorkflowManager;

namespace SAPRequestsLib.Models
{
    public class MonitoringWorkflow : MonitoringWorkflowExcutionInfo
    {
        public Guid WorkflowUID { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public int EnvironmentID { get; set; }
        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public string LastModifiedBy { get; set; }
        public string LastModifiedOnFormatted { get { return LastModifiedOn == null ? "--" : ((DateTime)LastModifiedOn).ToString("dd/MM/yyyy HH:mm"); } }
        public Guid ProcessID { get; set; }
        public bool MonitoringEnabled { get; set; }
        public int MonitoringIntervalInSeconds { get; set; }
        public bool ActivateAlerts { get; set; }
        public string SendAlertsEmailTo { get; set; }
        public bool ActivateWarningNotifications { get; set; }
        public string SendWarningNotificationsEmailTo { get; set; }
        public bool SendDailyReport { get; set; }
        public int DailyReportHour { get; set; }
        public int DailyReportMinute { get; set; }
        public string DailyReportEmailTo { get; set; }
        public DateTime? LastReportSentOn { get; set; }
        public List<MonitoringWorkflowAction> actions { get; set; }

        public IEnumerable<TreeItem<MonitoringWorkflowAction>> GetTree(int? rootID)
        {
            return GenerateTree(actions, c => c.ID, c => c.ParentID, rootID);
        }

    }
}