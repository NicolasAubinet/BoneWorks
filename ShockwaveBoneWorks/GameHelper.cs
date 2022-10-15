using MelonLoader;
using ModThatIsNotMod;
using UnityEngine;

namespace ShockwaveBoneWorks
{
    public class GameHelper
    {
        private static GameObject _player;
        private static GameObject _rigManagerPlayer;

        private static readonly string RigManagerName = "[RigManager (Default Brett)]";

        public static GameObject FindPlayer()
        {
            if (_player != null)
            {
                return _player;
            }

            GameObject[] gameObjectsWithTag = GameObject.FindGameObjectsWithTag(nameof(Player));
            for (int index = 0; index < gameObjectsWithTag.Length; ++index)
            {
                if (gameObjectsWithTag[index].name == "PlayerTrigger")
                {
                    _player = gameObjectsWithTag[index];
                    return gameObjectsWithTag[index];
                }
            }

            MelonLogger.Warning("Could not find player!");
            return null;
        }

        public static GameObject GetRigManagerPlayer()
        {
            if (_rigManagerPlayer != null)
            {
                return _rigManagerPlayer;
            }

            _rigManagerPlayer = GameObject.Find(RigManagerName);
            if (_rigManagerPlayer == null)
            {
                MelonLogger.Warning("Could not find rig manager player!");
            }
            return _rigManagerPlayer;
        }
    }
}