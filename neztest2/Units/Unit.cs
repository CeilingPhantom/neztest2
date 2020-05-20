using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Tiled;

namespace NezTest2.Units
{
    public abstract class Unit : Component, IUpdatable
    {
        protected string Name { get; private set; }
        protected string ContentPath { get; private set; }

        protected Dictionary<string, string[]> Animations;
        protected SpriteAnimator Animator;
        protected bool AnimationLock, MovementLock = false;

        protected TmxTileset AnimationFrames;

        protected string PrevAnimationFrameName;
        protected bool PrevCollidersFlipX = false;
        protected Dictionary<string, List<BoxCollider>> EntityColliders;
        protected Dictionary<string, List<BoxCollider>> AttackColliders;

        List<Collider> allColliders;

        // UpdateAnimations() responsible for clearing this list when an "attack" animation starts
        protected List<Unit> UnitsCurrAnimationHasAttacked = new List<Unit>();

        protected Vector2 TiledColliderTopLeft;
        protected float TiledColliderWidth, TiledColliderHeight;
        protected BoxCollider TiledCollider;
        protected TiledMapMover Mover;
        protected TiledMapMover.CollisionState TiledCollisionState;
        protected Vector2 Velocity = Vector2.Zero;

        protected float Speed = 70f;
        protected float JumpHeight = 50f;
        protected float Gravity = 600f;

        protected int Health = 100;
        protected int DamageLow = 30;
        protected int DamageHigh = 40;

        public Unit(string name, Vector2 tiledColliderTopLeft, float tiledColliderWidth, float tiledColliderHeight)
        {
            Name = name;
            ContentPath = $"Content/Units/{Name}";
            TiledColliderTopLeft = tiledColliderTopLeft;
            TiledColliderWidth = tiledColliderWidth;
            TiledColliderHeight = tiledColliderHeight;
        }

        #region Component Lifecycle

        public override void OnAddedToEntity()
        {
            Animator = Entity.AddComponent(new SpriteAnimator());
            AddAnimations();

            AddColliders();

            Mover = Entity.GetComponent<TiledMapMover>();
            TiledCollisionState = new TiledMapMover.CollisionState();
        }

        public override void OnRemovedFromEntity()
        {
            base.OnRemovedFromEntity();
        }

        protected abstract void AddAnimations();

        protected void AddColliders()
        {
            TiledCollider = Entity.AddComponent(new BoxCollider(TiledColliderTopLeft.X - AnimationFrames.TileWidth / 2, TiledColliderTopLeft.Y - AnimationFrames.TileHeight / 2, TiledColliderWidth, TiledColliderHeight));
            allColliders = new List<Collider> { TiledCollider };

            EntityColliders = new Dictionary<string, List<BoxCollider>>();
            AttackColliders = new Dictionary<string, List<BoxCollider>>();

            foreach (TmxTilesetTile frame in AnimationFrames.Tiles.Values)
            {
                string img = frame.Image.Source.Remove(0, $"{ContentPath}\\".Length);
                var entityColliders = new List<BoxCollider>();
                var attackColliders = new List<BoxCollider>();
                if (frame.ObjectGroups.Contains("collision"))
                    foreach (var colliderObj in frame.ObjectGroups["collision"].Objects)
                    {
                        var collider = Entity.AddComponent(new BoxCollider(colliderObj.X - AnimationFrames.TileWidth / 2, colliderObj.Y - AnimationFrames.TileHeight / 2, colliderObj.Width, colliderObj.Height));
                        collider.SetEnabled(false);
                        if (colliderObj.Name.Equals("entity"))
                        {
                            entityColliders.Add(collider);
                            Flags.SetFlagExclusive(ref collider.CollidesWithLayers, 1);
                            Flags.SetFlagExclusive(ref collider.PhysicsLayer, 1);
                        }
                        else if (colliderObj.Name.Equals("attack"))
                        {
                            attackColliders.Add(collider);
                            Flags.SetFlagExclusive(ref collider.CollidesWithLayers, 1);
                            Flags.SetFlagExclusive(ref collider.PhysicsLayer, 2);
                        }
                    }
                EntityColliders.Add(img, entityColliders);
                AttackColliders.Add(img, attackColliders);

                allColliders.AddRange(entityColliders);
                allColliders.AddRange(attackColliders);
            }
        }

        #endregion

        #region Per Frame Unit Updating

        public virtual void Update()
        {
            UpdateAnimations();
            UpdateMovement();

            UpdateColliders();

            CheckAttackUnit();
        }

        protected abstract void UpdateAnimations();

        protected virtual void UpdateMovement()
        {
            // falling motion
            Velocity.Y += Gravity * Time.DeltaTime;

            Mover.Move(Velocity * Time.DeltaTime, TiledCollider, TiledCollisionState);

            if (TiledCollisionState.Below || TiledCollisionState.Above)
                Velocity.Y = 0;
        }

        protected void UpdateColliders()
        {
            if (PrevAnimationFrameName != null) {
                if (PrevCollidersFlipX)
                    ColliderFlipX(TiledCollider);
                foreach (var collider in EntityColliders[PrevAnimationFrameName])
                {
                    if (PrevCollidersFlipX)
                        ColliderFlipX(collider);
                    collider.SetEnabled(false);
                }
                foreach (var collider in AttackColliders[PrevAnimationFrameName])
                {
                    if (PrevCollidersFlipX)
                        ColliderFlipX(collider);
                    collider.SetEnabled(false);
                }
            }

            PrevAnimationFrameName = Animations[Animator.CurrentAnimationName][Animator.CurrentFrame];
            PrevCollidersFlipX = false;
            if (Animator.FlipX)
            {
                PrevCollidersFlipX = true;
                ColliderFlipX(TiledCollider);
            }
            foreach (var collider in EntityColliders[PrevAnimationFrameName])
            {
                if (PrevCollidersFlipX)
                    ColliderFlipX(collider);
                collider.SetEnabled(true);
            }
            foreach (var collider in AttackColliders[PrevAnimationFrameName])
            {
                if (PrevCollidersFlipX)
                    ColliderFlipX(collider);
                collider.SetEnabled(true);
            }
        }

        protected void CheckAttackUnit()
        {
            if (Animator.CurrentAnimationName.Contains("attack"))
            {
                foreach (var collider in AttackColliders[PrevAnimationFrameName])
                {
                    if (collider.CollidesWithAnyBut(allColliders, out List<CollisionResult> results))
                    {
                        foreach (CollisionResult result in results)
                        {
                            var unit = result.Collider.Entity.GetComponent<Unit>();
                            if (unit != null && !UnitsCurrAnimationHasAttacked.Contains(unit))
                            {
                                AttackUnit(unit);
                                UnitsCurrAnimationHasAttacked.Add(unit);
                            }
                        }
                    }
                }
            }
        }

        protected virtual void AttackUnit(Unit unit)
        {
            unit.Attacked(Nez.Random.Range(DamageLow, DamageHigh));
        }

        #endregion

        protected void ColliderFlipX(Collider collider) => collider.SetLocalOffset(new Vector2(-collider.LocalOffset.X, collider.LocalOffset.Y));

        public virtual void Attacked(int damage)
        {
            Health -= damage;
            System.Diagnostics.Debug.WriteLine(Health);
            if (Health < 0)
                Entity.Destroy();
        }
    }
}
