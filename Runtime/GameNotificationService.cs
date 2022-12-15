using System.Collections.Generic;
using System.IO;
using LittleBit.Modules.Notifications.Android;
using Unity.Notifications.Android;
using UnityEngine;

namespace LittleBit.Modules.Notifications
{
    public class GameNotificationService:IGameNotificationService
    {
        // Default filename for notifications serializer
        private const string DefaultFilename = "notifications.bin";
        /// <summary>
        /// Gets a collection of notifications that are scheduled or queued.
        /// </summary>
        public List<PendingNotification> PendingNotifications { get; private set; }
        
        /// <summary>
        /// Gets the implementation of the notifications for the current platform;
        /// </summary>
        public IGameNotificationsPlatform Platform { get; private set; }
        
        /// <summary>
        /// Gets or sets the serializer to use to save pending notifications to disk if we're in
        /// <see cref="OperatingMode.RescheduleAfterClearing"/> mode.
        /// </summary>
        public IPendingNotificationsSerializer Serializer { get; set; }
        
        private readonly List<GameNotificationChannel> _channels;
        
        public GameNotificationService(List<GameNotificationChannel> channels)
        {
            _channels = channels;
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
                    CanShowBadge = notificationChannel.ShowsBadge,
                    EnableLights = notificationChannel.ShowLights,
                    EnableVibration = notificationChannel.Vibrates,
                    LockScreenVisibility = (LockScreenVisibility)notificationChannel.Privacy,
                };

                AndroidNotificationCenter.RegisterNotificationChannel(androidChannel);
            }
#endif
            if (Platform == null)
            {
                return;
            }

            PendingNotifications = new List<PendingNotification>();
            Platform.NotificationReceived += OnNotificationReceived;

            // Check serializer
            if (Serializer == null)
            {
                Serializer = new DefaultSerializer(Path.Combine(Application.persistentDataPath, DefaultFilename));
            }

            OnForegrounding();
        }

        private void OnForegrounding()
        {
            
        }

        private void OnNotificationReceived(IGameNotification obj)
        {
            
        }
    }
}