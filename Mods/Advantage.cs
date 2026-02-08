using Photon.Pun;
using StupidTemplate.Notifications;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using StupidTemplate.Mods;
using StupidTemplate.Classes;
using GorillaLocomotion;
using Fusion.Analyzer;
using GorillaTag;
using ExitGames.Client.Photon;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTagScripts;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using static StupidTemplate.Menu.Main;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Pathfinding;


namespace StupidTemplate.Mods

{
    internal class Advantages

    {
       
            
        
        public static bool IsTagged(VRRig VRRig)
        {
            string Player = VRRig.mainSkin.material.name.ToLower();
            return Player.Contains("fected");
        }
        
        public static void TagGun()
        {
            /*   var data = GunLib.Shoot();

               if (data.lockedPlayer != null && data.isTriggered)
               {
                   TagPlayer(data.lockedPlayer);
               }*/
        }
        public static GameObject soundboardAudioManager;
        public static bool AudioIsPlaying;
        public static float RecoverTime;
        public static void PlayAudio(AudioClip sound, bool disableMicrophone = false)
        {
            if (!PhotonNetwork.InRoom)
            {
                if (soundboardAudioManager == null)
                {
                    soundboardAudioManager = new GameObject("2DAudioMgr");
                    AudioSource temp = soundboardAudioManager.AddComponent<AudioSource>();
                    temp.spatialBlend = 0f;
                }

                AudioSource ausrc = soundboardAudioManager.GetComponent<AudioSource>();
                ausrc.volume = 1f;
                ausrc.clip = sound;
                ausrc.loop = false;
                ausrc.Play();

                AudioIsPlaying = true;
                RecoverTime = Time.time + sound.length;

                return;
              
            }

        }




        //ts is shit

        public static List<NetPlayer> InfectedList()
        {
            List<NetPlayer> infected = new List<NetPlayer>();

            if (!PhotonNetwork.InRoom)
                return infected;

            switch (GorillaGameManager.instance.GameType())
            {
                case GameModeType.Infection:
                case GameModeType.InfectionCompetitive:
                case GameModeType.SuperInfect:
                case GameModeType.FreezeTag:
                case GameModeType.PropHunt:
                    GorillaTagManager tagManager = (GorillaTagManager)GorillaGameManager.instance;
                    if (tagManager.isCurrentlyTag)
                        infected.Add(tagManager.currentIt);
                    else
                        infected.AddRange(tagManager.currentInfected);
                    break;
                case GameModeType.Ghost:
                case GameModeType.Ambush:
                    GorillaAmbushManager ghostManager = (GorillaAmbushManager)GorillaGameManager.instance;
                    if (ghostManager.isCurrentlyTag)
                        infected.Add(ghostManager.currentIt);
                    else
                        infected.AddRange(ghostManager.currentInfected);
                    break;
                case GameModeType.Paintbrawl:
                    GorillaPaintbrawlManager paintbrawlManager = (GorillaPaintbrawlManager)GorillaGameManager.instance;

                    infected.AddRange(paintbrawlManager.playerLives.Where(element => element.Value <= 0).Select(element => element.Key).ToArray().Select(deadPlayer => PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(deadPlayer)).Select(dummy => (NetPlayer)dummy));

                    if (!infected.Contains(NetworkSystem.Instance.LocalPlayer))
                        infected.Add(NetworkSystem.Instance.LocalPlayer);

                    break;
            }

            return infected;
        }

        public static void TagSelf()
        {
            foreach (VRRig Player in GorillaParent.instance.vrrigs)
            {
                if (Player != GorillaTagger.Instance.offlineVRRig)
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = false;
                    if (Player.mainSkin.material.name.Contains("infected"))
                    {
                        GorillaTagger.Instance.offlineVRRig.transform.position = Player.leftHandTransform.position;
                    }
                    if (InfectedList().Contains(PhotonNetwork.LocalPlayer))
                    {

                        GorillaTagger.Instance.offlineVRRig.enabled = true;




                    }

                }



            }

        }




