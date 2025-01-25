using System;
using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.NotificationAndTaskGrid
{
    public class QuanLyThongBaoGrid: GridItem
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Link { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string CreatedOnFormat { get; set; }
        public bool? IsRead { get; set; }
    }

    public class NotificationGrid : QuanLyThongBaoGrid
    {
        public long UserNotificationId { get; set; }
    }

    public class NotificationDetail
    {
        public long? CountUnRead { get; set; }
        public List<NotificationGrid> ListNotification { get; set; }
    }

    public class TaskGrid : QuanLyThongBaoGrid
    {
        public long UserTaskId { get; set; }
    }

    public class TaskDetail
    {
        public long CountUnRead { get; set; }
        public List<TaskGrid> ListTask { get; set; }
    }
}
