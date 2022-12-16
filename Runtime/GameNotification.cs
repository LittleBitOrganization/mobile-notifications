using System;
using LittleBit.Modules.CoreModule;

namespace LittleBit.Modules.Notifications
{
    [Serializable]
    public class GameNotification: Data,IGameNotification
    {
        public int? Id { get; set; }
        
        public string Title { get; set; }
        
        public string Body { get; set; }
        
        public string Subtitle { get; set; }
        
        public string Data { get; set; }
        
        public string Group { get; set; }
        
        public bool ShouldAutoCancel { get; set; }
        
        public DateTime? DeliveryTime { get; set; }
        
        public bool Scheduled { get; }
        
        public string SmallIcon { get; set; }
        
        public string LargeIcon { get; set; }

        public GameNotification()
        {
            
        }
    }
}