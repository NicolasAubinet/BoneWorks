using System.Collections.Generic;
using System.Threading.Tasks;
using ShockwaveAlyx;
using StressLevelZero;
using StressLevelZero.Props.Weapons;
using UnhollowerBaseLib;

namespace ShockwaveBoneWorks
{
    public class ShockwavePlayer
    {
        private bool _heartbeating = false;
        private bool _heartbeatingFast = false;

        private Player_Health _health;
        private Il2CppArrayBase<SaveSpot> _saveSpots;
        private Il2CppArrayBase<HandWeaponSlotReciever> _weaponSlots;

        private bool _leftShoulderSlotUsed = false;
        private bool _rightShoulderSlotUsed = false;
        private bool _leftSideArmSlotUsed = false;
        private bool _rightSideArmSlotUsed = false;
        private float _saveSpotPerc = 0f;

        private Task _rightHandSaveTask = null;
        private Task _leftHandSaveTask = null;
        private Task _bodySaveTask = null;

        private async void PlayHeartbeat(float intensity)
        {
            var pattern = new HapticIndexPattern(new[,]
            {
                { 16, 17, 24 },
            }, intensity, 25);

            ShockwaveEngine.PlayPattern(pattern);
            await Task.Delay(150);
            ShockwaveEngine.PlayPattern(pattern);
        }

        private async void HeartBeatFunc()
        {
            while (_heartbeating)
            {
                PlayHeartbeat(0.2f);
                await Task.Delay(1000);
            }
        }

        private async void HeartBeatFastFunc()
        {
            while (_heartbeatingFast)
            {
                PlayHeartbeat(0.5f);
                await Task.Delay(500);
            }
        }

        public void Initialize(Player_Health health, Il2CppArrayBase<SaveSpot> saveSpots, Il2CppArrayBase<HandWeaponSlotReciever> weaponSlots)
        {
            _health = health;
            _saveSpots = saveSpots;
            _weaponSlots = weaponSlots;
        }

        public void CheckHealth()
        {
            if (_health != null)
            {
                if (_health.alive && _health.curr_Health <= _health.max_Health * 0.1f && _health.curr_Health > 0.01f)
                {
                    _heartbeating = false;
                    if (!_heartbeatingFast)
                    {
                        _heartbeatingFast = true;
                        Task.Run(HeartBeatFastFunc);
                    }
                }
                else if (_health.alive && _health.curr_Health <= _health.max_Health * 0.2f && _health.curr_Health > 0.01f)
                {
                    _heartbeatingFast = false;
                    if (!_heartbeating)
                    {
                        _heartbeating = true;
                        Task.Run(HeartBeatFunc);
                    }
                }
                else
                {
                    _heartbeatingFast = false;
                    _heartbeating = false;
                }
            }
            else
            {
                _heartbeatingFast = false;
                _heartbeating = false;
            }
        }

        private async Task RightHandSaveHapticFunc()
        {
            var pattern = new HapticIndexPattern(new[]
            {
                54, 52, 50, 48, 38, 39, 36, 37, 49, 51, 53, 55
            }, 0.8f, 25);
            await ShockwaveEngine.PlayPatternFunc(pattern);
        }

        private async Task LeftHandSaveHapticFunc()
        {
            var pattern = new HapticIndexPattern(new[]
            {
                46, 44, 42, 40, 33, 32, 35, 34, 41, 43, 45, 47
            }, 0.8f, 25);
            await ShockwaveEngine.PlayPatternFunc(pattern);
        }

        private async Task BodySaveHapticFunc()
        {
            var pattern = new HapticIndexPattern(new[,]
            {
                { 38, 39, 32, 33 },
                { 30, 31, 24, 25 },
                { 22, 23, 16, 17 },
                { 14, 15, 8, 9 },
                { 6, 7, 0, 1 },
                { 64, 64, 56, 56 },
                { 66, 66, 58, 58 },
                { 68, 68, 60, 60 },
                { 70, 70, 62, 62 },
                { 63, 63, 71, 71 },
                { 61, 61, 69, 69 },
                { 59, 59, 67, 67 },
                { 57, 57, 65, 65 },
                { 2, 3, 4, 5 },
                { 10, 11, 12, 13 },
                { 18, 19, 20, 21 },
                { 26, 27, 28, 29 },
                { 34, 35, 36, 37 },
            }, _saveSpotPerc, 25);
            await ShockwaveEngine.PlayPatternFunc(pattern);
        }

