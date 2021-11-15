using LeagueSandbox.GameServer.Logging;
using log4net;

namespace LeagueSandbox.GameServer.Chatbox.Commands
{
    public class JunglespawnCommand : ChatCommandBase
    {
        private readonly ILog _logger;
        private readonly Game _game;
        public override string Command => "spawnjungle";
        public override string Syntax => $"{Command}";

        public JunglespawnCommand(ChatCommandManager chatCommandManager, Game game)
            : base(chatCommandManager, game)
        {
            _logger = LoggerProvider.GetLogger();
            _game = game;
        }

        public override void Execute(int userId, bool hasReceivedArguments, string arguments = "")
        {
            if(_game.Map.MapScript.JungleCamps == null || _game.Map.MapScript.JungleCamps.Count == 0)
            {
                ChatCommandManager.SendDebugMsgFormatted(DebugMsgType.INFO, "This map doesn't have jungle camps");
            }
            else
            {
                foreach (var jungleCamp in _game.Map.MapScript.JungleCamps)
                {
                    if (!jungleCamp.IsAlive())
                    {
                        jungleCamp.Spawn();
                    }
                }
            }
        }
    }
}
