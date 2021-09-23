using System.Collections.Generic;
using System.Numerics;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;

namespace LeagueSandbox.GameServer.GameObjects
{
    class MonsterCamp : IMonsterCamp
    {
        public MonsterCampType CampType { get; }

        public Vector2 Position { get; }

        public List<MonsterSpawnType> MonsterTypes { get; }
        public List<Vector2> MonsterSpawnPositions { get; }

        public float RespawnCooldown { get; set; }
        public float NextSpawnTime { get; protected set; } = 0f;

        private Game _game;
        private bool _notifiedClient;
        private bool _isAlive;
        private bool _setToSpawn;

        List<Monster> monsters = new List<Monster>();

        public MonsterCamp(Game game, MonsterCampType campType, Vector2 position, List<MonsterSpawnType> monsterTypes, List<Vector2> monsterSpawnPositions = null, float respawnCooldown = 1)
        {
            _game = game;
            CampType = campType;
            Position = position;
            MonsterTypes = monsterTypes;
            RespawnCooldown = respawnCooldown;
            MonsterSpawnPositions = monsterSpawnPositions;
        }

        private static string GetMonsterModel(MonsterSpawnType type)
        {
            switch (type)
            {
                //OLD SUMMONERS RIFT && OLD TWISTED TREELINE
                case MonsterSpawnType.WORM:
                    return "Worm";
                case MonsterSpawnType.DRAGON:
                    return "Dragon";
                case MonsterSpawnType.WRAITH:
                    return "Wraith";
                case MonsterSpawnType.ANCIENT_GOLEM:
                    return "AncientGolem";
                case MonsterSpawnType.YOUNG_LIZARD_ANCIENT:
                    return "YoungLizard";
                case MonsterSpawnType.GIANT_WOLF:
                    return "GiantWolf";
                case MonsterSpawnType.WOLF:
                    return "Wolf";
                case MonsterSpawnType.GREAT_WRAITH:
                    return "GreatWraith";
                case MonsterSpawnType.LESSER_WRAITH:
                    return "LesserWraith";
                case MonsterSpawnType.ELDER_LIZARD:
                    return "LizardElder";
                case MonsterSpawnType.YOUNG_LIZARD_ELDER:
                    return "YoungLizard";
                case MonsterSpawnType.GOLEM:
                    return "Golem";
                case MonsterSpawnType.LESSER_GOLEM:
                    return "SmallGolem";

                //NEW SUMMONERS RIFT
                case MonsterSpawnType.SRU_BARON:
                    return "SRU_Baron";
                case MonsterSpawnType.SRU_DRAGON:
                    return "SRU_Dragon";
                case MonsterSpawnType.SRU_GROMP:
                    return "SRU_Gromp";
                case MonsterSpawnType.SRU_BLUE:
                    return "SRU_Blue";
                case MonsterSpawnType.SRU_BLUEMINI:
                    return "SRU_BlueMini";
                case MonsterSpawnType.SRU_BLUEMINI2:
                    return "SRU_BlueMini2";
                case MonsterSpawnType.SRU_MURKWOLF:
                    return "SRU_Murkwolf";
                case MonsterSpawnType.SRU_MURKWOLF_MINI:
                    return "SRU_MurkwolfMini";
                case MonsterSpawnType.SRU_RAZORBEAK:
                    return "SRU_Razorbeak";
                case MonsterSpawnType.SRU_RAZORBRAK_MINI:
                    return "SRU_RazorbeakMini";
                case MonsterSpawnType.SRU_RED:
                    return "SRU_Red";
                case MonsterSpawnType.SRU_RED_MINI:
                    return "SRU_RedMini";
                case MonsterSpawnType.SRU_KRUG:
                    return "SRU_Krug";
                case MonsterSpawnType.SRU_KRUG_MINI:
                    return "SRU_KrugMini";

                //NEW TWISTED TREELINE
                case MonsterSpawnType.TT_SPIDERBOSS:
                    return "TT_Spiderboss";
                case MonsterSpawnType.TT_GOLEM:
                    return "TT_NGolem";
                case MonsterSpawnType.TT_GOLEM2:
                    return "TT_NGolem2";
                case MonsterSpawnType.TT_NWOLF:
                    return "TT_NWolf";
                case MonsterSpawnType.TT_NWOLF2:
                    return "TT_NWolf2";
                case MonsterSpawnType.TT_NWRAITH:
                    return "TT_NWraith";
                case MonsterSpawnType.TT_NWRAITH2:
                    return "TT_NWraith2";
                case MonsterSpawnType.TT_RELIC:
                    return "TT_Relic";

                default:
                    return "TestCubeRender";
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
            if (NextSpawnTime > _game.GameTime)
            {
                return false;
            }

            bool alive = false;
            foreach (var monster in monsters)
            {
                if (monster != null && !monster.IsDead)
                {
                    alive = true;
                }
            }

            _isAlive = alive;

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
                _notifiedClient = true;
            }

            if (_isAlive) return;

            monsters = new List<Monster>();

            int i = 0;
            foreach (var type in MonsterTypes)
            {
                if (MonsterSpawnPositions != null)
                {
                    var m = new Monster(_game, MonsterSpawnPositions[i], Position, type, GetMonsterModel(type), GetMonsterModel(type), CampType);
                    monsters.Add(m);
                    _game.ObjectManager.AddObject(m);
                }
                else
                {
                    var m = new Monster(_game, Position, Position, type, GetMonsterModel(type), GetMonsterModel(type), CampType);
                    monsters.Add(m);
                    _game.ObjectManager.AddObject(m);
                }
                i++;
            }

            _setToSpawn = false;
            _isAlive = true;
        }
    }
}