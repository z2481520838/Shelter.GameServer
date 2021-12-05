﻿using System;
using System.Collections.Generic;
using System.Numerics;
using GameServerCore.Domain;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Enums;
using GameServerCore.Maps;
using LeagueSandbox.GameServer.Content;

namespace MapScripts.Map1
{
    public class CLASSIC : IMapScript
    {
        public bool EnableBuildingProtection { get; set; } = true;
        public virtual IGlobalData GlobalData { get; set; } = new GlobalData();

        //General Map variable
        private IMapScriptHandler _map;

        //Stuff about minions
        public bool SpawnEnabled { get; set; }
        public long FirstSpawnTime { get; set; } = 90 * 1000;
        public long NextSpawnTime { get; set; } = 90 * 1000;
        public long SpawnInterval { get; set; } = 30 * 1000;
        public bool MinionPathingOverride { get; set; } = true;
        public List<IMonsterCamp> JungleCamps { get; set; }

        //General things that will affect players globaly, such as default gold per-second, Starting gold....
        public float GoldPerSecond { get; set; } = 1.9f;
        public float StartingGold { get; set; } = 475.0f;
        public bool HasFirstBloodHappened { get; set; } = false;
        public bool IsKillGoldRewardReductionActive { get; set; } = true;
        public int BluePillId { get; set; } = 2001;
        public long FirstGoldTime { get; set; } = 90 * 1000;

        //Tower type enumeration might vary slightly from map to map, so we set that up here
        public TurretType GetTurretType(int trueIndex, LaneID lane, TeamId teamId)
        {
            TurretType returnType = TurretType.FOUNTAIN_TURRET;

            if (lane == LaneID.MIDDLE)
            {
                if (trueIndex < 3)
                {
                    returnType = TurretType.NEXUS_TURRET;
                    return returnType;
                }

                trueIndex -= 2;
            }

            switch (trueIndex)
            {
                case 1:
                case 4:
                case 5:
                    returnType = TurretType.INHIBITOR_TURRET;
                    break;
                case 2:
                    returnType = TurretType.INNER_TURRET;
                    break;
                case 3:
                    returnType = TurretType.OUTER_TURRET;
                    break;
            }

            return returnType;
        }

        //Nexus models
        //Nexus and Inhibitor model changes dont seem to take effect in-game, has to be investigated.
        public Dictionary<TeamId, string> NexusModels { get; set; } = new Dictionary<TeamId, string>
        {
            {TeamId.TEAM_BLUE, "OrderNexus" },
            {TeamId.TEAM_PURPLE, "ChaosNexus" }
        };
        //Inhib models
        public Dictionary<TeamId, string> InhibitorModels { get; set; } = new Dictionary<TeamId, string>
        {
            {TeamId.TEAM_BLUE, "OrderInhibitor" },
            {TeamId.TEAM_PURPLE, "ChaosInhibitor" }
        };
        //Tower Models
        public Dictionary<TeamId, Dictionary<TurretType, string>> TowerModels { get; set; } = new Dictionary<TeamId, Dictionary<TurretType, string>>
        {
            {TeamId.TEAM_BLUE, new Dictionary<TurretType, string>
            {
                {TurretType.FOUNTAIN_TURRET, "OrderTurretShrine" },
                {TurretType.NEXUS_TURRET, "OrderTurretAngel" },
                {TurretType.INHIBITOR_TURRET, "OrderTurretDragon" },
                {TurretType.INNER_TURRET, "OrderTurretNormal2" },
                {TurretType.OUTER_TURRET, "OrderTurretNormal" },
            } },
            {TeamId.TEAM_PURPLE, new Dictionary<TurretType, string>
            {
                {TurretType.FOUNTAIN_TURRET, "ChaosTurretShrine" },
                {TurretType.NEXUS_TURRET, "ChaosTurretNormal" },
                {TurretType.INHIBITOR_TURRET, "ChaosTurretGiant" },
                {TurretType.INNER_TURRET, "ChaosTurretWorm2" },
                {TurretType.OUTER_TURRET, "ChaosTurretWorm" },
            } }
        };
        public Dictionary<MonsterSpawnType, string> MonsterModels { get; set; } = new Dictionary<MonsterSpawnType, string>
        {
            {MonsterSpawnType.WORM,"Worm"},
            {MonsterSpawnType.DRAGON, "Dragon"},
            {MonsterSpawnType.ELDER_LIZARD, "LizardElder"}, {MonsterSpawnType.YOUNG_LIZARD_ELDER, "YoungLizard"},
            {MonsterSpawnType.ANCIENT_GOLEM, "AncientGolem" }, {MonsterSpawnType.YOUNG_LIZARD_ANCIENT, "YoungLizard"},
            {MonsterSpawnType.GREAT_WRAITH, "GreatWraith" },
            {MonsterSpawnType.GIANT_WOLF, "GiantWolf" }, {MonsterSpawnType.WOLF, "Wolf"},
            {MonsterSpawnType.GOLEM, "Golem" }, {MonsterSpawnType.LESSER_GOLEM, "SmallGolem"},
            {MonsterSpawnType.WRAITH, "Wraith" }, {MonsterSpawnType.LESSER_WRAITH, "LesserWraith"}
        };

