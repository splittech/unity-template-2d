using System;

namespace Game.Core
{
    /// <summary>
    /// Null object for IProgress<T>
    /// </summary>
    public class EmptyProgress<T> : IProgress<T>
    {
        public static readonly EmptyProgress<T> Instance = new();

        private EmptyProgress() { }

        public void Report(T value) { }
    }
}
