using StressLevelZero.Combat;
using StressLevelZero.Interaction;
using StressLevelZero.Props.Weapons;
using StressLevelZero.VRMK;
using UnityEngine;

namespace ShockwaveBoneWorks
{
    public class Hooks
    {
        //Handles Player melee hits when holding an object/weapon
        private static void Haptic_Hit(Haptor __instance, float amp)
        {
        }

        private static void PlayerAttacked(PlayerDamageReceiver __instance, Attack attack)
        {
        }

        private static void SENDHAPTIC(Haptor __instance, float delay, float duration, float frequency, float amplitude)
        {
        }

        private static void TelekinesisActivateFunc(bool leftHand)
        {
        }

        private static void ForcePullGripOnStartAttach(Hand hand)
        {
        }

        private static void ForcePullGripOnForcePullComplete(Hand hand)
        {
        }

        private static void ForcePullGripCancelPull(Hand hand)
        {
        }

        private static void ForcePullGripOnFarHandHoverEnd(Hand hand)
        {
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