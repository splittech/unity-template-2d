using System;

namespace Game.Core
{
    public class ServerLogger : GameLogger
    {
        public ServerLogger(LogChannelConfig config) : base(config) { }

        public override void Log(string message)
        {
            throw new NotSupportedException();
        }
    }
}
