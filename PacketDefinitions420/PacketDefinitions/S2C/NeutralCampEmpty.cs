using GameServerCore.Domain;
using GameServerCore.Domain.GameObjects;
using GameServerCore.Enums;
using GameServerCore.Packets.Enums;

namespace PacketDefinitions420.PacketDefinitions.S2C
{
    public class NeutralCampEmpty : BasePacket
    {
        public NeutralCampEmpty(IMonsterCamp monsterCamp, IChampion killer)
            : base(PacketCmd.PKT_S2C_NEUTRAL_CAMP_EMPTY, killer is null ? 0 : killer.NetId)
        {
            Write((uint)(killer is null ? 0 : killer.NetId));
            Write((int)monsterCamp.CampType); // campId
            Write(monsterCamp.GetHashCode()); //TimerType
            Write(monsterCamp.NextSpawnTime); // TimerExpire (for baron -> 1200, dragon -> 150)
            Write(false); // DoPlayVo
        }
    }
}