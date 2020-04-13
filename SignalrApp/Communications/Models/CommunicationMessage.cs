namespace SignalrApp.Communications.Models
{
    public class CommunicationMessage
    {
        public RecipientType CommunicationType { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string OrganizationId { get; set; }
        public string UserId { get; set; }
        public bool IsPersist { get; set; }
    }
}