using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Tiled;

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
            var scene = Scene.CreateWithDefaultRenderer(Color.CornflowerBlue);

            var tiledTmx = scene.Content.LoadTiledMap("Content/test.tmx");
            var tiledMap = scene.CreateEntity("tiled-map");
            tiledMap.AddComponent(new TiledMapRenderer(tiledTmx, "1"));

            Entity player = scene.CreateEntity("player", new Vector2(20, 200));
            player.AddComponent(new Player());
            player.AddComponent(new TiledMapMover(tiledTmx.GetLayer<TmxLayer>("1")));

            //Scene.Camera.AddComponent(new FollowCamera(player));

            Scene = scene;
        }
    }
}
