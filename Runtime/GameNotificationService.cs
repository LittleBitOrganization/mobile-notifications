using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
#if UNITY_ANDROID
using LittleBit.Modules.Notifications.Android;
using Unity.Notifications.Android;
#elif UNITY_IOS
using LittleBit.Modules.Notifications.iOS;
#endif

namespace LittleBit.Modules.Notifications
{
    internal class GameNotificationService
    {
        public IGameNotificationsPlatform Platform { get; private set; }
        
        private readonly List<GameNotificationChannel> _channels;
        public bool IsInitialized { get; private set; }

        public GameNotificationService(List<GameNotificationChannel> channels)
        {
            _channels = channels;
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

            IsInitialized = true;
        }
        
        public void CancelNotification(int id)
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException("Must call Initialize() first.");
            }
            Platform?.CancelNotification(id);
        }
        public void CancelAllNotification()
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException("Must call Initialize() first.");
            }
            Platform?.CancelAllScheduledNotifications();
        }
        public IGameNotification GetLastNotifications() => Platform?.GetLastNotification();
        public IGameNotification CreateNotification()
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException("Must call Initialize() first.");
            }
            return Platform?.CreateNotification();
        }
        
        public void ScheduleNotification(IGameNotification notification)
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
        }
    }
}