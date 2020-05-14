﻿using System.Collections.Generic;
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

        protected float Health = 100f;
        protected float DamageLow = 30f;
        protected float DamageHigh = 40f;

        public Unit(string name, string contentPath, Vector2 tiledColliderTopLeft, float tiledColliderWidth, float tiledColliderHeight)
        {
            Name = name;
            ContentPath = contentPath;
            TiledColliderTopLeft = tiledColliderTopLeft;
            TiledColliderWidth = tiledColliderWidth;
            TiledColliderHeight = tiledColliderHeight;
        }

        #region Component Lifecycle

        public override void OnAddedToEntity()
        {
            Animator = Entity.AddComponent(new SpriteAnimator());
            AddAnimations();

            using (var stream = TitleContainer.OpenStream($"{ContentPath}/{Name}.tsx"))
                AnimationFrames = new TmxTileset().LoadTmxTileset(null, System.Xml.Linq.XDocument.Load(stream).Element("tileset"), 1, ContentPath);

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
                        Flags.SetFlagExclusive(ref collider.CollidesWithLayers, 1);
                        Flags.SetFlagExclusive(ref collider.PhysicsLayer, 1);
                        collider.SetEnabled(false);
                        if (colliderObj.Name.Equals("entity"))
                            entityColliders.Add(collider);
                        else if (colliderObj.Name.Equals("attack"))
                            attackColliders.Add(collider);
                    }
                EntityColliders.Add(img, entityColliders);
                AttackColliders.Add(img, attackColliders);
            }
        }

        #endregion

        #region Per Frame Unit Updating

        public virtual void Update()
        {
            UpdateAnimations();
            UpdateMovement();

            UpdateActiveEntityColliders();
            UpdateActiveAttackColliders();
            PrevAnimationFrameName = Animations[Animator.CurrentAnimationName][Animator.CurrentFrame];

            CheckAttackUnit();
        }

        protected abstract void UpdateAnimations();

        protected abstract void UpdateMovement();

        protected void UpdateActiveEntityColliders()
        {
            if (PrevAnimationFrameName != null)
                foreach (var collider in EntityColliders[PrevAnimationFrameName])
                {
                    if (PrevCollidersFlipX)
                        ColliderFlipX(collider);
                    collider.SetEnabled(false);
                }

            var img = Animations[Animator.CurrentAnimationName][Animator.CurrentFrame];
            foreach (var collider in EntityColliders[img])
            {
                if (Animator.FlipX)
                {
                    ColliderFlipX(collider);
                    PrevCollidersFlipX = true;
                }
                else
                    PrevCollidersFlipX = false;
                collider.SetEnabled(true);
            }
        }

        protected void UpdateActiveAttackColliders()
        {
            if (PrevAnimationFrameName != null)
                foreach (var collider in AttackColliders[PrevAnimationFrameName])
                {
                    if (PrevCollidersFlipX)
                        ColliderFlipX(collider);
                    collider.SetEnabled(false);
                }

            var img = Animations[Animator.CurrentAnimationName][Animator.CurrentFrame];
            foreach (var collider in AttackColliders[img])
            {
                if (Animator.FlipX)
                {
                    ColliderFlipX(collider);
                    PrevCollidersFlipX = true;
                }
                else
                    PrevCollidersFlipX = false;
                collider.SetEnabled(true);
            }
        }

        protected void CheckAttackUnit()
        {
            if (Animator.CurrentAnimationName.Contains("attack"))
            {
                var excludedColliders = new List<Collider> { TiledCollider };
                EntityColliders[PrevAnimationFrameName].ForEach(collider => excludedColliders.Add(collider));
                AttackColliders[PrevAnimationFrameName].ForEach(collider => excludedColliders.Add(collider));
                foreach (var collider in AttackColliders[PrevAnimationFrameName])
                {
                    var res = new CollisionResult();
                    if (collider.CollidesWithAnyBut(excludedColliders, out res))
                    {
                        var unit = res.Collider.Entity.GetComponent<Unit>();
                        if (unit != null && !UnitsCurrAnimationHasAttacked.Contains(unit))
                        {
                            AttackUnit(unit);
                            UnitsCurrAnimationHasAttacked.Add(unit);
                        }
                    }
                }
            }
        }

        #endregion

        protected void ColliderFlipX(Collider collider) => collider.SetLocalOffset(new Vector2(-collider.LocalOffset.X, collider.LocalOffset.Y));

        protected virtual void AttackUnit(Unit unit)
        {
            unit.Attacked(Random.Range(DamageLow, DamageHigh));
        }

        public virtual void Attacked(float damage)
        {
            Health -= damage;
            if (Health < 0)
                Entity.Destroy();
        }
    }
}
