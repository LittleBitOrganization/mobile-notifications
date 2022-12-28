using System;
using UnityEngine;

namespace LittleBit.Modules.Notifications
{
    [Serializable]
    public class GameNotificationConfig
    {
        [field:SerializeField] public string Key { get; private set; }
        [field:SerializeField] public string Title { get; private set; }
        [field:SerializeField] public string Body { get; private set; }
        [field:SerializeField] public GameNotificationChannel NotificationChannel { get; set; }
        
        [field:SerializeField] public string Data { get; set; }
        [field:SerializeField] public float TimeSpan { get; set; }
        [field:SerializeField] public string SmallIcon { get; set; }
        [field:SerializeField] public string LargeIcon { get; set; }
    }
}