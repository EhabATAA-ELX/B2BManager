using SAPRequestsLib;
using SAPRequestsLib.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace B2BMessagesMonitoringService
{
    public partial class B2BMessagesMonitoringService : ServiceBase
    {
        public B2BMessagesMonitoringService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Thread monitoringThread = new Thread(p => RunMonitoringMessagesThread());
                monitoringThread.Start();
                Thread monitoringWorkflowsThread = new Thread(p => RunMonitoringWorkflowsThread());
                monitoringWorkflowsThread.Start();
            }
            catch(Exception ex)
            {
                SendErrorEmail(ex);
                throw ex;
            }
        }

        public static void SendErrorEmail(Exception ex)
        {
            SAPRequester.SendErrorEmail(ex, ConfigurationManager.AppSettings["EmailFrom"], ConfigurationManager.AppSettings["EmailTo"], ConfigurationManager.AppSettings["EmailSMTPServer"], "B2B Messages Monitoring Service catched an error: ");
        }

        protected void RunMonitoringMessagesThread()
        {
            List<MonitoringMessage> monitoringMessages = new List<MonitoringMessage>();
            while (true)
            {
                Thread thread = null;
                try
                {
                    
                    foreach (MonitoringMessage messageToMonitor in MonitoringManager.FillMessageToMonitor(ConfigurationManager.ConnectionStrings["LogDB"].ConnectionString, ConfigurationManager.AppSettings["EmailFrom"], ConfigurationManager.AppSettings["EmailTo"], ConfigurationManager.AppSettings["EmailSMTPServer"]))
                    {
                        if (monitoringMessages.Where(p => p.MessageID == messageToMonitor.MessageID).Count() == 0)
                        {
                            monitoringMessages.Add(messageToMonitor);
                            thread = new Thread(t => MonitoringManager.MonitorMessage(messageToMonitor, ConfigurationManager.ConnectionStrings["LogDB"].ConnectionString, ConfigurationManager.AppSettings["EmailFrom"], ConfigurationManager.AppSettings["EmailTo"], ConfigurationManager.AppSettings["EmailSMTPServer"]));
                            thread.Start();
                        }
                    }
                }
                catch (Exception ex)
                {
                    if(thread != null)
                    {
                        try
                        {
                            if (thread.IsAlive)
                            {
                                thread.Abort();
                            }
                        }
                        catch(Exception err)
                        {
                            SendErrorEmail(err);
                        }
                    }
                    SendErrorEmail(ex);
                }
                finally
                {
                    Thread.Sleep(60000);
                }
            }
        }

        protected void RunMonitoringWorkflowsThread()
        {
            List<MonitoringWorkflow> monitoringWorkflows = new List<MonitoringWorkflow>();
            while (true)
            {
                Thread thread = null;
                try
                {
                    foreach (MonitoringWorkflow workflowToMonitor in MonitoringWorkflowManager.FillWorkflowsToMonitor(ConfigurationManager.ConnectionStrings["LogDB"].ConnectionString, ConfigurationManager.AppSettings["EmailFrom"], ConfigurationManager.AppSettings["EmailTo"], ConfigurationManager.AppSettings["EmailSMTPServer"]))
                {
                    if (monitoringWorkflows.Where(p => p.ID == workflowToMonitor.ID).Count() == 0)
                    {
                        monitoringWorkflows.Add(workflowToMonitor);
                        thread = new Thread(t => MonitoringWorkflowManager.MonitorWorkflow(workflowToMonitor, ConfigurationManager.ConnectionStrings["LogDB"].ConnectionString, ConfigurationManager.AppSettings["EmailFrom"], ConfigurationManager.AppSettings["EmailTo"], ConfigurationManager.AppSettings["EmailSMTPServer"]));
                        thread.Start();
                    }
                }
                }
                catch (Exception ex)
                {
                    if (thread != null)
                    {
                        try
                        {
                            if (thread.IsAlive)
                            {
                                thread.Abort();
                            }
                        }
                        catch (Exception err)
                        {
                            SendErrorEmail(err);
                        }
                    }
                    SendErrorEmail(ex);
                }
                finally
                {
                    Thread.Sleep(60000);
                }
            }
        }

        protected void RunSingleMonitoringThread()
        {
            string xmlFilePath = @"..\..\XML\Request_GET_PRICE_Monitoring.xml";
            if (ConfigurationManager.AppSettings["xmlFilePath"] != null)
            {
                xmlFilePath = ConfigurationManager.AppSettings["xmlFilePath"];
            }
            Thread monitoringThread = new Thread(p => B2BMessagesMonitoringManager.RunMonitoring(xmlFilePath,
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
                                                   ));
            monitoringThread.Start();
        }

        protected override void OnStop()
        {
        }
    }
}