        //Turret Items
        public Dictionary<TurretType, int[]> TurretItems { get; set; } = new Dictionary<TurretType, int[]>
        {
            { TurretType.OUTER_TURRET, new[] { 1500, 1501, 1502, 1503 } },
            { TurretType.INNER_TURRET, new[] { 1500, 1501, 1502, 1503, 1504 } },
            { TurretType.INHIBITOR_TURRET, new[] { 1501, 1502, 1503, 1505 } },
            { TurretType.NEXUS_TURRET, new[] { 1501, 1502, 1503, 1505 } }
        };

        //List of every path minions will take, separated by team and lane
        public Dictionary<LaneID, List<Vector2>> MinionPaths { get; set; } = new Dictionary<LaneID, List<Vector2>>
        {
                //Pathing coordinates for Top lane
                {LaneID.TOP, new List<Vector2> {
                    new Vector2(917.0f, 1725.0f),
                    new Vector2(1170.0f, 4041.0f),
                    new Vector2(861.0f, 6459.0f),
                    new Vector2(880.0f, 10180.0f),
                    new Vector2(1268.0f, 11675.0f),
                    new Vector2(2806.0f, 13075.0f),
                    new Vector2(3907.0f, 13243.0f),
                    new Vector2(7550.0f, 13407.0f),
                    new Vector2(10244.0f, 13238.0f),
                    new Vector2(10947.0f, 13135.0f),
                    new Vector2(12511.0f, 12776.0f) }
                },

                //Pathing coordinates for Mid lane
                {LaneID.MIDDLE, new List<Vector2> {
                    new Vector2(1418.0f, 1686.0f),
                    new Vector2(2997.0f, 2781.0f),
                    new Vector2(4472.0f, 4727.0f),
                    new Vector2(8375.0f, 8366.0f),
                    new Vector2(10948.0f, 10821.0f),
                    new Vector2(12511.0f, 12776.0f) }
                },

                //Pathing coordinates for Bot lane
                {LaneID.BOTTOM, new List<Vector2> {
                    new Vector2(1487.0f, 1302.0f),
                    new Vector2(3789.0f, 1346.0f),
                    new Vector2(6430.0f, 1005.0f),
                    new Vector2(10995.0f, 1234.0f),
                    new Vector2(12841.0f, 3051.0f),
                    new Vector2(13148.0f, 4202.0f),
                    new Vector2(13249.0f, 7884.0f),
                    new Vector2(12886.0f, 10356.0f),
                    new Vector2(12511.0f, 12776.0f) }
                }
        };