        private bool IsRunning(Task task)
        {
            return task != null && task.Status == TaskStatus.Running;
        }

        public void CheckSaveSpot()
        {
            bool isSaving = false;

            if (_saveSpots != null && _saveSpots.Count > 0)
            {
                foreach (var saveSpot in _saveSpots)
                {
                    if (saveSpot.ChargeAudio.isPlaying)
                    {
                        isSaving = true;
                        _saveSpotPerc = saveSpot.perc;
                    }

                    if (saveSpot._isRightGrabbed && !IsRunning(_rightHandSaveTask))
                    {
                        _rightHandSaveTask = Task.Run(RightHandSaveHapticFunc);
                    }

                    if (saveSpot._isLeftGrabbed && !IsRunning(_leftHandSaveTask))
                    {
                        _leftHandSaveTask = Task.Run(LeftHandSaveHapticFunc);
                    }
                }
            }

            if (isSaving && !IsRunning(_bodySaveTask))// && leftHoldingSaveSpot && rightHoldingSaveSpot)
            {
                _bodySaveTask = Task.Run(BodySaveHapticFunc);
            }
        }

        public void CheckWeaponSlots()
        {
            if (_weaponSlots != null && _weaponSlots.Count > 0)
            {
                foreach (var weaponSlot in _weaponSlots)
                {
                    if (weaponSlot != null)
                    {
                        if (weaponSlot.saveBodySlotType == SaveState.BodySlot.LSHOULDER)
                        {
                            if (!_leftShoulderSlotUsed && weaponSlot.m_SlottedWeapon != null)
                            {
                                var pattern = new HapticGroupPattern(
                                    new List<HapticGroupInfo>
                                    {
                                        new HapticGroupInfo(ShockwaveManager.HapticGroup.LEFT_SHOULDER_FRONT, 0.6f),
                                        new HapticGroupInfo(ShockwaveManager.HapticGroup.LEFT_SHOULDER_BACK, 0.6f),
                                        new HapticGroupInfo(ShockwaveManager.HapticGroup.LEFT_CHEST_BACK, 0.8f),
                                        new HapticGroupInfo(ShockwaveManager.HapticGroup.LEFT_SPINE_BACK, 0.8f),
                                        new HapticGroupInfo(ShockwaveManager.HapticGroup.LEFT_WAIST_BACK, 1.0f),
                                    }, 50);
                                ShockwaveEngine.PlayPattern(pattern);
                                _leftShoulderSlotUsed = true;
                            }
                            else if (_leftShoulderSlotUsed && weaponSlot.m_SlottedWeapon == null)
                            {
                                var pattern = new HapticGroupPattern(
                                    new List<HapticGroupInfo>
                                    {
                                        new HapticGroupInfo(ShockwaveManager.HapticGroup.LEFT_WAIST_BACK, 1.0f),
                                        new HapticGroupInfo(ShockwaveManager.HapticGroup.LEFT_SPINE_BACK, 0.8f),
                                        new HapticGroupInfo(ShockwaveManager.HapticGroup.LEFT_CHEST_BACK, 0.8f),
                                        new HapticGroupInfo(ShockwaveManager.HapticGroup.LEFT_SHOULDER_BACK, 0.6f),
                                        new HapticGroupInfo(ShockwaveManager.HapticGroup.LEFT_SHOULDER_FRONT, 0.6f),
                                    }, 50);
                                ShockwaveEngine.PlayPattern(pattern);
                                _leftShoulderSlotUsed = false;
                            }
                        }
                        else if (weaponSlot.saveBodySlotType == SaveState.BodySlot.RSHOULDER)
                        {
                            if (!_rightShoulderSlotUsed && weaponSlot.m_SlottedWeapon != null)
                            {
                                var pattern = new HapticGroupPattern(
                                    new List<HapticGroupInfo>
                                    {
                                        new HapticGroupInfo(ShockwaveManager.HapticGroup.RIGHT_SHOULDER_FRONT, 0.6f),
                                        new HapticGroupInfo(ShockwaveManager.HapticGroup.RIGHT_SHOULDER_BACK, 0.6f),
                                        new HapticGroupInfo(ShockwaveManager.HapticGroup.RIGHT_CHEST_BACK, 0.8f),
                                        new HapticGroupInfo(ShockwaveManager.HapticGroup.RIGHT_SPINE_BACK, 0.8f),
                                        new HapticGroupInfo(ShockwaveManager.HapticGroup.RIGHT_WAIST_BACK, 1.0f),
                                    }, 50);
                                ShockwaveEngine.PlayPattern(pattern);
                                _rightShoulderSlotUsed = true;
                            }
                            else if (_rightShoulderSlotUsed && weaponSlot.m_SlottedWeapon == null)
                            {
                                var pattern = new HapticGroupPattern(
                                    new List<HapticGroupInfo>
                                    {
                                        new HapticGroupInfo(ShockwaveManager.HapticGroup.RIGHT_WAIST_BACK, 1.0f),
                                        new HapticGroupInfo(ShockwaveManager.HapticGroup.RIGHT_SPINE_BACK, 0.8f),
                                        new HapticGroupInfo(ShockwaveManager.HapticGroup.RIGHT_CHEST_BACK, 0.8f),
                                        new HapticGroupInfo(ShockwaveManager.HapticGroup.RIGHT_SHOULDER_BACK, 0.6f),
                                        new HapticGroupInfo(ShockwaveManager.HapticGroup.RIGHT_SHOULDER_FRONT, 0.6f),
                                    }, 50);
                                ShockwaveEngine.PlayPattern(pattern);
                                _rightShoulderSlotUsed = false;
                            }
                        }
                        else if (weaponSlot.saveBodySlotType == SaveState.BodySlot.LSIDEARM)
                        {
                            if (!_leftSideArmSlotUsed && weaponSlot.m_SlottedWeapon != null)
                            {
                                var pattern = new HapticIndexPattern(new[,]
                                {
                                    { 16, 8 },
                                    { 17, 9 },
                                    { 18, 10 },
                                    { 19, 11 },
                                }, 1.0f, 50);
                                ShockwaveEngine.PlayPattern(pattern);
                                _leftSideArmSlotUsed = true;
                            }
                            else if (_leftSideArmSlotUsed && weaponSlot.m_SlottedWeapon == null)
                            {
                                var pattern = new HapticIndexPattern(new[,]
                                {
                                    { 19, 11 },
                                    { 18, 10 },
                                    { 17, 9 },
                                    { 16, 8 },
                                }, 1.0f, 50);
                                ShockwaveEngine.PlayPattern(pattern);
                                _leftSideArmSlotUsed = false;
                            }
                        }
                        else if (weaponSlot.saveBodySlotType == SaveState.BodySlot.RSIDEARM)
                        {
                            if (!_rightSideArmSlotUsed && weaponSlot.m_SlottedWeapon != null)
                            {
                                var pattern = new HapticIndexPattern(new[,]
                                {
                                    { 23, 15 },
                                    { 22, 14 },
                                    { 21, 13 },
                                    { 20, 12 },
                                }, 1.0f, 50);
                                ShockwaveEngine.PlayPattern(pattern);
                                _rightSideArmSlotUsed = true;
                            }
                            else if (_rightSideArmSlotUsed && weaponSlot.m_SlottedWeapon == null)
                            {
                                var pattern = new HapticIndexPattern(new[,]
                                {
                                    { 20, 12 },
                                    { 21, 13 },
                                    { 22, 14 },
                                    { 23, 15 },
                                }, 1.0f, 50);
                                ShockwaveEngine.PlayPattern(pattern);
                                _rightSideArmSlotUsed = false;
                            }
                        }
                    }
                }
            }
        }
    }
}