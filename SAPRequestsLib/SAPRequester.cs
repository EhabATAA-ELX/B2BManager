using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SAPRequestsLib
{
    public class SAPRequester
    {
        public static SAPReplyResult SendSAPMessage(string message, string wcfB2BWebServiceURL, string wcfMethodName = null, string wcfUserName = null, string wcfPassword = null, string globalId = null, string sessionId = null,string sopname = null)
        {
            string reply = ""; string correlId = "";

            SAPReplyResult sapReplyResult = new SAPReplyResult();
            XMLRequest xMLRequest = ValidateRequest(message,sopname);
            
            sapReplyResult.RequestedDate = DateTime.Now;
            sapReplyResult.ReceivedDate = null;
            sapReplyResult.sAPMessageType = xMLRequest.sAPMessageType;
            sapReplyResult.HasError = !xMLRequest.IsValid;
            sapReplyResult.error = xMLRequest.error;            

            if (xMLRequest.IsValid)
            {
                if (!(string.IsNullOrEmpty(wcfB2BWebServiceURL)))
                {
                    bool runWithSuccess = true;
                    Exception exception = new Exception("No reply was received from SAP or web service hybris call has failed.");
                    if (sapReplyResult.sAPMessageType != SAPMessageType.NotSpecified)
                    {
                        try
                        {
                            wcfB2BRef.IwcfB2BClient eclient = new wcfB2BRef.IwcfB2BClient();
                            ((System.ServiceModel.Description.ServiceEndpoint)eclient.Endpoint).Address = new System.ServiceModel.EndpointAddress(wcfB2BWebServiceURL);
                            message = message.ToString().Replace(@"<?xml version=""1.0"" encoding=""UTF-8""?>", "");
                            if (sapReplyResult.sAPMessageType != SAPMessageType.NotSpecified)
                            {
                                correlId = Str_GenerateCorrelID();    
                                switch (sapReplyResult.sAPMessageType)
                                {
                                    case SAPMessageType.Price:
                                        {
                                            if (string.IsNullOrEmpty(wcfMethodName))
                                            {
                                                reply = eclient.sendPrice(message, true);
                                            }
                                            else
                                            {
                                                if (xMLRequest.IsFromShoppingBasket)
                                                {
                                                    runWithSuccess= eclient.sendPrice_Hybris(message, correlId, new Guid(globalId), new Guid(sessionId), 0, xMLRequest.SOPID, wcfMethodName, wcfUserName, wcfPassword, 0);
                                                }
                                                else
                                                {
                                                    runWithSuccess = eclient.sendPriceCheckReplyPL_Hybris(message, correlId, xMLRequest.SOPID, wcfMethodName, wcfUserName, wcfPassword);
                                                }
                                            }

                                            break;
                                        }
                                    case SAPMessageType.Availability:
                                        {
                                            if (string.IsNullOrEmpty(wcfMethodName))
                                            {
                                                reply = eclient.sendAvailability(message, true);
                                            }
                                            else
                                            {
                                                if (xMLRequest.IsFromShoppingBasket)
                                                {
                                                    runWithSuccess = eclient.sendAvailability_Hybris(message, correlId, new Guid(globalId), new Guid(sessionId), 0, xMLRequest.SOPID, wcfMethodName, wcfUserName, wcfPassword, 0);
                                                }
                                                else
                                                {
                                                    runWithSuccess = eclient.sendAvailabilityCheckReplyPL_Hybris(message, correlId, xMLRequest.SOPID, wcfMethodName, wcfUserName, wcfPassword);
                                                }
                                            }
                                            break;
                                        }
                                    case SAPMessageType.DeliveryAddress:
                                        {
                                            reply = eclient.sendAddress(message, true);
                                            break;
                                        }
                                    case SAPMessageType.ConsumerAddress:
                                        {
                                            if (string.IsNullOrEmpty(wcfMethodName))
                                            {
                                                reply = eclient.sendConsumerAddress(message, true);
                                            }
                                            else
                                            {
                                                runWithSuccess = eclient.SendConsummerAddress(message, correlId, xMLRequest.SOPID, new Guid(globalId), new Guid(sessionId), wcfMethodName, wcfUserName, wcfPassword);
                                            }                                            
                                            break;
                                        }
                                    case SAPMessageType.PlaceOrder:
                                        {
                                            if (string.IsNullOrEmpty(wcfMethodName))
                                            {
                                                reply = eclient.sendOrderPlacement(message, true);
                                            }
                                            else
                                            {
                                                runWithSuccess = eclient.sendOrderPlacement_hybris(message, correlId, xMLRequest.SOPID, new Guid(globalId), new Guid(sessionId), wcfMethodName, wcfUserName, wcfPassword);
                                            }
                                            break;
                                        }
                                    case SAPMessageType.GetCalendar:
                                        {
                                            runWithSuccess = eclient.sendGETCalendar_hybris(message, correlId, xMLRequest.SOPID, wcfMethodName, wcfUserName, wcfPassword);
                                            break;
                                        }
                                    case SAPMessageType.CustomerRange:
                                        {
                                            reply = eclient.sendCustomerRange(message, true);
                                            break;
                                        }
                                    case SAPMessageType.OrderList:
                                        {
                                            if (string.IsNullOrEmpty(wcfMethodName))
                                            {
                                                reply = eclient.sendOrderStatus(message, true);
                                            }
                                            else
                                            {
                                                runWithSuccess = eclient.sendOrderList_hybris(message, correlId, xMLRequest.SOPID, new Guid(globalId), new Guid(sessionId), wcfMethodName, wcfUserName, wcfPassword);
                                            }
                                            break;
                                        }
                                    case SAPMessageType.MediaRequest:
                                        {
                                            var time = new TimeSpan(0, 2, 0);
                                            eclient.Endpoint.Binding.CloseTimeout = time;
                                            eclient.Endpoint.Binding.OpenTimeout = time;
                                            eclient.Endpoint.Binding.ReceiveTimeout = time;
                                            eclient.Endpoint.Binding.SendTimeout = time;
                                            runWithSuccess = eclient.sendMedia_Asset(xMLRequest.SOPID, correlId, message);

                                            break;
                                        }
                                    case SAPMessageType.PlaceOrder_Async:
                                        {
                                            eclient.sendPlaceOrder_Asyn(correlId, message, sopname, new Guid(globalId), new Guid(sessionId), wcfMethodName, wcfUserName, wcfPassword);
                                            break;
                                        }
                                    case SAPMessageType.TP2PlaceOrder_Async:
                                        {
                                            eclient.sendPlaceOrderTP2_Async(message, wcfMethodName, wcfUserName, wcfPassword);
                                            break;
                                        }
                                }
                            }
                            sapReplyResult.ReceivedDate = DateTime.Now;


                            eclient.Close();
                        }
                        catch (Exception ex)
                        {
                            exception = ex;
                            runWithSuccess = false;
                        }
                        finally
                        {
                            if (!runWithSuccess)
                            {
                                sapReplyResult.HasError = true;
                                sapReplyResult.error = exception;
                                exception = null;
                            }
                            sapReplyResult.CorrelID = correlId;
                            sapReplyResult.Reply = reply;
                        }
                    }
                    else
                    {
                        sapReplyResult.HasError = true;
                        sapReplyResult.error = new SAPMessageTypeNotMatchedException("SAP message type not matched");
                    }
                }
                else
                {
                    sapReplyResult.HasError = true;
                    sapReplyResult.error = new Exception("wcfB2BWebServiceURL not passed");
                }
            }

            return sapReplyResult;
        }

        public static XMLRequest ValidateRequest(string message, string sopname = null, int environmentID = 0, string connectionString = null)
        {
            XMLRequest xmlRequest = new XMLRequest();
            string SopId = null;
            xmlRequest.IsValid = true;
            bool IsFromShoppingBasket = false;
            bool TP2placeOrderAsync = false;
            // Validate the XML message
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(message);
                if (doc.GetElementsByTagName("SOP_INDICATOR").Count > 0)
                {
                    SopId = doc.GetElementsByTagName("SOP_INDICATOR")[0].InnerText;
                }

                if (doc.GetElementsByTagName("SNDPRN").Count > 0 && doc.GetElementsByTagName("SNDPRN")[0].InnerText == "TRADEPLACE")
                {
                    TP2placeOrderAsync = true;
                    
                }

                if (string.IsNullOrEmpty(SopId))
                {
                    SopId = sopname;
                }

                if (String.IsNullOrEmpty(SopId))
                {
                    xmlRequest.IsValid = false;
                    xmlRequest.error = new Exception("SOP indicator wasn't supplied or cannot be identified from the request");
                }
                else
                {
                    xmlRequest.SOPID = SopId;
                    if (doc.GetElementsByTagName("SHOPPING_BASKET").Count > 0)
                    {
                        IsFromShoppingBasket = (doc.GetElementsByTagName("SHOPPING_BASKET")[0].InnerText.Equals("1"));
                    }
                    xmlRequest.IsFromShoppingBasket = IsFromShoppingBasket;
                }


                if (xmlRequest.IsValid)
                {
                    SAPMessageType messageType = GetMessageTypeByMessage(message);
                    xmlRequest.sAPMessageType = messageType;

                    if (messageType == SAPMessageType.MediaRequest)
                    {
                        Guid globalID = Guid.Empty;
                        if (doc.GetElementsByTagName("U_GlobalID").Count > 0)
                        {
                            Guid.TryParse(doc.GetElementsByTagName("U_GlobalID")[0].InnerText,out globalID);
                        }
                        
                        if (Guid.Empty != globalID)
                        {
                            xmlRequest.GlobalID = globalID;
                        }
                    }
                    else
                    {
                        if (messageType == SAPMessageType.PlaceOrder_Async && TP2placeOrderAsync)
                        {
                            xmlRequest.sAPMessageType = SAPMessageType.TP2PlaceOrder_Async;
                        }
                            if (doc.GetElementsByTagName("WEB_USER_ID").Count > 0)
                        {
                            string webUserID = doc.GetElementsByTagName("WEB_USER_ID")[0].InnerText;
                            if (!string.IsNullOrEmpty(connectionString) && environmentID > 0 && !string.IsNullOrEmpty(SopId) && !string.IsNullOrWhiteSpace(webUserID))
                            {
                                using (SqlConnection cnx = new SqlConnection(connectionString))
                                {
                                    SqlCommand cmd = new SqlCommand("MessageRequester.GetSessionInformationFromUser", cnx);
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.Add(new SqlParameter("@WebUserID", webUserID));
                                    cmd.Parameters.Add(new SqlParameter("@SOPID", SopId));
                                    cmd.Parameters.Add(new SqlParameter("@EnvironmentID", environmentID));
                                    SqlDataAdapter sqlDataReader = new SqlDataAdapter(cmd);
                                    DataTable dataTable = new DataTable();
                                    sqlDataReader.Fill(dataTable);
                                    if(dataTable.Rows.Count == 1)
                                    {
                                        if(dataTable.Rows[0]["GLOBALID"] != DBNull.Value)
                                        {
                                            xmlRequest.GlobalID = (Guid)dataTable.Rows[0]["GLOBALID"];
                                        }
                                        if (dataTable.Rows[0]["SESSIONID"] != DBNull.Value)
                                        {
                                            xmlRequest.SessionID = (Guid)dataTable.Rows[0]["SESSIONID"];
                                        }
                                    }

                                }
                            }
                        }
                    }                    
                }
                else
                {
                    xmlRequest.sAPMessageType = SAPMessageType.NotSpecified;
                }
            }
            catch (Exception ex)
            {
                xmlRequest.IsValid = false;
                xmlRequest.error = ex;
            }           

            return xmlRequest;
        }

        public static string[] PRICE_MESSAGE_LIST = { "GET_PRICE", "SHOW_PRICE", "GET_PRODUCT_PRICE", "SHOW_PRODUCT_PRICE" };
        public static string[] AVAILABILITY_MESSAGE_LIST = { "GET_AVAILABILITY", "SHOW_AVAILABILITY", "GET_PRODUCT_AVAILABILITY", "SHOW_PRODUCT_AVAILABILITY" };
        public static string[] DELIVERY_ADDRESS_MESSAGE_LIST = { "GET_DELVADDRESS", "SHOW_DELVADDRESS" };
        public static string[] CONSUMER_ADDRESS_MESSAGE_LIST = { "GET_CONSUMER_ADDRESS" };
        public static string[] PLACE_ORDER_MESSAGE_LIST = { "PLACE_SALESORDER", "CONFIRM_SALESORDER" };
        public static string[] CUSTOMER_RANGE_MESSAGE_LIST = { "SHOW_CUSTOMERRANGE", "GET_CUSTOMERRANGE" };
        public static string[] ORDER_LIST_MESSAGE_LIST = { "SHOWLIST_SALESORDER", "ORDER_SEARCH", "GETLIST_SALESORDER" };
        public static string[] GET_CALENDAR_MESSAGE_LIST = { "SHOW_CALENDAR", "GET_CALENDAR" };
        public static string[] MEDIAS_REQUEST_LIST = { "FILTERS_MEDIAS_REQUEST", "SHOW_MEDIAS_REQUEST", "PRODUCTSMEDIASREQUEST" };
        public static string[] PLACE_SALESORDER_ASYNC_LIST = { "ZSDBM_ORDRS" };

        public static SAPMessageType GetMessageTypeByMessage(string message)
        {
            SAPMessageType sAPMessageType = SAPMessageType.NotSpecified;

            SetMessageTypeIfOccursInList(message, PRICE_MESSAGE_LIST, SAPMessageType.Price, ref sAPMessageType);
            SetMessageTypeIfOccursInList(message, AVAILABILITY_MESSAGE_LIST, SAPMessageType.Availability, ref sAPMessageType);
            SetMessageTypeIfOccursInList(message, DELIVERY_ADDRESS_MESSAGE_LIST, SAPMessageType.DeliveryAddress, ref sAPMessageType);
            SetMessageTypeIfOccursInList(message, CONSUMER_ADDRESS_MESSAGE_LIST, SAPMessageType.ConsumerAddress, ref sAPMessageType);
            SetMessageTypeIfOccursInList(message, PLACE_ORDER_MESSAGE_LIST, SAPMessageType.PlaceOrder, ref sAPMessageType);
            SetMessageTypeIfOccursInList(message, CUSTOMER_RANGE_MESSAGE_LIST, SAPMessageType.CustomerRange, ref sAPMessageType);
            SetMessageTypeIfOccursInList(message, ORDER_LIST_MESSAGE_LIST, SAPMessageType.OrderList, ref sAPMessageType);
            SetMessageTypeIfOccursInList(message, GET_CALENDAR_MESSAGE_LIST, SAPMessageType.GetCalendar, ref sAPMessageType);
            SetMessageTypeIfOccursInList(message, MEDIAS_REQUEST_LIST, SAPMessageType.MediaRequest, ref sAPMessageType);
            SetMessageTypeIfOccursInList(message, PLACE_SALESORDER_ASYNC_LIST, SAPMessageType.PlaceOrder_Async, ref sAPMessageType);
            SetMessageTypeIfOccursInList(message, PLACE_SALESORDER_ASYNC_LIST, SAPMessageType.TP2PlaceOrder_Async, ref sAPMessageType);
            return sAPMessageType;
        }

        private static void SetMessageTypeIfOccursInList(string message, string[] messageList, SAPMessageType messageType, ref SAPMessageType sAPMessageType)
        {
            if (sAPMessageType == SAPMessageType.NotSpecified)
            {
                foreach (string messageName in messageList)
                {
                  if (message.ToUpper().Contains(messageName))
                        {
                            sAPMessageType = messageType;
                            break;
                        }
                    
                   
                }
            }
        }

        public static void SendErrorEmail(Exception ex,string EmailFrom,string EmailTo,string EmailSMTPServer,string subjectPrefix)
        {
            try
            {
                string exceptionMessage = (!(string.IsNullOrEmpty(ex.Message))) ? ex.Message : "";
                string exceptionStackTrace = (!(string.IsNullOrEmpty(ex.StackTrace))) ? ex.StackTrace : "";


                System.Net.Mail.MailMessage oMail;
                oMail = new System.Net.Mail.MailMessage(EmailFrom, EmailTo);
                oMail.BodyEncoding = System.Text.Encoding.UTF8;
                oMail.IsBodyHtml = true;
                oMail.Body = String.Format("<b>Excepetion Message:</b></br>{0}</br>"
                        + "<b>Exception Stack Trace:</b></br>{1}</br>", exceptionMessage
                        , exceptionStackTrace);
                oMail.Priority = System.Net.Mail.MailPriority.High;
                oMail.Subject = subjectPrefix + exceptionMessage;
                System.Net.Mail.SmtpClient objSmtp = new System.Net.Mail.SmtpClient(EmailSMTPServer);
                objSmtp.Send(oMail);
            }
            catch { }
        }

        public static void SendEmail(string subject,string body, string EmailFrom, string EmailTo, string EmailSMTPServer)
        {
            try
            {
                System.Net.Mail.MailMessage oMail = new System.Net.Mail.MailMessage();
                System.Net.Mail.MailAddress fromMail = new System.Net.Mail.MailAddress(EmailFrom);
                oMail.From = fromMail;
                oMail.To.Add(EmailTo.Replace(";", ","));
                oMail.Bcc.Add("hamdi.kharrat@electrolux.com");
                oMail.BodyEncoding = System.Text.Encoding.UTF8;
                oMail.IsBodyHtml = true;
                oMail.Body = body;
                oMail.Priority = System.Net.Mail.MailPriority.High;
                oMail.Subject = subject;
                System.Net.Mail.SmtpClient objSmtp = new System.Net.Mail.SmtpClient(EmailSMTPServer);
                objSmtp.Send(oMail);
            }
            catch { }
        }

        public static string Str_GenerateCorrelID()
        {
            return "B2" + DateTime.Now.ToString("ddMMyyyyHHmmss") + (Guid.NewGuid().ToString().Replace("-", ""));
        }

        
           
    }

    public class SAPReplyResult
    {
        public string Status
        {
            get
            {
                return (HasError) ? "Error" : "Success";
            }
        }
        public string CorrelID { get; set; }
        public bool HasError { get; set; }
        public string Reply { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public SAPMessageType sAPMessageType { get; set; }
        public Exception error { get; set; }
        public DateTime? IN_APIDatetimeSAP { get; set; }
        public DateTime? OUT_APIDatetimeSAP { get; set; }


    }

    public class XMLRequest
    {
        public bool IsValid { get; set; }
        public SAPMessageType sAPMessageType { get; set; }
        public string SOPID { get; set; }
        public Guid SessionID { get; set; }
        public Guid GlobalID { get; set; }
        public Exception error { get; set; }

        public bool IsFromShoppingBasket { get; set; }
    }


    public enum SAPMessageType { Price, Availability, PlaceOrder, DeliveryAddress, ConsumerAddress, OrderList, CustomerRange, NotSpecified , GetCalendar , MediaRequest, PlaceOrder_Async, TP2PlaceOrder_Async };

    public class SAPMessageTypeNotMatchedException : Exception
    {
        public SAPMessageTypeNotMatchedException()
        {
        }

        public SAPMessageTypeNotMatchedException(string message)
            : base(message)
        {
        }

        public SAPMessageTypeNotMatchedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }


}
