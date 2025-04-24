using Microsoft.AspNetCore.Mvc;
using Notification.API.Entities;
using Notification.API.Services;
using Notification.API.DTO; // ton namespace pour CreateNotificationDto
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson; // pour ObjectId

namespace Notification.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService _notificationService;

        public NotificationController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // POST /api/Notification
        [HttpPost]
        public async Task<IActionResult> CreateNotification(CreateNotificationDto dto)
        {
            var notification = new NotificationEntity
            {
                Id_notif = ObjectId.GenerateNewId().ToString(),
                DestinataireId = dto.DestinataireId,
                Message = dto.Message,
                Date = dto.Date,
                Lu = dto.Lu,
                Type = dto.Type
            };
            await _notificationService.CreateNotification(notification);
            return Ok(notification);
        }

        // GET /api/Notification
        [HttpGet]
        public async Task<ActionResult<List<NotificationEntity>>> GetAllNotifications()
        {
            var all = await _notificationService.GetAllNotifications();
            return Ok(all);
        }

        // GET /api/Notification/user/{destinataireId}
        [HttpGet("user/{destinataireId}")]
        public async Task<ActionResult<List<NotificationEntity>>> GetNotificationsByDestinataireId(string destinataireId)
        {
            var notifications = await _notificationService.GetNotificationsByDestinataireId(destinataireId);
            return Ok(notifications);
        }

        // PUT /api/Notification/{id_notif}/read
        [HttpPut("{id_notif}/read")]
        public async Task<IActionResult> MarkAsRead(string id_notif)
        {
            await _notificationService.MarkAsRead(id_notif);
            return NoContent();
        }

        // DELETE /api/Notification/{id_notif}
        [HttpDelete("{id_notif}")]
        public async Task<IActionResult> DeleteNotification(string id_notif)
        {
            await _notificationService.DeleteNotification(id_notif);
            return NoContent();
        }
        [HttpDelete("ClearAll/{destinataireId}")]
public async Task<IActionResult> ClearAllNotifications(string destinataireId)
{
    await _notificationService.DeleteAllNotificationsByUser(destinataireId);
    return NoContent();
}

    }
}
