using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ApiDoc = ApiClient.ApiDocumentations.NotificationsApiDocumentation;
using ClsHelper = ApiClient.Helper;

namespace ApiClient.ApiCalls
{
    public static class NotificationsApiCalls
    {
        public static int GetEmailRecipientsNumber(string environmentName, IEnumerable<string> objectIds)
        {
            var res = CallAsync(environmentName, objectIds, ApiDoc.GetEmailRecipientsNumber,
                JsonConvert.DeserializeObject<ApiDoc.GetEmailRecipientsNumberResponse>, x => x?.EmailRecipientsNumber ?? -1)
                .GetAwaiter().GetResult();
            return res;
        }

        public static int MailingNotificationByObjectIds(string environmentName, IEnumerable<string> objectIds)
        {
            var res = CallAsync(environmentName, objectIds, ApiDoc.MailingNotificationByObjectIds,
                JsonConvert.DeserializeObject<ApiDoc.MailingNotificationByObjectIdsResponse>, x => x?.SentEmailsNbre ?? -1)
                .GetAwaiter().GetResult();
            return res;
        }

        private static async Task<int> CallAsync<T>(
            string environmentName, IEnumerable<string> objectIds, string endPoint, Func<string, T> deserializeObject, Func<T, int> extractRes)
        {
            var jsonContent = new StringContent(JsonConvert.SerializeObject(
                new { ObjectIds = new List<string>(objectIds) }), Encoding.UTF8, "application/json");
            var requestUrl = ClsHelper.GetNotificationApiUrl(environmentName) + endPoint;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true };
            try
            {
                using (var httpClient = new HttpClient(handler))
                {
                    httpClient.DefaultRequestHeaders.Add("x-api-key", ConfigurationManager.AppSettings["NotificationsApiKey"]);
                    var response = await httpClient.PostAsync(requestUrl, jsonContent).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var resObject = deserializeObject(responseContent);
                        return extractRes(resObject);
                    }
                    else
                    {
                        Console.WriteLine("Error from NotificationAPI: " + response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return -1;
        }
    }
}