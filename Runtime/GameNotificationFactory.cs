using System;
using System.Collections.Generic;

namespace LittleBit.Modules.Notifications
{
    public class GameNotificationFactory
    {
        
        private readonly GameNotificationService _gameNotificationService;

        public GameNotificationFactory(List<GameNotificationChannel> channels)
        {
            _gameNotificationService = new GameNotificationService(channels);
        }

        public IGameNotification Create(GameNotificationConfig config, float? timeSpan = null )
        {
            IGameNotification notification = _gameNotificationService.CreateNotification();

            if (notification == null)
            {
                return null;
            }
            var deliveryTime =timeSpan == null
                ? DateTime.Now.ToLocalTime() + TimeSpan.FromSeconds(config.TimeSpan)
                : DateTime.Now.ToLocalTime() + TimeSpan.FromSeconds(timeSpan.Value); 
            notification.Title = config.Title;
            notification.Body = config.Body;
            notification.Key = config.Key;
            notification.Group = config.NotificationChannel.Id;
            notification.DeliveryTime = deliveryTime;
            notification.SmallIcon = config.SmallIcon;
            notification.LargeIcon = config.LargeIcon;
            
            _gameNotificationService.ScheduleNotification(notification);

            return notification;
        }
        
        public void CancelNotification(int id)
        {
            _gameNotificationService.CancelNotification(id);
        }

        public void CancelAllNotifications()
        {
            _gameNotificationService.CancelAllNotification();
        }
    }
}