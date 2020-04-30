using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using NezTest2.Backend;

namespace NezTest2
{
    public class Player : Component, IUpdatable
    {
        SpriteAnimator animator;
        AnimationChainer animationChainer;

        Mover mover;
        float speed = 70f;

        SubpixelVector2 subpixelVector2 = new SubpixelVector2();
        VirtualIntegerAxis xAxisInput;
        VirtualIntegerAxis yAxisInput;
        VirtualButton primaryAttack;

        bool animationLock = false;

        public override void OnAddedToEntity()
        {
            animator = Entity.AddComponent(new SpriteAnimator());
            AddAnimations();

            animationChainer = Entity.AddComponent(new AnimationChainer());

            mover = Entity.AddComponent(new Mover());

            SetupInput();
        }

        public override void OnRemovedFromEntity()
        {
            xAxisInput.Deregister();
            yAxisInput.Deregister();
        }

        void AddAnimations()
        {
            string path = "Content/Player/adventurer-";
            animator.AddAnimation("Idle", 4, new Sprite[]
            {
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}idle-00.png")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}idle-01.png")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}idle-02.png")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}idle-03.png")),
            });
            animator.AddAnimation("Run", 9, new Sprite[]
            {
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}run-00.png")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}run-01.png")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}run-02.png")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}run-03.png")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}run-04.png")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}run-05.png")),
            });
            animator.AddAnimation("Attack1", 5 * 2, new Sprite[]
            {
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}attack1-00.png")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}attack1-01.png")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}attack1-02.png")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}attack1-03.png")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}attack1-04.png")),
            });
            animator.AddAnimation("Attack2", 5 * 2, new Sprite[]
            {
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}attack2-00.png")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}attack2-01.png")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}attack2-02.png")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}attack2-03.png")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}attack2-04.png")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}attack2-05.png")),
            });
            animator.AddAnimation("Attack3", 5 * 2, new Sprite[]
            {
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}attack3-00.png")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}attack3-01.png")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}attack3-02.png")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}attack3-03.png")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}attack3-04.png")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}attack3-05.png")),
            });
            animator.AddAnimation("UseItem", 10, new Sprite[]
            {
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}items-00.png")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}items-01.png")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}items-02.png")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}items-01.png")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}items-00.png")),
            });
        }

        void SetupInput()
        {
            xAxisInput = new VirtualIntegerAxis();
            xAxisInput.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.Left, Keys.Right));

            yAxisInput = new VirtualIntegerAxis();
            yAxisInput.Nodes.Add(new VirtualAxis.KeyboardKeys(VirtualInput.OverlapBehavior.TakeNewer, Keys.Up, Keys.Down));

            primaryAttack = new VirtualButton();
            primaryAttack.Nodes.Add(new VirtualButton.KeyboardKey(Keys.Z));
        }

        void IUpdatable.Update()
        {
            var moveDir = new Vector2(xAxisInput.Value, yAxisInput.Value);
            string animation;
            var loopMode = SpriteAnimator.LoopMode.Loop;
            
            if (animator.IsCompleted &&
                animator.CurrentAnimationName.Contains("Attack"))
                animationChainer.Start(animator.CurrentAnimationName);

            if (!animationLock || (animationLock && animator.IsCompleted))
            {
                // animation locking actions
                if (primaryAttack.IsDown)
                {
                    animation = "Attack1";
                    if (animationChainer.WithinChainTime()) {
                        if (animationChainer.PrevAnimation == "Attack1")
                            animation = "Attack2";
                        if (animationChainer.PrevAnimation == "Attack2")
                            animation = "Attack3";
                    }
                    animationChainer.End();
                    animationLock = true;
                    loopMode = SpriteAnimator.LoopMode.Once;
                }
                // movement
                else if (moveDir.X != 0)
                {
                    animation = "Run";
                    animationLock = false;
                    if (moveDir.X < 0)
                        animator.FlipX = true;
                    else
                        animator.FlipX = false;
                }
                // idle
                else
                {
                    animation = "Idle";
                    animationLock = false;
                }

                if (!animator.IsAnimationActive(animation))
                    animator.Play(animation, loopMode);
            }

            if (moveDir != Vector2.Zero && animator.IsAnimationActive("Run"))
            {
                var movement = moveDir * speed * Time.DeltaTime;
                subpixelVector2.Update(ref movement);
                mover.CalculateMovement(ref movement, out var res);
                mover.ApplyMovement(movement);
            }
        }
    }
}
