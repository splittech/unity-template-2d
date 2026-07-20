using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Core
{
    public class CompositeProgress
    {
        private readonly List<float> subProgressValues;
        private readonly Action<float> onProgressChanged;

        public CompositeProgress(Action<float> onProgressChanged)
        {
            this.onProgressChanged = onProgressChanged;

            subProgressValues = new List<float>();
        }

        public IProgress<float> CreateSubProgress()
        {
            int newIndex = subProgressValues.Count;
            subProgressValues.Add(0f);

            return new Progress<float>(subValue =>
            {
                subProgressValues[newIndex] = subValue;
                RecalculateTotalProgress();
            });

        }

        private void RecalculateTotalProgress()
        {
            float totalProgress = subProgressValues.Sum() / subProgressValues.Count();
            onProgressChanged?.Invoke(totalProgress);
        }
    }
}