using System;

namespace SAPRequestsLib.Models
{
    public class MonitoringWorkflowAction : MonitoringWorkflowExcutionInfo
    {
        public int ID { get; set; }

        public int? ParentID { get; set; }

        public int EnvironmentID { get; set; }

        public int? LinkedActionID { get; set; }
        public string Comments { get; set; }
        public string ServerName { get; set; }
        public string InputParameter { get; set; }
        public string CustomImageUrl { get; set; }
        public decimal? WarningRatio { get; set; }
        public decimal? AlertRatio { get; set; }

        public decimal? ResultRatio { get; set; }
        public bool? GroupChildren { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        
        public string OutputValue { get; set; }
        public bool? ExecuteChildrenActionOnFailure { get; set; }

        public string ComparableStatus { get; set; }
        public string ProcessingClassName
        {
            get
            {
                switch (Passed)
                {
                    case false:
                        {
                            return "error";
                        }
                    default:
                        {
                            if (Passed == null)
                            {
                                return "inprogress";
                            }
                            else
                            {
                                if (monitoringActionType == MonitoringWorkflowManager.MonitoringActionType.SQLQuery)
                                {
                                    if (ResultRatio != null)
                                    {
                                        if (ResultRatio > 0)
                                        {
                                            if (ResultRatio <= WarningRatio)
                                            {
                                                return "warning";
                                            }
                                        }
                                    }
                                }
                                return "success";
                            }
                        }                    
                }
            }
        }

        public int LogDisplayPriority
        {
            get
            {
                switch (Passed)
                {
                    case false:
                        {
                            return 2;
                        }
                    default:
                        {
                            if (Passed == null)
                            {
                                return 0;
                            }
                            else
                            {
                                if (monitoringActionType == MonitoringWorkflowManager.MonitoringActionType.SQLQuery)
                                {
                                    if (ResultRatio != null)
                                    {
                                        if (ResultRatio > 0)
                                        {
                                            if (ResultRatio <= WarningRatio)
                                            {
                                                return 1;
                                            }
                                        }
                                    }
                                }
                                return 0;
                            }
                        }
                }
            }
        }

        public MonitoringWorkflowManager.MonitoringActionType monitoringActionType { get; set; }

    }
}