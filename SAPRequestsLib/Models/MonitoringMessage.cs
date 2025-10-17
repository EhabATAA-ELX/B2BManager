using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAPRequestsLib.Models
{
    public class MonitoringMessage
    {
        public MonitoringMessage(Guid messageID, Guid c_GLOBALID, bool activatePerformanceDegradationAlerts, string dailyReportEmailTo, int? dailyReportHour, int? dailyReportMinute, int expectedResponseTimeInMilliseconds, bool isActive, int messageIntervalInSeconds, string messageXML, bool sendDailyReport, int? performanceDegradationAlertMessagesCount, Guid sessionID, string wcfB2BWebServiceURL, string wcfMethodName, string wcfPassword, string wcfUserName, int worstAcceptableResponseTimeInMilliseconds,DateTime? lastReportSentOn)
        {
            MessageID = messageID;
            C_GLOBALID = c_GLOBALID;
            ActivatePerformanceDegradationAlerts = activatePerformanceDegradationAlerts;
            DailyReportEmailTo = dailyReportEmailTo;
            DailyReportHour = dailyReportHour;
            DailyReportMinute = dailyReportMinute;
            ExpectedResponseTimeInMilliseconds = expectedResponseTimeInMilliseconds;
            IsActive = isActive;
            MessageIntervalInSeconds = messageIntervalInSeconds;
            MessageXML = messageXML;
            SendDailyReport = sendDailyReport;
            PerformanceDegradationAlertMessagesCount = performanceDegradationAlertMessagesCount;
            SessionID = sessionID;
            WcfB2BWebServiceURL = wcfB2BWebServiceURL;
            WcfMethodName = wcfMethodName;
            WcfPassword = wcfPassword;
            WcfUserName = wcfUserName;
            WorstAcceptableResponseTimeInMilliseconds = worstAcceptableResponseTimeInMilliseconds;
            LastReportSentOn = lastReportSentOn;
        }

        public Guid MessageID { get; set; }
        public Guid C_GLOBALID { get; set; }
        public bool ActivatePerformanceDegradationAlerts { get; set; }
        public string DailyReportEmailTo { get; set; }
        public int? DailyReportHour { get; set; }
        public int? DailyReportMinute { get; set; }
        public int ExpectedResponseTimeInMilliseconds { get; set; }
        public bool IsActive { get; set; }
        public int MessageIntervalInSeconds { get; set; }
        public string MessageXML { get; set; }
        public bool SendDailyReport { get; set; }
        public int? PerformanceDegradationAlertMessagesCount { get; set; }
        public Guid SessionID { get; set; }
        public string WcfB2BWebServiceURL { get; set; }
        public string WcfMethodName { get; set; }
        public string WcfPassword { get; set; }
        public string WcfUserName { get; set; }
        public int WorstAcceptableResponseTimeInMilliseconds { get; set; }
        public DateTime? LastReportSentOn { get; set; }
        public List<MonitoringMessagePauseInterval> pauseIntervals { get; set; }
    }
}
