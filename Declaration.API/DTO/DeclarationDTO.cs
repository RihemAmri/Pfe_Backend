namespace Notification.API.DTO{
public class DeclarationDTO
{
    public string SenderId { get; set; }
    public string RecipientId { get; set; }
    public string Content { get; set; }
    public DateTime SentAt { get; set; }
    public bool IsReadByRecipient { get; set; }
    public string Status { get; set; }  // Par exemple : "En attente", "Répondu"
}}
