namespace ApiClient.ApiDocumentations
{
    public static class NotificationsApiDocumentation
    {
        #region Responses

        public class GetEmailRecipientsNumberResponse : MessageProperty
        {
            public int? EmailRecipientsNumber { get; set; }
        }

        public class MailingNotificationByObjectIdsResponse : MessageProperty
        {
            public int? SentEmailsNbre { get; set; }
        }

        public class MessageProperty
        {
            public string Message { get; set; } = string.Empty;
        }

        #endregion

        #region Methods

        public static readonly string GetEmailRecipientsNumber = "getEmailRecipientsNumber";
        public static readonly string MailingNotificationByObjectIds = "mailingNotificationByObjectIds";

        #endregion
    }
}