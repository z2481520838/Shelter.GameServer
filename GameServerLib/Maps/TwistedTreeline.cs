using System;
using System.Collections.Generic;
using System.Numerics;
using GameServerCore.Domain;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Enums;
using GameServerCore.Maps;
using LeagueSandbox.GameServer.GameObjects;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.Buildings.AnimatedBuildings;
using LeagueSandbox.GameServer.GameObjects.Other;
using LeagueSandbox.GameServer.Scripting.CSharp;

namespace LeagueSandbox.GameServer.Maps
{
    internal class TwistedTreeline : IMapProperties
    {
        private static readonly List<Vector2> BlueTopWaypoints = new List<Vector2>
        {
            new Vector2(1960f, 6684f),
            new Vector2(1410.101f, 8112.747f),
            new Vector2(2153f, 9228.292f),
            new Vector2(3254.654f, 8801.554f),
            new Vector2(3967.261f, 7689.167f),
            new Vector2(5187.636f, 7075.806f),
            new Vector2(6742.38f, 6797.492f),
            new Vector2(8587.43f, 7340.201f),
            new Vector2(9798.584f, 8079.067f),
            new Vector2(10926.82f, 9423.166f),
            new Vector2(12042.19f, 8413.8f),
            new Vector2(11758.64f, 6586.07f)
        };

        private static readonly List<Vector2> BlueBotWaypoints = new List<Vector2>
        {
            new Vector2(1960f, 5977f),
            new Vector2(1340.854f, 4566.956f),
            new Vector2(2218.08f, 3771.371f),
            new Vector2(3822.184f, 3735.409f),
            new Vector2(5268.443f, 3410.666f),
            new Vector2(6766.503f, 3435.633f),
            new Vector2(8776.482f, 3423.031f),
            new Vector2(10120.18f, 3735.568f),
            new Vector2(11622.86f, 3707.629f),
            new Vector2(12086.33f, 4369.503f),
            new Vector2(11797.77f, 6077.822f)
        };

        private static readonly List<Vector2> RedTopWaypoints = new List<Vector2>
        {
            new Vector2(11420f, 6617f),
            new Vector2(11980.1f, 7735.383f),
            new Vector2(12017.56f, 8453.665f),
            new Vector2(10831.11f, 9381.894f),
            new Vector2(9815.62f, 8208.034f),
            new Vector2(8462.342f, 7282.162f),
            new Vector2(6748.594f, 6805.058f),
            new Vector2(5014.874f, 7184.514f),
            new Vector2(3769.57f, 7920.32f),
            new Vector2(2894.403f, 9219.018f),
            new Vector2(1467.724f, 8491.606f),
            new Vector2(1498.947f, 7408.595f),
            new Vector2(1636.39f, 6486.415f)
        };

        private static readonly List<Vector2> RedBotWaypoints = new List<Vector2>
        {
            new Vector2(11420f, 6024f),
            new Vector2(11999.55f, 5145.43f),
            new Vector2(12037.32f, 4089.385f),
            new Vector2(10113.69f, 3693.272f),
            new Vector2(8829.546f, 3412.617f),
            new Vector2(6729.558f, 3454.635f),
            new Vector2(4695.755f, 3401.182f),
            new Vector2(3374.252f, 3714.507f),
            new Vector2(2374.252f, 4000.507f),
            new Vector2(1384.365f, 4558.863f),
            new Vector2(1582.978f, 6066.552f)
        };



        private static readonly List<MinionSpawnType> RegularMinionWave = new List<MinionSpawnType>
        {
            MinionSpawnType.MINION_TYPE_MELEE,
            MinionSpawnType.MINION_TYPE_MELEE,
            MinionSpawnType.MINION_TYPE_MELEE,
            MinionSpawnType.MINION_TYPE_CASTER,
            MinionSpawnType.MINION_TYPE_CASTER,
            MinionSpawnType.MINION_TYPE_CASTER
        };
        private static readonly List<MinionSpawnType> CannonMinionWave = new List<MinionSpawnType>
        {
            MinionSpawnType.MINION_TYPE_MELEE,
            MinionSpawnType.MINION_TYPE_MELEE,
            MinionSpawnType.MINION_TYPE_MELEE,
            MinionSpawnType.MINION_TYPE_CANNON,
            MinionSpawnType.MINION_TYPE_CASTER,
            MinionSpawnType.MINION_TYPE_CASTER,
            MinionSpawnType.MINION_TYPE_CASTER
        };
        private static readonly List<MinionSpawnType> SuperMinionWave = new List<MinionSpawnType>
        {
            MinionSpawnType.MINION_TYPE_SUPER,
            MinionSpawnType.MINION_TYPE_MELEE,
            MinionSpawnType.MINION_TYPE_MELEE,
            MinionSpawnType.MINION_TYPE_MELEE,
            MinionSpawnType.MINION_TYPE_CASTER,
            MinionSpawnType.MINION_TYPE_CASTER,
            MinionSpawnType.MINION_TYPE_CASTER
        };
        private static readonly List<MinionSpawnType> DoubleSuperMinionWave = new List<MinionSpawnType>
        {
            MinionSpawnType.MINION_TYPE_SUPER,
            MinionSpawnType.MINION_TYPE_SUPER,
            MinionSpawnType.MINION_TYPE_MELEE,
            MinionSpawnType.MINION_TYPE_MELEE,
            MinionSpawnType.MINION_TYPE_MELEE,
            MinionSpawnType.MINION_TYPE_CASTER,
            MinionSpawnType.MINION_TYPE_CASTER,
            MinionSpawnType.MINION_TYPE_CASTER
        };


