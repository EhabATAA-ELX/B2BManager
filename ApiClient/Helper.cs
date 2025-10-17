using System.Collections.Generic;
using System.Configuration;

namespace ApiClient
{
    public static class Helper
    {
        // Table [B2BManager].[Logger].[R_Environments]
        private static Dictionary<string, int> Environments { get; } = new Dictionary<string, int>();

        static Helper()
        {
            Environments.Add("DEV", (int)Environment.DEV);
            Environments.Add("TEST", (int)Environment.SIT);
            Environments.Add("UAT", (int)Environment.UAT);
            Environments.Add("PRODUCTION", (int)Environment.Production);
        }

        private enum Environment
        {
            DEV,
            SIT,
            UAT,
            Production
        }

        public static string GetNotificationApiUrl(string environmentName)
        {
            if (!Environments.TryGetValue(environmentName, out int environmentId)) return string.Empty;
            string res;
            switch (environmentId)
            {
                case (int)Environment.SIT:
                    res = ConfigurationManager.AppSettings["NotificationsApiTestUrl"];
                    break;
                case (int)Environment.UAT:
                    res = ConfigurationManager.AppSettings["NotificationsApiUatUrl"];
                    break;
                case (int)Environment.Production:
                    res = ConfigurationManager.AppSettings["NotificationsApiProdUrl"];
                    break;
                default:
                    res = ConfigurationManager.AppSettings["NotificationsApiDevUrl"];
                    break;
            }

            return res;
        }
    }
}