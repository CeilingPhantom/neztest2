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
            var tiledEntity = scene.CreateEntity("tiled-map");
            tiledEntity.AddComponent(new CameraBounds(Vector2.Zero, new Vector2(tiledTmx.TileWidth * tiledTmx.Width, tiledTmx.TileWidth * tiledTmx.Height)));

            var tiledMap1 = tiledEntity.AddComponent(new TiledMapRenderer(tiledTmx, "1"));
            tiledMap1.SetLayersToRender("1", "2", "3");

            var tiledMap2 = tiledEntity.AddComponent(new TiledMapRenderer(tiledTmx));
            tiledMap2.SetLayersToRender("0");
            tiledMap2.RenderLayer = -1;

            Entity player = scene.CreateEntity("player", new Vector2(50, 200));
            player.AddComponent(new Player());
            player.AddComponent(new TiledMapMover(tiledTmx.GetLayer<TmxLayer>("1")));

            Entity slime1 = scene.CreateEntity("slime1", new Vector2(300, 200));
            slime1.AddComponent(new Slime());
            slime1.AddComponent(new TiledMapMover(tiledTmx.GetLayer<TmxLayer>("1")));

            Entity slime2 = slime1.Clone(new Vector2(350, 200));
            slime2.Name = "slime2";
            scene.AddEntity(slime2);

            scene.Camera.AddComponent(new FollowCamera(player));

            Scene = scene;
        }
    }
}