        //List of every wave type
        public Dictionary<string, List<MinionSpawnType>> MinionWaveTypes = new Dictionary<string, List<MinionSpawnType>>
        { {"RegularMinionWave", new List<MinionSpawnType>
        {
            MinionSpawnType.MINION_TYPE_MELEE,
            MinionSpawnType.MINION_TYPE_MELEE,
            MinionSpawnType.MINION_TYPE_MELEE,
            MinionSpawnType.MINION_TYPE_CASTER,
            MinionSpawnType.MINION_TYPE_CASTER,
            MinionSpawnType.MINION_TYPE_CASTER }
        },
        {"CannonMinionWave", new List<MinionSpawnType>{
            MinionSpawnType.MINION_TYPE_MELEE,
            MinionSpawnType.MINION_TYPE_MELEE,
            MinionSpawnType.MINION_TYPE_MELEE,
            MinionSpawnType.MINION_TYPE_CANNON,
            MinionSpawnType.MINION_TYPE_CASTER,
            MinionSpawnType.MINION_TYPE_CASTER,
            MinionSpawnType.MINION_TYPE_CASTER }
        },
        {"SuperMinionWave", new List<MinionSpawnType>{
            MinionSpawnType.MINION_TYPE_SUPER,
            MinionSpawnType.MINION_TYPE_MELEE,
            MinionSpawnType.MINION_TYPE_MELEE,
            MinionSpawnType.MINION_TYPE_MELEE,
            MinionSpawnType.MINION_TYPE_CASTER,
            MinionSpawnType.MINION_TYPE_CASTER,
            MinionSpawnType.MINION_TYPE_CASTER }
        },
        {"DoubleSuperMinionWave", new List<MinionSpawnType>{
            MinionSpawnType.MINION_TYPE_SUPER,
            MinionSpawnType.MINION_TYPE_SUPER,
            MinionSpawnType.MINION_TYPE_MELEE,
            MinionSpawnType.MINION_TYPE_MELEE,
            MinionSpawnType.MINION_TYPE_MELEE,
            MinionSpawnType.MINION_TYPE_CASTER,
            MinionSpawnType.MINION_TYPE_CASTER,
            MinionSpawnType.MINION_TYPE_CASTER }
        }
        };

        //Here you setup the conditions of which wave will be spawned
        public Tuple<int, List<MinionSpawnType>> MinionWaveToSpawn(float gameTime, int cannonMinionCount, bool isInhibitorDead, bool areAllInhibitorsDead)
        {
            var cannonMinionTimestamps = new List<Tuple<long, int>>
            {
                new Tuple<long, int>(0, 2),
                new Tuple<long, int>(20 * 60 * 1000, 1),
                new Tuple<long, int>(35 * 60 * 1000, 0)
            };
            var cannonMinionCap = 2;

            foreach (var timestamp in cannonMinionTimestamps)
            {
                if (gameTime >= timestamp.Item1)
                {
                    cannonMinionCap = timestamp.Item2;
                }
            }
            var list = "RegularMinionWave";
            if (cannonMinionCount >= cannonMinionCap)
            {
                list = "CannonMinionWave";
            }

            if (isInhibitorDead)
            {
                list = "SuperMinionWave";
            }

            if (areAllInhibitorsDead)
            {
                list = "DoubleSuperMinionWave";
            }
            return new Tuple<int, List<MinionSpawnType>>(cannonMinionCap, MinionWaveTypes[list]);
        }

        //Minion models for this map
        public Dictionary<TeamId, Dictionary<MinionSpawnType, string>> MinionModels { get; set; } = new Dictionary<TeamId, Dictionary<MinionSpawnType, string>>
        {
            {TeamId.TEAM_BLUE, new Dictionary<MinionSpawnType, string>{
                {MinionSpawnType.MINION_TYPE_MELEE, "Blue_Minion_Basic"},
                {MinionSpawnType.MINION_TYPE_CASTER, "Blue_Minion_Wizard"},
                {MinionSpawnType.MINION_TYPE_CANNON, "Blue_Minion_MechCannon"},
                {MinionSpawnType.MINION_TYPE_SUPER, "Blue_Minion_MechMelee"}
            }},
            {TeamId.TEAM_PURPLE, new Dictionary<MinionSpawnType, string>{
                {MinionSpawnType.MINION_TYPE_MELEE, "Red_Minion_Basic"},
                {MinionSpawnType.MINION_TYPE_CASTER, "Red_Minion_Wizard"},
                {MinionSpawnType.MINION_TYPE_CANNON, "Red_Minion_MechCannon"},
                {MinionSpawnType.MINION_TYPE_SUPER, "Red_Minion_MechMelee"}
            }}
        };

