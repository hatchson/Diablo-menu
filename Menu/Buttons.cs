using rainxyz.Mods;
using StupidTemplate;
using StupidTemplate.Classes;
using StupidTemplate.Mods;
using static StupidTemplate.Menu.Main;
using static StupidTemplate.Settings;

namespace StupidTemplate.Menu
{
    public class Buttons
    {
        /*
         * Here is where all of your buttons are located.
         * To create a button, you may use the following code:
         * 
         * Move to Category:
         *   new ButtonInfo { buttonText = "Settings", method =() => currentCategory = 1, isTogglable = false, toolTip = "Opens the main settings page for the menu."},
         *   new ButtonInfo { buttonText = "Return to Main", method =() => currentCategory = 0, isTogglable = false, toolTip = "Returns to the main page of the menu."},
         * 
         * Togglable Mod:
         *   new ButtonInfo { buttonText = "Platforms", method =() => Movement.Platforms(), toolTip = "Spawns platforms on your hands when pressing grip."},
         */

        public static ButtonInfo[][] buttons = new ButtonInfo[][]
        {
            new ButtonInfo[] { // Main Mods [0]
                new ButtonInfo { buttonText = "Settings", method =() => currentCategory = 1, isTogglable = false, toolTip = "Opens the main settings page for the menu."},

                new ButtonInfo { buttonText = "Room Mods", method =() => currentCategory = 4, isTogglable = false, toolTip = "Opens the room mods tab."},
                new ButtonInfo { buttonText = "Movement Mods", method =() => currentCategory = 5, isTogglable = false, toolTip = "Opens the movement mods tab."},
                new ButtonInfo { buttonText = "Safety Mods", method =() => currentCategory = 6, isTogglable = false, toolTip = "Opens the safety mods tab."},
                new ButtonInfo { buttonText = "Advantages", method =() => currentCategory = 7, isTogglable = false, toolTip = "Opens the advantages tab."},
                new ButtonInfo { buttonText = "Fun", method =() => currentCategory = 8, isTogglable = false, toolTip = "Fun Mods."},
            },

            new ButtonInfo[] { // Settings [1]
                new ButtonInfo { buttonText = "Return to Main", method =() => currentCategory = 0, isTogglable = false, toolTip = "Returns to the main page of the menu."},
                new ButtonInfo { buttonText = "Menu", method =() => currentCategory = 2, isTogglable = false, toolTip = "Opens the settings for the menu."},
                new ButtonInfo { buttonText = "Movement", method =() => currentCategory = 3, isTogglable = false, toolTip = "Opens the movement settings for the menu."},
            },

            new ButtonInfo[] { // Menu Settings [2]
                new ButtonInfo { buttonText = "Return to Settings", method =() => currentCategory = 1, isTogglable = false, toolTip = "Returns to the main settings page for the menu."},
                new ButtonInfo { buttonText = "Right Hand", enableMethod =() => rightHanded = true, disableMethod =() => rightHanded = false, toolTip = "Puts the menu on your right hand."},
                new ButtonInfo { buttonText = "Notifications", enableMethod =() => disableNotifications = false, disableMethod =() => disableNotifications = true, enabled = !disableNotifications, toolTip = "Toggles the notifications."},
                new ButtonInfo { buttonText = "FPS Counter", enableMethod =() => fpsCounter = true, disableMethod =() => fpsCounter = false, enabled = fpsCounter, toolTip = "Toggles the FPS counter."},
                new ButtonInfo { buttonText = "Disconnect Button", enableMethod =() => disconnectButton = true, disableMethod =() => disconnectButton = false, enabled = disconnectButton, toolTip = "Toggles the disconnect button."},
                new ButtonInfo { buttonText = "Click Gui", enableMethod =() => useClickGuiLayout = true, disableMethod =() => useClickGuiLayout = false, enabled = useClickGuiLayout, toolTip = "Toggles a euphoria like click gui this is also similar to iis stupid menu click gui."},
            },

            new ButtonInfo[] { // Movement Settings [3]
                new ButtonInfo { buttonText = "Return to Settings", method =() => currentCategory = 1, isTogglable = false, toolTip = "Returns to the main settings page for the menu."},

                new ButtonInfo { buttonText = "Change Fly Speed", overlapText = "Change Fly Speed [Normal]", method =() => Mods.Settings.Movement.ChangeFlySpeed(), isTogglable = false, toolTip = "Changes the speed of the fly mod."},
            },

            new ButtonInfo[] { // Room Mods [4]
                new ButtonInfo { buttonText = "Return to Main", method =() => currentCategory = 0, isTogglable = false, toolTip = "Returns to the main page of the menu."},

                new ButtonInfo { buttonText = "Disconnect", method =() => NetworkSystem.Instance.ReturnToSinglePlayer(), isTogglable = false, toolTip = "Disconnects you from the room."},
            },

            new ButtonInfo[] { // Movement Mods [5]
                new ButtonInfo { buttonText = "Return to Main", method =() => currentCategory = 0, isTogglable = false, toolTip = "Returns to the main page of the menu."},

                new ButtonInfo { buttonText = "Platforms", method =() => Movement.Platforms(), toolTip = "Spawns platforms on your hands when pressing grip."},
                new ButtonInfo { buttonText = "Fly", method =() => Movement.Fly(), toolTip = "Sends you forward when holding A."},
                new ButtonInfo { buttonText = "Teleport Gun", method =() => Movement.TeleportGun(), toolTip = "Teleports you to wherever your pointer is when pressing trigger."},
            },

            new ButtonInfo[] { // Safety Mods [6]
                new ButtonInfo { buttonText = "Return to Main", method =() => currentCategory = 0, isTogglable = false, toolTip = "Returns to the main page of the menu."},

                new ButtonInfo { buttonText = "Anti Report", method =() => Safety.AntiReportDisconnect(), toolTip = "Disconnects you when someone tries to report you."},
            },
            new ButtonInfo[] {
                new ButtonInfo {buttonText = "Return to Main", method =() => currentCategory = 0, isTogglable = false, toolTip = "Returns to the main page of the menu."},
                new ButtonInfo {buttonText = "Tag Self", method =() => Advantages.TagSelf(), toolTip = "Tags Yourself." },
                new ButtonInfo {buttonText = "Instant Tag All", method =() => Advantages.TagAllV2(), toolTip = "Instantly tags all players in the room."},
                new ButtonInfo {buttonText = "Tag Gun", method =() => Advantages.TagGunv2(), toolTip = "Instantly Tags Whoever The Gun Is Shooting At"},

            },
            new ButtonInfo[] {
                new ButtonInfo {buttonText = "Return to Main", method =() => currentCategory = 0, isTogglable = false, toolTip = "Returns to the main page of the menu."},
                new ButtonInfo {buttonText = "Spin Bug", method =() => Fun.BugMixer(), toolTip = "Makes the bug spin." },
                new ButtonInfo {buttonText = "Bug Telekineses", method =() => Fun.TelekenisisBug(), toolTip = "Makes the bug follow your head, fun with spin bug."},
                new ButtonInfo {buttonText = "Bug Hat", method =() => Fun.BugTeleport(), toolTip = "Bug Hat Makes the bug sit on your head, also fun with spin bug."},
                new ButtonInfo {buttonText = "Bug Head", method =() => Fun.BugHead(), toolTip = "Makes The Bug Teleport To the bottom of your body "},
                new ButtonInfo {buttonText = "Bug Spaz", method =() => Fun.BugSpaz(), toolTip = "Makes The Bug spaz, use this with telekineses bug to make the bug not be able to move around"},
                
            }
        };
    }
}
