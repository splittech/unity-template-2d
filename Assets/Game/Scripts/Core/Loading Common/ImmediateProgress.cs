using System;

namespace Game.Core
{
    public class ImmediateProgress<T> : IProgress<T>
    {
        private readonly Action<T> handler;

        public ImmediateProgress(Action<T> handler)
        {
            this.handler = handler;
        }

        public void Report(T value)
        {
            handler(value);
        }
    }
}
