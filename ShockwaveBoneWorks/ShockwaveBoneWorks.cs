using Il2CppSystem.Reflection;
using MelonLoader;
using UnityEngine;
using ModThatIsNotMod.MonoBehaviours;
using ModThatIsNotMod;
using StressLevelZero.Interaction;
using StressLevelZero.Props.Weapons;
using StressLevelZero.VRMK;

namespace ShockwaveBoneWorks
{
 
    public static class BuildInfo
    {
        public const string Name = "Shockwave";
        public const string Author = "Akshay & Alchemist";
        public const string Company = "Shockwave VR";
        public const string Version = "1.0.0";
        public const string DownloadLink = null;
    }

    public class ShockwaveBoneWorks : MelonMod
    {
        ShockwaveManager suit;
        Animator playerAnimator;
        Transform playArea;
        Transform Physbody;

        public override void OnApplicationStart()
        {
         //   CustomMonoBehaviourHandler.RegisterMonoBehaviourInIl2Cpp<AvatarTransform_IK>();
            CustomMonoBehaviourHandler.RegisterMonoBehaviourInIl2Cpp<ShockwaveCollider>();
            suit = ShockwaveManager.Instance;
            suit.InitializeSuit();

            RegisterHooks();

            MelonLogger.Msg("OnApplicationStart");
        }

        void RegisterHooks()
        {
            Hooking.OnPostFireGun += GunHooks_OnGunFire;

            const System.Reflection.BindingFlags originalBindingFlags =
                (System.Reflection.BindingFlags)(BindingFlags.Instance | BindingFlags.Public);
            const System.Reflection.BindingFlags hookBindingFlags =
                (System.Reflection.BindingFlags)(BindingFlags.Static | BindingFlags.NonPublic);

            Hooking.CreateHook(
                typeof(PlayerDamageReceiver).GetMethod("ReceiveAttack", originalBindingFlags),
                typeof(Hooks).GetMethod("PlayerAttacked", hookBindingFlags), true);
            Hooking.CreateHook(
                typeof(ForcePullGrip).GetMethod("OnStartAttach", originalBindingFlags),
                typeof(Hooks).GetMethod("ForcePullGripOnStartAttach", hookBindingFlags));
            Hooking.CreateHook(
                typeof(ForcePullGrip).GetMethod("OnFarHandHoverEnd", originalBindingFlags),
                typeof(Hooks).GetMethod("ForcePullGripOnFarHandHoverEnd", hookBindingFlags));
            Hooking.CreateHook(
                typeof(ForcePullGrip).GetMethod("OnForcePullComplete", originalBindingFlags),
                typeof(Hooks).GetMethod("ForcePullGripOnForcePullComplete", hookBindingFlags));
            Hooking.CreateHook(
                typeof(ForcePullGrip).GetMethod("CancelPull", originalBindingFlags),
                typeof(Hooks).GetMethod("ForcePullGripCancelPull", hookBindingFlags));
            Hooking.CreateHook(
                typeof(HandWeaponSlotReciever).GetMethod("OnHandHoverBegin", originalBindingFlags),
                typeof(Hooks).GetMethod("HandWeaponSlotRecieverHoverBegin", hookBindingFlags));
            Hooking.CreateHook(
                typeof(HandWeaponSlotReciever).GetMethod("OnHandHoverEnd", originalBindingFlags),
                typeof(Hooks).GetMethod("HandWeaponSlotRecieverHoverEnd", hookBindingFlags));
            Hooking.CreateHook(
                typeof(PhysGrounder).GetMethod("OnCollisionEnter", originalBindingFlags),
                typeof(Hooks).GetMethod("PhysGrounderCollisionEnter", hookBindingFlags));
            Hooking.CreateHook(
                typeof(ArenaBalloonGun).GetMethod("OnFire", originalBindingFlags),
                typeof(Hooks).GetMethod("ArenaBalloonGunOnFire", hookBindingFlags));
            Hooking.CreateHook(
                typeof(BalloonGun).GetMethod("OnFire", originalBindingFlags),
                typeof(Hooks).GetMethod("BalloonGunOnFire", hookBindingFlags));
            Hooking.CreateHook(
                typeof(DevManipulatorGun).GetMethod("Blast", originalBindingFlags),
                typeof(Hooks).GetMethod("DevManipulatorGunBlast", hookBindingFlags));
            Hooking.CreateHook(
                typeof(DevManipulatorGun).GetMethod("OnBallGripUpdate", originalBindingFlags),
                typeof(Hooks).GetMethod("DevManipulatorGunOnBallGripUpdate", hookBindingFlags));
            Hooking.CreateHook(
                typeof(GrapplingHook).GetMethod("FireHook", originalBindingFlags),
                typeof(Hooks).GetMethod("GrapplingHookFire", hookBindingFlags));
            Hooking.CreateHook(
                typeof(StressLevelZero.Interaction.GravityGun).GetMethod("Grab", originalBindingFlags),
                typeof(Hooks).GetMethod("GravityGunGrab", hookBindingFlags));
            Hooking.CreateHook(
                typeof(StressLevelZero.Interaction.GravityGun).GetMethod("Push", originalBindingFlags),
                typeof(Hooks).GetMethod("GravityGunPush", hookBindingFlags));
            Hooking.CreateHook(
                typeof(StressLevelZero.Interaction.GravityGun).GetMethod("Pull", originalBindingFlags),
                typeof(Hooks).GetMethod("GravityGunPull", hookBindingFlags));
            Hooking.CreateHook(
                typeof(GrenadeLauncher).GetMethod("OnFire", originalBindingFlags),
                typeof(Hooks).GetMethod("GrenadeLauncherOnFire", hookBindingFlags));
            Hooking.CreateHook(
                typeof(SpawnGun).GetMethod("OnFire", originalBindingFlags),
                typeof(Hooks).GetMethod("SpawnGunOnFire", hookBindingFlags));
            Hooking.CreateHook(
                typeof(FlyingGun).GetMethod("OnTriggerGripUpdate", originalBindingFlags),
                typeof(Hooks).GetMethod("FlyingGunOnTriggerGripUpdate", hookBindingFlags));
            Hooking.CreateHook(
                typeof(Haptor).GetMethod("Haptic_Hit", originalBindingFlags),
                typeof(Hooks).GetMethod("Haptic_Hit", hookBindingFlags));
            Hooking.CreateHook(
                typeof(Haptor).GetMethod("SENDHAPTIC", originalBindingFlags),
                typeof(Hooks).GetMethod("SENDHAPTIC", hookBindingFlags));

        }

