﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using Nez.Tiled;
using NezTest2.Util;

namespace NezTest2.Units
{
    public class Player : Unit
    {
        static TmxTileset PlayerAnimationFrames;
        static Dictionary<string, string[]> PlayerAnimations;

        enum ActionLock
        {
            None,
            Attack,
        }
        ActionLock actionLock = ActionLock.None;

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

            Health = 100;
        }

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            SetupAnimationChainer();
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
                        "die",
                        new string[]
                        {
                            "adventurer-die-00.png",
                            "adventurer-die-01.png",
                            "adventurer-die-02.png",
                            "adventurer-die-03.png",
                            "adventurer-die-04.png",
                            "adventurer-die-05.png",
                            "adventurer-die-06.png",
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
            Animator.AddAnimation("die", new SpriteAnimation(new Sprite[]
            {
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["die"][0]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["die"][1]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["die"][2]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["die"][3]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["die"][4]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["die"][5]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["die"][5]}")),
            }, 5));
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

        void SetupAnimationChainer()
        {
            animationChainer = Entity.AddComponent(new AnimationChainer());
            animationChainer.AddChainableAnimation("attack1");
            animationChainer.AddChainableAnimation("attack2");
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
            if (Animator.IsCompleted)
                actionLock = ActionLock.None;

            if (Animator.IsCompleted && animationChainer.IsChainableAnimation(Animator.CurrentAnimationName))
                animationChainer.Start(Animator.CurrentAnimationName);

            if (actionLock == ActionLock.None)
            {
                string animation;
                var loopMode = SpriteAnimator.LoopMode.Loop;

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
                    actionLock = ActionLock.Attack;
                    loopMode = SpriteAnimator.LoopMode.ClampForever;
                    UnitsCurrAnimationHasAttacked.Clear();
                }
                // jumping
                else if ((TiledCollisionState.Below && jumpInput.IsPressed) || (!TiledCollisionState.Below && Velocity.Y <= 0))
                {
                    animation = "jump";
                    actionLock = ActionLock.None;
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
                    actionLock = ActionLock.None;
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
                    actionLock = ActionLock.None;
                    if (Velocity.X < 0)
                        Animator.FlipX = true;
                    else
                        Animator.FlipX = false;
                }
                // idle
                else
                {
                    animation = "idle";
                    actionLock = ActionLock.None;
                }

                if (!Animator.IsAnimationActive(animation))
                    Animator.Play(animation, loopMode);
            }
        }

        protected override void UpdateMovement()
        {
            Velocity.X = 0;
            if (actionLock == ActionLock.None)
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

        public override void Stunned() { }

        protected override bool Die()
        {
            if (!Animator.IsAnimationActive("die"))
                Animator.Play("die", SpriteAnimator.LoopMode.ClampForever);
            if (Animator.IsCompleted)
                return true;
            return false;
        }
    }
}
