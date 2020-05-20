using Microsoft.Xna.Framework;
using Nez;
using Nez.Tiled;
using NezTest2.Units;
using NezTest2.Util;

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
            tiledMap.AddComponent(new CameraBounds(Vector2.Zero, new Vector2(tiledTmx.TileWidth * tiledTmx.Width, tiledTmx.TileWidth * tiledTmx.Height)));

            Entity player = scene.CreateEntity("player", new Vector2(50, 200));
            player.AddComponent(new Player());
            player.AddComponent(new TiledMapMover(tiledTmx.GetLayer<TmxLayer>("1")));

            Entity slime = scene.CreateEntity("slime", new Vector2(300, 200));
            slime.AddComponent(new Slime());
            slime.AddComponent(new TiledMapMover(tiledTmx.GetLayer<TmxLayer>("1")));

            Entity slime2 = slime.Clone(new Vector2(350, 200));
            slime2.Name = "slime2";
            scene.AddEntity(slime2);

            scene.Camera.AddComponent(new FollowCamera(player));

            Scene = scene;
        }
    }
}
