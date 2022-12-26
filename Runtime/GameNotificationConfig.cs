using System;
using UnityEngine;

namespace LittleBit.Modules.Notifications
{
    [Serializable]
    public class GameNotificationConfig
    {
        [field:SerializeField] public string Title { get; private set; }
        public string Body { get; private set; }
        
        public GameNotificationChannel NotificationChannel { get; set; }
        public float DeliveryTime { get; set; }
        public string SmallIcon { get; set; }
        public string LargeIcon { get; set; }
    }
}