using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.API;
using LeagueSandbox.GameServer.GameObjects.Stats;
using LeagueSandbox.GameServer.Maps;
using GameServerCore.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.API;
using System;
using GameServerCore.Domain;

namespace LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI
{
    public class Monster : Minion, IMonster
    {
        public Vector2 Facing { get; private set; }
        public string SpawnAnimation { get; private set; }
        public MonsterCampType CampId { get; private set; }
        public byte CampUnk { get; private set; }
        public float SpawnAnimationTime { get; private set; }

        public MonsterSpawnType MinionSpawnType { get; }
        public Vector2 spawnPosition;
        public int chaseDistance;
        private Game _game;

        public Monster(
            Game game,
            Vector2 position,
            Vector2 facing,
            MonsterSpawnType spawnType,
            string model,
            string name,
            MonsterCampType campId,
            string spawnAnimation = "",
            byte campUnk = 0x2A,
            float spawnAnimationTime = 0.0f,
            uint netId = 0
        ) : base(game, null, new Vector2(), model, name, netId, team: TeamId.TEAM_NEUTRAL)
        {

            var teams = Enum.GetValues(typeof(TeamId)).Cast<TeamId>();
            foreach (var team in teams)
            {
                SetVisibleByTeam(team, true);
            }

            Facing = facing;
            Name = name;
            SpawnAnimation = spawnAnimation;
            CampId = campId;
            CampUnk = campUnk;
            SpawnAnimationTime = spawnAnimationTime;
            _game = game;

            IsLaneMinion = false;
            _aiPaused = false;

            MinionSpawnType = spawnType;

            if (MinionSpawnType == MonsterSpawnType.ELDER_LIZARD)
            {
                // TODO : Give infinite version of buff
                var buff = new Buff(_game, "RedBuff", 60f, 1, this.AutoAttackSpell, this, this, true);
                AddBuff(buff);
            }

            if (MinionSpawnType == MonsterSpawnType.ANCIENT_GOLEM)
            {
                // TODO : Give infinite version of buff
                var buff = new Buff(_game, "BlueBuff", 60f, 1, this.AutoAttackSpell, this, this, true);
                AddBuff(buff);
            }

            SetPosition(position.X, position.Y);
            chaseDistance = 800;  // TODO: unhardcode

            StopMovement();

            MoveOrder = OrderType.Hold;
            Replication = new ReplicationMonster(this);
            spawnPosition = position;

            ApiEventManager.OnTakeDamage.AddListener(this, this, TakeDamage, false);
        }
        public void TakeDamage(IAttackableUnit unit, IAttackableUnit source)
        {
            if (resetting) return;
            AlertCamp(source);
            SetTargetUnit(source, true);
        }

        public void AlertCamp(IAttackableUnit source)
        {
            var nearestObjects = _game.Map.CollisionHandler.QuadDynamic.GetNearestObjects(this);
            foreach (var it in nearestObjects.OrderBy(x => Vector2.DistanceSquared(Position, x.Position) - (Stats.Range.Total * Stats.Range.Total)))
            {
                if (!(it is IMonster u) ||
                    u.IsDead ||
                    Vector2.DistanceSquared(Position, u.Position) > DETECT_RANGE * DETECT_RANGE ||
                    u.TargetUnit != null)
                {
                    continue;
                }

                u.SetTargetUnit(source, true);
            }
        }

        public override void OnAdded()
        {
            base.OnAdded();
        }

        public override void Update(float diff)
        {
            base.Update(diff);
        }

        bool resetting = false;
        public override bool AIMove()
        {

            // TODO: Find better way of determining if position is stopped and reached spawn point
            if (resetting && IsPathEnded())
            {
                resetting = false;
                Stats.CurrentHealth = Stats.HealthPoints.Total;
                return true;
            }
            if (resetting)
            {
                var tempPath = _game.Map.NavigationGrid.GetPath(Position, spawnPosition);
                SetWaypoints(tempPath);
                UpdateMoveOrder(OrderType.MoveTo);
                return false;
            }

            if (Vector2.DistanceSquared(spawnPosition, Position) > chaseDistance * chaseDistance)
            {
                resetting = true;
                CancelAutoAttack(false);
                SetTargetUnit(null, true);
                return false;
            }

            if (TargetUnit != null && !TargetUnit.IsDead) // returns true if we have a target
            {
                if (!RecalculateAttackPosition())
                {
                    KeepFocusingTarget(); // attack/follow target
                }
                return false;
            }
            return true;
        }


    }
}
