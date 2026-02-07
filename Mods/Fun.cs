using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace rainxyz.Mods
{
    internal class Fun
    {
        public static void BugTeleport()
        {
            if (GameObject.Find("Floating Bug Holdable").GetComponent<ThrowableBug>().targetRig == VRRig.LocalRig)
            {
                GameObject.Find("Floating Bug Holdable").transform.position = Camera.main.transform.position + Camera.main.transform.up * 0.2f;
            }

        }
        public static void BugHead()
        {
            if (GameObject.Find("Floating Bug Holdable").GetComponent<ThrowableBug>().targetRig == VRRig.LocalRig)
            {
                GameObject.Find("Floating Bug Holdable").transform.position = Camera.main.transform.position + Camera.main.transform.up * -0.5f;
            }

        }
        public static void TelekenisisBug()
        {
            if (GameObject.Find("Floating Bug Holdable").GetComponent<ThrowableBug>().targetRig == VRRig.LocalRig)
            {
                GameObject.Find("Floating Bug Holdable").transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1f;
            }
        }
        public static void BugSpaz()
        {
            if (GameObject.Find("Floating Bug Holdable").GetComponent<ThrowableBug>().targetRig == VRRig.LocalRig)
            {
                GameObject.Find("Floating Bug Holdable").transform.Rotate(1000f, 1000f * Time.deltaTime, 1000f);
            }
        }
        public static void BugMixer()
        {
            if (GameObject.Find("Floating Bug Holdable").GetComponent<ThrowableBug>().targetRig == VRRig.LocalRig)
            {
                GameObject.Find("Floating Bug Holdable").transform.Rotate(0f, 1000f * Time.deltaTime, 0f);
            }
        }
        public static void BugEsp()
        {

            if (GameObject.Find("Floating Bug Holdable").GetComponent<ThrowableBug>().targetRig == VRRig.LocalRig)
            {   
                    UnityEngine.Color thecolor = Color.blue;
                    GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    box.transform.position = GameObject.Find("Floating Bug Holdable").transform.position;
                    UnityEngine.Object.Destroy(box.GetComponent<BoxCollider>());
                    box.transform.localScale = new Vector3(0.3f, 0.3f, 0f);
                    box.transform.LookAt(GorillaTagger.Instance.headCollider.transform.position);
                    box.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                    box.GetComponent<Renderer>().material.color = thecolor;
                    UnityEngine.Object.Destroy(box, Time.deltaTime);
                
            }
        }
    }
}
