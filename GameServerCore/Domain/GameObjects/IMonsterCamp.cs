using GameServerCore.Enums;
using System.Collections.Generic;
using System.Numerics;

namespace GameServerCore.Domain.GameObjects
{
    public interface IMonsterCamp
    {
        MonsterCampType CampType { get; }
        Vector2 Position { get; }
        public Dictionary<Vector2, MonsterSpawnType> MonsterList { get; set; }
        float RespawnCooldown { get; set; }
        float NextSpawnTime { get; }
        bool IsAlive();
        void Spawn();
    }
}