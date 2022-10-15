using UnityEngine;

namespace ShockwaveBoneWorks
{
    public static class Utility
    {
       public static UnityEngine.Vector3 ToXZ(this UnityEngine.Vector3 direction)
       {
           direction.y = 0f;
           return direction;
       }

        public static float GetAngleForPosition(this UnityEngine.Vector3 pos, Vector3 forward, Vector3 position)
        {
            float angle = 0.0f;

            angle = UnityEngine.Vector3.SignedAngle(forward.ToXZ(), pos.ToXZ() - position.ToXZ(), Vector3.up);
            if (angle < 0)
            {
                angle = -180 - angle;
            }
            else
            {
                angle = 180 - angle;
            }

            angle += 180.0f;


            return angle;
        }
    }
}