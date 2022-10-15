using System.Reflection;
using MelonLoader;
using ModThatIsNotMod;
using ModThatIsNotMod.MonoBehaviours;
using StressLevelZero;
using StressLevelZero.Interaction;
using StressLevelZero.Player;
using StressLevelZero.Props.Weapons;
using StressLevelZero.Rig;
using StressLevelZero.VRMK;
using UnityEngine;
using GravityGun = StressLevelZero.Interaction.GravityGun;

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
        private ShockwaveManager suit;
        private Animator playerAnimator;
        private Transform playArea;
        private Transform Physbody;

        private ShockwavePlayer shockwavePlayer = new ShockwavePlayer();

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
            Hooking.OnPostFireGun += Hooks.GunHooks_OnGunFire;

            const BindingFlags originalBindingFlags =
                (BindingFlags)(Il2CppSystem.Reflection.BindingFlags.Instance | Il2CppSystem.Reflection.BindingFlags.Public);
            const BindingFlags hookBindingFlags =
                (BindingFlags)(Il2CppSystem.Reflection.BindingFlags.Static | Il2CppSystem.Reflection.BindingFlags.NonPublic);

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
                typeof(GravityGun).GetMethod("Grab", originalBindingFlags),
                typeof(Hooks).GetMethod("GravityGunGrab", hookBindingFlags));
            Hooking.CreateHook(
                typeof(GravityGun).GetMethod("Push", originalBindingFlags),
                typeof(Hooks).GetMethod("GravityGunPush", hookBindingFlags));
            Hooking.CreateHook(
                typeof(GravityGun).GetMethod("Pull", originalBindingFlags),
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

            var health = Object.FindObjectOfType<Player_Health>();
            var saveSpots = Object.FindObjectsOfType<SaveSpot>();
            var weaponSlots = Object.FindObjectsOfType<HandWeaponSlotReciever>();
            shockwavePlayer.Initialize(health, saveSpots, weaponSlots);
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            MelonLogger.Msg("OnLevelWasInitialized: " + sceneName);

            Physbody = GameObject.FindObjectOfType<PhysicsRig>().transform;
            playerAnimator = GameObject.FindObjectOfType<CharacterAnimationManager>().gameObject.GetComponent<Animator>(); ;
            playArea = GameObject.FindObjectOfType<SteamControllerRig>().transform.GetChild(0);
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

        public override void OnUpdate()
        {
            shockwavePlayer.CheckHealth();
            shockwavePlayer.CheckSaveSpot();
            shockwavePlayer.CheckWeaponSlots();
        }

        public override void OnApplicationQuit()
        {
            suit.DisconnectSuit();
        }
    }
}