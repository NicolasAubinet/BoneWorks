using System.Collections;
using Il2CppSystem.Collections.Generic;
using UnityEngine;
using System;
using MelonLoader;
/*The collision script attached to monitor haptics over body regions.
 * The collisions are registerd on hitting a non kinematic rigibody.
 */
namespace ModThatIsNotMod.MonoBehaviours
{
    public class ShockwaveCollider : MonoBehaviour
    {
        public ShockwaveCollider(IntPtr ptr) : base(ptr) { }
      
        public ColliderRegion region;
        public Vector3 bodyForward = Vector3.forward;
        public Animator animator;
        ShockwaveManager.HapticRegion hapticRegion;
        Vector3 velocity, lastPosition;
        Quaternion angularVelocity, quat;
        ShockwaveManager manager;

        float regionHeight;
        Transform attachedBone;
        Quaternion initialBoneRotation;
        float boneY = 0;
       
        // Start is called before the first frame update
        void Start()
        {
          
            manager = ShockwaveManager.Instance;
            if (animator == null)
                animator = transform.root.GetComponentInChildren<Animator>();
            regionHeight = (animator.GetBoneTransform(HumanBodyBones.Neck).position - animator.GetBoneTransform(HumanBodyBones.Hips).position).magnitude;
            //bodyForward = transform.root.forward;
            switch (region)
            {
                case ColliderRegion.PELVIS:
                    attachedBone = animator.GetBoneTransform(HumanBodyBones.Hips);
                    initialBoneRotation = Quaternion.Inverse(Quaternion.LookRotation(bodyForward, animator.GetBoneTransform(HumanBodyBones.Spine).position - animator.GetBoneTransform(HumanBodyBones.Hips).position));
                    hapticRegion = ShockwaveManager.HapticRegion.TORSO;
                    break;
                case ColliderRegion.SPINE:
                    boneY = animator.GetBoneTransform(HumanBodyBones.Spine).position.y - animator.GetBoneTransform(HumanBodyBones.Hips).position.y;
                    attachedBone = animator.GetBoneTransform(HumanBodyBones.Spine);
                    initialBoneRotation = Quaternion.Inverse(Quaternion.LookRotation(bodyForward, animator.GetBoneTransform(HumanBodyBones.Chest).position - animator.GetBoneTransform(HumanBodyBones.Spine).position));
                    hapticRegion = ShockwaveManager.HapticRegion.TORSO;
                    break;
                case ColliderRegion.CHEST:
                    boneY = animator.GetBoneTransform(HumanBodyBones.Chest).position.y - animator.GetBoneTransform(HumanBodyBones.Hips).position.y;
                    attachedBone = animator.GetBoneTransform(HumanBodyBones.Chest);
                    initialBoneRotation = Quaternion.Inverse(Quaternion.LookRotation(bodyForward, animator.GetBoneTransform(HumanBodyBones.Neck).position - animator.GetBoneTransform(HumanBodyBones.Chest).position));
                    hapticRegion = ShockwaveManager.HapticRegion.TORSO;
                    break;
                case ColliderRegion.LEFTUPPERARM:
                    attachedBone = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
                    hapticRegion = ShockwaveManager.HapticRegion.LEFTUPPERARM;
                    initialBoneRotation = Quaternion.Inverse(Quaternion.LookRotation(bodyForward, animator.GetBoneTransform(HumanBodyBones.LeftLowerArm).position - animator.GetBoneTransform(HumanBodyBones.LeftUpperArm).position));
                    regionHeight = (animator.GetBoneTransform(HumanBodyBones.LeftLowerArm).position - animator.GetBoneTransform(HumanBodyBones.LeftUpperArm).position).magnitude;
                    break;
                case ColliderRegion.LEFTLOWERARM:
                    attachedBone = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
                    hapticRegion = ShockwaveManager.HapticRegion.LEFTLOWERARM;
                    initialBoneRotation = Quaternion.Inverse(Quaternion.LookRotation(bodyForward, animator.GetBoneTransform(HumanBodyBones.LeftHand).position - animator.GetBoneTransform(HumanBodyBones.LeftLowerArm).position));
                    regionHeight = (animator.GetBoneTransform(HumanBodyBones.LeftHand).position - animator.GetBoneTransform(HumanBodyBones.LeftLowerArm).position).magnitude;
                    break;
                case ColliderRegion.RIGHTUPPERARM:
                    attachedBone = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
                    hapticRegion = ShockwaveManager.HapticRegion.RIGHTUPPERARM;
                    initialBoneRotation = Quaternion.Inverse(Quaternion.LookRotation(bodyForward, animator.GetBoneTransform(HumanBodyBones.RightLowerArm).position - animator.GetBoneTransform(HumanBodyBones.RightUpperArm).position));
                    regionHeight = (animator.GetBoneTransform(HumanBodyBones.RightLowerArm).position - animator.GetBoneTransform(HumanBodyBones.RightUpperArm).position).magnitude;
                    break;
                case ColliderRegion.RIGHTLOWERARM:
                    attachedBone = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
                    hapticRegion = ShockwaveManager.HapticRegion.RIGHTLOWERARM;
                    initialBoneRotation = Quaternion.Inverse(Quaternion.LookRotation(bodyForward, animator.GetBoneTransform(HumanBodyBones.RightHand).position - animator.GetBoneTransform(HumanBodyBones.RightLowerArm).position));
                    regionHeight = (animator.GetBoneTransform(HumanBodyBones.RightHand).position - animator.GetBoneTransform(HumanBodyBones.RightLowerArm).position).magnitude;
                    break;
                case ColliderRegion.LEFTUPPERLEG:
                    attachedBone = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
                    hapticRegion = ShockwaveManager.HapticRegion.LEFTUPPERLEG;
                    initialBoneRotation = Quaternion.Inverse(Quaternion.LookRotation(bodyForward, animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg).position - animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg).position));
                    regionHeight = (animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg).position - animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg).position).magnitude;
                    break;
                case ColliderRegion.LEFTLOWERLEG:
                    attachedBone = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
                    hapticRegion = ShockwaveManager.HapticRegion.LEFTLOWERLEG;
                    initialBoneRotation = Quaternion.Inverse(Quaternion.LookRotation(bodyForward, animator.GetBoneTransform(HumanBodyBones.LeftFoot).position - animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg).position));
                    regionHeight = (animator.GetBoneTransform(HumanBodyBones.LeftFoot).position - animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg).position).magnitude;
                    break;
                case ColliderRegion.RIGHTUPPERLEG:
                    attachedBone = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
                    hapticRegion = ShockwaveManager.HapticRegion.RIGHTUPPERLEG;
                    initialBoneRotation = Quaternion.Inverse(Quaternion.LookRotation(bodyForward, animator.GetBoneTransform(HumanBodyBones.RightLowerLeg).position - animator.GetBoneTransform(HumanBodyBones.RightUpperLeg).position));
                    regionHeight = (animator.GetBoneTransform(HumanBodyBones.RightLowerLeg).position - animator.GetBoneTransform(HumanBodyBones.RightUpperLeg).position).magnitude;
                    break;
                case ColliderRegion.RIGHTLOWERLEG:
                    attachedBone = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
                    hapticRegion = ShockwaveManager.HapticRegion.RIGHTLOWERLEG;
                    initialBoneRotation = Quaternion.Inverse(Quaternion.LookRotation(bodyForward, animator.GetBoneTransform(HumanBodyBones.RightFoot).position - animator.GetBoneTransform(HumanBodyBones.RightLowerLeg).position));
                    regionHeight = (animator.GetBoneTransform(HumanBodyBones.RightFoot).position - animator.GetBoneTransform(HumanBodyBones.RightLowerLeg).position).magnitude;
                    break;
                default:
                    attachedBone = animator.GetBoneTransform(HumanBodyBones.Hips);
                    break;
            }
            initialBoneRotation = initialBoneRotation * attachedBone.rotation;

        }

        void FixedUpdate()
        {
            velocity = (transform.position - lastPosition) / Time.fixedDeltaTime;
            lastPosition = transform.position;
            angularVelocity = (transform.rotation * quat);//This is change not angular velocity.but subtracting the identity quaternion operation per delta time is the angular velocity.
            quat = Quaternion.Inverse(transform.rotation);
          
        }
        Vector3 GetVelocityAtPoint(Vector3 worldPos)
        {
            Vector3 relativePos = worldPos - transform.position;
            return velocity + (angularVelocity * (relativePos) - relativePos) / Time.fixedDeltaTime;
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.contactCount <= 0 || collision.contacts.Length <= 0)
            {
                return;
            }

            try
            {
                ContactPoint[] contacts = new ContactPoint[collision.contactCount];
                collision.GetContacts(contacts);

                float impactPerPoint;
                ContactPoint contact = collision.GetContact(0);

                 impactPerPoint = 0.4f * collision.impulse.magnitude / collision.contactCount;//For Unconstrained RigidBodies

              //  foreach (ContactPoint contact in contacts)
                {
                    Vector3 localPosition = initialBoneRotation * Quaternion.Inverse(attachedBone.rotation) * (contact.point - attachedBone.position);


                    float Ang = Vector3.SignedAngle(Vector3.forward, new Vector3(localPosition.x, 0, localPosition.z), Vector3.up);

                    if (Ang < 0)
                        Ang = -Ang;
                    else
                        Ang = 360 - Ang;

                    if (impactPerPoint > 6)
                    {
                        manager.sendHapticsPulsewithPositionInfo(hapticRegion, impactPerPoint, Ang, localPosition.y + boneY, regionHeight, 250);

                    }
                    else if (impactPerPoint > 2)
                    {
                        manager.sendHapticsPulsewithPositionInfo(hapticRegion, impactPerPoint, Ang, localPosition.y + boneY, regionHeight, 150 + (impactPerPoint - 2) * 25);

                    }
                    else if (impactPerPoint > 1)
                    {
                        manager.sendHapticsPulsewithPositionInfo(hapticRegion, impactPerPoint, Ang, localPosition.y + boneY, regionHeight, 100 + (impactPerPoint - 1) * 50);

                    }
                    else if (impactPerPoint > 0.5f)
                    {
                        manager.sendHapticsPulsewithPositionInfo(hapticRegion, impactPerPoint * 2, Ang, localPosition.y + boneY, regionHeight, 150 + impactPerPoint * 100);

                    }
                    else
                    {
                        manager.sendHapticsPulsewithPositionInfo(hapticRegion, impactPerPoint * 3f, Ang, localPosition.y + boneY, regionHeight, 50);

                    }
                }
            }
            catch
            {
            }
        }

        void OnCollisionStay(Collision collision)
        {
            if (collision.contactCount <= 0 || collision.contacts.Length <= 0)
            {
                return;
            }

            try
            {
                ContactPoint[] contacts = new ContactPoint[collision.contactCount];
                collision.GetContacts(contacts);

                float impactPerPoint;

                ContactPoint contact = collision.GetContact(0);

                impactPerPoint = 0.5f;
               // foreach (ContactPoint contact in contacts)
                {
                    Vector3 localPosition = initialBoneRotation * Quaternion.Inverse(attachedBone.rotation) * (contact.point - attachedBone.position);
                    float Ang = Vector3.SignedAngle(Vector3.forward, new Vector3(localPosition.x, 0, localPosition.z), Vector3.up);

                    if (Ang < 0)
                        Ang = -Ang;
                    else
                        Ang = 360 - Ang;

                    manager.sendHapticsPulsewithPositionInfo(hapticRegion, impactPerPoint, Ang, localPosition.y + boneY, regionHeight, 25);
                }
            }
            catch
            {
            }
        }

        void OnParticleCollision(GameObject partSystem)
        {

            ParticleSystem particles = partSystem.GetComponent<ParticleSystem>();
            List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
            int contacts =ParticlePhysicsExtensions.GetCollisionEvents(particles,gameObject, collisionEvents);
            //  particles.GetCollisionEvents(gameObject, collisionEvents);


            float impactPerPoint = 0;

           

            for (int i = 0; i < contacts; i++)
            {
                Vector3 localPosition = initialBoneRotation * Quaternion.Inverse(attachedBone.rotation) * (collisionEvents[i].intersection - attachedBone.position);
                float Ang = Vector3.SignedAngle(Vector3.forward, new Vector3(localPosition.x, 0, localPosition.z), Vector3.up);

                if (Ang < 0)
                    Ang = -Ang;
                else
                    Ang = 360 - Ang;

                Vector3 vel = collisionEvents[i].velocity - GetVelocityAtPoint(collisionEvents[i].intersection);//Relative Velocity
              
                float mass = 0.01f;
                //   if (partSystem.GetComponent<Shockwave_ParticleMass>() != null)
                //       mass=partSystem.GetComponent<Shockwave_ParticleMass>().massOfParticle;
                impactPerPoint = Mathf.Abs(Vector3.Dot(collisionEvents[i].normal, vel))  * mass;//Here you can use your own factor instead of mass or attach a script to the particle system to sepcify their mass.
                manager.sendHapticsPulsewithPositionInfo(hapticRegion, impactPerPoint * 2, Ang, localPosition.y + boneY, regionHeight, 150 + impactPerPoint * 100);
               


            }




        }


    }
    public enum ColliderRegion
    {
        PELVIS,
        SPINE,
        CHEST,
        SHOULDERS,
        LEFTUPPERARM,
        LEFTLOWERARM,
        RIGHTUPPERARM,
        RIGHTLOWERARM,
        LEFTUPPERLEG,
        LEFTLOWERLEG,
        RIGHTUPPERLEG,
        RIGHTLOWERLEG
    }
}