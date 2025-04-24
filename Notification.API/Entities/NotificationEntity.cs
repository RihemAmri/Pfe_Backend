using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Notification.API.Entities
{
    public class NotificationEntity  
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id_notif { get; set; }

        public string DestinataireId { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public bool Lu { get; set; } = false;
        public string Type { get; set; }
    }
}
