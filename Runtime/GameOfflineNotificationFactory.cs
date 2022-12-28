using System;
using System.Collections.Generic;
using System.Linq;
using LittleBit.Modules.TimeServiceModule;

namespace LittleBit.Modules.Notifications
{
    public class GameOfflineNotificationFactory
    {
        private const float _secondsBefore = 2.0f;

        private readonly TimeTweenerFactory _tweenerFactory;

        private readonly GameNotificationFactory _gameNotificationFactory;

        private List<GameNotificationWithTweener> _execudedNotifications;

        public event Action<IGameNotification> OnLocalNotificationExpiried;
        
        public GameOfflineNotificationFactory(TimeTweenerFactory tweenerFactory,
            GameNotificationFactory gameNotificationFactory)
        {
            _tweenerFactory = tweenerFactory;
            _gameNotificationFactory = gameNotificationFactory;
        }

        public IGameNotification Create(GameNotificationConfig config, float? timeSpan = null)
        {
            var notification = _gameNotificationFactory.Create(config, timeSpan);
            notification.DeleteAutoReload = true;

            float duration = timeSpan == null ? config.TimeSpan - _secondsBefore : timeSpan.Value - _secondsBefore;

            TimeTweener timeTweener = _tweenerFactory.CreateFromNow(duration);

            var notificationWithTweener = new GameNotificationWithTweener(notification, timeTweener);
            _execudedNotifications.Add(notificationWithTweener);
            
            timeTweener.OnComplete += () =>
            {
                _gameNotificationFactory.CancelNotification(notification.Id);
                _execudedNotifications.Remove(notificationWithTweener);
                OnLocalNotificationExpiried?.Invoke(notification);
            };

            return notification;
        }
        

        public void CancelAllNotifications()
        { 
            _gameNotificationFactory.CancelAllNotifications();
            _execudedNotifications.Clear();
        } 

        public void CancelNotification(int notificationId)
        {
            _gameNotificationFactory.CancelNotification(notificationId);
            var notificationWithTweener = GetGameNotificationWithTweener(notificationId);
            if (notificationWithTweener != null)
            {
                notificationWithTweener.TimeTweener.Kill(false);
                _execudedNotifications.Remove(notificationWithTweener);
            }
        }
        private GameNotificationWithTweener GetGameNotificationWithTweener(int notificationId)
        {
            return _execudedNotifications.FirstOrDefault(n =>n.Id == notificationId);
        }
    }

    internal class GameNotificationWithTweener
    {
        private IGameNotification _notification;
        private TimeTweener _timeTweener;

        public TimeTweener TimeTweener=>_timeTweener;
        public GameNotificationWithTweener(IGameNotification notification, TimeTweener timeTweener)
        {
            _notification = notification;
            _timeTweener = timeTweener;
        }

        public int Id => _notification.Id;
        public string Key => _notification.Key;
    }    

}