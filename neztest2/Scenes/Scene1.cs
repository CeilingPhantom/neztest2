using Microsoft.Xna.Framework;
using Nez;
using Nez.Tiled;
using NezTest2.Units;
using NezTest2.Util;

namespace NezTest2.Scenes
{
    public class Scene1 : Scene
    {
        Player player;

        public Scene1(Player player) : base()
        {
            this.player = player;
        }

        public override void Initialize()
        {
            AddRenderer(new DefaultRenderer());

            var tiledTmx = Content.LoadTiledMap("Content/test.tmx");
            var tiledEntity = CreateEntity("tiled-map");
            tiledEntity.AddComponent(new CameraBounds(Vector2.Zero, new Vector2(tiledTmx.TileWidth * tiledTmx.Width, tiledTmx.TileWidth * tiledTmx.Height)));

            var tiledMap1 = tiledEntity.AddComponent(new TiledMapRenderer(tiledTmx, "1"));
            tiledMap1.SetLayersToRender("1", "2", "3");

            var tiledMap2 = tiledEntity.AddComponent(new TiledMapRenderer(tiledTmx));
            tiledMap2.SetLayersToRender("0");
            tiledMap2.RenderLayer = -1;

            Entity player = CreateEntity("player", new Vector2(50, 200));
            if (this.player == null)
                this.player = NezTest2.UpdatePlayer();
            player.AddComponent(this.player);
            player.AddComponent(new TiledMapMover(tiledTmx.GetLayer<TmxLayer>("1")));

            Entity slime1 = CreateEntity("slime1", new Vector2(300, 200));
            slime1.AddComponent(new Slime());
            slime1.AddComponent(new TiledMapMover(tiledTmx.GetLayer<TmxLayer>("1")));

            Entity slime2 = CreateEntity("slime2", new Vector2(350, 200));
            slime2.AddComponent(new Slime());
            slime2.AddComponent(new TiledMapMover(tiledTmx.GetLayer<TmxLayer>("1")));

            var endObj = tiledTmx.ObjectGroups["obj"].Objects["end"];
            Entity end = CreateEntity("end", new Vector2(endObj.X, endObj.Y));
            end.AddComponent(new SceneTransitioner("1"));
            end.AddComponent(new BoxCollider(0, 0, endObj.Width, endObj.Height));

            Camera.AddComponent(new FollowCamera(player));
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
