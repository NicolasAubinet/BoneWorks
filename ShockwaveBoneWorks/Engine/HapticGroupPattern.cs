﻿using System.Collections.Generic;

namespace ShockwaveAlyx
{
    public struct HapticGroupPattern
    {
        public List<HapticGroupInfo> groupInfos;
        public int delay;

        public HapticGroupPattern(List<HapticGroupInfo> groupInfos, int delay)
        {
            this.groupInfos = groupInfos;
            this.delay = delay;
        }

        public HapticGroupPattern(ShockwaveManager.HapticGroup group, float intensity, int delay)
        {
            this.groupInfos = new List<HapticGroupInfo> {new HapticGroupInfo(group, intensity)};
            this.delay = delay;
        }
    }
}