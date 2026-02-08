using BepInEx;
using GorillaLocomotion;
using HarmonyLib;
using StupidTemplate.Classes;
using StupidTemplate.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR;
using static StupidTemplate.Menu.Buttons;
using static StupidTemplate.Settings;
/*
 * Hello, current and future developers!
 * This is ii's Stupid Template, a base mod menu template for Gorilla Tag.
 *
 * Comments are placed around the code showing you how certain classes work, such as the settings, buttons, and notifications.
 *
 * If you need help with the template, you may join my Discord server: https://discord.gg/iidk
 * It's full of talented developers that can show you the way and how things work.
 *
 * If you want to support my, check out my Patreon: https://patreon.com/iiDk
 * Any support is appreciated, and it helps me make more free content for you all!
 *
 * Thank you, and enjoy the template!
 */
namespace StupidTemplate.Menu
{
    [HarmonyPatch(typeof(GTPlayer), "LateUpdate")]
    public class Main : MonoBehaviour
    {
        // Constant
        public static void Prefix()
        {
            try
            {
                bool wantsOpen = (!rightHanded && ControllerInputPoller.instance.leftControllerSecondaryButton) ||
                                 (rightHanded && ControllerInputPoller.instance.rightControllerSecondaryButton) ||
                                 UnityInput.Current.GetKey(keyboardButton);
                if (menu == null)
                {
                    if (wantsOpen)
                    {
                        CreateMenu();
                        if (reference == null)
                            CreateReference(rightHanded);
                        BoardManager.CreateCustomBoards();
                    }
                }
                else
                {
                    if (wantsOpen)
                    {
                        RecenterMenu(rightHanded, UnityInput.Current.GetKey(keyboardButton));
                        menu.SetActive(true);
                        if (reference != null) reference.SetActive(true);
                    }
                    else
                    {
                        menu.SetActive(false);
                        if (reference != null) reference.SetActive(false);
                    }
                }
            }
            catch (Exception exc)
            {
                Debug.LogError(string.Format("{0} // Error initializing menu: {1}\n{2}", PluginInfo.Name, exc.Message, exc.StackTrace));
            }
            // Constant part (FPS + mod execution) – unchanged
            try
            {
                if (fpsObject != null)
                    fpsObject.text = "FPS: " + Mathf.Ceil(1f / Time.unscaledDeltaTime).ToString();
                foreach (ButtonInfo button in buttons
                    .SelectMany(list => list)
                    .Where(button => button.enabled && button.method != null))
                {
                    try
                    {
                        button.method.Invoke();
                    }
                    catch (Exception exc)
                    {
                        Debug.LogError(string.Format("{0} // Error executing mod {1}: {2}\n{3}", PluginInfo.Name, button.buttonText, exc.Message, exc.StackTrace));
                    }
                }
            }
            catch (Exception exc)
            {
                Debug.LogError(string.Format("{0} // Error in mod loop: {1}\n{2}", PluginInfo.Name, exc.Message, exc.StackTrace));
            }
        }
        // Functions
        public static void CreateMenu()
        {
            // Menu Holder
            menu = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(menu.GetComponent<Rigidbody>());
            Destroy(menu.GetComponent<BoxCollider>());
            Destroy(menu.GetComponent<Renderer>());
            menu.transform.localScale = new Vector3(0.1f, 0.3f, 0.3825f);
            // Menu Background
            menuBackground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(menuBackground.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(menuBackground.GetComponent<BoxCollider>());
            menuBackground.transform.parent = menu.transform;
            menuBackground.transform.rotation = Quaternion.identity;
            menuBackground.transform.localScale = Settings.menuSize;
            menuBackground.GetComponent<Renderer>().material.color = Settings.backgroundColor.colors[0].color;
            menuBackground.transform.position = new Vector3(0.05f, 0f, 0f);
            ColorChanger colorChanger = menuBackground.AddComponent<ColorChanger>();
            colorChanger.colors = Settings.backgroundColor;
            colorChanger.Start();

            // Canvas
            canvasObject = new GameObject();
            canvasObject.transform.parent = menu.transform;
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvasScaler.dynamicPixelsPerUnit = 1000f;
            // Title and FPS
            Text text = new GameObject
            {
                transform =
                    {
                        parent = canvasObject.transform
                    }
            }.AddComponent<Text>();
            text.font = currentFont;
            text.text = PluginInfo.Name + " <color=grey>[</color><color=white>" + (pageNumber + 1).ToString() + "</color><color=grey>]</color>";
            text.fontSize = 1;
            text.color = textColors[0];
            text.supportRichText = true;
            text.fontStyle = FontStyle.Italic;
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 0;
            RectTransform component = text.GetComponent<RectTransform>();
            component.localPosition = Vector3.zero;
            component.sizeDelta = new Vector2(0.28f, 0.05f);
            component.position = new Vector3(0.06f, 0f, 0.165f);
            component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            if (fpsCounter)
            {
                fpsObject = new GameObject
                {
                    transform =
                        {
                            parent = canvasObject.transform
                        }
                }.AddComponent<Text>();
                fpsObject.font = currentFont;
                fpsObject.text = "FPS: " + Mathf.Ceil(1f / Time.unscaledDeltaTime).ToString();
                fpsObject.color = textColors[0];
                fpsObject.fontSize = 1;
                fpsObject.supportRichText = true;
                fpsObject.fontStyle = FontStyle.Italic;
                fpsObject.alignment = TextAnchor.MiddleCenter;
                fpsObject.horizontalOverflow = HorizontalWrapMode.Overflow;
                fpsObject.resizeTextForBestFit = true;
                fpsObject.resizeTextMinSize = 0;
                RectTransform component2 = fpsObject.GetComponent<RectTransform>();
                component2.localPosition = Vector3.zero;
                component2.sizeDelta = new Vector2(0.28f, 0.02f);
                component2.position = new Vector3(0.06f, 0f, 0.135f);
                component2.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            }
            // Buttons
            // Disconnect
            if (disconnectButton)
            {
                GameObject disconnectbutton = GameObject.CreatePrimitive(PrimitiveType.Cube);
                if (!UnityInput.Current.GetKey(keyboardButton))
                    disconnectbutton.layer = 2;
                Destroy(disconnectbutton.GetComponent<Rigidbody>());
                disconnectbutton.GetComponent<BoxCollider>().isTrigger = true;
                disconnectbutton.transform.parent = menu.transform;
                disconnectbutton.transform.rotation = Quaternion.identity;
                disconnectbutton.transform.localScale = new Vector3(0.09f, 0.9f, 0.08f);
                disconnectbutton.transform.localPosition = new Vector3(0.56f, 0f, 0.6f);
                disconnectbutton.GetComponent<Renderer>().material.color = buttonColors[0].colors[0].color;
                disconnectbutton.AddComponent<Classes.Button>().relatedText = "Disconnect";
                colorChanger = disconnectbutton.AddComponent<ColorChanger>();
                colorChanger.colors = buttonColors[0];
                Text discontext = new GameObject
                {
                    transform =
                            {
                                parent = canvasObject.transform
                            }
                }.AddComponent<Text>();
                discontext.text = "Disconnect";
                discontext.font = currentFont;
                discontext.fontSize = 1;
                discontext.color = textColors[0];
                discontext.alignment = TextAnchor.MiddleCenter;
                discontext.resizeTextForBestFit = true;
                discontext.resizeTextMinSize = 0;
                RectTransform rectt = discontext.GetComponent<RectTransform>();
                rectt.localPosition = Vector3.zero;
                rectt.sizeDelta = new Vector2(0.2f, 0.03f);
                rectt.localPosition = new Vector3(0.064f, 0f, 0.23f);
                rectt.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
                // or clean up


            }
            // Page Buttons
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (!UnityInput.Current.GetKey(keyboardButton))
                gameObject.layer = 2;
            Destroy(gameObject.GetComponent<Rigidbody>());
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
            gameObject.transform.parent = menu.transform;
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localScale = new Vector3(0.08f, 0.15f, 0.84f);
            gameObject.transform.localPosition = new Vector3(0.56f, 0.65f, 0);
            gameObject.GetComponent<Renderer>().material.color = buttonColors[0].colors[0].color;
            gameObject.AddComponent<Classes.Button>().relatedText = "PreviousPage";
            colorChanger = gameObject.AddComponent<ColorChanger>();
            colorChanger.colors = buttonColors[0];
            text = new GameObject
            {
                transform =
                        {
                            parent = canvasObject.transform
                        }
            }.AddComponent<Text>();
            text.font = currentFont;
            text.text = "<";
            text.fontSize = 1;
            text.color = textColors[0];
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 0;
            component = text.GetComponent<RectTransform>();
            component.localPosition = Vector3.zero;
            component.sizeDelta = new Vector2(0.2f, 0.03f);
            component.localPosition = new Vector3(0.064f, 0.195f, 0f);
            component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (!UnityInput.Current.GetKey(keyboardButton))
            {
                gameObject.layer = 2;
            }
            Destroy(gameObject.GetComponent<Rigidbody>());
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
            gameObject.transform.parent = menu.transform;
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localScale = new Vector3(0.08f, 0.15f, 0.84f);
            gameObject.transform.localPosition = new Vector3(0.56f, -0.65f, 0);
            gameObject.GetComponent<Renderer>().material.color = buttonColors[0].colors[0].color;
            gameObject.AddComponent<Classes.Button>().relatedText = "NextPage";
            colorChanger = gameObject.AddComponent<ColorChanger>();
            colorChanger.colors = buttonColors[0];
            text = new GameObject
            {
                transform =
                        {
                            parent = canvasObject.transform
                        }
            }.AddComponent<Text>();
            text.font = currentFont;
            text.text = ">";
            text.fontSize = 1;
            text.color = textColors[0];
            text.alignment = TextAnchor.MiddleCenter;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 0;
            component = text.GetComponent<RectTransform>();
            component.localPosition = Vector3.zero;
            component.sizeDelta = new Vector2(0.2f, 0.03f);
            component.localPosition = new Vector3(0.064f, -0.195f, 0f);
            component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

            // Mod Buttons
            ButtonInfo[] activeButtons = buttons[currentCategory].Skip(pageNumber * buttonsPerPage).Take(buttonsPerPage).ToArray();
            for (int i = 0; i < activeButtons.Length; i++)
                CreateButton(i * 0.1f, activeButtons[i]);

        }
        public static void MenuRoundObj(GameObject toRound)
        {
            float Bevel = 0.04f;
            Renderer ToRoundRenderer = toRound.GetComponent<Renderer>();
            GameObject BaseA = GameObject.CreatePrimitive(PrimitiveType.Cube);
            BaseA.GetComponent<Renderer>().enabled = ToRoundRenderer.enabled;
            UnityEngine.Object.Destroy(BaseA.GetComponent<Collider>());
            BaseA.transform.parent = menu.transform;
            BaseA.transform.rotation = Quaternion.identity;
            BaseA.transform.localPosition = toRound.transform.localPosition;
            BaseA.transform.localScale = toRound.transform.localScale + new Vector3(0f, Bevel * -2.55f, 0f);
            GameObject BaseB = GameObject.CreatePrimitive(PrimitiveType.Cube);
            BaseB.GetComponent<Renderer>().enabled = ToRoundRenderer.enabled;
            UnityEngine.Object.Destroy(BaseB.GetComponent<Collider>());
            BaseB.transform.parent = menu.transform;
            BaseB.transform.rotation = Quaternion.identity;
            BaseB.transform.localPosition = toRound.transform.localPosition;
            BaseB.transform.localScale = toRound.transform.localScale + new Vector3(0f, 0f, -Bevel * 2f);
            GameObject RoundCornerA = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            RoundCornerA.GetComponent<Renderer>().enabled = ToRoundRenderer.enabled;
            UnityEngine.Object.Destroy(RoundCornerA.GetComponent<Collider>());
            RoundCornerA.transform.parent = menu.transform;
            RoundCornerA.transform.rotation = Quaternion.identity * Quaternion.Euler(0f, 0f, 90f);
            RoundCornerA.transform.localPosition = toRound.transform.localPosition + new Vector3(0f, (toRound.transform.localScale.y / 2f) - (Bevel * 1.275f), (toRound.transform.localScale.z / 2f) - Bevel);
            RoundCornerA.transform.localScale = new Vector3(Bevel * 2.55f, toRound.transform.localScale.x / 2f, Bevel * 2f);
            GameObject RoundCornerB = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            RoundCornerB.GetComponent<Renderer>().enabled = ToRoundRenderer.enabled;
            UnityEngine.Object.Destroy(RoundCornerB.GetComponent<Collider>());
            RoundCornerB.transform.parent = menu.transform;
            RoundCornerB.transform.rotation = Quaternion.identity * Quaternion.Euler(0f, 0f, 90f);
            RoundCornerB.transform.localPosition = toRound.transform.localPosition + new Vector3(0f, -(toRound.transform.localScale.y / 2f) + (Bevel * 1.275f), (toRound.transform.localScale.z / 2f) - Bevel);
            RoundCornerB.transform.localScale = new Vector3(Bevel * 2.55f, toRound.transform.localScale.x / 2f, Bevel * 2f);
            GameObject RoundCornerC = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            RoundCornerC.GetComponent<Renderer>().enabled = ToRoundRenderer.enabled;
            UnityEngine.Object.Destroy(RoundCornerC.GetComponent<Collider>());
            RoundCornerC.transform.parent = menu.transform;
            RoundCornerC.transform.rotation = Quaternion.identity * Quaternion.Euler(0f, 0f, 90f);
            RoundCornerC.transform.localPosition = toRound.transform.localPosition + new Vector3(0f, (toRound.transform.localScale.y / 2f) - (Bevel * 1.275f), -(toRound.transform.localScale.z / 2f) + Bevel);
            RoundCornerC.transform.localScale = new Vector3(Bevel * 2.55f, toRound.transform.localScale.x / 2f, Bevel * 2f);
            GameObject RoundCornerD = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            RoundCornerD.GetComponent<Renderer>().enabled = ToRoundRenderer.enabled;
            UnityEngine.Object.Destroy(RoundCornerD.GetComponent<Collider>());
            RoundCornerD.transform.parent = menu.transform;
            RoundCornerD.transform.rotation = Quaternion.identity * Quaternion.Euler(0f, 0f, 90f);
            RoundCornerD.transform.localPosition = toRound.transform.localPosition + new Vector3(0f, -(toRound.transform.localScale.y / 2f) + (Bevel * 1.275f), -(toRound.transform.localScale.z / 2f) + Bevel);
            RoundCornerD.transform.localScale = new Vector3(Bevel * 2.55f, toRound.transform.localScale.x / 2f, Bevel * 2f);
            GameObject[] ToChange = new GameObject[]
            {
         BaseA,
         BaseB,
         RoundCornerA,
         RoundCornerB,
         RoundCornerC,
         RoundCornerD
            };
        }
        public static void CreateButton(float offset, ButtonInfo method)
        {
            GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (!UnityInput.Current.GetKey(keyboardButton))
                gameObject.layer = 2;

            Destroy(gameObject.GetComponent<Rigidbody>());
            gameObject.GetComponent<BoxCollider>().isTrigger = true;
            gameObject.transform.parent = menu.transform;
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localScale = new Vector3(0.09f, 0.9f, 0.08f);
            gameObject.transform.localPosition = new Vector3(0.56f, 0f, 0.28f - offset);
            gameObject.AddComponent<Classes.Button>().relatedText = method.buttonText;
            ColorChanger colorChanger = gameObject.AddComponent<ColorChanger>();
            colorChanger.colors = method.enabled ? buttonColors[1] : buttonColors[0];
            Text text = new GameObject
            {
                transform =
                {
                    parent = canvasObject.transform
                }
            }.AddComponent<Text>();
            text.font = currentFont;
            text.text = method.buttonText;
            if (method.overlapText != null)
                text.text = method.overlapText;

            text.supportRichText = true;
            text.fontSize = 1;
            text.color = method.enabled ? textColors[1] : textColors[0];
            text.alignment = TextAnchor.MiddleCenter;
            text.fontStyle = FontStyle.Italic;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 0;
            RectTransform component = text.GetComponent<RectTransform>();
            component.localPosition = Vector3.zero;
            component.sizeDelta = new Vector2(.2f, .03f);
            component.localPosition = new Vector3(.064f, 0, .111f - offset / 2.6f);
            component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
        }
        public static void RecreateMenu()
        {
            if (menu != null)
            {
                Destroy(menu);
                menu = null;
                CreateMenu();
                RecenterMenu(rightHanded, UnityInput.Current.GetKey(keyboardButton));
            }
        }
        public static void RecenterMenu(bool isRightHanded, bool isKeyboardCondition)
        {
            if (menu == null) return;
            // Get main VR camera
            Camera playerCamera = Camera.main;
            if (playerCamera == null)
            {
                playerCamera = GameObject.FindObjectOfType<Camera>();

                if (playerCamera == null) return;
            }
            // Position: 0.69f in front + slight up offset
            Vector3 targetPosition = playerCamera.transform.position + playerCamera.transform.forward * 0.65f;
            targetPosition += playerCamera.transform.up * -0.15f; // lift a bit so not in your eyes
            // Smooth position
            menu.transform.position = Vector3.Lerp(menu.transform.position, targetPosition, Time.deltaTime * 20f);
            // Rotation: menu faces you, text upright, disconnect at top
            // Inside RecenterMenu, after setting position...
            // Direction the menu should face: straight toward the player's eyes
            Vector3 toPlayer = playerCamera.transform.position - menu.transform.position;
            toPlayer.y = 0f; // Optional: ignore vertical for flatter look (remove if you want full look-at)
            // LookRotation makes the menu's forward point toward you
            Quaternion baseLook = Quaternion.LookRotation(toPlayer.normalized, playerCamera.transform.up);
            // Final adjustments to make text upright and readable:
            // - Y 180° to flip horizontally (prevents mirror/backwards text)
            // - Z 180° to flip vertically if upside-down (test 0f or 180f here)
            Quaternion finalRot = baseLook * Quaternion.Euler(-90f, -90f, 0f);
            // Or try these alternatives if the above is still off:
            // finalRot = baseLook * Quaternion.Euler(180f, 0f, 0f);
            // finalRot = baseLook * Quaternion.Euler(0f, 180f, 0f);
            // finalRot = baseLook * Quaternion.Euler(180f, 180f, 180f);
            menu.transform.rotation = Quaternion.Slerp(menu.transform.rotation, finalRot, Time.deltaTime * 25f);
            // Pointer sphere follows right hand (for touching buttons)
            if (reference != null)
            {
                Transform rightHand = GorillaTagger.Instance.rightHandTransform;
                reference.transform.position = rightHand.position + rightHand.forward * 0f + rightHand.up * -0.1f;
                reference.transform.rotation = rightHand.rotation;
                reference.SetActive(true);
            }
            // Optional: disable old shoulder camera stuff completely
            try
            {
                var shoulderCam = GameObject.Find("Shoulder Camera");
                if (shoulderCam != null)
                    shoulderCam.transform.Find("CM vcam1")?.gameObject.SetActive(false);
            }
            catch { }
        }
        public static void CreateReference(bool isRightHanded)
        {
            reference = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            reference.transform.parent = isRightHanded ? GorillaTagger.Instance.leftHandTransform : GorillaTagger.Instance.rightHandTransform;
            reference.GetComponent<Renderer>().material.color = backgroundColor.colors[0].color;
            reference.transform.localPosition = new Vector3(0f, -0.1f, 0f);
            reference.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            buttonCollider = reference.GetComponent<SphereCollider>();
            ColorChanger colorChanger = reference.AddComponent<ColorChanger>();
            colorChanger.colors = backgroundColor;
        }
        public static void Toggle(string buttonText)
        {
            int lastPage = ((buttons[currentCategory].Length + buttonsPerPage - 1) / buttonsPerPage) - 1;
            if (buttonText == "PreviousPage")
            {
                pageNumber--;
                if (pageNumber < 0)
                    pageNumber = lastPage;
            }
            else
            {
                if (buttonText == "NextPage")
                {
                    pageNumber++;
                    if (pageNumber > lastPage)
                        pageNumber = 0;
                }
                else
                {
                    ButtonInfo target = GetIndex(buttonText);
                    if (target != null)
                    {
                        if (target.isTogglable)
                        {
                            target.enabled = !target.enabled;
                            if (target.enabled)
                            {
                                NotifiLib.SendNotification("<color=grey>[</color><color=green>ENABLE</color><color=grey>]</color> " + target.toolTip);
                                if (target.enableMethod != null)
                                    try { target.enableMethod.Invoke(); } catch { }
                            }
                            else
                            {
                                NotifiLib.SendNotification("<color=grey>[</color><color=red>DISABLE</color><color=grey>]</color> " + target.toolTip);
                                if (target.disableMethod != null)
                                    try { target.disableMethod.Invoke(); } catch { }
                            }
                        }
                        else
                        {
                            NotifiLib.SendNotification("<color=grey>[</color><color=green>ENABLE</color><color=grey>]</color> " + target.toolTip);
                            if (target.method != null)
                                try { target.method.Invoke(); } catch { }
                        }
                    }
                    else
                        Debug.LogError(buttonText + " does not exist");
                }
            }
            RecreateMenu();
        }
        private static readonly Dictionary<string, (int Category, int Index)> cacheGetIndex = new Dictionary<string, (int Category, int Index)>(); // Looping through 800 elements is not a light task :/
        public static ButtonInfo GetIndex(string buttonText)
        {
            if (buttonText == null)
                return null;
            if (cacheGetIndex.ContainsKey(buttonText))
            {
                var CacheData = cacheGetIndex[buttonText];
                try
                {
                    if (buttons[CacheData.Category][CacheData.Index].buttonText == buttonText)
                        return buttons[CacheData.Category][CacheData.Index];
                }
                catch { cacheGetIndex.Remove(buttonText); }
            }
            int categoryIndex = 0;
            foreach (ButtonInfo[] buttons in buttons)
            {
                int buttonIndex = 0;
                foreach (ButtonInfo button in buttons)
                {
                    if (button.buttonText == buttonText)
                    {
                        try
                        {
                            cacheGetIndex.Add(buttonText, (categoryIndex, buttonIndex));
                        }
                        catch
                        {
                            if (cacheGetIndex.ContainsKey(buttonText))
                                cacheGetIndex.Remove(buttonText);
                        }
                        return button;
                    }
                    buttonIndex++;
                }
                categoryIndex++;
            }
            return null;
        }
        public static Vector3 RandomVector3(float range = 1f) =>
            new Vector3(UnityEngine.Random.Range(-range, range),
                        UnityEngine.Random.Range(-range, range),
                        UnityEngine.Random.Range(-range, range));
        public static Quaternion RandomQuaternion(float range = 360f) =>
            Quaternion.Euler(UnityEngine.Random.Range(0f, range),
                        UnityEngine.Random.Range(0f, range),
                        UnityEngine.Random.Range(0f, range));
        public static Color RandomColor(byte range = 255, byte alpha = 255) =>
            new Color32((byte)UnityEngine.Random.Range(0, range),
                        (byte)UnityEngine.Random.Range(0, range),
                        (byte)UnityEngine.Random.Range(0, range),
                        alpha);
        public static (Vector3 position, Quaternion rotation, Vector3 up, Vector3 forward, Vector3 right) TrueLeftHand()
        {
            Quaternion rot = GorillaTagger.Instance.leftHandTransform.rotation * GTPlayer.Instance.LeftHand.handRotOffset;
            return (GorillaTagger.Instance.leftHandTransform.position + GorillaTagger.Instance.leftHandTransform.rotation * GTPlayer.Instance.LeftHand.handOffset, rot, rot * Vector3.up, rot * Vector3.forward, rot * Vector3.right);
        }
        public static (Vector3 position, Quaternion rotation, Vector3 up, Vector3 forward, Vector3 right) TrueRightHand()
        {
            Quaternion rot = GorillaTagger.Instance.rightHandTransform.rotation * GTPlayer.Instance.RightHand.handRotOffset;
            return (GorillaTagger.Instance.rightHandTransform.position + GorillaTagger.Instance.rightHandTransform.rotation * GTPlayer.Instance.RightHand.handOffset, rot, rot * Vector3.up, rot * Vector3.forward, rot * Vector3.right);
        }
        public static void WorldScale(GameObject obj, Vector3 targetWorldScale)
        {
            Vector3 parentScale = obj.transform.parent.lossyScale;
            obj.transform.localScale = new Vector3(
                targetWorldScale.x / parentScale.x,
                targetWorldScale.y / parentScale.y,
                targetWorldScale.z / parentScale.z
            );
        }
        public static void FixStickyColliders(GameObject platform)
        {
            Vector3[] localPositions = new Vector3[]
            {
                new Vector3(0, 1f, 0),
                new Vector3(0, -1f, 0),
                new Vector3(1f, 0, 0),
                new Vector3(-1f, 0, 0),
                new Vector3(0, 0, 1f),
                new Vector3(0, 0, -1f)
            };
            Quaternion[] localRotations = new Quaternion[]
            {
                Quaternion.Euler(90, 0, 0),
                Quaternion.Euler(-90, 0, 0),
                Quaternion.Euler(0, -90, 0),
                Quaternion.Euler(0, 90, 0),
                Quaternion.identity,
                Quaternion.Euler(0, 180, 0)
            };
            for (int i = 0; i < localPositions.Length; i++)
            {
                GameObject side = GameObject.CreatePrimitive(PrimitiveType.Cube);
                try
                {
                    if (platform.GetComponent<GorillaSurfaceOverride>() != null)
                    {
                        side.AddComponent<GorillaSurfaceOverride>().overrideIndex = platform.GetComponent<GorillaSurfaceOverride>().overrideIndex;
                    }
                }
                catch { }
                float size = 0.025f;
                side.transform.SetParent(platform.transform);
                side.transform.position = localPositions[i] * (size / 2);
                side.transform.rotation = localRotations[i];
                WorldScale(side, new Vector3(size, size, 0.01f));
                side.GetComponent<Renderer>().enabled = false;
            }
        }
        private static int? noInvisLayerMask;
        public static int NoInvisLayerMask()
        {
            noInvisLayerMask ??= ~(
                1 << LayerMask.NameToLayer("TransparentFX") |
                1 << LayerMask.NameToLayer("Ignore Raycast") |
                1 << LayerMask.NameToLayer("Zone") |
                1 << LayerMask.NameToLayer("Gorilla Trigger") |
                1 << LayerMask.NameToLayer("Gorilla Boundary") |
                1 << LayerMask.NameToLayer("GorillaCosmetics") |
                1 << LayerMask.NameToLayer("GorillaParticle"));
            return noInvisLayerMask ?? GTPlayer.Instance.locomotionEnabledLayers;
        }
        public static bool gunLocked;
        public static VRRig lockTarget;
        public static (RaycastHit Ray, GameObject NewPointer) RenderGun(int? overrideLayerMask = null)
        {
            Transform GunTransform = GorillaTagger.Instance.rightHandTransform;
            Vector3 StartPosition = GunTransform.position;
            Vector3 Direction = GunTransform.forward;
            Physics.Raycast(StartPosition + Direction / 4f, Direction, out var Ray, 512f, overrideLayerMask ?? NoInvisLayerMask());
            Vector3 EndPosition = gunLocked ? lockTarget.transform.position : Ray.point;
            if (EndPosition == Vector3.zero)
                EndPosition = StartPosition + Direction * 512f;
            if (GunPointer == null)
                GunPointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            GunPointer.SetActive(true);
            GunPointer.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            GunPointer.transform.position = EndPosition;
            Renderer PointerRenderer = GunPointer.GetComponent<Renderer>();
            PointerRenderer.material.shader = Shader.Find("GUI/Text Shader");
            PointerRenderer.material.color = gunLocked || ControllerInputPoller.TriggerFloat(XRNode.RightHand) > 0.5f ? buttonColors[1].GetCurrentColor() : buttonColors[0].GetCurrentColor();
            Destroy(GunPointer.GetComponent<Collider>());
            if (GunLine == null)
            {
                GameObject line = new GameObject("iiMenu_GunLine");
                GunLine = line.AddComponent<LineRenderer>();
            }
            GunLine.gameObject.SetActive(true);
            GunLine.material.shader = Shader.Find("GUI/Text Shader");
            GunLine.startColor = backgroundColor.GetCurrentColor();
            GunLine.endColor = backgroundColor.GetCurrentColor(0.5f);
            GunLine.startWidth = 0.025f;
            GunLine.endWidth = 0.025f;
            GunLine.useWorldSpace = true;
            // Wavy line modification starts here
            const int numPoints = 128; // Higher for smoother waves
            GunLine.positionCount = numPoints;
            Vector3 start = StartPosition;
            Vector3 end = EndPosition;
            Vector3 dir = (end - start).normalized;
            // Compute two orthogonal perpendicular vectors for 3D waving
            Vector3 perp1 = Vector3.Cross(dir, Vector3.up);
            if (perp1.sqrMagnitude < 0.001f)
                perp1 = Vector3.Cross(dir, Vector3.right);
            perp1 = perp1.normalized;
            Vector3 perp2 = Vector3.Cross(dir, perp1).normalized;
            float phase = Time.time * 8f; // Wave speed
            float amp1 = 0.1f; // Primary wave amplitude
            float amp2 = 0.5f; // Secondary wave amplitude (smaller for subtlety)
            float freq1 = 35f; // Primary frequency (waves along the line)
            float freq2 = 50f; // Secondary frequency (slightly different for organic feel)
            for (int i = 0; i < numPoints; i++)
            {
                float t = i / (float)(numPoints - 1);
                // Base position along the straight line
                Vector3 pos = Vector3.Lerp(start, end, t);
                // Smooth fade for wave amplitude at start/end to keep endpoints fixed
                float edgeFade = Mathf.SmoothStep(0f, 0.1f, t) * Mathf.SmoothStep(1f, 0.92f, t);
                // Compute waves
                float wave1 = Mathf.Sin(t * Mathf.PI * freq1 + phase) * amp1 * edgeFade;
                float wave2 = Mathf.Cos(t * Mathf.PI * freq2 + phase * 1.1f) * amp2 * edgeFade;
                // Apply offsets
                pos += perp1 * wave1;
                pos += perp2 * wave2;
                GunLine.SetPosition(i, pos);
            }
            // Wavy line modification ends here
            if (!ControllerInputPoller.instance.rightGrab)
            {
                GameObject.Destroy(GunPointer);
                GameObject.Destroy(GunLine);
            }
            return (Ray, GunPointer);

        }
        // Variables
        // Important
        // Objects
        public static GameObject menu;
        public static GameObject menuBackground;
        public static GameObject reference;
        public static GameObject canvasObject;
        public static SphereCollider buttonCollider;
        public static Camera TPC;
        public static Text fpsObject;
        private static GameObject GunPointer;
        private static LineRenderer GunLine;
        // Data
        public static int pageNumber = 0;
        public static int _currentCategory;
        public static int currentCategory
        {
            get => _currentCategory;
            set
            {
                _currentCategory = value;
                pageNumber = 0;
            }
        }
    }
}