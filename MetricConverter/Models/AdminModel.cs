using MetricConverter.Data;
using System.Collections.Generic;

namespace MetricConverter.Models
{
    public class AdminModel
    {
        public List<AppUser> Users { get; set; }
        public List<ConversionHistoryEntry> ConversionHistoryEntries { get; set; }
        public List<NotificationModel> UserNotifications { get; set; }
    }
}
