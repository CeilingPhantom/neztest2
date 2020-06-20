using System;
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

        enum ActionLock
        {
            None,
            Attack,
            Stunned,
        }
        ActionLock actionLock = ActionLock.None;

        Entity player;
        float distanceSlimeFrontToPlayer;
        static readonly float attackCooldownDuration = 2;
        float attackCooldown = 0;

        public Slime(bool newStatReq=true) : base("slime", new Vector2(4, 8), 24, 15, newStatReq)
        {
            if (SlimeAnimationFrames == null)
                using (var stream = TitleContainer.OpenStream($"{ContentPath}/{Name}.tsx"))
                    SlimeAnimationFrames = new TmxTileset().LoadTmxTileset(null, System.Xml.Linq.XDocument.Load(stream).Element("tileset"), 1, ContentPath);
            AnimationFrames = SlimeAnimationFrames;
        }

        protected override void SetupNewStat()
        {
            Stats = Entity.AddComponent(new UnitStat(speed: 30, health: 50));
        }

        public override void OnAddedToEntity()
        {
            base.OnAddedToEntity();
            player = Entity.Scene.FindEntity("player");
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
                        "stunned",
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
            }, 5));
            Animator.AddAnimation("stunned", new SpriteAnimation(new Sprite[]
            {
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["stunned"][0]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["stunned"][1]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["stunned"][2]}")),
                new Sprite(Entity.Scene.Content.LoadTexture($"{ContentPath}/{Animations["stunned"][3]}")),
            }, 5));
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
            }, 5));
        }

        protected override void UpdateAnimations()
        {
            //System.Diagnostics.Debug.WriteLine(Stats.Health);
            if (Animator.IsCompleted)
                actionLock = ActionLock.None;

            if (attackCooldown != 0)
                attackCooldown -= Time.DeltaTime;

            if (actionLock == ActionLock.None)
            {
                string animation = "idle";
                var loopMode = SpriteAnimator.LoopMode.Loop;

                if (PlayerNearby())
                {
                    if (distanceSlimeFrontToPlayer != 0)
                    {
                        animation = "move";
                        actionLock = ActionLock.None;
                        if (distanceSlimeFrontToPlayer > 0)
                            Animator.FlipX = true;
                        else
                            Animator.FlipX = false;
                    }
                    else if (attackCooldown <= 0)
                    {
                        animation = "attack";
                        actionLock = ActionLock.Attack;
                        loopMode = SpriteAnimator.LoopMode.ClampForever;
                        UnitsCurrAnimationHasAttacked.Clear();
                        attackCooldown = attackCooldownDuration;
                    }
                }
                if (animation == "idle")
                    actionLock = ActionLock.None;

                if (!Animator.IsAnimationActive(animation) || (Animator.IsAnimationActive("attack") && Animator.IsCompleted))
                    Animator.Play(animation, loopMode);
            }
        }

        protected override void UpdateMovement()
        {
            Velocity.X = 0;
            if (actionLock == ActionLock.None)
            {
                if (distanceSlimeFrontToPlayer != 0)
                    Velocity.X = distanceSlimeFrontToPlayer / Math.Abs(distanceSlimeFrontToPlayer) * Stats.Speed;
            }

            base.UpdateMovement();
        }

        bool PlayerNearby()
        {
            distanceSlimeFrontToPlayer = 0;
            if (player != null && !player.IsDestroyed)
            {
                var pos1 = Entity.Transform.Position + (Animator.FlipX ? 1 : -1) * Vector2.UnitX * TiledColliderWidth / 4;
                var pos2 = player.Transform.Position;
                var d = pos1 - pos2;
                if (d.Length() < 200)
                {
                    if (d.Length() > player.GetComponent<BoxCollider>().Width / 2)
                    {
                        if (Entity.Transform.Position.X < pos2.X)
                            distanceSlimeFrontToPlayer = d.Length();
                        else if (Entity.Transform.Position.X > pos2.X)
                            distanceSlimeFrontToPlayer = -d.Length();
                    }
                    return true;
                }
            }
            return false;
        }

        protected override void AttackUnit(Unit unit)
        {
            if (unit.Entity.GetComponent<Player>() != null)
                base.AttackUnit(unit);
        }

        public override void Attacked(Unit unit, int damage)
        {
            Stunned();
            base.Attacked(unit, damage);
        }

        public override void Stunned()
        {
            actionLock = ActionLock.Stunned;
            Animator.Play("stunned", SpriteAnimator.LoopMode.ClampForever);
        }

        protected override bool Die()
        {
            if (!Animator.IsAnimationActive("die"))
                Animator.Play("die", SpriteAnimator.LoopMode.ClampForever);
            UpdateColliders();
            if (Animator.IsCompleted)
                return true;
            return false;
        }
    }
}
