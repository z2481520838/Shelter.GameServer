using GameServerCore.Enums;
using System.Collections.Generic;
using System.Numerics;

namespace GameServerCore.Domain.GameObjects
{
    public interface IMonsterCamp
    {
        MonsterCampType CampType { get; }
        Vector2 Position { get; }
        List<MonsterSpawnType> MonsterTypes { get; }
        float RespawnCooldown { get; }
        public float NextSpawnTime { get; }

        bool IsAlive();

        void Spawn();
    }
}