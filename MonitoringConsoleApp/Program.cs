using SAPRequestsLib;
using System;
using System.Linq;
using System.Configuration;
using SAPRequestsLib.Models;
using System.Threading;
using System.Collections.Generic;

namespace MonitoringConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            List<MonitoringMessage> monitoringMessages = new List<MonitoringMessage>();
            while (true)
            {
                foreach (MonitoringMessage messageToMonitor in MonitoringManager.FillMessageToMonitor(ConfigurationManager.ConnectionStrings["LogDB"].ConnectionString, ConfigurationManager.AppSettings["EmailFrom"], ConfigurationManager.AppSettings["EmailTo"], ConfigurationManager.AppSettings["EmailSMTPServer"]))
                {
                    if (monitoringMessages.Where(p => p.MessageID == messageToMonitor.MessageID).Count() == 0)
                    {
                        monitoringMessages.Add(messageToMonitor);
                        Thread thread = new Thread(t => MonitoringManager.MonitorMessage(messageToMonitor, ConfigurationManager.ConnectionStrings["LogDB"].ConnectionString, ConfigurationManager.AppSettings["EmailFrom"], ConfigurationManager.AppSettings["EmailTo"], ConfigurationManager.AppSettings["EmailSMTPServer"]));
                        thread.Start();
                    }
                }
                Thread.Sleep(60000);
            }
        }

        private static void RunSingleMonitoring()
        {
            string xmlFilePath = @"..\..\XML\Request_GET_PRICE_Monitoring.xml";
            if (ConfigurationManager.AppSettings["xmlFilePath"] != null)
            {
                xmlFilePath = ConfigurationManager.AppSettings["xmlFilePath"];
            }

            B2BMessagesMonitoringManager.RunMonitoring(xmlFilePath,
                                                    ConfigurationManager.AppSettings["wcfB2BWebServiceURL"],
                                                    ConfigurationManager.AppSettings["wcfMethodName"],
                                                    ConfigurationManager.AppSettings["wcfUserName"],
                                                    ConfigurationManager.AppSettings["wcfPassword"],
                                                    ConfigurationManager.AppSettings["globalId"],
                                                    ConfigurationManager.AppSettings["sessionId"],
                                                    ConfigurationManager.AppSettings["EmailFrom"],
                                                    ConfigurationManager.AppSettings["EmailTo"],
                                                    ConfigurationManager.AppSettings["EmailSMTPServer"],
                                                    int.Parse(ConfigurationManager.AppSettings["MessageIntervalInSeconds"]),
                                                    bool.Parse(ConfigurationManager.AppSettings["SendReport"]),
                                                    int.Parse(ConfigurationManager.AppSettings["ReportHour"].ToString()),
                                                    int.Parse(ConfigurationManager.AppSettings["ReportMinute"].ToString()),
                                                    ConfigurationManager.AppSettings["ReportEmailTo"]
                                                    );
        }

        public static void SendErrorEmail(Exception ex)
        {
            SAPRequester.SendErrorEmail(ex, ConfigurationManager.AppSettings["EmailFrom"], ConfigurationManager.AppSettings["EmailTo"], ConfigurationManager.AppSettings["EmailSMTPServer"], "B2B Messages Monitoring Service catched an error: ");
        }

    }
}
