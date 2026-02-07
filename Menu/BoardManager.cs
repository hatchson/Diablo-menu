
using GorillaNetworking;
using StupidTemplate.Menu;
using Photon.Pun;
using StupidTemplate.Patches;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using static StupidTemplate.PluginInfo;



namespace StupidTemplate.Menu
{
    internal class BoardManager
    {
        private static string MOTD = "Failed to set board text";

        private static string MOTDtext = "Failed to set board text";

        private static string CoCTitle = "Failed to set board text";

        private static string CoCText = "Failed to set board text";

        private static string ScreenText = "Failed to set board text";


        private static GameObject MOTD_head = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/motdHeadingText"); // GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/motdHeadingText");
        private static GameObject MOTD_main = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/motdBodyText"); //GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/motdBodyText");
        private static GameObject COC_head = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/CodeOfConductHeadingText");// GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/CodeOfConductHeadingText");
        private static GameObject COC_main = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/COCBodyText_TitleData");// GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/COCBodyText_TitleData");
        private static GameObject SCREEN = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/ScreenText (1)");//GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/ScreenText (1)");
        public static void CreateCustomBoards()
        {
            SetBoardText();
            MOTD_head.GetComponent<TextMeshPro>().text = MOTD;
            MOTD_main.GetComponent<TextMeshPro>().text = MOTDtext;
            COC_head.GetComponent<TextMeshPro>().text = CoCTitle;
            COC_main.GetComponent<TextMeshPro>().text = CoCText;
            SCREEN.GetComponent<TextMeshPro>().text = ScreenText;

            GameObject crystalGameOBJ = GameObject.Find("Environment Objects/LocalObjects_Prefab/ForestToCave/C_Crystal_Chunk");
            if (crystalGameOBJ != null)
            {

                Material crystalMat = crystalGameOBJ.GetComponent<Renderer>().material;
            }

        }


        //use this is kinda complicated 
        public static void SetBoardText()
        {

            MOTD = "[ <color=red>Diablo Menu</color> ]";

            MOTDtext = "using [ Diablo Menu ]  \n" +
            " ===================================================================\n " +
            " We are not responsible for any <color=red> BANS. </color> This menu is <color=green> FREE. </color> ";

            CoCTitle = "[ <color=red>Diablo Menu " + PluginInfo.Version + "</color> ]";

            CoCText = "Using Diablo Menu:  Free \n" +
            " ==============================================\n" +
            " \n Meanings Of Symbols: \n" +
            "  W = Working \n  NW = Not Working  \n  D = Detected  \n  UND = Undetected  \n  DB = Delay Ban  \n \n Enjoy The Menu, Version " + PluginInfo.Version;

            ScreenText = "Welcome Back: " + "[ " + PhotonNetwork.NickName + " ]\n Dont Get Banned Thats All On You";


        }
        //legacy system
        public static async void CheckVersion()
        {
            try
            {
                await Task.Delay(500);
                System.Net.WebClient WebClient = new System.Net.WebClient();
                string Real_Version = WebClient.DownloadString("https://raw.githubusercontent.com/lerpyypluh/LunarConsole/refs/heads/main/verson.txt").Trim();
                Debug.Log("real version: " + Real_Version + ", current version: " + PluginInfo.Version);

                if (Real_Version != PluginInfo.Version.Trim() && Real_Version != "disabled")
                {

                    MOTD = "[ Diablo Menu ]";
                    MOTDtext = "[ OutDated version ]";
                    CoCTitle = "[ OutDated version ]";
                    CoCText = "[ Current version is outdated \n please upgrade to the newest version ]";
                    

                    

                    GorillaComputer.instance.GeneralFailureMessage("Hatchson Menu Temp is outdated, please switch\ndownload new version in the server");

                }
                if (Real_Version == "disabled")
                {
                    MOTD = "[ Diablo Menu ]";
                    MOTDtext = "[ Menu is currently disabled \n check the discord ]";
                    CoCTitle = "[ Menu is currently disabled \n check the discord ]";
                    CoCText = "[ Current version is disabled \n check the discord and be patient ]";

                    

                    GorillaComputer.instance.GeneralFailureMessage("Diablo Menu is curently Disabled \nplease go to the discord for any furthur information  ");
                    await Task.Delay(3000);
                    PhotonNetwork.Disconnect();
                }
            }
            catch (Exception ex)
            {
                
            }
            await Task.Delay(500);
        }
        public static void CompareGorillatagAppVersions()
        {
            try
            {
                System.Net.WebClient WebClient = new System.Net.WebClient();
                string LastKnownVersion = WebClient.DownloadString("https://raw.githubusercontent.com/THEURL").Trim();
                string version = PhotonNetwork.AppVersion;
                if (version != LastKnownVersion)
                {
                    Debug.LogError("Gorilla tag app version does not match last recorded app version (App version: " + version + " Last Known Version: " + LastKnownVersion + " )");
                    
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        public static Material? originalMat1;
        public static Material? originalMat2;
        public static void ChangeBoardMaterial(string parentPath, string boardID, int targetIndex, Material newMaterial, ref Material originalMat)
        {
            GameObject parent = GameObject.Find(parentPath);
            if (parent == null)
                return;
            int currentIndex = 0;
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                GameObject childObj = parent.transform.GetChild(i).gameObject;
                if (childObj.name.Contains(boardID))
                {
                    currentIndex++;
                    if (currentIndex == targetIndex)
                    {
                        Renderer renderer = childObj.GetComponent<Renderer>();
                        if (originalMat == null)
                            originalMat = renderer.material;
                        renderer.material = newMaterial;
                        break;
                    }
                }
            }
        }
        public static float delay;
        public static bool contBool = false;
        public static bool prevLPrimaryState = false;
    }
}


