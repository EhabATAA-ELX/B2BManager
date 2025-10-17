using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SAPRequestsLib.MonitoringWorkflowManager;

namespace SAPRequestsLib.Models
{
    public class MonitoringActionTypeInfo
    {
        public int ActionTypeID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public decimal? DefaultWarningRatio { get; set; }
        public decimal? DefaultAlertRatio { get; set; }

        public string DefaultImageName { get; set; }
        public bool RequiresConfigurationRatio { get; set; }
        public MonitoringActionType monitoringActionType { get { return (MonitoringActionType)ActionTypeID; } }

    }
}
