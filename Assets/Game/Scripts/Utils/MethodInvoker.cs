using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Utils
{
    public class MethodInvoker : MonoBehaviour
    {
        public UnityEvent UnityEvent;

        [BoxGroup("Invoke")]
        [Button]
        public void Invoke()
        {
            UnityEvent?.Invoke();
        }
    }
}