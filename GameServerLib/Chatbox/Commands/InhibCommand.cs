using GameServerCore;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.GameObjects;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using System;

namespace LeagueSandbox.GameServer.Chatbox.Commands
{
    public class InibinCommand : ChatCommandBase
    {
        private readonly IPlayerManager _playerManager;

        public override string Command => "inibin";
        public override string Syntax => $"{Command} inibin";

        public InibinCommand(ChatCommandManager chatCommandManager, Game game)
            : base(chatCommandManager, game)
        {
            _playerManager = game.PlayerManager;
        }



        public override void Execute(int userId, bool hasReceivedArguments, string arguments = "")
        {

            var split = arguments.Split(' ');
            if (split.Length < 2)
            {
                ChatCommandManager.SendDebugMsgFormatted(DebugMsgType.SYNTAXERROR);
                ShowSyntax();
            }

            try
            {
                var sender = _playerManager.GetPeerInfo(userId);
                var monsterType = MonsterSpawnType.DRAGON;
                var m = new Monster(
                    Game,
                    sender.Champion.Position,
                    sender.Champion.Position,
                    monsterType,
                    split[1],
                    split[1],
                    MonsterCampType.BARON
                );
                Game.ObjectManager.AddObject(m);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}