        void SuitAdded(GameObject suitObject)
        {
            MelonLogger.Msg("OnCustomItemAdded: " + suitObject.name);
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            MelonLogger.Msg("OnLevelWasLoaded: " + sceneName);
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            MelonLogger.Msg("OnLevelWasInitialized: " + sceneName);
            
         
            Physbody = GameObject.FindObjectOfType<StressLevelZero.Rig.PhysicsRig>().transform;
            playerAnimator = GameObject.FindObjectOfType<StressLevelZero.Player.CharacterAnimationManager>().gameObject.GetComponent<Animator>(); ;
            playArea = GameObject.FindObjectOfType<StressLevelZero.Rig.SteamControllerRig>().transform.GetChild(0);
       //     CapsuleCollider[] torsoColliders = Physbody.GetChild(3).GetComponentsInChildren<CapsuleCollider>();
       //     BoxCollider[] chestFront = Physbody.GetChild(3).GetComponentsInChildren<BoxCollider>();
            CapsuleCollider legCollider = Physbody.GetChild(4).GetComponentInChildren<CapsuleCollider>();
            ShockwaveCollider shockwaveCollider = Physbody.GetChild(3).gameObject.AddComponent<ShockwaveCollider>();
            shockwaveCollider.animator = playerAnimator;
            shockwaveCollider.bodyForward = playArea.forward;
            shockwaveCollider.region = ColliderRegion.PELVIS;

            ShockwaveCollider leftThighCollider = legCollider.transform.parent.gameObject.AddComponent<ShockwaveCollider>();
            ShockwaveCollider leftCalfCollider = legCollider.transform.parent.gameObject.AddComponent<ShockwaveCollider>();
            ShockwaveCollider rightThighCollider = legCollider.transform.parent.gameObject.AddComponent<ShockwaveCollider>();
            ShockwaveCollider rightCalfCollider = legCollider.transform.parent.gameObject.AddComponent<ShockwaveCollider>();
            leftThighCollider.region = ColliderRegion.LEFTUPPERLEG;
            leftCalfCollider.region = ColliderRegion.LEFTLOWERLEG;
            rightThighCollider.region = ColliderRegion.RIGHTUPPERLEG;
            rightCalfCollider.region = ColliderRegion.RIGHTLOWERLEG;
            leftThighCollider.animator = playerAnimator;
            leftCalfCollider.animator = playerAnimator;
            rightThighCollider.animator = playerAnimator;
            rightCalfCollider.animator = playerAnimator;
            leftThighCollider.bodyForward = playArea.forward;
            leftCalfCollider.bodyForward = playArea.forward;
            rightThighCollider.bodyForward = playArea.forward;
            rightCalfCollider.bodyForward = playArea.forward;
        }
       
        public override void OnApplicationQuit()
        {
            suit.DisconnectSuit();
        }
        
        private void GunHooks_OnGunFire(Gun gun)
        {
        }
    }
}