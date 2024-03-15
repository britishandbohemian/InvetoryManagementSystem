using InventoryAPI.Model.UserModels;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryAPI.Model.NotificationsModel
{
    public class Notifications
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NotificationId { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public PriorityLevel Priority { get; set; }
        public NotificationType Type { get; set; }  // New property
        public string Product { get; set; }  // New property
        public int Quantity { get; set; }  // New property

        public enum PriorityLevel
        {
            Low,
            Medium,
            High,
            Urgent
        }

        public enum NotificationType  // New enum
        {
            Prepare,
            Reorder
        }
    }
}
