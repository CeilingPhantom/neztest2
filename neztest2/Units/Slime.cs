using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using Nez.Tiled;

namespace NezTest2.Units
{
    public class Slime : Unit
    {
        static TmxTileset SlimeAnimationFrames;
        static Dictionary<string, string[]> SlimeAnimations;

        public Slime() : base("slime", new Vector2(4, 3), 24, 20)
        {
            if (SlimeAnimationFrames == null)
                using (var stream = TitleContainer.OpenStream($"{ContentPath}/{Name}.tsx"))
                    SlimeAnimationFrames = new TmxTileset().LoadTmxTileset(null, System.Xml.Linq.XDocument.Load(stream).Element("tileset"), 1, ContentPath);
            AnimationFrames = SlimeAnimationFrames;
        }

        protected override void AddAnimations()
        {
            if (SlimeAnimations == null)
            {
                SlimeAnimations = new Dictionary<string, string[]>
                {
                    {
                        "attack",
                        new string[]
                        {
                            "slime-attack-0.png",
                            "slime-attack-1.png",
                            "slime-attack-2.png",
                            "slime-attack-3.png",
                            "slime-attack-4.png",
                        }
                    },
                    {
                        "die",
                        new string[]
                        {
                            "slime-die-0.png",
                            "slime-die-1.png",
                            "slime-die-2.png",
                            "slime-die-3.png",
                        }
                    },
                    {
                        "hurt",
                        new string[]
                        {
                            "slime-hurt-0.png",
                            "slime-hurt-1.png",
                            "slime-hurt-2.png",
                            "slime-hurt-3.png",
                        }
                    },
                    {
                        "idle",
                        new string[]
                        {
                            "slime-idle-0.png",
                            "slime-idle-1.png",
                            "slime-idle-2.png",
                            "slime-idle-3.png",
                        }
                    },
                    {
                        "move",
                        new string[]
                        {
                            "slime-move-0.png",
                            "slime-move-1.png",
                            "slime-move-2.png",
                            "slime-move-3.png",
                        }
                    },
                };
            }
            Animations = SlimeAnimations;

            Animator.AddAnimation("attack", new SpriteAnimation(new Sprite[]
            {
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["attack"][0]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["attack"][1]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["attack"][2]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["attack"][3]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["attack"][4]}")),
            }, 10));
            Animator.AddAnimation("die", new SpriteAnimation(new Sprite[]
            {
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["die"][0]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["die"][1]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["die"][2]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["die"][3]}")),
            }, 10));
            Animator.AddAnimation("hurt", new SpriteAnimation(new Sprite[]
            {
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["hurt"][0]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["hurt"][1]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["hurt"][2]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["hurt"][3]}")),
            }, 10));
            Animator.AddAnimation("idle", new SpriteAnimation(new Sprite[]
            {
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["idle"][0]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["idle"][1]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["idle"][2]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["idle"][3]}")),
            }, 5));
            Animator.AddAnimation("move", new SpriteAnimation(new Sprite[]
            {
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["move"][0]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["move"][1]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["move"][2]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["move"][3]}")),
            }, 10));
        }

        protected override void UpdateAnimations()
        {
            var animation = "idle";
            var playerNearby = PlayerNearby();
            if (playerNearby != 0)
            {
                animation = "move";
                if (playerNearby == 1)
                    Animator.FlipX = true;
                else
                    Animator.FlipX = false;
            }

            if (!Animator.IsAnimationActive(animation))
                Animator.Play(animation);
        }

        protected override void UpdateMovement()
        {
            Velocity.X = PlayerNearby() * Speed;

            base.UpdateMovement();
        }

        int PlayerNearby()
        {
            var d = Entity.Transform.Position - Entity.Scene.FindEntity("player").Transform.Position;
            if (d.Length() < 200)
            {
                if (Entity.Transform.Position.X < Entity.Scene.FindEntity("player").Transform.Position.X)
                    return 1;
                else if (Entity.Transform.Position.X > Entity.Scene.FindEntity("player").Transform.Position.X)
                    return -1;
            }
            return 0;
        }
    }
}
