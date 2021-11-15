using System.Collections.Generic;
using System.Numerics;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;

namespace LeagueSandbox.GameServer.GameObjects
{
    public class MonsterCamp : IMonsterCamp
    {
        public MonsterCampType CampType { get; }

        public Vector2 Position { get; }

        public Dictionary<Vector2, MonsterSpawnType> MonsterList { get; set; } = new Dictionary<Vector2, MonsterSpawnType>();
        public Vector2 FacingDirection { get; }

        public float RespawnCooldown { get; set; }
        public float NextSpawnTime { get; protected set; } = 0f;

        private Game _game;
        private bool _notifiedClient;
        private bool _isAlive;
        private bool _setToSpawn;

        List<Monster> monsters = new List<Monster>();

        public MonsterCamp(Game game, MonsterCampType campType, Vector2 position, Dictionary<Vector2, MonsterSpawnType> listOfMonsters, float respawnCooldown = 1, Vector2 facingDirection = default)
        {
            _game = game;
            CampType = campType;
            Position = position;
            RespawnCooldown = respawnCooldown * 1000;
            foreach (Vector2 coord in listOfMonsters.Keys)
            {
                MonsterList.Add(coord, listOfMonsters[coord]);
            }
            if (facingDirection == default)
            {
                FacingDirection = Position;
            }
            else
            {
                FacingDirection = facingDirection;
            }
        }

        private static string GetMinimapIcon(MonsterCampType type)
        {
            switch (type)
            {
                case MonsterCampType.BARON:
                    return "Baron";
                case MonsterCampType.DRAGON:
                case MonsterCampType.BLUE_BLUE_BUFF:
                case MonsterCampType.BLUE_RED_BUFF:
                case MonsterCampType.RED_BLUE_BUFF:
                case MonsterCampType.RED_RED_BUFF:
                    return "Camp";
                default:
                    return "LesserCamp";
            }
        }

        // TODO: method is used to evaluate whether camp should respawn as well as funcitoning as a getter for _isAlive,
        // should probably split that functionality into separate methods
        public bool IsAlive()
        {
            List<bool> alive = new List<bool>();
            foreach (var monster in monsters)
            {
                if (monster != null && !monster.IsDead)
                {
                    alive.Add(true);
                }
                else
                {
                    alive.Add(false);
                }
            }

            _isAlive = alive.Contains(true);

            if (!_isAlive && !_setToSpawn)
            {
                NextSpawnTime = _game.GameTime + RespawnCooldown * 1000f;
                _game.PacketNotifier.NotifyMonsterCampEmpty(this, null);
                _setToSpawn = true;
            }

            return _isAlive;
        }

        public void Spawn()
        {
            if (!_notifiedClient)
            {
                _game.PacketNotifier.NotifyCreateMonsterCamp(Position, (byte)CampType, 0, GetMinimapIcon(CampType));
                //_notifiedClient = true;
            }

            if (_isAlive) return;

            monsters = new List<Monster>();

            foreach (var coord in MonsterList.Keys)
            {
                var m = new Monster(_game, coord, FacingDirection, MonsterList[coord], _game.Map.MapScript.MonsterModels[MonsterList[coord]], _game.Map.MapScript.MonsterModels[MonsterList[coord]], CampType);
                _game.ObjectManager.AddObject(m);
                monsters.Add(m);
            }

            _setToSpawn = false;
            _isAlive = true;
        }
    }
}