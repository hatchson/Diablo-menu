using StupidTemplate.Classes;
using UnityEngine;

namespace StupidTemplate
{
    public class Settings
    {
        /*
         * These are the settings for the menu.
         * 
         * To change the colors, you need to modify the ExtGradient variables.
         * Here are some examples on how to use ExtGradient:
         * 
         * Solid Color:
         *  new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.black) }
         *  
         * Simple Gradient:
         *  new ExtGradient { colors = ExtGradient.GetSimpleGradient(Color.black, Color.white) }
         * 
         * Rainbow Color:
         *   new ExtGradient { rainbow = true }
         *   
         * Epileptic Color (random color every frame):
         *   new ExtGradient { epileptic = true }
         *   
         * Self Color:
         *   new ExtGradient { copyRigColor = true }
         *   
         * To change the font, you may use the following code:
         *   Font.CreateDynamicFontFromOSFont("Comic Sans MS", 24)
         */

        public static ExtGradient backgroundColor = new ExtGradient { colors = ExtGradient.GetSimpleGradient(Color.red, Color.black) };
       
        public static ExtGradient[] buttonColors = new ExtGradient[]
        {
            new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.orangeRed) }, // Disabled
            new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.darkRed) } // Enabled
        };
        public static Color[] textColors = new Color[]
        {
            Color.white, // Disabled
            Color.white // Enabled
        };

        public static Font currentFont = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;

        public static bool fpsCounter = true;
        public static bool disconnectButton = true;
        public static bool rightHanded;
        public static bool disableNotifications;
        public static bool useClickGuiLayout = false;

        public static KeyCode keyboardButton = KeyCode.Q;

        public static Vector3 menuSize = new Vector3(0.1f, 1f, 1f); // Depth, width, height
        public static int buttonsPerPage = 8;

        public static float gradientSpeed = 0.5f; // Speed of colors
        public static void lightmenu()
        {
           backgroundColor = new ExtGradient { colors = ExtGradient.GetSimpleGradient(Color.lightCyan, Color.lightBlue) };
            buttonColors = new ExtGradient[]
            {
                new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.cyan) }, // Disabled
                new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.blue) } // Enabled
            };
             textColors = new Color[]
            {
                Color.white, // Disabled
                Color.white // Enabled
            };

        }
        public static void normalmenu()
        {
            backgroundColor = new ExtGradient { colors = ExtGradient.GetSimpleGradient(Color.red, Color.black) };
            buttonColors = new ExtGradient[]
                {
                    new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.orangeRed) }, // Disabled
                    new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.darkRed) } // Enabled
                };
                textColors = new Color[]
                {
                    Color.white, // Disabled
                    Color.white // Enabled
                };

        }
    }
    
}