        //This function is executed in-between Loading the map structures and applying the structure protections. Is the first thing on this script to be executed
        public virtual void Init(IMapScriptHandler map)
        {
            _map = map;

            SpawnEnabled = map.IsMinionSpawnEnabled();
            map.AddSurrender(1200000.0f, 300000.0f, 30.0f);

            //Due to riot's questionable map-naming scheme some towers are missplaced into other lanes during outomated setup, so we have to manually fix them.
            map.ChangeTowerOnMapList("Turret_T1_C_06_A", TeamId.TEAM_BLUE, LaneID.MIDDLE, LaneID.TOP);
            map.ChangeTowerOnMapList("Turret_T1_C_07_A", TeamId.TEAM_BLUE, LaneID.MIDDLE, LaneID.BOTTOM);

            // Welcome to "Map"
            map.AddAnnouncement(30 * 1000, EventID.OnStartGameMessage1, true);
            // 30 seconds until minions spawn
            map.AddAnnouncement(FirstSpawnTime - 30 * 1000, EventID.OnStartGameMessage2, true);
            // Minions have spawned
            map.AddAnnouncement(FirstSpawnTime, EventID.OnMinionsSpawn, false);

            //Map props
            _map.AddLevelProp("LevelProp_Yonkey", "Yonkey", new Vector2(12465.0f, 14422.257f), 101.0f, new Vector3(0.0f, 66.0f, 0.0f), new Vector3(-33.3334f, 122.2222f, -133.3333f), Vector3.One);
            _map.AddLevelProp("LevelProp_Yonkey1", "Yonkey", new Vector2(-76.0f, 1769.1589f), 94.0f, new Vector3(0.0f, 30.0f, 0.0f), new Vector3(0.0f, -11.1111f, -22.2222f), Vector3.One);
            _map.AddLevelProp("LevelProp_ShopMale", "ShopMale", new Vector2(13374.17f, 14245.673f), 194.9741f, new Vector3(0.0f, 224f, 0.0f), new Vector3(0.0f, 33.3333f, -44.4445f), Vector3.One);
            _map.AddLevelProp("LevelProp_ShopMale1", "ShopMale", new Vector2(-99.5613f, 855.6632f), 191.4039f, new Vector3(0.0f, 158.0f, 0.0f), Vector3.Zero, Vector3.One);
        }

