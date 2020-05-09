using System.Collections.Generic;
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
        Dictionary<string, string[]> animations;
        SpriteAnimator animator;
        AnimationChainer animationChainer;

        TmxTileset animationFrames;

        string prevAnimationFrameName;
        bool prevCollidersFlipX = false;
        Dictionary<string, List<BoxCollider>> entityColliders;
        Dictionary<string, List<BoxCollider>> attackColliders;

        BoxCollider tiledCollider;
        TiledMapMover mover;
        TiledMapMover.CollisionState tiledCollisionState;
        Vector2 velocity = Vector2.Zero;

        readonly float speed = 70f;
        readonly float jumpHeight = 50f;
        readonly float gravity = 600f;

        VirtualIntegerAxis xAxisInput;
        VirtualIntegerAxis yAxisInput;
        VirtualButton primaryAttackInput, jumpInput;

        bool animationLock = false;
        bool movementLock = false;

        public override void OnAddedToEntity()
        {
            animator = Entity.AddComponent(new SpriteAnimator());
            AddAnimations();

            using (var stream = TitleContainer.OpenStream("Content/player/player.tsx"))
                animationFrames = new TmxTileset().LoadTmxTileset(null, System.Xml.Linq.XDocument.Load(stream).Element("tileset"), 1, "Content/player");

            AddEntityColliders();
            AddAttackColliders();

            animationChainer = Entity.AddComponent(new AnimationChainer());

            tiledCollider = Entity.AddComponent(new BoxCollider(15 - animationFrames.TileWidth / 2, 6 - animationFrames.TileHeight / 2, 20, 30));
            mover = Entity.GetComponent<TiledMapMover>();
            tiledCollisionState = new TiledMapMover.CollisionState();

            SetupInput();
        }

        public override void OnRemovedFromEntity()
        {
            xAxisInput.Deregister();
            yAxisInput.Deregister();
        }

        void AddAnimations()
        {
            string path = "Content/Player/";
            animations = new Dictionary<string, string[]>();

            animations.Add("idle", new string[]
            {
                "adventurer-idle-00.png",
                "adventurer-idle-01.png",
                "adventurer-idle-02.png",
                "adventurer-idle-03.png",
            });
            animations.Add("run", new string[]
            {
                "adventurer-run-00.png",
                "adventurer-run-01.png",
                "adventurer-run-02.png",
                "adventurer-run-03.png",
                "adventurer-run-04.png",
                "adventurer-run-05.png",
            });
            animations.Add("attack1", new string[]
            {
                "adventurer-attack1-00.png",
                "adventurer-attack1-01.png",
                "adventurer-attack1-02.png",
                "adventurer-attack1-03.png",
                "adventurer-attack1-04.png",
            });
            animations.Add("attack2", new string[]
            {
                "adventurer-attack2-00.png",
                "adventurer-attack2-01.png",
                "adventurer-attack2-02.png",
                "adventurer-attack2-03.png",
                "adventurer-attack2-04.png",
                "adventurer-attack2-05.png",
            });
            animations.Add("attack3", new string[]
            {
                "adventurer-attack3-00.png",
                "adventurer-attack3-01.png",
                "adventurer-attack3-02.png",
                "adventurer-attack3-03.png",
                "adventurer-attack3-04.png",
                "adventurer-attack3-05.png",
            });
            animations.Add("jump", new string[]
            {
                "adventurer-jump-02.png",
                "adventurer-jump-03.png",
            });
            animations.Add("fall", new string[]
            {
                "adventurer-fall-00.png",
                "adventurer-fall-01.png",
            });
            animations.Add("useItem", new string[]
            {
                "adventurer-items-00.png",
                "adventurer-items-01.png",
                "adventurer-items-02.png",
                "adventurer-items-01.png",
                "adventurer-items-00.png",
            });

            animator.AddAnimation("idle", new SpriteAnimation(new Sprite[]
            {
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}{animations["idle"][0]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}{animations["idle"][1]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}{animations["idle"][2]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}{animations["idle"][3]}")),
            }, 5));
            animator.AddAnimation("run", new SpriteAnimation(new Sprite[]
            {
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}{animations["run"][0]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}{animations["run"][1]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}{animations["run"][2]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}{animations["run"][3]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}{animations["run"][4]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}{animations["run"][5]}")),
            }, 10));
            animator.AddAnimation("attack1", new SpriteAnimation(new Sprite[]
            {
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}{animations["attack1"][0]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}{animations["attack1"][1]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}{animations["attack1"][2]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}{animations["attack1"][3]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}{animations["attack1"][4]}")),
            }, 10));
            animator.AddAnimation("attack2", new SpriteAnimation(new Sprite[]
            {
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}{animations["attack2"][0]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}{animations["attack2"][1]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}{animations["attack2"][2]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}{animations["attack2"][3]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}{animations["attack2"][4]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}{animations["attack2"][5]}")),
            }, 10));
            animator.AddAnimation("attack3", new SpriteAnimation(new Sprite[]
            {
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}{animations["attack3"][0]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}{animations["attack3"][1]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}{animations["attack3"][2]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}{animations["attack3"][3]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}{animations["attack3"][4]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}{animations["attack3"][5]}")),
            }, 10));
            animator.AddAnimation("jump", new SpriteAnimation(new Sprite[]
            {
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}{animations["jump"][0]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}{animations["jump"][1]}")),
            }, 10));
            animator.AddAnimation("fall", new SpriteAnimation(new Sprite[]
            {
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}{animations["fall"][0]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}{animations["fall"][1]}")),
            }, 10));
            animator.AddAnimation("useItem", new SpriteAnimation(new Sprite[]
            {
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}{animations["useItem"][0]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}{animations["useItem"][1]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}{animations["useItem"][2]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}{animations["useItem"][3]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{path}{animations["useItem"][4]}")),
            }, 10));
        }

        void AddEntityColliders()
        {
            entityColliders = new Dictionary<string, List<BoxCollider>>();
            foreach (TmxTilesetTile frame in animationFrames.Tiles.Values)
            {
                string img = frame.Image.Source.Remove(0, "Content/player\\".Length);
                var colliders = new List<BoxCollider>();
                if (frame.ObjectGroups.Contains("entityCollision"))
                    foreach (var colliderObj in frame.ObjectGroups["entityCollision"].Objects)
                    {
                        var collider = Entity.AddComponent(new BoxCollider(colliderObj.X - animationFrames.TileWidth / 2, colliderObj.Y - animationFrames.TileHeight / 2, colliderObj.Width, colliderObj.Height));
                        collider.SetEnabled(false);
                        colliders.Add(collider);
                    }
                entityColliders.Add(img, colliders);
            }
        }

        void AddAttackColliders()
        {
            attackColliders = new Dictionary<string, List<BoxCollider>>();
            foreach (TmxTilesetTile frame in animationFrames.Tiles.Values)
            {
                string img = frame.Image.Source.Remove(0, "Content/player\\".Length);
                var colliders = new List<BoxCollider>();
                if (frame.ObjectGroups.Contains("attackCollision"))
                    foreach (var colliderObj in frame.ObjectGroups["attackCollision"].Objects)
                    {
                        var collider = Entity.AddComponent(new BoxCollider(colliderObj.X - animationFrames.TileWidth / 2, colliderObj.Y - animationFrames.TileHeight / 2, colliderObj.Width, colliderObj.Height));
                        collider.SetEnabled(false);
                        colliders.Add(collider);
                    }
                attackColliders.Add(img, colliders);
            }
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

            UpdateActiveEntityColliders();
            UpdateActiveAttackColliders();
            prevAnimationFrameName = animations[animator.CurrentAnimationName][animator.CurrentFrame];


        }

        void UpdateAnimations()
        {
            string animation;
            var loopMode = SpriteAnimator.LoopMode.Loop;

            if (animator.IsCompleted &&
                animator.CurrentAnimationName.Contains("attack"))
                animationChainer.Start(animator.CurrentAnimationName);

            if (!animationLock || (animationLock && animator.IsCompleted))
            {
                // animation locking actions
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
                    animationLock = true;
                    movementLock = true;
                    loopMode = SpriteAnimator.LoopMode.Once;
                }
                // jumping
                else if ((tiledCollisionState.Below && jumpInput.IsPressed) || (!tiledCollisionState.Below && velocity.Y <= 0))
                {
                    animation = "jump";
                    animationLock = false;
                    movementLock = false;
                    loopMode = SpriteAnimator.LoopMode.ClampForever;
                    if (velocity.X < 0)
                        animator.FlipX = true;
                    else if (velocity.X > 0)
                        animator.FlipX = false;
                }
                // falling
                else if (!tiledCollisionState.Below && velocity.Y > 0)
                {
                    animation = "fall";
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
                    animation = "run";
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
                    animation = "idle";
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

                if (tiledCollisionState.Below && jumpInput.IsPressed)
                    velocity.Y = -Mathf.Sqrt(jumpHeight * gravity);
            }

            velocity.Y += gravity * Time.DeltaTime;

            if (velocity != Vector2.Zero)
                mover.Move(velocity * Time.DeltaTime, tiledCollider, tiledCollisionState);

            if (tiledCollisionState.Below)
                velocity.Y = 0;
        }

        void ColliderFlipX(Collider collider) => collider.SetLocalOffset(new Vector2(-collider.LocalOffset.X, collider.LocalOffset.Y));

        void UpdateActiveEntityColliders()
        {
            if (prevAnimationFrameName != null)
                foreach (var collider in entityColliders[prevAnimationFrameName])
                {
                    if (prevCollidersFlipX)
                        ColliderFlipX(collider);
                    collider.SetEnabled(false);
                }

            var img = animations[animator.CurrentAnimationName][animator.CurrentFrame];
            foreach (var collider in entityColliders[img])
            {
                if (animator.FlipX)
                {
                    ColliderFlipX(collider);
                    prevCollidersFlipX = true;
                }
                else
                    prevCollidersFlipX = false;
                collider.SetEnabled(true);
            }
        }

        void UpdateActiveAttackColliders()
        {
            if (prevAnimationFrameName != null)
                foreach (var collider in attackColliders[prevAnimationFrameName])
                {
                    if (prevCollidersFlipX)
                        ColliderFlipX(collider);
                    collider.SetEnabled(false);
                }

            var img = animations[animator.CurrentAnimationName][animator.CurrentFrame];
            foreach (var collider in attackColliders[img])
            {
                if (animator.FlipX)
                {
                    ColliderFlipX(collider);
                    prevCollidersFlipX = true;
                }
                else
                    prevCollidersFlipX = false;
                collider.SetEnabled(true);
            }
        }

        void CheckAttacked() { }
    }
}
