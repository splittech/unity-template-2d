namespace Game.Core
{
    public class EmptyLogger : GameLogger
    {
        public EmptyLogger(LogChannelConfig config) : base(config) { }

        public override void Log(string message)
        {
            // Pass.
        }
    }
}