        public virtual void OnMatchStart()
        {
            JungleCamps = new List<IMonsterCamp>
            {
                //Neutral Camp
                _map.CreateMonsterCamp(MonsterCampType.BARON, new Vector2(4591.434f, 10215.344f),
                new Dictionary<Vector2, MonsterSpawnType>{
                { new Vector2(4591.434f, 10215.344f), MonsterSpawnType.WORM}},
                1200.0f),

                _map.CreateMonsterCamp(MonsterCampType.DRAGON, new Vector2(9430.364f, 4184.46f),
                new Dictionary<Vector2, MonsterSpawnType>{
                { new Vector2(9430.364f, 4184.46f), MonsterSpawnType.DRAGON}},
                150.0f),

                //Blue side
                _map.CreateMonsterCamp(MonsterCampType.BLUE_RED_BUFF, new Vector2(7384f, 3844f),
                new Dictionary<Vector2, MonsterSpawnType>{
                { new Vector2(7450f, 3897f), MonsterSpawnType.ELDER_LIZARD},
                { new Vector2(7254f, 3885f), MonsterSpawnType.YOUNG_LIZARD_ELDER},
                { new Vector2(7473f, 3708f), MonsterSpawnType.YOUNG_LIZARD_ELDER}},
                115.0f),

                _map.CreateMonsterCamp(MonsterCampType.BLUE_BLUE_BUFF, new Vector2(3450f, 7722f),
                new Dictionary<Vector2, MonsterSpawnType>{
                { new Vector2(3556f, 7643f), MonsterSpawnType.ANCIENT_GOLEM},
                { new Vector2(3335f, 7605f), MonsterSpawnType.YOUNG_LIZARD_ANCIENT},
                { new Vector2(3469f, 7825f), MonsterSpawnType.YOUNG_LIZARD_ANCIENT}},
                115.0f),

                _map.CreateMonsterCamp(MonsterCampType.BLUE_GOLEMS, new Vector2(7903f, 2478f),
                new Dictionary<Vector2, MonsterSpawnType>{
                { new Vector2(7903f, 2478f), MonsterSpawnType.GOLEM},
                { new Vector2(8116f, 2492f), MonsterSpawnType.LESSER_GOLEM}},
                125.0f),

                _map.CreateMonsterCamp(MonsterCampType.BLUE_WRAITHS, new Vector2(6536.759f, 5235.117f),
                new Dictionary<Vector2, MonsterSpawnType>{
                { new Vector2(6439f, 5220f), MonsterSpawnType.WRAITH},
                { new Vector2(6622f, 5283f), MonsterSpawnType.LESSER_WRAITH},
                { new Vector2(6493f, 5134f), MonsterSpawnType.LESSER_WRAITH},
                { new Vector2(6651f, 5169f), MonsterSpawnType.LESSER_WRAITH}},
                125.0f),

                _map.CreateMonsterCamp(MonsterCampType.BLUE_WOLVES, new Vector2(3353f, 6163f),
                new Dictionary<Vector2, MonsterSpawnType>{
                { new Vector2(3373f, 6208f), MonsterSpawnType.GIANT_WOLF},
                { new Vector2(3339f, 6365f), MonsterSpawnType.WOLF},
                { new Vector2(3516f, 6192f), MonsterSpawnType.WOLF}},
                125.0f),

                _map.CreateMonsterCamp(MonsterCampType.BLUE_GROMP, new Vector2(1982.3355f, 8250.126f),
                new Dictionary<Vector2, MonsterSpawnType>{
                { new Vector2(1820.6832f, 8176.1597f), MonsterSpawnType.GREAT_WRAITH}},
                125.0f),

                //Red side
                _map.CreateMonsterCamp(MonsterCampType.RED_RED_BUFF, new Vector2(6652.7f, 10654.1f),
                new Dictionary<Vector2, MonsterSpawnType>{
                { new Vector2(6523.898f, 10504.402f), MonsterSpawnType.ELDER_LIZARD},
                { new Vector2(6736.2334f, 10514.005f), MonsterSpawnType.YOUNG_LIZARD_ELDER},
                { new Vector2(6549.071f, 10735.53f), MonsterSpawnType.YOUNG_LIZARD_ELDER}},
                115.0f),

                _map.CreateMonsterCamp(MonsterCampType.RED_BLUE_BUFF, new Vector2(10584.8f, 6720.3f),
                new Dictionary<Vector2, MonsterSpawnType>{
                { new Vector2(10493.184f, 6763.5405f), MonsterSpawnType.ANCIENT_GOLEM},
                { new Vector2(10654.244f, 6821.699f), MonsterSpawnType.YOUNG_LIZARD_ANCIENT},
                { new Vector2(10525.28f, 6662.5273f), MonsterSpawnType.YOUNG_LIZARD_ANCIENT}},
                115.0f),

                _map.CreateMonsterCamp(MonsterCampType.RED_GOLEMS, new Vector2(5981.8f, 11976.6f),
                new Dictionary<Vector2, MonsterSpawnType>{
                { new Vector2(6055.785f, 11905.778f), MonsterSpawnType.GOLEM},
                { new Vector2(5879.59f, 11880.846f), MonsterSpawnType.LESSER_GOLEM}},
                125.0f),

                _map.CreateMonsterCamp(MonsterCampType.RED_WRAITHS, new Vector2(7453.7f, 9239.1f),
                new Dictionary<Vector2, MonsterSpawnType>{
                { new Vector2(7562.0127f, 9189.633f), MonsterSpawnType.WRAITH},
                { new Vector2(7419.434f, 9111.344f), MonsterSpawnType.LESSER_WRAITH},
                { new Vector2(7361.434f, 9261.344f), MonsterSpawnType.LESSER_WRAITH},
                { new Vector2(7538.852f, 9307.1455f), MonsterSpawnType.LESSER_WRAITH}},
                125.0f),

                _map.CreateMonsterCamp(MonsterCampType.RED_WOLVES, new Vector2(10666.2f, 8213.46f),
                new Dictionary<Vector2, MonsterSpawnType>{
                { new Vector2(10630.8f, 8164.3228f), MonsterSpawnType.GIANT_WOLF},
                { new Vector2(10505.131f, 8224.73f), MonsterSpawnType.WOLF},
                { new Vector2(10684.68f, 8089.6265f), MonsterSpawnType.WOLF}},
                125.0f),

                _map.CreateMonsterCamp(MonsterCampType.RED_GROMP, new Vector2(12142.5f, 6186.6f),
                new Dictionary<Vector2, MonsterSpawnType>{
                { new Vector2(12191.434f, 6213.3438f), MonsterSpawnType.GREAT_WRAITH}},
                125.0f),
            };
        }
        //This function gets executed every server tick
        public void Update(float diff)
        {
            foreach (var camp in JungleCamps)
            {
                if (!camp.IsAlive())
                {
                    camp.RespawnCooldown -= diff;

                    if (camp.RespawnCooldown <= 0)
                    {
                        camp.Spawn();
                        camp.RespawnCooldown = GetMonsterSpawnInterval(camp.CampType);
                    }
                }
            }
        }
        public int GetMonsterSpawnInterval(MonsterCampType monsterType)
        {
            switch (monsterType)
            {
                case MonsterCampType.BLUE_BLUE_BUFF:
                case MonsterCampType.BLUE_RED_BUFF:
                case MonsterCampType.RED_BLUE_BUFF:
                case MonsterCampType.RED_RED_BUFF:
                    return 300;
                case MonsterCampType.DRAGON:
                    return 660;
                case MonsterCampType.BARON:
                    return 720;
                default:
                    return 50;
            }
        }
        public float GetGoldFor(IAttackableUnit u)
        {
            if (u is IChampion c)
            {

                var gold = 300.0f; //normal gold for a kill
                if (c.KillDeathCounter < 5 && c.KillDeathCounter >= 0)
                {
                    if (c.KillDeathCounter == 0)
                    {
                        return gold;
                    }

                    for (var i = c.KillDeathCounter; i > 1; --i)
                    {
                        gold += gold * 0.165f;
                    }

                    return gold;
                }

                if (c.KillDeathCounter >= 5)
                {
                    return 500.0f;
                }

                if (c.KillDeathCounter >= 0)
                    return 0.0f;

                var firstDeathGold = gold - gold * 0.085f;

                if (c.KillDeathCounter == -1)
                {
                    return firstDeathGold;
                }

                for (var i = c.KillDeathCounter; i < -1; ++i)
                {
                    firstDeathGold -= firstDeathGold * 0.2f;
                }

                if (firstDeathGold < 50)
                {
                    firstDeathGold = 50;
                }

                return firstDeathGold;
            }

            else if (u is ILaneMinion mi)
            {
                var dic = new Dictionary<MinionSpawnType, float>
                {
                    { MinionSpawnType.MINION_TYPE_MELEE, 19.8f + 0.2f * (int)(_map.GameTime() / (90 * 1000)) },
                    { MinionSpawnType.MINION_TYPE_CASTER, 16.8f + 0.2f * (int)(_map.GameTime() / (90 * 1000)) },
                    { MinionSpawnType.MINION_TYPE_CANNON, 40.0f + 0.5f * (int)(_map.GameTime() / (90 * 1000)) },
                    { MinionSpawnType.MINION_TYPE_SUPER, 40.0f + 1.0f * (int)(_map.GameTime() / (180 * 1000)) }
                };

                if (!dic.ContainsKey(mi.MinionSpawnType))
                {
                    return 0.0f;
                }

                return dic[mi.MinionSpawnType];
            }

            else if (u is IMonster mo)
            {
                var dic = new Dictionary<MonsterSpawnType, float>
            {
                { MonsterSpawnType.GREAT_WRAITH, 35.0f },
                { MonsterSpawnType.LESSER_WRAITH, 4.0f },
                { MonsterSpawnType.GIANT_WOLF, 40.0f },
                { MonsterSpawnType.WOLF, 8.0f },
                { MonsterSpawnType.GOLEM, 55.0f },
                { MonsterSpawnType.LESSER_GOLEM, 15.0f },
                { MonsterSpawnType.WRAITH, 65.0f },
                { MonsterSpawnType.ANCIENT_GOLEM, 60.0f },
                { MonsterSpawnType.ELDER_LIZARD, 60.0f },
                { MonsterSpawnType.YOUNG_LIZARD_ANCIENT, 7.0f },
                { MonsterSpawnType.YOUNG_LIZARD_ELDER, 7.0f },
                { MonsterSpawnType.DRAGON, 150.0f },
                { MonsterSpawnType.WORM, 320.0f },
            };

                if (!dic.ContainsKey(mo.MinionSpawnType))
                {
                    return 0.0f;
                }

                return dic[mo.MinionSpawnType];
            }

            return 0.0f;
        }

