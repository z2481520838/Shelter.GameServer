using GameServerCore.Enums;
using System.Numerics;

namespace GameServerCore.Domain.GameObjects
{
    public interface IMonster : IObjAiBase
    {
        Vector2 Facing { get; }
        string Name { get; }
        string SpawnAnimation { get; }
        MonsterCampType CampId { get; }
        byte CampUnk { get; }
        float SpawnAnimationTime { get; }

        MonsterSpawnType MinionSpawnType { get; }
    }
}