using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Core
{
    public class CompositeProgress
    {
        private readonly List<float> subProgressValues;
        private readonly Action<float> handler;

        public CompositeProgress(Action<float> handler)
        {
            this.handler = handler;
            subProgressValues = new List<float>();
        }

        public IProgress<float> CreateSubProgress()
        {
            int newIndex = subProgressValues.Count;
            subProgressValues.Add(0f);

            return new ImmediateProgress<float>(subValue =>
            {
                subProgressValues[newIndex] = subValue;
                RecalculateTotalProgress();
            });

        }

        private void RecalculateTotalProgress()
        {
            float totalProgress = subProgressValues.Sum() / subProgressValues.Count();
            handler(totalProgress);
        }
    }
}