        private static readonly Dictionary<TeamId, Vector3> EndGameCameraPosition = new Dictionary<TeamId, Vector3>
        {
            { TeamId.TEAM_BLUE, new Vector3(1877, 6351, 188) },
            { TeamId.TEAM_PURPLE, new Vector3(11118, 6245, 110) }
        };

        private static readonly Dictionary<TeamId, Vector2> SpawnsByTeam = new Dictionary<TeamId, Vector2>
        {
            {TeamId.TEAM_BLUE, new Vector2(24,6350)},
            {TeamId.TEAM_PURPLE, new Vector2(13500, 6350)}
        };

        private static readonly Dictionary<TurretType, int[]> TurretItems = new Dictionary<TurretType, int[]>
        {
            { TurretType.OUTER_TURRET, new[] { 1500, 1501, 1502, 1503 } },
            { TurretType.INNER_TURRET, new[] { 1500, 1501, 1502, 1503, 1504 } },
            { TurretType.INHIBITOR_TURRET, new[] { 1501, 1502, 1503, 1505 } },
            { TurretType.NEXUS_TURRET, new[] { 1501, 1502, 1503, 1505 } }
        };


        private Game _game;
        private int _cannonMinionCount;
        private int _minionNumber;
        private readonly long _firstSpawnTime = 90 * 1000;
        private long _nextSpawnTime = 90 * 1000;
        private long _jungleFirstSpawnTime = 90 * 1000;
        private readonly long _dragonFirstSpawnTime = 150 * 1000;
        private readonly long _spawnInterval = 30 * 1000;
        private readonly Dictionary<TeamId, Fountain> _fountains;
        private readonly Dictionary<TeamId, SurrenderHandler> _surrenders;

        public float GoldPerSecond { get; set; } = 1.9f;
        public float StartingGold { get; set; } = 800.0f;
        public bool HasFirstBloodHappened { get; set; } = false;
        public bool IsKillGoldRewardReductionActive { get; set; } = true;
        public int BluePillId { get; set; } = 2001;
        public long FirstGoldTime { get; set; } = 90 * 1000;
        public bool SpawnEnabled { get; set; }
        bool HasFirstJungleSpawnHappened { get; set; } = false;


        private readonly List<LaneTurret> _blueOuterTurrets = new List<LaneTurret>(3);
        private readonly List<LaneTurret> _blueInnerTurrets = new List<LaneTurret>(3);
        private readonly List<LaneTurret> _blueInhibTurrets = new List<LaneTurret>(3);
        private readonly List<LaneTurret> _blueNexusTurrets = new List<LaneTurret>(3);

        private readonly List<LaneTurret> _purpleOuterTurrets = new List<LaneTurret>(3);
        private readonly List<LaneTurret> _purpleInnerTurrets = new List<LaneTurret>(3);
        private readonly List<LaneTurret> _purpleInhibTurrets = new List<LaneTurret>(3);
        private readonly List<LaneTurret> _purpleNexusTurrets = new List<LaneTurret>(3);

        private readonly List<Inhibitor> _blueInhibitors = new List<Inhibitor>(3);
        private readonly List<Inhibitor> _purpleInhibitors = new List<Inhibitor>(3);

        private List<MonsterCamp> _monsterCamps = new List<MonsterCamp>();

        private Nexus _blueNexus;
        private Nexus _purpleNexus;

        public TwistedTreeline(Game game)
        {
            _game = game;
            _fountains = new Dictionary<TeamId, Fountain>
            {
                { TeamId.TEAM_BLUE, new Fountain(game, TeamId.TEAM_BLUE, new Vector2(24, 6400), 1000) },
                { TeamId.TEAM_PURPLE, new Fountain(game, TeamId.TEAM_PURPLE, new Vector2(13360, 6400), 1000) }
            };
            _surrenders = new Dictionary<TeamId, SurrenderHandler>
            {
                { TeamId.TEAM_BLUE, new SurrenderHandler(game, TeamId.TEAM_BLUE, 1877.0414f, 6351.439f, 30.0f) },
                { TeamId.TEAM_PURPLE, new SurrenderHandler(game, TeamId.TEAM_PURPLE, 11118.405f, 6245.9873f, 30.0f) }
            };
            SpawnEnabled = _game.Config.MinionSpawnsEnabled;

        }

