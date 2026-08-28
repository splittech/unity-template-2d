using System;

namespace Game.Core
{
    public class FileLogger : GameLogger
    {
        public FileLogger(LogChannelConfig config) : base(config) { }

        public override void Log(string message)
        {
            throw new NotSupportedException();
        }
    }
}
