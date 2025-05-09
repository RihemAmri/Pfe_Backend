namespace Credit.API.DTOs
{
public class CreateNotificationDto
{
    public string DestinataireId { get; set; }
    public string Message { get; set; }
    public DateTime Date { get; set; }
    public bool Lu { get; set; }
    public string Type { get; set; } // "Déclaration" ou "Réponse"
}}
