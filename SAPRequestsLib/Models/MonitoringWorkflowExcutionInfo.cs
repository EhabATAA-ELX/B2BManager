using System;

namespace SAPRequestsLib.Models
{
    public class MonitoringWorkflowExcutionInfo
    {
        public DateTime? StartedOn { get; set; }
        public DateTime? FinishedOn { get; set; }

        public bool? Passed { get; set; }

        public string ErrorMessage { get; set; }
        
        public string OverallStatus { get; set; }
    }
}