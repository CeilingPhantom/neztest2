using System;
using Nez;

namespace NezTest2
{
    public class AnimationChainer : Component, IUpdatable
    {
        public string PrevAnimation { private set; get; }
        float elapsedTime = -1;
        readonly static float maxTime = 0.3f;

        public AnimationChainer() => SetUpdateOrder(-1);

        public void Start(string prevAnimation)
        {
            elapsedTime = 0;
            PrevAnimation = prevAnimation;
        }

        public void End() => elapsedTime = -1;

        void IUpdatable.Update() {
            if (elapsedTime != -1)
                elapsedTime += Time.DeltaTime;
        }

        public bool WithinChainTime()
        {
            return elapsedTime != -1 && elapsedTime <= maxTime;
        }
    }
}
