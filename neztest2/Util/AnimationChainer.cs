using System.Collections.Generic;
using Nez;

namespace NezTest2.Util
{
    public class AnimationChainer : Component, IUpdatable
    {
        Dictionary<string, string> chainableAnimations = new Dictionary<string, string>();
        public string PrevAnimation { private set; get; }
        float elapsedTime = -1;
        readonly static float maxTime = 0.3f;

        public AnimationChainer() => SetUpdateOrder(-1);

        public void AddChainableAnimation(string initial, string chainsInto)
        {
            chainableAnimations[initial] = chainsInto;
        }

        public bool IsChainableAnimation(string animation) => chainableAnimations.ContainsKey(animation);

        public string NextAnimation() => chainableAnimations[PrevAnimation];

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
