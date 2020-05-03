using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using Nez.Tiled;

namespace NezTest2
{
    public class Player : Component, IUpdatable
    {
        SpriteAnimator animator;
        AnimationChainer animationChainer;

        BoxCollider collider;
        TiledMapMover mover;
        TiledMapMover.CollisionState collisionState;
        Vector2 velocity = Vector2.Zero;

        float speed = 70f;
        float jumpHeight = 50f;
        float gravity = 600f;

        VirtualIntegerAxis xAxisInput;
        VirtualIntegerAxis yAxisInput;
        VirtualButton primaryAttackInput, jumpInput;

        bool animationLock = false;
        bool movementLock = false;

        public override void OnAddedToEntity()
        {
            animator = Entity.AddComponent(new SpriteAnimator());
            AddAnimations();

            animationChainer = Entity.AddComponent(new AnimationChainer());

            collider = Entity.GetComponent<BoxCollider>();
            mover = Entity.GetComponent<TiledMapMover>();
            collisionState = new TiledMapMover.CollisionState();

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
            animator.AddAnimation("Jump", 5 * 2, new Sprite[]
            {
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}jump-02.png")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}jump-03.png")),
            });
            animator.AddAnimation("Fall", 5 * 2, new Sprite[]
            {
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}jump-03.png")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}jump-02.png")),
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

            primaryAttackInput = new VirtualButton();
            primaryAttackInput.Nodes.Add(new VirtualButton.KeyboardKey(Keys.Z));

            jumpInput = new VirtualButton();
            jumpInput.Nodes.Add(new VirtualButton.KeyboardKey(Keys.Space));
        }

        void IUpdatable.Update()
        {
            UpdateAnimations();
            UpdateMovement();
        }

        void UpdateAnimations()
        {
            string animation;
            var loopMode = SpriteAnimator.LoopMode.Loop;

            if (animator.IsCompleted &&
                animator.CurrentAnimationName.Contains("Attack"))
                animationChainer.Start(animator.CurrentAnimationName);

            if (!animationLock || (animationLock && animator.IsCompleted))
            {
                // animation locking actions
                if (primaryAttackInput.IsDown)
                {
                    animation = "Attack1";
                    if (animationChainer.WithinChainTime())
                    {
                        if (animationChainer.PrevAnimation == "Attack1")
                            animation = "Attack2";
                        if (animationChainer.PrevAnimation == "Attack2")
                            animation = "Attack3";
                    }
                    animationChainer.End();
                    animationLock = true;
                    movementLock = true;
                    loopMode = SpriteAnimator.LoopMode.Once;
                }
                // jumping
                else if (!collisionState.Below && velocity.Y <= 0)
                {
                    animation = "Jump";
                    animationLock = false;
                    movementLock = false;
                    loopMode = SpriteAnimator.LoopMode.ClampForever;
                    if (velocity.X < 0)
                        animator.FlipX = true;
                    else if (velocity.X > 0)
                        animator.FlipX = false;
                }
                // falling
                else if (!collisionState.Below && velocity.Y > 0)
                {
                    animation = "Fall";
                    animationLock = false;
                    movementLock = false;
                    loopMode = SpriteAnimator.LoopMode.ClampForever;
                    if (velocity.X < 0)
                        animator.FlipX = true;
                    else if (velocity.X > 0)
                        animator.FlipX = false;
                }
                // movement
                else if (velocity.X != 0)
                {
                    animation = "Run";
                    animationLock = false;
                    movementLock = false;
                    if (velocity.X < 0)
                        animator.FlipX = true;
                    else
                        animator.FlipX = false;
                }
                // idle
                else
                {
                    animation = "Idle";
                    animationLock = false;
                    movementLock = false;
                }

                if (!animator.IsAnimationActive(animation))
                    animator.Play(animation, loopMode);
            }
        }

        void UpdateMovement()
        {
            velocity.X = 0;
            if (!movementLock)
            {
                if (xAxisInput.Value < 0)
                    velocity.X = -speed;
                else if (xAxisInput > 0)
                    velocity.X = speed;

                if (collisionState.Below && jumpInput.IsPressed)
                    velocity.Y = -Mathf.Sqrt(jumpHeight * gravity);
            }

            velocity.Y += gravity * Time.DeltaTime;

            if (velocity != Vector2.Zero)
                mover.Move(velocity * Time.DeltaTime, collider, collisionState);

            if (collisionState.Below)
                velocity.Y = 0;
        }
    }
}
