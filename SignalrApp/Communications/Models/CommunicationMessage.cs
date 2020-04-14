namespace SignalrApp.Communications.Models
{
    //we can add some fluent validation here, if needed
    public class CommunicationMessage
    {
        public RecipientType CommunicationType { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string OrganizationId { get; set; }
        public string UserId { get; set; }
        public bool IsPersist { get; set; }
        public bool IsRead { get; set; }
    }
}