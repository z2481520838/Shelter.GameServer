using GameServerCore.Domain.GameObjects;
using GameServerCore.Packets.Enums;

namespace PacketDefinitions420.PacketDefinitions.S2C
{
    public class AttachMinimapIcon : BasePacket
    {
        public AttachMinimapIcon(IAttackableUnit unit, bool ChangeIcon, string IconCategory, bool ChangeBorder, string BorderCategory, string BorderScriptName)
            : base(PacketCmd.PKT_S2C_ATTACH_MINIMAP_ICON)
        {
            WriteNetId(unit);
            Write(ChangeIcon);
            WriteConstLengthString(IconCategory, 64); // This is probably the icon name, but sometimes it's empty; Example: "Quest"
            Write(ChangeBorder);
            WriteConstLengthString(BorderCategory, 64); // Example: "Recall"
            WriteConstLengthString(BorderScriptName, 64); // Example "OdinRecall", "odinrecallimproved"
        }
    }
}