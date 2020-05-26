using System.Collections.Generic;
using Nez;

namespace NezTest2.Util
{
    public class AnimationChainer : Component, IUpdatable
    {
        Dictionary<string, List<string>> chainableAnimations = new Dictionary<string, List<string>>();
        public string PrevAnimation { private set; get; }
        float elapsedTime = -1;
        readonly static float maxTime = 0.3f;

        public AnimationChainer() => SetUpdateOrder(-1);

        public void AddChainableAnimation(string initial, string chainsInto)
        {
            if (!chainableAnimations.ContainsKey(initial))
                chainableAnimations.Add(initial, new List<string> { chainsInto });
            else
                chainableAnimations[initial].Add(chainsInto);
        }

        public bool IsChainableAnimation(string animation) => chainableAnimations.ContainsKey(animation);

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
