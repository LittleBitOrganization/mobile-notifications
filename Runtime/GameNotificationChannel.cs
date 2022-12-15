using UnityEngine;

namespace LittleBit.Modules.Notifications
{
    [CreateAssetMenu(fileName = "GameNotificationChannel", menuName = "ScriptableObjects/Game Notifications", order = 1)]
    public class GameNotificationChannel:ScriptableObject
    {
        /// <summary>
        /// The style of notification shown for this channel. Corresponds to the Importance setting of
        /// an Android notification, and do nothing on iOS.
        /// </summary>
        public enum NotificationStyle
        {
            /// <summary>
            /// Notification does not appear in the status bar.
            /// </summary>
            None = 0,
            /// <summary>
            /// Notification makes no sound.
            /// </summary>
            NoSound = 2,
            /// <summary>
            /// Notification plays sound.
            /// </summary>
            Default = 3,
            /// <summary>
            /// Notification also displays a heads-up popup.
            /// </summary>
            Popup = 4
        }

        /// <summary>
        /// Controls how notifications display on the device lock screen.
        /// </summary>
        public enum PrivacyMode
        {
            /// <summary>
            /// Notifications aren't shown on secure lock screens.
            /// </summary>
            Secret = -1,
            /// <summary>
            /// Notifications display an icon, but content is concealed on secure lock screens.
            /// </summary>
            Private = 0,
            /// <summary>
            /// Notifications display on all lock screens.
            /// </summary>
            Public
        }

        /// <summary>
        /// The identifier for the channel.
        /// </summary>
        public readonly string Id;

        /// <summary>
        /// The name of the channel as displayed to the user.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The description of the channel as displayed to the user.
        /// </summary>
        public readonly string Description;

        /// <summary>
        /// A flag determining whether messages on this channel can show a badge. Defaults to true.
        /// </summary>
        public readonly bool ShowsBadge = true;

        /// <summary>
        /// A flag determining whether messages on this channel cause the device light to flash. Defaults to false.
        /// </summary>
        public readonly bool ShowLights = false;

        /// <summary>
        /// A flag determining whether messages on this channel cause the device to vibrate. Defaults to true.
        /// </summary>
        public readonly bool Vibrates = true;

        /// <summary>
        /// A flag determining whether messages on this channel bypass do not disturb settings. Defaults to false.
        /// </summary>
        public readonly bool HighPriority = false;

        /// <summary>
        /// The display style for this notification. Defaults to <see cref="NotificationStyle.Popup"/>.
        /// </summary>
        public readonly NotificationStyle Style = NotificationStyle.Popup;

        /// <summary>
        /// The privacy setting for this notification. Defaults to <see cref="PrivacyMode.Public"/>.
        /// </summary>
        public readonly PrivacyMode Privacy = PrivacyMode.Public;
    }
}