        public int[] GetTurretItems(TurretType type)
        {
            if (!TurretItems.ContainsKey(type))
            {
                return null;
            }

            return TurretItems[type];
        }

        public void Init()
        {
            // Announcer events
            _game.Map.AnnouncerEvents.Add(new Announce(_game, 30 * 1000, Announces.WELCOME_TO_SR, true)); // Welcome to SR
            if (_firstSpawnTime - 30 * 1000 >= 0.0f)
                _game.Map.AnnouncerEvents.Add(new Announce(_game, _firstSpawnTime - 30 * 1000, Announces.THIRY_SECONDS_TO_MINIONS_SPAWN, true)); // 30 seconds until minions spawn
            _game.Map.AnnouncerEvents.Add(new Announce(_game, _firstSpawnTime, Announces.MINIONS_HAVE_SPAWNED, false)); // Minions have spawned (90 * 1000)
            _game.Map.AnnouncerEvents.Add(new Announce(_game, _firstSpawnTime, Announces.MINIONS_HAVE_SPAWNED2, false)); // Minions have spawned [2] (90 * 1000)

            // TODO: Generate & use exact positions from content files

            //TODO: Unhardcode everything (preferably by reading from content)
            var inhibRadius = 325;
            var nexusRadius = 350;
            var sightRange = 1700;



            // Inner top - mid - bot turrets
            _blueInnerTurrets.Add(new LaneTurret(_game, "Turret_T1_L_02_A", "OrderTurretNormal2", new Vector2(3925.5483f, 8144.099f), TeamId.TEAM_BLUE,
                TurretType.INNER_TURRET, GetTurretItems(TurretType.INNER_TURRET), 0, LaneID.TOP));
            _blueInnerTurrets.Add(new LaneTurret(_game, "Turret_T1_R_02_A", "OrderTurretNormal2", new Vector2(3291.2434f, 3356.354f), TeamId.TEAM_BLUE,
                TurretType.INNER_TURRET, GetTurretItems(TurretType.INNER_TURRET), 0, LaneID.BOTTOM));

            // Inhibitor top - mid - bot turrets
            _blueInhibTurrets.Add(new LaneTurret(_game, "Turret_T1_C_06_A", "OrderTurretDragon", new Vector2(1116.1393f, 8615.207f), TeamId.TEAM_BLUE,
                TurretType.INHIBITOR_TURRET, GetTurretItems(TurretType.INHIBITOR_TURRET), 0, LaneID.TOP));
            _blueInhibTurrets.Add(new LaneTurret(_game, "Turret_T1_C_07_A", "OrderTurretDragon", new Vector2(1086.4207f, 3944.9966f), TeamId.TEAM_BLUE,
                TurretType.INHIBITOR_TURRET, GetTurretItems(TurretType.INHIBITOR_TURRET), 0, LaneID.BOTTOM));

            // Inhibitors
            _blueInhibitors.Add(new Inhibitor(_game, "OrderInhibitor", LaneID.TOP, TeamId.TEAM_BLUE, inhibRadius, new Vector2(794.1595f, 8262.635f), sightRange, 0xffd23c3e));
            _blueInhibitors.Add(new Inhibitor(_game, "OrderInhibitor", LaneID.BOTTOM, TeamId.TEAM_BLUE, inhibRadius, new Vector2(844.5272f, 4563.2153f), sightRange, 0xff9303e1));

            // Nexus turrets
            _blueNexusTurrets.Add(new LaneTurret(_game, "Turret_T1_C_01_A", "OrderTurretAngel", new Vector2(1536.0298f, 6339.67f), TeamId.TEAM_BLUE,
             TurretType.NEXUS_TURRET, GetTurretItems(TurretType.NEXUS_TURRET)));



            // PURPLE TEAM


            // Inner top - mid - bot turrets
            _purpleInnerTurrets.Add(new LaneTurret(_game, "Turret_T2_L_02_A", "ChaosTurretWorm2", new Vector2(9539.091f, 8212.636f), TeamId.TEAM_PURPLE,
                TurretType.INNER_TURRET, GetTurretItems(TurretType.INNER_TURRET), 0, LaneID.TOP));
            _purpleInnerTurrets.Add(new LaneTurret(_game, "Turret_T2_R_02_A", "ChaosTurretWorm2", new Vector2(10056.046f, 3381.5234f), TeamId.TEAM_PURPLE,
                TurretType.INNER_TURRET, GetTurretItems(TurretType.INNER_TURRET), 0, LaneID.BOTTOM));

            // Inhibitor top - mid - bot turrets
            _purpleInhibTurrets.Add(new LaneTurret(_game, "Turret_T2_L_01_A", "ChaosTurretGiant", new Vector2(12311.476f, 8549.189f), TeamId.TEAM_PURPLE,
                TurretType.INHIBITOR_TURRET, GetTurretItems(TurretType.INHIBITOR_TURRET), 0, LaneID.TOP));
            _purpleInhibTurrets.Add(new LaneTurret(_game, "Turret_T2_R_01_A", "ChaosTurretGiant", new Vector2(12358.072f, 4000.189f), TeamId.TEAM_PURPLE,
                TurretType.INHIBITOR_TURRET, GetTurretItems(TurretType.INHIBITOR_TURRET), 0, LaneID.TOP));

            // Inhibitors
            _purpleInhibitors.Add(new Inhibitor(_game, "ChaosInhibitor", LaneID.TOP, TeamId.TEAM_PURPLE, inhibRadius, new Vector2(13345.678f, 8578.578f), sightRange, 0xff6793d0));
            _purpleInhibitors.Add(new Inhibitor(_game, "ChaosInhibitor", LaneID.BOTTOM, TeamId.TEAM_PURPLE, inhibRadius, new Vector2(13356.347f, 6295.329f), sightRange, 0xff26ac0f));

            // Nexus turrets
            _purpleNexusTurrets.Add(new LaneTurret(_game, "Turret_T2_C_01_A", "ChaosTurretNormal", new Vector2(11738.697f, 6289.903f), TeamId.TEAM_PURPLE,
                TurretType.NEXUS_TURRET, GetTurretItems(TurretType.NEXUS_TURRET)));


            _game.ObjectManager.AddObject(new LaneTurret(_game, "Turret_OrderTurretShrine_A", "OrderTurretShrine", new Vector2(-250.4423f, 6339.05f), TeamId.TEAM_BLUE,
                TurretType.FOUNTAIN_TURRET, GetTurretItems(TurretType.FOUNTAIN_TURRET)));
            _game.ObjectManager.AddObject(new LaneTurret(_game, "Turret_ChaosTurretShrine_A", "ChaosTurretShrine", new Vector2(13619.625f, 6371.949f), TeamId.TEAM_PURPLE,
                TurretType.FOUNTAIN_TURRET, GetTurretItems(TurretType.FOUNTAIN_TURRET)));

            for (int i = 0; i < _blueOuterTurrets.Count; i++)
            {
                _game.ProtectionManager.AddProtection(_purpleInhibitors[i], false, _purpleInhibTurrets[i]);
                _game.ProtectionManager.AddProtection(_purpleInhibTurrets[i], false, _purpleInnerTurrets[i]);
                _game.ProtectionManager.AddProtection(_purpleInnerTurrets[i], false, _purpleOuterTurrets[i]);

                _game.ProtectionManager.AddProtection(_blueInhibitors[i], false, _blueInhibTurrets[i]);
                _game.ProtectionManager.AddProtection(_blueInhibTurrets[i], false, _blueInnerTurrets[i]);
                _game.ProtectionManager.AddProtection(_blueInnerTurrets[i], false, _blueOuterTurrets[i]);

            }

            _game.ProtectionManager.AddProtection(_blueNexusTurrets[0], false, new Inhibitor[] { _blueInhibitors[0], _blueInhibitors[1] });


            _game.ProtectionManager.AddProtection(_purpleNexusTurrets[0], false, new Inhibitor[] { _purpleInhibitors[0], _purpleInhibitors[1] });



            foreach (var element in _blueInnerTurrets) _game.ObjectManager.AddObject(element);
            foreach (var element in _blueInhibTurrets) _game.ObjectManager.AddObject(element);
            foreach (var element in _blueInhibitors) _game.ObjectManager.AddObject(element);
            foreach (var element in _blueNexusTurrets) _game.ObjectManager.AddObject(element);


            foreach (var element in _purpleInnerTurrets) _game.ObjectManager.AddObject(element);
            foreach (var element in _purpleInhibTurrets) _game.ObjectManager.AddObject(element);
            foreach (var element in _purpleInhibitors) _game.ObjectManager.AddObject(element);
            foreach (var element in _purpleNexusTurrets) _game.ObjectManager.AddObject(element);



            _game.ObjectManager.AddObject(new LevelProp(_game, new Vector2(12465.0f, 14422.257f), 101.0f, new Vector3(0.0f, 0.0f, 0.0f), 0.0f, 0.0f, "LevelProp_Yonkey", "Yonkey"));
            _game.ObjectManager.AddObject(new LevelProp(_game, new Vector2(-76.0f, 1769.1589f), 94.0f, new Vector3(0.0f, 0.0f, 0.0f), 0.0f, 0.0f, "LevelProp_Yonkey1", "Yonkey"));
            _game.ObjectManager.AddObject(new LevelProp(_game, new Vector2(13374.17f, 14245.673f), 194.9741f, new Vector3(224.0f, 33.33f, 0.0f), 0.0f, -44.44f, "LevelProp_ShopMale", "ShopMale"));
            _game.ObjectManager.AddObject(new LevelProp(_game, new Vector2(-99.5613f, 855.6632f), 191.4039f, new Vector3(158.0f, 0.0f, 0.0f), 0.0f, 0.0f, "LevelProp_ShopMale1", "ShopMale"));


            _blueNexus = new Nexus(_game, "OrderNexus", TeamId.TEAM_BLUE, nexusRadius, new Vector2(1877.0414f, 6351.439f),
                sightRange, 0xfff97db5);
            _purpleNexus = new Nexus(_game, "ChaosNexus", TeamId.TEAM_PURPLE, nexusRadius, new Vector2(11118.405f, 6245.9873f),
                sightRange, 0xfff02c0f);

            _game.ProtectionManager.AddProtection(_blueNexus, new LaneTurret[] { _blueNexusTurrets[0] }, new Inhibitor[] { _blueInhibitors[0], _blueInhibitors[1] });
            _game.ProtectionManager.AddProtection(_purpleNexus, new LaneTurret[] { _purpleNexusTurrets[0] }, new Inhibitor[] { _purpleInhibitors[0], _purpleInhibitors[1] });

            _game.ObjectManager.AddObject(_blueNexus);
            _game.ObjectManager.AddObject(_purpleNexus);

            _monsterCamps = new List<MonsterCamp>() {

                new MonsterCamp(_game, MonsterCampType.DRAGON, new Vector2(6748.594f, 7878f),
                new List<MonsterSpawnType>() { MonsterSpawnType.DRAGON },
                new List<Vector2>() { new Vector2(6748.594f, 7878f)},
                GetMonsterSpawnInterval(MonsterCampType.DRAGON)),

                 new MonsterCamp(_game, MonsterCampType.BLUE_RED_BUFF, new Vector2(6748.594f, 5000.0f),
                 new List<MonsterSpawnType>() { MonsterSpawnType.ELDER_LIZARD},
                 new List<Vector2>() { new Vector2(6748.594f, 5000.0f) },
                 GetMonsterSpawnInterval(MonsterCampType.BLUE_RED_BUFF)),

                //Blue 
                new MonsterCamp(_game, MonsterCampType.BLUE_GOLEMS, new Vector2(4750.874f, 4800.0f),
                new List<MonsterSpawnType>() { MonsterSpawnType.GOLEM, MonsterSpawnType.LESSER_GOLEM },
                new List<Vector2>() { new Vector2(4650.874f, 4800.0f), new Vector2(4750.874f, 4800.0f) },
                GetMonsterSpawnInterval(MonsterCampType.BLUE_GOLEMS)),

                new MonsterCamp(_game, MonsterCampType.BLUE_WRAITHS, new Vector2(5514.874f, 9423.166f),
                new List<MonsterSpawnType>() { MonsterSpawnType.WRAITH, MonsterSpawnType.LESSER_WRAITH, MonsterSpawnType.LESSER_WRAITH },
                new List<Vector2>() { new Vector2(5514.874f, 9323.166f), new Vector2(5514.874f, 9423.166f), new Vector2(5414.874f, 9423.166f)},
                GetMonsterSpawnInterval(MonsterCampType.BLUE_WRAITHS)),

                new MonsterCamp(_game, MonsterCampType.BLUE_WOLVES, new Vector2(4350.874f, 9423.166f),
                new List<MonsterSpawnType>() { MonsterSpawnType.GIANT_WOLF, MonsterSpawnType.WOLF },
                new List<Vector2>() { new Vector2(4350.874f, 9423.166f), new Vector2(4250.874f, 9423.166f)},
                GetMonsterSpawnInterval(MonsterCampType.BLUE_WOLVES)),
                
                 //Purple 
                new MonsterCamp(_game, MonsterCampType.RED_GOLEMS, new Vector2(8750.874f, 4800.0f),
                new List<MonsterSpawnType>() { MonsterSpawnType.GOLEM, MonsterSpawnType.LESSER_GOLEM },
                new List<Vector2>() { new Vector2(8650.874f, 4800.0f), new Vector2(8750.874f, 4800.0f)},
                GetMonsterSpawnInterval(MonsterCampType.RED_GOLEMS)),


                new MonsterCamp(_game, MonsterCampType.RED_WRAITHS, new Vector2(7940.874f, 9423.166f),
                new List<MonsterSpawnType>() { MonsterSpawnType.WRAITH, MonsterSpawnType.LESSER_WRAITH, MonsterSpawnType.LESSER_WRAITH},
                new List<Vector2>() { new Vector2(7940.874f, 9323.166f), new Vector2(7940.874f, 9423.166f), new Vector2(7840.874f, 9423.166f) },
                GetMonsterSpawnInterval(MonsterCampType.RED_WRAITHS)),

                new MonsterCamp(_game, MonsterCampType.RED_WOLVES, new Vector2(9200.874f, 9423.166f),
                new List<MonsterSpawnType>() { MonsterSpawnType.GIANT_WOLF, MonsterSpawnType.WOLF },
                new List<Vector2>() { new Vector2(9200.874f, 9423.166f), new Vector2(9200.874f, 9423.166f)},
                GetMonsterSpawnInterval(MonsterCampType.RED_WOLVES)),
            };

        }

