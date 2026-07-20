using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Core
{
    public abstract class LoadingScreen : MonoBehaviour
    {
        private IProgress<float> progress;

        public IProgress<float> Progress => progress ??= new Progress<float>(SetProgress);

        public abstract UniTask Show(CancellationToken ct = default);
        public abstract UniTask Hide(CancellationToken ct = default);
        public abstract void ShowImmediate();
        public abstract void HideImmediate();
        public abstract void SetProgressDescription(string description);
        protected abstract void SetProgress(float progress);
    }
}
