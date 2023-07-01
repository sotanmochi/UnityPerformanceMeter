using System;
using UnityEngine;

namespace UnityPerformanceMeter
{
    public interface ICameraEventNotifier
    {
        event Action OnPreCullEvent;
        event Action OnPreRenderEvent;
        event Action OnPostRenderEvent;
    }

    [RequireComponent(typeof(Camera))]
    public sealed class CameraEventNotifier : MonoBehaviour, ICameraEventNotifier
    {
        public event Action OnPreCullEvent;
        public event Action OnPreRenderEvent;
        public event Action OnPostRenderEvent;

        void OnPreCull()
        {
            OnPreCullEvent?.Invoke();
        }

        void OnPreRender()
        {
            OnPreRenderEvent?.Invoke();
        }

        void OnPostRender()
        {
            OnPostRenderEvent?.Invoke();
        }
    }
}