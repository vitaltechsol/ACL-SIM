using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACLSim
{
    using System;
    internal class EncoderTorqueTracker
    {
        private readonly int countsPerRev;
        public int torqueMin { get; set; }
        public int torqueMax { get; set; }
        private double countsToTorque { get; set; } // torque increment per count
        private int? lastCount;                 // last raw encoder count (0..countsPerRev-1)
        private int unwrapped;                 // unwrapped counts (can be negative / multi-rev)
        private int? home;                     // unwrapped home position (set on first update)

        /// <summary>
        /// Create a tracker for a cyclic encoder that reads 0..countsPerRev-1.
        /// Torque will scale linearly from minTorque at home up to torqueMax at max distance.
        /// </summary>
        public EncoderTorqueTracker(int countsPerRev = 10000, int torqueMin = 0, int torqueMax = 80)
        {
            if (countsPerRev <= 0) throw new ArgumentOutOfRangeException(nameof(countsPerRev));
            if (torqueMax <= torqueMin) throw new ArgumentOutOfRangeException(nameof(torqueMax),
                "torqueMax must be greater than minTorque");

            this.countsPerRev = countsPerRev;
            this.torqueMin = torqueMin;
            this.torqueMax = torqueMax;
            SetMinMax(torqueMin, torqueMax);
        }

        /// <summary>
        /// Explicitly set current position as home (min torque reference).
        /// If not called, the first Update() call sets home automatically.
        /// </summary>
        public void SetHome(int currentCount)
        {
            lastCount = Normalize(currentCount);
            unwrapped = lastCount.Value;
            home = unwrapped;
        }

        public void SetMinMax(int min, int max)
        {
            torqueMin = min;
            torqueMax = max;
            // Slope is based on available torque range (max - min)
            countsToTorque = (double)(torqueMax - torqueMin) / countsPerRev;
        }

        public int GetHome()
        {
            if (home != null)
                return (int)home;

            return 0;
        }

        /// <summary>
        /// Feed the latest encoder reading (0..countsPerRev-1).
        /// Returns integer torque [minTorque..torqueMax].
        /// </summary>
        public int Update(int currentCount)
        {
            int curr = Normalize(currentCount);

            if (!lastCount.HasValue)
            {
                lastCount = curr;
                unwrapped = curr;
                if (home == null) home = unwrapped;
                return torqueMin;
            }

            // Compute smallest signed delta across wrap (range roughly -N/2..+N/2)
            int delta = curr - lastCount.Value;
            int half = countsPerRev / 2;
            if (delta > half) delta -= countsPerRev;
            else if (delta < -half) delta += countsPerRev;

            // Accumulate unwrapped position
            unwrapped += delta;
            lastCount = curr;

            // Distance from home in counts (absolute)
            long distCounts = Math.Abs(unwrapped - (home ?? unwrapped));

            // Linear mapping: minTorque at home, torqueMax at full revolution
            double torque = torqueMin + distCounts * countsToTorque;

            // Clamp to [minTorque, torqueMax]
            if (torque > torqueMax) torque = torqueMax;
            if (torque < torqueMin) torque = torqueMin;

            return (int)Math.Round(torque);
        }

        public long UnwrappedCounts => unwrapped;

        private int Normalize(int c)
        {
            int r = c % countsPerRev;
            if (r < 0) r += countsPerRev;
            return r;
        }
    }


}
