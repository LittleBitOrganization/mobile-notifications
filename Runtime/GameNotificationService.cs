using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LittleBit.Modules.Notifications.Android;
using Unity.Notifications.Android;
using UnityEngine;

namespace LittleBit.Modules.Notifications
{
    public class GameNotificationService:IGameNotificationService
    {
        // Minimum amount of time that a notification should be into the future before it's queued when we background.
        private static readonly TimeSpan MinimumNotificationTime = new TimeSpan(0, 0, 2);
        public List<PendingNotification> PendingNotifications { get; private set; }
        
        public IGameNotificationsPlatform Platform { get; private set; }
        public IPendingNotificationsSerializer Serializer { get; set; }
        
        private readonly List<GameNotificationChannel> _channels;
        public bool IsInitialized { get; private set; }
        public bool InForeground { get; private set; }
        public event Action<PendingNotification> LocalNotificationDelivered;
        
        public GameNotificationService(IPendingNotificationsSerializer serializer,
                                       List<GameNotificationChannel> channels)
        {
            _channels = channels;
            Serializer = serializer;
            Initialize();
        }

        private void Initialize()
        {
#if UNITY_ANDROID
            Platform = new AndroidNotificationsPlatform();
            // Register the notification channels
            var doneDefault = false;
            foreach (GameNotificationChannel notificationChannel in _channels)
            {
                if (!doneDefault)
                {
                    doneDefault = true;
                    ((AndroidNotificationsPlatform)Platform).DefaultChannelId = notificationChannel.Id;
                }
                // Wrap channel in Android object
                var androidChannel = new AndroidNotificationChannel(notificationChannel.Id, notificationChannel.Name,
                    notificationChannel.Description,
                    (Importance)notificationChannel.Style)
                {
                    CanBypassDnd = notificationChannel.HighPriority,
                    EnableLights = notificationChannel.ShowLights,
                    EnableVibration = notificationChannel.Vibrates,
                    LockScreenVisibility = (LockScreenVisibility)notificationChannel.Privacy,
                };

                AndroidNotificationCenter.RegisterNotificationChannel(androidChannel);
            }
#elif UNITY_IOS
            Platform = new iOSNotificationsPlatform();
#endif
            if (Platform == null)
            {
                return;
            }
            PendingNotifications = new List<PendingNotification>();
            Platform.NotificationReceived += OnNotificationReceived;
            OnForegrounding();

            IsInitialized = true;
        }

        
        public IGameNotification SendNotification(string title, string body, DateTime deliveryTime, string channelId = null,
            string smallIcon = null, string largeIcon = null)
        {
            IGameNotification notification = CreateNotification();

            if (notification == null)
            {
                return null;
            }

            notification.Title = title;
            notification.Body = body;
            notification.Group = !string.IsNullOrEmpty(channelId) ? channelId : "ChannelId";
            notification.DeliveryTime = deliveryTime;
            notification.SmallIcon = smallIcon;
            notification.LargeIcon = largeIcon;

            ScheduleNotification(notification);
            SaveNotifications();
            return notification;
        }
        private void ScheduleNotification(IGameNotification notification)
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException("Must call Initialize() first.");
            }

            if (notification == null || Platform == null)
            {
                return;
            }

            Platform.ScheduleNotification(notification);

            // Register pending notification
            var result = new PendingNotification(notification);
            PendingNotifications.Add(result);
        }
        private IGameNotification CreateNotification()
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException("Must call Initialize() first.");
            }
            return Platform?.CreateNotification();
        }
        private void SaveNotifications()
        {
            var notificationsToSave = new List<PendingNotification>(PendingNotifications.Count);
            foreach (PendingNotification pendingNotification in PendingNotifications)
            {
                // In non-clear mode, just add all scheduled notifications
                if (pendingNotification.Notification.Scheduled)
                {
                    notificationsToSave.Add(pendingNotification);
                }
            }
            // Save to disk
            Serializer?.Serialize(notificationsToSave);
        }
        private void OnForegrounding()
        {
            PendingNotifications.Clear();
            Platform.OnForeground();
            // Deserialize saved items
            IList<IGameNotification> loaded = Serializer?.Deserialize(Platform);

            // Just create PendingNotification wrappers for all deserialized items.
            // We're not rescheduling them because they were not cleared
            if (loaded == null)
            {
                return;
            }
            foreach (IGameNotification savedNotification in loaded)
            {
                if (savedNotification.DeliveryTime > DateTime.Now)
                {
                    PendingNotifications.Add(new PendingNotification(savedNotification));
                }
            }
        }

        private void OnNotificationReceived(IGameNotification deliveredNotification)
        {
            // Find in pending list
            int deliveredIndex =
                PendingNotifications.FindIndex(scheduledNotification =>
                    scheduledNotification.Notification.Id == deliveredNotification.Id);
            if (deliveredIndex >= 0)
            {
                LocalNotificationDelivered?.Invoke(PendingNotifications[deliveredIndex]);

                PendingNotifications.RemoveAt(deliveredIndex);
            }
        }
    }
}