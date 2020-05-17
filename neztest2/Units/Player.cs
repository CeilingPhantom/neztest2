using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using Nez.Tiled;

namespace NezTest2.Units
{
    public class Player : Unit
    {
        static TmxTileset PlayerAnimationFrames;
        static Dictionary<string, string[]> PlayerAnimations;

        AnimationChainer animationChainer;

        VirtualIntegerAxis xAxisInput;
        VirtualIntegerAxis yAxisInput;
        VirtualButton primaryAttackInput, jumpInput;

        public Player() : base("player", new Vector2(15, 5), 20, 30)
        {
            if (PlayerAnimationFrames == null)
                using (var stream = TitleContainer.OpenStream($"{ContentPath}/{Name}.tsx"))
                    PlayerAnimationFrames = new TmxTileset().LoadTmxTileset(null, System.Xml.Linq.XDocument.Load(stream).Element("tileset"), 1, ContentPath);
            AnimationFrames = PlayerAnimationFrames;
        }

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();

            animationChainer = Entity.AddComponent(new AnimationChainer());

            SetupInput();
        }

        public override void OnRemovedFromEntity()
        {
            xAxisInput.Deregister();
            yAxisInput.Deregister();
        }

        protected override void AddAnimations()
        {
            if (PlayerAnimations == null)
            {
                PlayerAnimations = new Dictionary<string, string[]>
                {
                    {
                        "attack1",
                        new string[]
                        {
                            "adventurer-attack1-00.png",
                            "adventurer-attack1-01.png",
                            "adventurer-attack1-02.png",
                            "adventurer-attack1-03.png",
                            "adventurer-attack1-04.png",
                        }
                    },
                    {
                        "attack2",
                        new string[]
                        {
                            "adventurer-attack2-00.png",
                            "adventurer-attack2-01.png",
                            "adventurer-attack2-02.png",
                            "adventurer-attack2-03.png",
                            "adventurer-attack2-04.png",
                            "adventurer-attack2-05.png",
                        }
                    },
                    {
                        "attack3",
                        new string[]
                        {
                            "adventurer-attack3-00.png",
                            "adventurer-attack3-01.png",
                            "adventurer-attack3-02.png",
                            "adventurer-attack3-03.png",
                            "adventurer-attack3-04.png",
                            "adventurer-attack3-05.png",
                        }
                    },
                    {
                        "fall",
                        new string[]
                        {
                            "adventurer-fall-00.png",
                            "adventurer-fall-01.png",
                        }
                    },
                    {
                        "idle",
                        new string[]
                        {
                            "adventurer-idle-00.png",
                            "adventurer-idle-01.png",
                            "adventurer-idle-02.png",
                            "adventurer-idle-03.png",
                        }
                    },
                    {
                        "jump",
                        new string[]
                        {
                            "adventurer-jump-02.png",
                            "adventurer-jump-03.png",
                        }
                    },
                    {
                        "run",
                        new string[]
                        {
                            "adventurer-run-00.png",
                            "adventurer-run-01.png",
                            "adventurer-run-02.png",
                            "adventurer-run-03.png",
                            "adventurer-run-04.png",
                            "adventurer-run-05.png",
                        }
                    },
                    {
                        "useItem",
                        new string[]
                        {
                            "adventurer-items-00.png",
                            "adventurer-items-01.png",
                            "adventurer-items-02.png",
                            "adventurer-items-01.png",
                            "adventurer-items-00.png",
                        }
                    },
                };
            }
            Animations = PlayerAnimations;

            Animator.AddAnimation("attack1", new SpriteAnimation(new Sprite[]
            {
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["attack1"][0]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["attack1"][1]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["attack1"][2]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["attack1"][3]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["attack1"][4]}")),
            }, 10));
            Animator.AddAnimation("attack2", new SpriteAnimation(new Sprite[]
            {
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["attack2"][0]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["attack2"][1]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["attack2"][2]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["attack2"][3]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["attack2"][4]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["attack2"][5]}")),
            }, 10));
            Animator.AddAnimation("attack3", new SpriteAnimation(new Sprite[]
            {
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["attack3"][0]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["attack3"][1]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["attack3"][2]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["attack3"][3]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["attack3"][4]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["attack3"][5]}")),
            }, 10));
            Animator.AddAnimation("fall", new SpriteAnimation(new Sprite[]
            {
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["fall"][0]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["fall"][1]}")),
            }, 10));
            Animator.AddAnimation("idle", new SpriteAnimation(new Sprite[]
            {
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["idle"][0]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["idle"][1]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["idle"][2]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["idle"][3]}")),
            }, 5));
            Animator.AddAnimation("jump", new SpriteAnimation(new Sprite[]
            {
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["jump"][0]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["jump"][1]}")),
            }, 10));
            Animator.AddAnimation("run", new SpriteAnimation(new Sprite[]
            {
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["run"][0]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["run"][1]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["run"][2]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["run"][3]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["run"][4]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["run"][5]}")),
            }, 10));
            Animator.AddAnimation("useItem", new SpriteAnimation(new Sprite[]
            {
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["useItem"][0]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["useItem"][1]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["useItem"][2]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["useItem"][3]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["useItem"][4]}")),
            }, 10));
        }

        void SetupInput()
        {
            xAxisInput = new VirtualIntegerAxis();
            xAxisInput.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.Left, Keys.Right));

            yAxisInput = new VirtualIntegerAxis();
            yAxisInput.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.Up, Keys.Down));

            primaryAttackInput = new VirtualButton();
            primaryAttackInput.Nodes.Add(new VirtualButton.KeyboardKey(Keys.Z));

            jumpInput = new VirtualButton();
            jumpInput.Nodes.Add(new VirtualButton.KeyboardKey(Keys.Space));
        }

        protected override void UpdateAnimations()
        {
            string animation;
            var loopMode = SpriteAnimator.LoopMode.Loop;

            if (Animator.IsCompleted &&
                Animator.CurrentAnimationName.Contains("attack"))
                animationChainer.Start(Animator.CurrentAnimationName);

            if (!AnimationLock || (AnimationLock && Animator.IsCompleted))
            {
                // normal attack
                if (primaryAttackInput.IsDown)
                {
                    animation = "attack1";
                    if (animationChainer.WithinChainTime())
                    {
                        if (animationChainer.PrevAnimation == "attack1")
                            animation = "attack2";
                        if (animationChainer.PrevAnimation == "attack2")
                            animation = "attack3";
                    }
                    animationChainer.End();
                    AnimationLock = true;
                    MovementLock = true;
                    loopMode = SpriteAnimator.LoopMode.Once;
                    UnitsCurrAnimationHasAttacked.Clear();
                }
                // jumping
                else if ((TiledCollisionState.Below && jumpInput.IsPressed) || (!TiledCollisionState.Below && Velocity.Y <= 0))
                {
                    animation = "jump";
                    AnimationLock = false;
                    MovementLock = false;
                    loopMode = SpriteAnimator.LoopMode.ClampForever;
                    if (Velocity.X < 0)
                        Animator.FlipX = true;
                    else if (Velocity.X > 0)
                        Animator.FlipX = false;
                }
                // falling
                else if (!TiledCollisionState.Below && Velocity.Y > 0)
                {
                    animation = "fall";
                    AnimationLock = false;
                    MovementLock = false;
                    loopMode = SpriteAnimator.LoopMode.ClampForever;
                    if (Velocity.X < 0)
                        Animator.FlipX = true;
                    else if (Velocity.X > 0)
                        Animator.FlipX = false;
                }
                // movement
                else if (Velocity.X != 0)
                {
                    animation = "run";
                    AnimationLock = false;
                    MovementLock = false;
                    if (Velocity.X < 0)
                        Animator.FlipX = true;
                    else
                        Animator.FlipX = false;
                }
                // idle
                else
                {
                    animation = "idle";
                    AnimationLock = false;
                    MovementLock = false;
                }

                if (!Animator.IsAnimationActive(animation))
                    Animator.Play(animation, loopMode);
            }
        }

        protected override void UpdateMovement()
        {
            Velocity.X = 0;
            if (!MovementLock)
            {
                if (xAxisInput.Value < 0)
                    Velocity.X = -Speed;
                else if (xAxisInput > 0)
                    Velocity.X = Speed;

                if (TiledCollisionState.Below && jumpInput.IsPressed)
                    Velocity.Y = -Mathf.Sqrt(JumpHeight * Gravity);
            }

            base.UpdateMovement();
        }
    }
}
