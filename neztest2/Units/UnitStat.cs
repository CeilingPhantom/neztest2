using System;
using Nez;

namespace NezTest2.Units
{
    public class UnitStat : Component
    {
        public float Speed { get; private set; }
        public float JumpHeight { get; private set; }
        public float Gravity { get; private set; }

        public int Health { get; private set; }
        public int Armour { get; private set; }

        public float DamageLow { get; private set; }
        public float DamageHigh { get; private set; }

        public UnitStat(float speed=70, float jumpHeight=60, float gravity=600, int health=100, 
                        int armour=0, float damageLow=30, float damageHigh=40)
        {
            Speed = speed;
            JumpHeight = jumpHeight;
            Gravity = gravity;
            Health = health;
            Armour = armour;
            DamageLow = damageLow;
            DamageHigh = damageHigh;
        }

        internal int TakeDamage(int damage)
        {
            Health -= damage;
            return Health;
        }
    }
}
