using Nez;
using NezTest2.Units;

namespace NezTest2.Scenes
{
    public class SceneBase : Scene
    {
        protected static UnitStat PlayerStats { get; private set; } = new UnitStat();

        public SceneBase() { }

        public virtual void UpdatePersistence()
        {
            PlayerStats = (UnitStat)PlayerStats.Clone();
        }
    }
}
