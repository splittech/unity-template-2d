using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Core
{
    public abstract class LoadingScreen : MonoBehaviour
    {
        private IProgress<float> _progress;
        public IProgress<float> Progress => _progress ??= new Progress<float>(UpdateProgress);

        public abstract UniTask Show(CancellationToken ct = default);
        public abstract UniTask Hide(CancellationToken ct = default);
        public abstract void ShowImmediate();
        public abstract void HideImmediate();
        public abstract void SetProgressDescription(string description);
        protected abstract void UpdateProgress(float progress);
    }
}
