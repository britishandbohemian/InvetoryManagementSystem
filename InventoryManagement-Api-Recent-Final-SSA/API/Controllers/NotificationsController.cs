using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using InventoryAPI.Data;
using InventoryAPI.Model.NotificationsModel;

namespace InventoryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public NotificationsController(DatabaseContext context)
        {
            _context = context;
        }

        // POST: api/Notifications
        // POST: api/Notifications
        [HttpPost]
        public ActionResult<Notifications> AddNotification(Notifications newNotification)
        {
            if (newNotification == null)
            {
                return BadRequest("Notification details are not provided");
            }

            // You can include validation logic here for the new fields, for example:
            if (string.IsNullOrWhiteSpace(newNotification.Product))
            {
                return BadRequest("Product name is not provided");
            }

            if (newNotification.Quantity <= 0)
            {
                return BadRequest("Invalid quantity");
            }

            // Proceed to add the new notification
            _context.Notification.Add(newNotification);
            _context.SaveChanges();

            return CreatedAtAction(nameof(AddNotification), new { id = newNotification.NotificationId }, newNotification);
        }

        // DELETE: api/Notifications/5
        [HttpDelete("{id}")]
        public ActionResult<Notifications> DeleteNotification(int id)
        {
            var notification = _context.Notification.FirstOrDefault(n => n.NotificationId == id);
            if (notification == null)
            {
                return NotFound($"Notification with ID {id} not found");
            }

            _context.Notification.Remove(notification);
            _context.SaveChanges();

            return Ok(notification);
        }
        // GET: api/Notifications
        [HttpGet]
        public ActionResult<IEnumerable<Notifications>> GetNotifications()
        {
            return Ok(_context.Notification.ToList());
        }
    }


}
