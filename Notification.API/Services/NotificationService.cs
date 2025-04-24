using MongoDB.Driver;
using Notification.API.Entities;  // Utiliser le nouveau nom de classe NotificationEntity
using Microsoft.Extensions.Options;
using System;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notification.API.Services
{
    public class NotificationService
    {
        private readonly IMongoCollection<NotificationEntity> _notifications; // Utiliser NotificationEntity

        public NotificationService(IOptions<DatabaseSettings> databaseSettings)
        {
            // Connexion à MongoDB
            var client = new MongoClient(databaseSettings.Value.ConnectionString);
            var database = client.GetDatabase(databaseSettings.Value.DatabaseName);
            _notifications = database.GetCollection<NotificationEntity>(databaseSettings.Value.CollectionName); // Utiliser NotificationEntity
        }

        // Créer une nouvelle notification
        public async Task CreateNotification(NotificationEntity notification) // Utiliser NotificationEntity
        {
            await _notifications.InsertOneAsync(notification);
        }

        // Récupérer toutes les notifications pour un utilisateur spécifique
        public async Task<List<NotificationEntity>> GetNotificationsByDestinataireId(string destinataireId) // Utiliser NotificationEntity
        {
            return await _notifications.Find(n => n.DestinataireId == destinataireId).ToListAsync();
        }

        // Marquer une notification comme lue
        public async Task MarkAsRead(string id)
        {
           
var filter = Builders<NotificationEntity>.Filter.Eq(n => n.Id_notif, id);
            var update = Builders<NotificationEntity>.Update.Set(n => n.Lu, true); // Utiliser NotificationEntity
            await _notifications.UpdateOneAsync(filter, update);
        }
        public async Task<List<NotificationEntity>> GetAllNotifications()
{
    return await _notifications.Find(_ => true).ToListAsync();
}

public async Task DeleteNotification(string id_notif)
{
    await _notifications.DeleteOneAsync(n => n.Id_notif == id_notif);
}
public async Task DeleteAllNotificationsByUser(string destinataireId)
{
    var filter = Builders<NotificationEntity>.Filter.Eq(n => n.DestinataireId, destinataireId);
    await _notifications.DeleteManyAsync(filter);
}


    }
}
