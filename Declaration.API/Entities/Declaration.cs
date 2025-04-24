using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Notification.API.Entities{

public class Declaration
{
    [BsonId]  
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id_Declaration { get; set; }  

    public string SenderId { get; set; }
    public string RecipientId { get; set; }
    public string Content { get; set; }
    public DateTime SentAt { get; set; }
    public bool IsReadByRecipient { get; set; }
    public string Status { get; set; } // par exemple "En attente", "Répondu"
}}
