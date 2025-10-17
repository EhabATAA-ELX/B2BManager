using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SAPRequestsLib
{
   public class MediaRequester
    {
        public static string GetToKenMedia(string TokenUrl,string Media_Request_Authentication_ClientID, string Media_Request_Authentication_Secret )
        {
            try
            {
                var dictionaryForUrl = new Dictionary<string, string>()
                {
                        {"grant_type", "client_credentials" },
                        {"client_id",Media_Request_Authentication_ClientID},
                        {"client_secret",Media_Request_Authentication_Secret}
                };
                HttpClient auth = new HttpClient();
                HttpContent httpContent = new FormUrlEncodedContent(dictionaryForUrl);
                Task<HttpResponseMessage> httpResponse = auth.PostAsync(TokenUrl, httpContent);
                Task<string> message = httpResponse.Result.Content.ReadAsStringAsync();
                var dynamicObject = JsonConvert.DeserializeObject<dynamic>(message.Result);
                var token = dynamicObject.access_token;
                return token;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public static string PostMessage_media(string AccessToken, string Newcontent, string Media_Notify_Endpoint_URL,string Media_Request_URL)
        {
            try
            {
             
                using (var clientHandler = new HttpClientHandler())
                {
                    var client = new HttpClient(clientHandler);
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Add("X-Notify-Endpoint-URL", Media_Notify_Endpoint_URL);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
                    var httpContent = new StringContent(Newcontent, Encoding.UTF8, "application/json");
                    Task<HttpResponseMessage> t = client.PostAsync(Media_Request_URL, httpContent);
                    Task<string> message = t.Result.Content.ReadAsStringAsync();
                    return message.Result.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex; 
            }
        }

        public static string Str_GenerateCorrelID()
        {
            return "B2" + DateTime.Now.ToString("ddMMyyyyHHmmss") + (Guid.NewGuid().ToString().Replace("-", ""));
        }

    }

}
