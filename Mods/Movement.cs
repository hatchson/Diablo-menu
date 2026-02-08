using GorillaLocomotion;
using StupidTemplate.Classes;
using UnityEngine;
using UnityEngine.XR;
using static StupidTemplate.Menu.Main;

namespace StupidTemplate.Mods
{
    public class Movement
    {
        public static void Fly()
        {
            if (ControllerInputPoller.instance.rightControllerPrimaryButton)
            {
                GTPlayer.Instance.transform.position += GorillaTagger.Instance.headCollider.transform.forward * Time.deltaTime * Settings.Movement.flySpeed;
                GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
            }
        }

        public static GameObject platl;
        public static GameObject platr;

        public static void Platforms()
        {
            if (ControllerInputPoller.instance.leftGrab)
            {
                if (platl == null)
                {
                    platl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    platl.transform.localScale = new Vector3(0.025f, 0.3f, 0.4f);
                    platl.transform.position = GorillaTagger.Instance.leftHandTransform.position;
                    platl.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;
                    platl.GetComponent<Renderer>().material.color = Color.lightBlue;
                }
            }
            else
            {
                if (platl != null)
                {
                    UnityEngine.Object.Destroy(platl);
                    platl = null;
                }
            }

            if (ControllerInputPoller.instance.rightGrab)
            {
                if (platr == null)
                {
                    platr = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    platr.transform.localScale = new Vector3(0.025f, 0.3f, 0.4f);
                    platr.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                    platr.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
                    platr.GetComponent<Renderer>().material.color = Color.lightCyan;
                }
            }
            else
            {
                if (platr != null)
                {
                    UnityEngine.Object.Destroy(platr);
                    platr = null;
                }
            }
        }

        public static void InvisPlatforms()
        {
            if (ControllerInputPoller.instance.leftGrab)
            {
                if (platl == null)
                {
                    platl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    platl.transform.localScale = new Vector3(0.025f, 0.3f, 0.4f);
                    platl.transform.position = GorillaTagger.Instance.leftHandTransform.position;
                    platl.transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;
                    Object.Destroy(platl.GetComponent<Renderer>());

                }
            }
            else
            {
                if (platl != null)
                {
                    UnityEngine.Object.Destroy(platl);
                    platl = null;
                }
            }

            if (ControllerInputPoller.instance.rightGrab)
            {
                if (platr == null)
                {
                    platr = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    platr.transform.localScale = new Vector3(0.025f, 0.3f, 0.4f);
                    platr.transform.position = GorillaTagger.Instance.rightHandTransform.position;
                    platr.transform.rotation = GorillaTagger.Instance.rightHandTransform.rotation;
                    Object.Destroy(platr.GetComponent<Renderer>());
                }
            }
            else
            {
                if (platr != null)
                {
                    UnityEngine.Object.Destroy(platr);
                    platr = null;
                }
            }
        }

        public static bool previousTeleportTrigger;
        public static void TeleportGun()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;
                

                if (ControllerInputPoller.TriggerFloat(XRNode.RightHand) > 0.5f && !previousTeleportTrigger)
                {
                    GTPlayer.Instance.TeleportTo(NewPointer.transform.position + Vector3.up, GTPlayer.Instance.transform.rotation);
                    GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.zero;
                }

                previousTeleportTrigger = ControllerInputPoller.TriggerFloat(XRNode.RightHand) > 0.5f;
            }
            else
            {
                GameObject.Destroy(RenderGun().NewPointer);
            }
        }
    }
}
