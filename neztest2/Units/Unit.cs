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

        protected TmxTileset AnimationFrames;

        protected string PrevAnimationFrameName;
        protected bool PrevCollidersFlipX = false;
        protected Dictionary<string, List<BoxCollider>> HitboxColliders;
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

        private readonly bool NewStatReq;
        protected UnitStat Stats;

        public Unit(string name, Vector2 tiledColliderTopLeft, float tiledColliderWidth, float tiledColliderHeight, bool newStatReq=true)
        {
            Name = name;
            ContentPath = $"Content/Units/{Name}";
            TiledColliderTopLeft = tiledColliderTopLeft;
            TiledColliderWidth = tiledColliderWidth;
            TiledColliderHeight = tiledColliderHeight;
            NewStatReq = newStatReq;
        }

        #region Component Lifecycle

        public override void OnAddedToEntity()
        {
            if (NewStatReq)
                SetupNewStat();
            else
                Stats = Entity.GetComponent<UnitStat>();

            Animator = Entity.AddComponent(new SpriteAnimator());
            AddAnimations();

            AddColliders();

            Mover = Entity.GetComponent<TiledMapMover>();
            TiledCollisionState = new TiledMapMover.CollisionState();
        }

        protected virtual void SetupNewStat()
        {
            Stats = Entity.AddComponent(new UnitStat());
        }

        protected abstract void AddAnimations();

        protected void AddColliders()
        {
            TiledCollider = Entity.AddComponent(new BoxCollider(TiledColliderTopLeft.X - AnimationFrames.TileWidth / 2, TiledColliderTopLeft.Y - AnimationFrames.TileHeight / 2, TiledColliderWidth, TiledColliderHeight));
            allColliders = new List<Collider> { TiledCollider };

            HitboxColliders = new Dictionary<string, List<BoxCollider>>();
            AttackColliders = new Dictionary<string, List<BoxCollider>>();

            foreach (TmxTilesetTile frame in AnimationFrames.Tiles.Values)
            {
                string img = frame.Image.Source.Remove(0, $"{ContentPath}\\".Length);
                var hitboxColliders = new List<BoxCollider>();
                var attackColliders = new List<BoxCollider>();
                if (frame.ObjectGroups.Contains("collision"))
                    foreach (var colliderObj in frame.ObjectGroups["collision"].Objects)
                    {
                        var collider = Entity.AddComponent(new BoxCollider(colliderObj.X - AnimationFrames.TileWidth / 2, colliderObj.Y - AnimationFrames.TileHeight / 2, colliderObj.Width, colliderObj.Height));
                        collider.SetEnabled(false);
                        if (colliderObj.Name.Equals("hitbox"))
                        {
                            hitboxColliders.Add(collider);
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
                HitboxColliders.Add(img, hitboxColliders);
                AttackColliders.Add(img, attackColliders);

                allColliders.AddRange(hitboxColliders);
                allColliders.AddRange(attackColliders);
            }
        }

        #endregion

        #region Per Frame Unit Updating

        public virtual void Update()
        {
            if (Stats.Health > 0)
            {
                UpdateAnimations();
                UpdateMovement();

                UpdateColliders();

                CheckAttackUnit();
            }
            else if (Die())
                Entity.Destroy();
        }

        protected abstract void UpdateAnimations();

        protected virtual void UpdateMovement()
        {
            // falling motion
            Velocity.Y += Stats.Gravity * Time.DeltaTime;

            Mover.Move(Velocity * Time.DeltaTime, TiledCollider, TiledCollisionState);

            if (TiledCollisionState.Below || TiledCollisionState.Above)
                Velocity.Y = 0;
        }

        protected void UpdateColliders()
        {
            if (PrevAnimationFrameName != null) {
                if (PrevCollidersFlipX)
                    ColliderFlipX(TiledCollider);
                foreach (var collider in HitboxColliders[PrevAnimationFrameName])
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
            foreach (var collider in HitboxColliders[PrevAnimationFrameName])
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
            unit.Attacked(this, (int) Math.Round(Nez.Random.Range(Stats.DamageLow, Stats.DamageHigh)));
        }

        #endregion

        protected void ColliderFlipX(Collider collider) => collider.SetLocalOffset(new Vector2(-collider.LocalOffset.X, collider.LocalOffset.Y));

        /// <summary>
        /// called when attacked by a unit
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="damage"></param>
        public virtual void Attacked(Unit unit, int damage)
        {
            if (Stats.Health > 0)
            {
                if (Stats.TakeDamage(damage) <= 0)
                    Die();
            }
        }

        public abstract void Stunned();

        /// <summary>
        /// returns true when death animation, etc have finished
        /// </summary>
        /// <returns></returns>
        protected abstract bool Die();
    }
}
