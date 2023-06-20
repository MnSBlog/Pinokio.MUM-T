using System;
using System.Collections.Generic;
using System.Text;

namespace Pinokio.Simulation.Cont
{
    public static class ContSimParameter
    {
        private static int _tick = 100; // Unit: ms
        private static int _runSpeed = 1; 
        public static int Tick { get => _tick; }
        public static int RunSpeed { get => _runSpeed; }

        /// <summary>
        /// Continu
        /// </summary>
        /// <param name="tick"></param>
        public static void SetTick(int tick)
        {
            _tick = tick;
        }

        public static void SetRunSpeed(int runSpeed)
        {
            _runSpeed = runSpeed;
        }
    }
}