        public void Update(float diff)
        {
            if (_game.GameTime >= 120 * 1000)
            {
                IsKillGoldRewardReductionActive = false;
            }

            if (SpawnEnabled)
            {
                if (_minionNumber > 0)
                {
                    if (_game.GameTime >= _nextSpawnTime + _minionNumber * 8 * 100)
                    { // Spawn new wave every 0.8s
                        if (Spawn())
                        {
                            _minionNumber = 0;
                            _nextSpawnTime += _spawnInterval;
                        }
                        else
                        {
                            _minionNumber++;
                        }
                    }
                }
                else if (_game.GameTime >= _nextSpawnTime)
                {
                    Spawn();
                    _minionNumber++;
                }

                if (_game.GameTime >= _jungleFirstSpawnTime)
                {
                    if (!HasFirstJungleSpawnHappened)
                    {
                        //Jungle Spawning has to be optmized, since it can cause some laggining when it is first spawned
                        SpawnJungle();
                        foreach (var camp in _monsterCamps)
                        {
                            camp.RespawnCooldown = GetMonsterSpawnInterval(camp.CampType);
                        }
                        HasFirstJungleSpawnHappened = true;
                    }

                    foreach (var camp in _monsterCamps)
                    {
                        if (!camp.IsAlive())
                        {
                            if (camp.CampType == MonsterCampType.DRAGON && _game.GameTime <= _dragonFirstSpawnTime)
                            {
                                continue;
                            }

                            camp.RespawnCooldown -= diff;

                            if (camp.RespawnCooldown <= 0)
                            {
                                camp.Spawn();
                                camp.RespawnCooldown = GetMonsterSpawnInterval(camp.CampType);
                            }
                        }
                        else
                        {
                            camp.RespawnCooldown = GetMonsterSpawnInterval(camp.CampType);
                        }
                    }
                }
            }
            foreach (var fountain in _fountains.Values)
            {
                fountain.Update(diff);
            }
            foreach (var surrender in _surrenders.Values)
                surrender.Update(diff);


        }

