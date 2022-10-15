using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ModThatIsNotMod;
using ShockwaveAlyx;
using StressLevelZero;
using StressLevelZero.Combat;
using StressLevelZero.Interaction;
using StressLevelZero.Props.Weapons;
using StressLevelZero.VRMK;
using UnityEngine;
using Random = System.Random;

namespace ShockwaveBoneWorks
{
    public class Hooks
    {
        private static bool TelekinesisActiveLeft = false;
        private static bool TelekinesisActiveRight = false;

        public static void GunHooks_OnGunFire(Gun gun)
        {
            if (gun != null)
            {
                int gunInstance = gun.GetInstanceID();
                Gun rightGun = Player.GetGunInHand(Player.rightHand);
                if ((rightGun != null && gunInstance == rightGun.GetInstanceID()))
                {
                    var pattern = new HapticGroupPattern(new List<HapticGroupInfo>
                    {
                        new HapticGroupInfo(ShockwaveManager.HapticGroup.RIGHT_FOREARM, 1f),
                        new HapticGroupInfo(ShockwaveManager.HapticGroup.RIGHT_ARM, 0.8f),
                        new HapticGroupInfo(ShockwaveManager.HapticGroup.RIGHT_BICEP, 0.4f),
                        new HapticGroupInfo(ShockwaveManager.HapticGroup.RIGHT_SHOULDER, 0.05f),
                    }, 10);
                    ShockwaveEngine.PlayPattern(pattern);
                }

                Gun leftGun = Player.GetGunInHand(Player.leftHand);
                if ((leftGun != null && gunInstance == leftGun.GetInstanceID()))
                {
                    var pattern = new HapticGroupPattern(new List<HapticGroupInfo>
                    {
                        new HapticGroupInfo(ShockwaveManager.HapticGroup.LEFT_FOREARM, 1f),
                        new HapticGroupInfo(ShockwaveManager.HapticGroup.LEFT_ARM, 0.8f),
                        new HapticGroupInfo(ShockwaveManager.HapticGroup.LEFT_BICEP, 0.4f),
                        new HapticGroupInfo(ShockwaveManager.HapticGroup.LEFT_SHOULDER, 0.05f),
                    }, 10);
                    ShockwaveEngine.PlayPattern(pattern);
                }
            }
        }

        // Handles Player melee hits when holding an object/weapon
        private static void Haptic_Hit(Haptor __instance, float amp)
        {
            if (amp < 0.1f)
            {
                amp = 0.1f;
            }

            if (__instance?.device_Controller != null)
            {
                GameObject objectInHand = null;
                int[] indices = Array.Empty<int>();

                if (__instance.device_Controller.handedness == Handedness.LEFT)
                {
                    objectInHand = Player.GetObjectInHand(Player.leftHand);
                    indices = new[]{ 46, 44, 45 };
                }
                else if (__instance.device_Controller.handedness == Handedness.RIGHT)
                {
                    objectInHand = Player.GetObjectInHand(Player.rightHand);
                    indices = new[]{ 54, 52, 53 };
                }

                if (objectInHand != null && objectInHand.transform.root.gameObject.GetComponent<PowerPuncher>() != null)
                {
                    amp *= 3.0f;
                }

                if (indices.Length > 0)
                {
                    var pattern = new HapticIndexPattern(indices, amp, 30);
                    ShockwaveEngine.PlayPattern(pattern);
                }
            }
        }