        public float GetExperienceFor(IAttackableUnit u)
        {
            if (!(u is ILaneMinion m))
            {
                return 0.0f;
            }

            var dic = new Dictionary<MinionSpawnType, float>
            {
                { MinionSpawnType.MINION_TYPE_MELEE, 64.0f },
                { MinionSpawnType.MINION_TYPE_CASTER, 32.0f },
                { MinionSpawnType.MINION_TYPE_CANNON, 92.0f },
                { MinionSpawnType.MINION_TYPE_SUPER, 97.0f }
            };

            if (!dic.ContainsKey(m.MinionSpawnType))
            {
                return 0.0f;
            }

            return dic[m.MinionSpawnType];
        }

        public void SetMinionStats(ILaneMinion m)
        {
            // Same for all minions
            m.Stats.MoveSpeed.BaseValue = 325.0f;

            switch (m.MinionSpawnType)
            {
                case MinionSpawnType.MINION_TYPE_MELEE:
                    m.Stats.CurrentHealth = 475.0f + 20.0f * (int)(_map.GameTime() / (180 * 1000));
                    m.Stats.HealthPoints.BaseValue = 475.0f + 20.0f * (int)(_map.GameTime() / (180 * 1000));
                    m.Stats.AttackDamage.BaseValue = 12.0f + 1.0f * (int)(_map.GameTime() / (180 * 1000));
                    m.Stats.Range.BaseValue = 180.0f;
                    m.Stats.AttackSpeedFlat = 1.250f;
                    m.IsMelee = true;
                    break;
                case MinionSpawnType.MINION_TYPE_CASTER:
                    m.Stats.CurrentHealth = 279.0f + 7.5f * (int)(_map.GameTime() / (90 * 1000));
                    m.Stats.HealthPoints.BaseValue = 279.0f + 7.5f * (int)(_map.GameTime() / (90 * 1000));
                    m.Stats.AttackDamage.BaseValue = 23.0f + 1.0f * (int)(_map.GameTime() / (90 * 1000));
                    m.Stats.Range.BaseValue = 600.0f;
                    m.Stats.AttackSpeedFlat = 0.670f;
                    break;
                case MinionSpawnType.MINION_TYPE_CANNON:
                    m.Stats.CurrentHealth = 700.0f + 27.0f * (int)(_map.GameTime() / (180 * 1000));
                    m.Stats.HealthPoints.BaseValue = 700.0f + 27.0f * (int)(_map.GameTime() / (180 * 1000));
                    m.Stats.AttackDamage.BaseValue = 40.0f + 3.0f * (int)(_map.GameTime() / (180 * 1000));
                    m.Stats.Range.BaseValue = 450.0f;
                    m.Stats.AttackSpeedFlat = 1.0f;
                    break;
                case MinionSpawnType.MINION_TYPE_SUPER:
                    m.Stats.CurrentHealth = 1500.0f + 200.0f * (int)(_map.GameTime() / (180 * 1000));
                    m.Stats.HealthPoints.BaseValue = 1500.0f + 200.0f * (int)(_map.GameTime() / (180 * 1000));
                    m.Stats.AttackDamage.BaseValue = 190.0f + 10.0f * (int)(_map.GameTime() / (180 * 1000));
                    m.Stats.Range.BaseValue = 170.0f;
                    m.Stats.AttackSpeedFlat = 0.694f;
                    m.Stats.Armor.BaseValue = 30.0f;
                    m.Stats.MagicResist.BaseValue = -30.0f;
                    m.IsMelee = true;
                    break;
            }
        }
    }
}