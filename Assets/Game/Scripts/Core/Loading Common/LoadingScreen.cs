using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Core.LoadingCommon
{
    public class LoadingScreen : MonoBehaviour
    {
        private IProgress<float> progress;

        public IProgress<float> Progress => progress ??= new Progress<float>(SetProgress);

        public async UniTask ShowAsync(CancellationToken ct = default)
        {

        }
        public async UniTask HideAsync(CancellationToken ct = default)
        {

        }
        public void ShowImmediate() { }
        public void HideImmediate() { }
        public void SetProgress(float value) { }
        public void SetProgressDescription(string description) { }
    }
}