        private static void PlayerAttacked(PlayerDamageReceiver __instance, Attack attack)
        {
            GameObject player = GameHelper.GetRigManagerPlayer();

            if (__instance == null || attack == null || player == null || attack.collider == null)
            {
                return;
            }

            if (attack.collider.gameObject.name == "PlayerTrigger" || attack.collider.gameObject.name == "Trigger")
            {
                if (attack.collider.transform?.root?.gameObject != null &&
                    (attack.collider.transform.root.gameObject.name.Contains("OmniTurret") || attack.collider.transform.root.gameObject.name.Contains("Ford") || attack.collider.transform.root.gameObject.name.Contains("Null")))
                {
                    return;
                }

                Random random = new Random();
                float intensity = Math.Max(Math.Abs(attack.damage) / 5.0f, 0.1f);
                int duration = 50;

                if (__instance.bodyPart == PlayerDamageReceiver.BodyPart.Head
                    || __instance.bodyPart == PlayerDamageReceiver.BodyPart.Chest
                    || __instance.bodyPart == PlayerDamageReceiver.BodyPart.Pelvis)
                {
                    float hitAngle = 0f;
                    if (attack.collider.transform != null && __instance.gameObject != null)
                    {
                        Vector3 instancePosition = __instance.gameObject.transform.position;
                        Vector3 contactPosition = attack.collider.ClosestPointOnBounds(instancePosition);
                        hitAngle = contactPosition.GetAngleForPosition(player.transform.forward, instancePosition);
                    }

                    int position;
                    const int regionHeight = 10;
                    if (__instance.bodyPart == PlayerDamageReceiver.BodyPart.Head)
                    {
                        position = regionHeight;
                    }
                    else
                    {
                        position = random.Next(regionHeight); // TODO find correct position based on contactPosition
                    }

                    ShockwaveManager.Instance.sendHapticsPulsewithPositionInfo(ShockwaveManager.HapticRegion.TORSO, intensity,
                        hitAngle, position, regionHeight, duration);
                }
                else if (__instance.bodyPart == PlayerDamageReceiver.BodyPart.LeftArm)
                {
                    ShockwaveManager.HapticGroup group = random.Next(1) == 0
                        ? ShockwaveManager.HapticGroup.LEFT_ARM
                        : ShockwaveManager.HapticGroup.LEFT_FOREARM;
                    ShockwaveEngine.PlayPattern(new HapticGroupPattern(group, intensity, duration));
                }
                else if (__instance.bodyPart == PlayerDamageReceiver.BodyPart.RightArm)
                {
                    ShockwaveManager.HapticGroup group = random.Next(1) == 0
                        ? ShockwaveManager.HapticGroup.RIGHT_ARM
                        : ShockwaveManager.HapticGroup.RIGHT_FOREARM;
                    ShockwaveEngine.PlayPattern(new HapticGroupPattern(group, intensity, duration));
                }
                else if (__instance.bodyPart == PlayerDamageReceiver.BodyPart.LeftLeg)
                {
                    ShockwaveManager.HapticGroup group = random.Next(1) == 0
                        ? ShockwaveManager.HapticGroup.LEFT_LEG
                        : ShockwaveManager.HapticGroup.LEFT_LOWER_LEG;
                    ShockwaveEngine.PlayPattern(new HapticGroupPattern(group, intensity, duration));
                }
                else if (__instance.bodyPart == PlayerDamageReceiver.BodyPart.RightLeg)
                {
                    ShockwaveManager.HapticGroup group = random.Next(1) == 0
                        ? ShockwaveManager.HapticGroup.RIGHT_LEG
                        : ShockwaveManager.HapticGroup.RIGHT_LOWER_LEG;
                    ShockwaveEngine.PlayPattern(new HapticGroupPattern(group, intensity, duration));
                }
            }
        }

        private static void SENDHAPTIC(Haptor __instance, float delay, float duration, float frequency, float amplitude)
        {
            const float TOLERANCE = 0.01f;

            if (Math.Abs(duration - 0.025f) < TOLERANCE && Math.Abs(frequency - 100.0f) < TOLERANCE)
            {
                if (__instance?.device_Controller != null)
                {
                    if (__instance.device_Controller.handedness == Handedness.LEFT)
                    {
                        GameObject leftObject = Player.GetObjectInHand(Player.leftHand);

                        if (!(leftObject?.transform?.root?.gameObject != null && leftObject.transform.root.gameObject.GetComponent<DevManipulatorGun>() != null))
                        {
                            var pattern = new HapticIndexPattern(new []{ 46, 45 }, amplitude * 3, 30);
                            ShockwaveEngine.PlayPattern(pattern);
                        }
                    }
                    else if (__instance.device_Controller.handedness == Handedness.RIGHT)
                    {
                        GameObject rightObject = Player.GetObjectInHand(Player.rightHand);

                        if (!(rightObject?.transform?.root?.gameObject != null && rightObject.transform.root.gameObject.GetComponent<DevManipulatorGun>() != null))
                        {
                            var pattern = new HapticIndexPattern(new []{ 54, 53 }, amplitude * 3, 30);
                            ShockwaveEngine.PlayPattern(pattern);
                        }
                    }

                }
            }
        }

