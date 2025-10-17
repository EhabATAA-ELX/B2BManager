using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAPRequestsLib.Models
{
    [Serializable]
    public class MonitoringMessagePauseInterval
    {
        public MonitoringMessagePauseInterval()
        {

        }
        public MonitoringMessagePauseInterval(int pauseIntervalID,int startPauseTimeHour, int startPauseTimeMinute, int endPauseTimeHour, int endPauseTimeMinute, bool occursEveryDay, DateTime? startDayOfOccurence, DateTime? endDayOfOccurence)
        {
            PauseIntervalID = pauseIntervalID;
            StartPauseTimeHour = startPauseTimeHour;
            StartPauseTimeMinute = startPauseTimeMinute;
            EndPauseTimeHour = endPauseTimeHour;
            EndPauseTimeMinute = endPauseTimeMinute;
            OccursEveryDay = occursEveryDay;
            StartDayOfOccurence = startDayOfOccurence;
            EndDayOfOccurence = endDayOfOccurence;
        }        

        public int PauseIntervalID { get; set; }
        public int StartPauseTimeHour { get; set; }
        public int StartPauseTimeMinute { get; set; }
        public int EndPauseTimeHour { get; set; }
        public int EndPauseTimeMinute { get; set; }
        public bool OccursEveryDay { get; set; }
        public DateTime? StartDayOfOccurence { get; set; }
        public DateTime? EndDayOfOccurence { get; set; }
        public string StartDayOfOccurenceFormatted
        {
            get
            {
                if(StartDayOfOccurence != null)
                {
                    return ((DateTime) StartDayOfOccurence).ToString("dd/MM/yyyy");
                }
                else
                {
                    return "";
                }
            }
        }

        public string EndDayOfOccurenceFormatted
        {
            get
            {
                if (EndDayOfOccurence != null)
                {
                    return ((DateTime)EndDayOfOccurence).ToString("dd/MM/yyyy");
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