        public static void InstantRoundEnd()
        {
                VRRig localRig = Advantages.GetYourOwnRig(true);
                bool istagged = Advantages.IsTaggedv2(localRig);
            if (localRig == null || !Advantages.IsTaggedv2(localRig)) return;

                Vector3 prev_selfpos = localRig.transform.position;

            foreach (VRRig targetRig in GorillaParent.instance.vrrigs)
            {
                if (targetRig == null || targetRig.isLocal) continue;

                if (!Advantages.IsTaggedv2(targetRig))
                {
                    PhotonView photonView = GameObject
                        .Find("Player Objects/RigCache/Network Parent/GameMode(Clone)")
                        ?.GetComponent<PhotonView>();

                    if (photonView == null)
                    {
                        UnityEngine.Debug.LogError("PhotonView not found.");
                        continue;
                    }

                    GorillaTagger.Instance.offlineVRRig.enabled = false;
                    GorillaTagger.Instance.offlineVRRig.transform.position = targetRig.transform.position + new Vector3(0f, -0.2f, 0f);
                    localRig.transform.position = targetRig.transform.position;

                    Advantages.SendViewUpdate();

                    GorillaGameModes.GameMode.ReportTag(Advantages.VRRIGtoNetPlayer(targetRig));

                    photonView.RPC("RPC_ReportTag", RpcTarget.Others, new object[] { targetRig.Creator.ActorNumber });
                    photonView.RPC("RPC_ReportTag", RpcTarget.Others, new object[] { targetRig.OwningNetPlayer.ActorNumber });

                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                    localRig.transform.position = prev_selfpos;
                    Advantages.SendViewUpdate();
                }
                
            }
        }
        public static void BreakInfection()
        {
            VRRig localRig = Advantages.GetYourOwnRig(true);
            bool istagged = Advantages.IsTaggedv2(localRig);

            if (localRig == null || !Advantages.IsTaggedv2(localRig)) return;

            Vector3 prev_selfpos = localRig.transform.position;

            foreach (VRRig targetRig in GorillaParent.instance.vrrigs)
            {
                if (targetRig == null || targetRig.isLocal) continue;

                if (!Advantages.IsTaggedv2(targetRig))
                {
                    PhotonView photonView = GameObject
                        .Find("Player Objects/RigCache/Network Parent/GameMode(Clone)")
                        ?.GetComponent<PhotonView>();

                    if (photonView == null)
                    {
                        UnityEngine.Debug.LogError("PhotonView not found.");
                        continue;
                    }

                    GorillaTagger.Instance.offlineVRRig.enabled = false;
                    GorillaTagger.Instance.offlineVRRig.transform.position = targetRig.transform.position + new Vector3(0f, -0.2f, 0f);
                    localRig.transform.position = targetRig.transform.position;

                    Advantages.SendViewUpdate();

                    GorillaGameModes.GameMode.ReportTag(Advantages.VRRIGtoNetPlayer(targetRig));

                    photonView.RPC("RPC_ReportTag", RpcTarget.Others, new object[] { targetRig.Creator.ActorNumber });
                    photonView.RPC("RPC_ReportTag", RpcTarget.Others, new object[] { targetRig.OwningNetPlayer.ActorNumber });

                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                    localRig.transform.position = prev_selfpos;
                    Advantages.SendViewUpdate();
                    TagSelf();
                }
            } 
        }
        public static void VoldemortWand()
        {

            GameObject wand = GameObject.CreatePrimitive(PrimitiveType.Cube);

            wand = GameObject.CreatePrimitive(PrimitiveType.Cube);
            UnityEngine.Object.Destroy(wand.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(wand.GetComponent<BoxCollider>());
            wand.transform.localScale = new Vector3(0.05f, 0.05f, 0.2f);
            wand.transform.position = VRRig.LocalRig.rightHandTransform.position;
            wand.transform.localPosition = VRRig.LocalRig.rightHandTransform.position;
            GameObject.Destroy(wand, Time.deltaTime);

        }

        public static void TagPlayerv2(VRRig VRRig)
        {
            if (VRRig != null)
            {

                Vector3 prev_selfpos = Advantages.GetYourOwnRig().transform.position;
                if (!IsTagged(VRRig) && IsTagged(Advantages.GetYourOwnRig(true)))
                {
                    PhotonView photonView = GameObject.Find("Player Objects/RigCache/Network Parent/GameMode(Clone)").GetComponent<Photon.Pun.PhotonView>();
                    GorillaTagger.Instance.offlineVRRig.enabled = false;
                    GorillaTagger.Instance.offlineVRRig.transform.position = VRRig.transform.position + new Vector3(0f, -0.2f, 0f);
                    Advantages.GetYourOwnRig().transform.position = VRRig.transform.position;

                    Advantages.SendViewUpdate();
                    GorillaGameModes.GameMode.ReportTag(Advantages.VRRIGtoNetPlayer(VRRig));
                    photonView.RPC("RPC_ReportTag", RpcTarget.Others, new object[]
                       {
                     VRRig.Creator.ActorNumber
                       });
                    photonView.RPC("RPC_ReportTag", RpcTarget.Others, new object[]
                    {
                     VRRig.OwningNetPlayer.ActorNumber
                    });

                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                    Advantages.GetYourOwnRig().transform.position = prev_selfpos;
                    Advantages.SendViewUpdate();

                    if (photonView == null) UnityEngine.Debug.LogError("Photonview is null");
                }
            }
        }


        public static VRRig GetYourOwnRig(bool UseGorillaTagger = false)
        {
            if (UseGorillaTagger)
            {
                return GorillaTagger.Instance.offlineVRRig;
            }
            else
            {
                return VRRig.LocalRig;
            }
        }




        public static NetPlayer VRRIGtoNetPlayer(VRRig VRRig)
        {
            return VRRig.Creator;
        }

        public static void SendViewUpdate()
        {
            MethodInfo method = typeof(PhotonNetwork).GetMethod("RunViewUpdate", BindingFlags.Static | BindingFlags.NonPublic);

            method.Invoke(typeof(PhotonNetwork), Array.Empty<object>());
        }
        public static readonly Dictionary<int, float> LastTaggedTime = new Dictionary<int, float>();






        public static void TagGunv2()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                var GunData = RenderGun();
                GameObject NewPointer = GunData.NewPointer;

                if (ControllerInputPoller.TriggerFloat(XRNode.RightHand) > 0.5f)
                {
                    TagPlayerv2(lockTarget);
                }

            }
            else
            {
                GameObject.Destroy(RenderGun().NewPointer);
            }
        }



        public static int TagDistanceCycle = 1;
        public static float TagDistance = 3f;

        public static void ChangeTagDistance()
        {
            var presets = new List<(float distance, string label)>
        {
        (10f, "MAX"),
        (1f,   "CLOSE"),
        (5f,   "MEDIUM"),
        (6f,   "FAR"),
        (9f,   "INSANE")
        };

            TagDistanceCycle = (TagDistanceCycle + 1) % presets.Count;
            var (dist, label) = presets[TagDistanceCycle];

            TagDistance = dist;
            NotifiLib.SendNotification($"Current Distance: [{label}]");
        }



        public static void TagAuraTuff()
        {
            var tagger = GorillaTagger.Instance;
            var rigs = GorillaParent.instance.vrrigs;
            var left = tagger.leftHandTransform.position;
            var right = tagger.rightHandTransform.position;

            foreach (var rig in rigs)
            {
                if (rig == tagger.offlineVRRig) continue;

                var head = rig.headMesh.transform.position;
                var inRange = Vector3.Distance(left, head) < Advantages.TagDistance || Vector3.Distance(right, head) < Advantages.TagDistance;
                if (!inRange) continue;

                var obj = GameObject.Find("Player Objects/RigCache/Network Parent/GameMode(Clone)");
                var view = PunExtensions.GetPhotonView(obj);
                if (view != null)
                {
                    view.RPC("RPC_ReportTag", RpcTarget.All, rig.Creator.ActorNumber);
                }
            }
        }
        public static float TagHandDistance = 1f;
        public static void TagReach()
        {
            var tagger = GorillaTagger.Instance;
            var left = tagger.leftHandTransform;
            var right = tagger.rightHandTransform;

            UnityEngine.Object.Destroy(left, Time.deltaTime);
            UnityEngine.Object.Destroy(right, Time.deltaTime);

            var rigs = GorillaParent.instance.vrrigs;
            var distance = TagHandDistance;

            foreach (var rig in rigs)
            {
                if (rig == tagger.offlineVRRig) continue;

                var head = rig.headMesh.transform.position;
                var lDist = Vector3.Distance(tagger.leftHandTransform.position, head);
                var rDist = Vector3.Distance(tagger.rightHandTransform.position, head);

                if (lDist < distance || rDist < distance)
                {
                    var obj = GameObject.Find("Player Objects/RigCache/Network Parent/GameMode(Clone)");
                    var view = PunExtensions.GetPhotonView(obj);
                    view?.RPC("RPC_ReportTag", RpcTarget.All, rig.Creator.ActorNumber);
                }
            }
        }
        public static bool IsTaggedv2(VRRig VRRig)
        {
            string Player = VRRig.mainSkin.material.name.ToLower();
            return Player.Contains("fected");
        }
        public static void TagAllV2()
        {
            VRRig localRig = Advantages.GetYourOwnRig(true);
            if (localRig == null || !Advantages.IsTaggedv2(localRig)) return;

            Vector3 prev_selfpos = localRig.transform.position;

            foreach (VRRig targetRig in GorillaParent.instance.vrrigs)
            {
                if (targetRig == null || targetRig.isLocal) continue;

                if (!Advantages.IsTaggedv2(targetRig))
                {
                    PhotonView photonView = GameObject
                        .Find("Player Objects/RigCache/Network Parent/GameMode(Clone)")
                        ?.GetComponent<PhotonView>();

                    if (photonView == null)
                    {
                        UnityEngine.Debug.LogError("PhotonView not found.");
                        continue;
                    }

                    GorillaTagger.Instance.offlineVRRig.enabled = false;
                    GorillaTagger.Instance.offlineVRRig.transform.position = targetRig.transform.position + new Vector3(0f, -0.2f, 0f);
                    localRig.transform.position = targetRig.transform.position;

                    Advantages.SendViewUpdate();

                    GorillaGameModes.GameMode.ReportTag(Advantages.VRRIGtoNetPlayer(targetRig));

                    photonView.RPC("RPC_ReportTag", RpcTarget.Others, new object[] { targetRig.Creator.ActorNumber });
                    photonView.RPC("RPC_ReportTag", RpcTarget.Others, new object[] { targetRig.OwningNetPlayer.ActorNumber });

                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                    localRig.transform.position = prev_selfpos;
                    Advantages.SendViewUpdate();
                }
            }
        }

        //credits pugs
        public static float range = 10f;
        public static float Notifdelay;
        public static void NotifyWhenLavaIsNear()
        {
            if (PhotonNetwork.InRoom)
                try
                {
                    foreach (VRRig vRRig in GorillaParent.instance.vrrigs)
                        if (vRRig.isLocal)
                        {
                            float playerpos = UnityEngine.Vector3.Distance(vRRig.bodyTransform.position, GTPlayer.Instance.transform.position);
                            if (playerpos < range)
                                if (Time.time > Notifdelay)
                                    if (((GorillaTagManager)GorillaGameManager.instance).currentInfected.Contains(vRRig.Creator))
                                    {
                                        Notifdelay = Time.time + 0.6f;
                                        NotifiLib.SendNotification("<color=purple>[WARNING]</color> Lava near you");
                                    }
                        }
                }
                catch { } // no reason
        }


    }
}
