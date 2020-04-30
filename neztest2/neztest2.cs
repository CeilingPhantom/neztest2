using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;

namespace NezTest2
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class NezTest2 : Core
    {
        public NezTest2() : base(640, 640)
        {
            Window.AllowUserResizing = true;
        }
        
        protected override void Initialize()
        {
            base.Initialize();
            Scene.SetDefaultDesignResolution(320, 320, Scene.SceneResolutionPolicy.ShowAllPixelPerfect);
            Scene = Scene.CreateWithDefaultRenderer(Color.CornflowerBlue);

            Entity player = Scene.CreateEntity("player", new Vector2(200, 200));
            player.AddComponent(new Player());

            //Scene.Camera.AddComponent(new FollowCamera(player));
        }
    }
}