        private static void SetTelekinesisActive(Hand hand, bool active)
        {
            if (hand == Player.leftHand)
            {
                TelekinesisActiveLeft = active;
            }
            else
            {
                TelekinesisActiveRight = active;
            }
        }

        private static async void TelekinesisActivateFunc(bool leftHand)
        {
            ShockwaveManager.HapticGroup group = leftHand
                ? ShockwaveManager.HapticGroup.LEFT_FOREARM
                : ShockwaveManager.HapticGroup.RIGHT_FOREARM;
            const int duration = 200;

            while ((TelekinesisActiveLeft && leftHand) || (TelekinesisActiveRight && !leftHand))
            {
                ShockwaveEngine.PlayPattern(new HapticGroupPattern(group, 0.5f, duration));
                await Task.Delay(duration);
            }
        }

        private static void ForcePullGripOnStartAttach(Hand hand)
        {
            SetTelekinesisActive(hand, true);

            Task.Run(() => TelekinesisActivateFunc(hand == Player.leftHand));
        }

        private static void ForcePullGripOnForcePullComplete(Hand hand)
        {
            SetTelekinesisActive(hand, false);

            HapticIndexPattern pattern = (hand == Player.leftHand)
                ? new HapticIndexPattern(new []{ 46, 45 }, 1.0f, 30)
                : new HapticIndexPattern(new []{ 54, 53 }, 1.0f, 30);
            ShockwaveEngine.PlayPattern(pattern);
        }

        private static void ForcePullGripCancelPull(Hand hand)
        {
            SetTelekinesisActive(hand, false);

            HapticIndexPattern pattern = (hand == Player.leftHand)
                ? new HapticIndexPattern(new []{ 45, 46 }, 0.7f, 20)
                : new HapticIndexPattern(new []{ 53, 54 }, 0.7f, 20);
            ShockwaveEngine.PlayPattern(pattern);
        }

        private static void ForcePullGripOnFarHandHoverEnd(Hand hand)
        {
            SetTelekinesisActive(hand, false);
        }

        private static void ArenaBalloonGunOnFire(ArenaBalloonGun __instance)
        {
        }

        private static void BalloonGunOnFire(BalloonGun __instance)
        {
        }

        private static void DevManipulatorGunBlast(DevManipulatorGun __instance)
        {
        }

        private static void DevManipulatorGunOnBallGripUpdate(DevManipulatorGun __instance, Hand hand)
        {
        }

        private static void FlyingGunOnTriggerGripUpdate(FlyingGun __instance, Hand hand)
        {
        }

        private static void GrapplingHookFire(GrapplingHook __instance)
        {
        }

        private static void GravityGunGrab(StressLevelZero.Interaction.GravityGun __instance)
        {
        }

        private static void GravityGunPush(StressLevelZero.Interaction.GravityGun __instance)
        {
        }

        private static void GravityGunPull(StressLevelZero.Interaction.GravityGun __instance)
        {
        }

        private static void GrenadeLauncherOnFire(GrenadeLauncher __instance)
        {
        }

        private static void SpawnGunOnFire(SpawnGun __instance)
        {
        }

        private static void PhysGrounderCollisionEnter(PhysGrounder __instance, Collision c)
        {
        }

        private static void HandWeaponSlotReceiverHoverBegin(HandWeaponSlotReciever __instance, Hand hand)
        {
        }

        private static void HandWeaponSlotReceiverHoverEnd(HandWeaponSlotReciever __instance, Hand hand)
        {
        }
    }
}