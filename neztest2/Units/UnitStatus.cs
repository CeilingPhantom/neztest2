using System;
using Nez;

namespace NezTest2.Units
{
    public class UnitStatus : Component
    {
        public float Speed { get; private set; } = 70;
        public float JumpHeight { get; private set; } = 60;
        public float Gravity { get; private set; } = 600;

        public int Health { get; private set; } = 100;
        public int Armour { get; private set; } = 0;

        public float DamageLow { get; private set; } = 30;
        public float DamageHigh { get; private set; } = 40;

        public UnitStatus()
        {
        }
    }
}