        public Vector2 GetRespawnLocation(TeamId team)
        {
            if (!SpawnsByTeam.ContainsKey(team))
            {
                return new Vector2(25.90f, 280);
            }

            return SpawnsByTeam[team];
        }

        public string GetMinionModel(TeamId team, MinionSpawnType type)
        {
            var teamDictionary = new Dictionary<TeamId, string>
            {
                {TeamId.TEAM_BLUE, "Blue"},
                {TeamId.TEAM_PURPLE, "Red"}
            };

            var typeDictionary = new Dictionary<MinionSpawnType, string>
            {
                {MinionSpawnType.MINION_TYPE_MELEE, "Basic"},
                {MinionSpawnType.MINION_TYPE_CASTER, "Wizard"},
                {MinionSpawnType.MINION_TYPE_CANNON, "MechCannon"},
                {MinionSpawnType.MINION_TYPE_SUPER, "MechMelee"}

            };

            if (!teamDictionary.ContainsKey(team) || !typeDictionary.ContainsKey(type))
            {
                return string.Empty;
            }

            return $"{teamDictionary[team]}_Minion_{typeDictionary[type]}";
        }

        public float GetGoldFor(IAttackableUnit u)
        {
            if (!(u is ILaneMinion m))
            {
                if (!(u is IMonster s))
                {
                    if (!(u is IChampion c))
                    {
                        return 0.0f;
                    }

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
            }

            if (u is ILaneMinion mi)
            {
                var dic = new Dictionary<MinionSpawnType, float>
                {
                    { MinionSpawnType.MINION_TYPE_MELEE, 19.8f + 0.2f * (int)(_game.GameTime / (90 * 1000)) },
                    { MinionSpawnType.MINION_TYPE_CASTER, 16.8f + 0.2f * (int)(_game.GameTime / (90 * 1000)) },
                    { MinionSpawnType.MINION_TYPE_CANNON, 40.0f + 0.5f * (int)(_game.GameTime / (90 * 1000)) },
                    { MinionSpawnType.MINION_TYPE_SUPER, 40.0f + 1.0f * (int)(_game.GameTime / (180 * 1000)) }
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
                { MonsterSpawnType.WRAITH, 35.0f },
                { MonsterSpawnType.LESSER_WRAITH, 4.0f },
                { MonsterSpawnType.GIANT_WOLF, 40.0f },
                { MonsterSpawnType.WOLF, 8.0f },
                { MonsterSpawnType.GOLEM, 55.0f },
                { MonsterSpawnType.LESSER_GOLEM, 15.0f },
                { MonsterSpawnType.ANCIENT_GOLEM, 60.0f },
                { MonsterSpawnType.ELDER_LIZARD, 60.0f },
                { MonsterSpawnType.YOUNG_LIZARD_ANCIENT, 7.0f },
                { MonsterSpawnType.YOUNG_LIZARD_ELDER, 7.0f },
                { MonsterSpawnType.DRAGON, 150.0f },
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
            if ((u is ILaneMinion m))
            {
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

            else if ((u is IMonster mo))
            {
                var dic = new Dictionary<MonsterSpawnType, float>
            {
                { MonsterSpawnType.WRAITH, 90.0f },
                { MonsterSpawnType.LESSER_WRAITH, 20.0f },
                { MonsterSpawnType.GIANT_WOLF, 110.0f },
                { MonsterSpawnType.WOLF, 25.0f },
                { MonsterSpawnType.GOLEM, 140.0f },
                { MonsterSpawnType.LESSER_GOLEM, 40.0f },
                { MonsterSpawnType.ANCIENT_GOLEM, 260.0f },
                { MonsterSpawnType.ELDER_LIZARD, 260.0f },
                { MonsterSpawnType.YOUNG_LIZARD_ELDER, 20.0f },
                { MonsterSpawnType.YOUNG_LIZARD_ANCIENT, 20.0f },
                { MonsterSpawnType.DRAGON, 150.0f },
            };

                if (!dic.ContainsKey(mo.MinionSpawnType))
                {
                    return 0.0f;
                }

                return dic[mo.MinionSpawnType];
            }

            return 0.0f;
        }

        public int GetMonsterSpawnInterval(MonsterCampType monsterCampType)
        {
            switch (monsterCampType)
            {
                case MonsterCampType.DRAGON:
                    return 360 * 1000;
                case MonsterCampType.BLUE_RED_BUFF:
                    return 300 * 1000;
                default:
                    return 50 * 1000;
            }
        }
        public Tuple<TeamId, Vector2> GetMinionSpawnPosition(string spawnPosition)
        {
            switch (spawnPosition)
            {
                case "__P_Order_Spawn_Barracks__L01":
                    return new Tuple<TeamId, Vector2>(TeamId.TEAM_BLUE, new Vector2(1960, 6684));
                case "__P_Order_Spawn_Barracks__R01":
                    return new Tuple<TeamId, Vector2>(TeamId.TEAM_BLUE, new Vector2(1960, 5977));
                case "__P_Chaos_Spawn_Barracks__L01":
                    return new Tuple<TeamId, Vector2>(TeamId.TEAM_PURPLE, new Vector2(11420, 6617));
                case "__P_Chaos_Spawn_Barracks__R01":
                    return new Tuple<TeamId, Vector2>(TeamId.TEAM_PURPLE, new Vector2(11420, 6024));
            }
            return new Tuple<TeamId, Vector2>(0, new Vector2());
        }


        public void SetMinionStats(ILaneMinion m)
        {
            // Same for all minions
            m.Stats.MoveSpeed.BaseValue = 325.0f;

            switch (m.MinionSpawnType)
            {
                case MinionSpawnType.MINION_TYPE_MELEE:
                    m.Stats.CurrentHealth = 475.0f + 20.0f * (int)(_game.GameTime / (180 * 1000));
                    m.Stats.HealthPoints.BaseValue = 475.0f + 20.0f * (int)(_game.GameTime / (180 * 1000));
                    m.Stats.AttackDamage.BaseValue = 12.0f + 1.0f * (int)(_game.GameTime / (180 * 1000));
                    m.Stats.Range.BaseValue = 180.0f;
                    m.Stats.AttackSpeedFlat = 1.250f;
                    m.IsMelee = true;
                    break;
                case MinionSpawnType.MINION_TYPE_CASTER:
                    m.Stats.CurrentHealth = 279.0f + 7.5f * (int)(_game.GameTime / (90 * 1000));
                    m.Stats.HealthPoints.BaseValue = 279.0f + 7.5f * (int)(_game.GameTime / (90 * 1000));
                    m.Stats.AttackDamage.BaseValue = 23.0f + 1.0f * (int)(_game.GameTime / (90 * 1000));
                    m.Stats.Range.BaseValue = 600.0f;
                    m.Stats.AttackSpeedFlat = 0.670f;
                    break;
                case MinionSpawnType.MINION_TYPE_CANNON:
                    m.Stats.CurrentHealth = 700.0f + 27.0f * (int)(_game.GameTime / (180 * 1000));
                    m.Stats.HealthPoints.BaseValue = 700.0f + 27.0f * (int)(_game.GameTime / (180 * 1000));
                    m.Stats.AttackDamage.BaseValue = 40.0f + 3.0f * (int)(_game.GameTime / (180 * 1000));
                    m.Stats.Range.BaseValue = 450.0f;
                    m.Stats.AttackSpeedFlat = 1.0f;
                    break;
                case MinionSpawnType.MINION_TYPE_SUPER:
                    m.Stats.CurrentHealth = 1500.0f + 200.0f * (int)(_game.GameTime / (180 * 1000));
                    m.Stats.HealthPoints.BaseValue = 1500.0f + 200.0f * (int)(_game.GameTime / (180 * 1000));
                    m.Stats.AttackDamage.BaseValue = 190.0f + 10.0f * (int)(_game.GameTime / (180 * 1000));
                    m.Stats.Range.BaseValue = 170.0f;
                    m.Stats.AttackSpeedFlat = 0.694f;
                    m.Stats.Armor.BaseValue = 30.0f;
                    m.Stats.MagicResist.BaseValue = -30.0f;
                    m.IsMelee = true;
                    break;
            }
        }
        public void SpawnMinion(List<MinionSpawnType> list, int minionNo, string barracksName, List<Vector2> waypoints)
        {
            if (list.Count <= minionNo)
            {
                return;
            }

            var team = GetMinionSpawnPosition(barracksName).Item1;
            var m = new LaneMinion(_game, list[minionNo], barracksName, waypoints, GetMinionModel(team, list[minionNo]), 0, team);
            _game.ObjectManager.AddObject(m);
        }
        public void SpawnJungle()
        {
            foreach (var camp in _monsterCamps)
            {
                camp.Spawn();
            }
        }
        public bool Spawn()
        {
            var barracks = new List<string>
            {
                "__P_Order_Spawn_Barracks__L01",
                "__P_Order_Spawn_Barracks__R01",

                "__P_Chaos_Spawn_Barracks__L01",
                "__P_Chaos_Spawn_Barracks__R01",
            };

            var cannonMinionTimestamps = new List<Tuple<long, int>>
            {
                new Tuple<long, int>(0, 2),
                new Tuple<long, int>(20 * 60 * 1000, 1),
                new Tuple<long, int>(35 * 60 * 1000, 0)
            };

            var spawnToWaypoints = new Dictionary<string, Tuple<List<Vector2>, uint>>
            {
                {"__P_Order_Spawn_Barracks__L01", Tuple.Create(BlueTopWaypoints, 0xff6793d0)},
                {"__P_Order_Spawn_Barracks__R01", Tuple.Create(BlueBotWaypoints, 0xff26ac0f)},

                {"__P_Chaos_Spawn_Barracks__L01", Tuple.Create(RedTopWaypoints, 0xffd23c3e)},
                {"__P_Chaos_Spawn_Barracks__R01", Tuple.Create(RedBotWaypoints, 0xff9303e1)}
            };
            var cannonMinionCap = 2;

            foreach (var timestamp in cannonMinionTimestamps)
            {
                if (_game.GameTime >= timestamp.Item1)
                {
                    cannonMinionCap = timestamp.Item2;
                }
            }

            foreach (var barracksName in barracks)
            {
                var waypoints = spawnToWaypoints[barracksName].Item1;
                var inhibitorId = spawnToWaypoints[barracksName].Item2;
                var inhibitor = _game.ObjectManager.GetInhibitorById(inhibitorId);
                var isInhibitorDead = inhibitor.InhibitorState == InhibitorState.DEAD && !inhibitor.RespawnAnnounced;

                var oppositeTeam = TeamId.TEAM_BLUE;
                if (inhibitor.Team == TeamId.TEAM_PURPLE)
                {
                    oppositeTeam = TeamId.TEAM_PURPLE;
                }

                var areAllInhibitorsDead = _game.ObjectManager.AllInhibitorsDestroyedFromTeam(oppositeTeam) && !inhibitor.RespawnAnnounced;

                var list = RegularMinionWave;
                if (_cannonMinionCount >= cannonMinionCap)
                {
                    list = CannonMinionWave;
                }

                if (isInhibitorDead)
                {
                    list = SuperMinionWave;
                }

                if (areAllInhibitorsDead)
                {
                    list = DoubleSuperMinionWave;
                }

                SpawnMinion(list, _minionNumber, barracksName, waypoints);
            }


            if (_minionNumber < 8)
            {
                return false;
            }

            if (_cannonMinionCount >= cannonMinionCap)
            {
                _cannonMinionCount = 0;
            }
            else
            {
                _cannonMinionCount++;
            }
            return true;
        }

        public Vector3 GetEndGameCameraPosition(TeamId team)
        {
            if (!EndGameCameraPosition.ContainsKey(team))
            {
                return new Vector3(0, 0, 0);
            }

            return EndGameCameraPosition[team];
        }

        public void HandleSurrender(int userId, IChampion who, bool vote)
        {
            if (_surrenders.ContainsKey(who.Team))
                _surrenders[who.Team].HandleSurrender(userId, who, vote);
        }
    }
}
