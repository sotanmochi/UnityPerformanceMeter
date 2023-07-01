using System;
using System.Diagnostics;
using UnityPlayerLooper;

namespace UnityPerformanceMeter
{
    public sealed class CpuPerformanceMeter : IDisposable
    {
        public bool Initialized => _initialized;

#region Ring Buffer for Zero Allocation
        private const int _bufferSize = 4; // The buffer size must be a power of two.
        private const long _bufferMask = _bufferSize - 1;
        private readonly CpuPerformanceRecord[] _recordBuffer = new CpuPerformanceRecord[_bufferSize];
        private long _bufferHead = 0;
        private long _bufferTail = 0;
#endregion

        private readonly ICameraEventNotifier _cameraEventNotifier;
        private readonly Stopwatch _stopwatch = new();

        private bool _initialized = false;

        private long _frameCount = 0;
        private long _onPostTimeUpdateTicks = -1;

        private long _onPreBehaviourFixedUpdateTicks;
        private long _onPostBehaviourFixedUpdateTicks;
        private long _onPreBehaviourUpdateTicks;
        private long _onPostBehaviourUpdateTicks;
        private long _onPreDelayedTasksTicks;
        private long _onPostDelayedTasksTicks;
        private long _onPreBehaviourLateUpdateTicks;
        private long _onPostBehaviourLateUpdateTicks;
        private long _onPrePhysicsFixedUpdateTicks;
        private long _onPostPhysicsFixedUpdateTicks;
        private long _onPreUpdateAllRenderersTicks;
        private long _onPostUpdateAllRenderersTicks;
        private long _onPreUpdateAllSkinnedMeshesTicks;
        private long _onPostUpdateAllSkinnedMeshesTicks;
        private long _onPreDirectorUpdateAnimationBeginTicks;
        private long _onPostDirectorUpdateAnimationEndTicks;
        private long _onPrePlayerUpdateCanvasesTicks;
        private long _onPostPlayerUpdateCanvasesTicks;
        private long _onPreUpdateAudioTicks;
        private long _onPostUpdateAudioTicks;
        private long _onPreCullTicks;
        private long _onPreRenderTicks;
        private long _onPostRenderTicks;

        public CpuPerformanceMeter(ICameraEventNotifier cameraEventNotifier)
        {
            _cameraEventNotifier = cameraEventNotifier;

            for (var i = 0; i < _recordBuffer.Length; i++)
            {
                _recordBuffer[i] = new CpuPerformanceRecord();
            }
        }

        public void Initialize()
        {
            if (_initialized) return;
            _initialized = true;

            GlobalPlayerLooper.Register(OnPreTimeUpdate, LoopTiming.PreTimeUpdate);
            GlobalPlayerLooper.Register(OnPostTimeUpdate, LoopTiming.PostTimeUpdate);
            GlobalPlayerLooper.Register(OnPreBehaviourFixedUpdate, LoopTiming.PreBehaviourFixedUpdate);
            GlobalPlayerLooper.Register(OnPostBehaviourFixedUpdate, LoopTiming.PostBehaviourFixedUpdate);
            GlobalPlayerLooper.Register(OnPreBehaviourUpdate, LoopTiming.PreBehaviourUpdate);
            GlobalPlayerLooper.Register(OnPostBehaviourUpdate, LoopTiming.PostBehaviourUpdate);
            GlobalPlayerLooper.Register(OnPreDelayedTasks, LoopTiming.PreDelayedTasks);
            GlobalPlayerLooper.Register(OnPostDelayedTasks, LoopTiming.PostDelayedTasks);
            GlobalPlayerLooper.Register(OnPreBehaviourLateUpdate, LoopTiming.PreBehaviourLateUpdate);
            GlobalPlayerLooper.Register(OnPostBehaviourLateUpdate, LoopTiming.PostBehaviourLateUpdate);
            GlobalPlayerLooper.Register(OnPrePhysicsFixedUpdate, LoopTiming.PrePhysicsFixedUpdate);
            GlobalPlayerLooper.Register(OnPostPhysicsFixedUpdate, LoopTiming.PostPhysicsFixedUpdate);
            GlobalPlayerLooper.Register(OnPreUpdateAllRenderers, LoopTiming.PreUpdateAllRenderers);
            GlobalPlayerLooper.Register(OnPostUpdateAllRenderers, LoopTiming.PostUpdateAllRenderers);
            GlobalPlayerLooper.Register(OnPreUpdateAllSkinnedMeshes, LoopTiming.PreUpdateAllSkinnedMeshes);
            GlobalPlayerLooper.Register(OnPostUpdateAllSkinnedMeshes, LoopTiming.PostUpdateAllSkinnedMeshes);
            GlobalPlayerLooper.Register(OnPreDirectorUpdateAnimationBegin, LoopTiming.PreDirectorUpdateAnimationBegin);
            GlobalPlayerLooper.Register(OnPostDirectorUpdateAnimationEnd, LoopTiming.PostDirectorUpdateAnimationEnd);
            GlobalPlayerLooper.Register(OnPrePlayerUpdateCanvases, LoopTiming.PrePlayerUpdateCanvases);
            GlobalPlayerLooper.Register(OnPostPlayerUpdateCanvases, LoopTiming.PostPlayerUpdateCanvases);
            GlobalPlayerLooper.Register(OnPreUpdateAudio, LoopTiming.PreUpdateAudio);
            GlobalPlayerLooper.Register(OnPostUpdateAudio, LoopTiming.PostUpdateAudio);

            if (_cameraEventNotifier is not null)
            {
                _cameraEventNotifier.OnPreCullEvent += OnPreCull;
                _cameraEventNotifier.OnPreRenderEvent += OnPreRender;
                _cameraEventNotifier.OnPostRenderEvent += OnPostRender;
            }

            _stopwatch.Start();
        }

        public void Dispose()
        {
            GlobalPlayerLooper.Unregister(this, LoopTiming.PreTimeUpdate);
            GlobalPlayerLooper.Unregister(this, LoopTiming.PostTimeUpdate);
            GlobalPlayerLooper.Unregister(this, LoopTiming.PreBehaviourFixedUpdate);
            GlobalPlayerLooper.Unregister(this, LoopTiming.PostBehaviourFixedUpdate);
            GlobalPlayerLooper.Unregister(this, LoopTiming.PreBehaviourUpdate);
            GlobalPlayerLooper.Unregister(this, LoopTiming.PostBehaviourUpdate);
            GlobalPlayerLooper.Unregister(this, LoopTiming.PreDelayedTasks);
            GlobalPlayerLooper.Unregister(this, LoopTiming.PostDelayedTasks);
            GlobalPlayerLooper.Unregister(this, LoopTiming.PreBehaviourLateUpdate);
            GlobalPlayerLooper.Unregister(this, LoopTiming.PostBehaviourLateUpdate);
            GlobalPlayerLooper.Unregister(this, LoopTiming.PrePhysicsFixedUpdate);
            GlobalPlayerLooper.Unregister(this, LoopTiming.PostPhysicsFixedUpdate);
            GlobalPlayerLooper.Unregister(this, LoopTiming.PreUpdateAllRenderers);
            GlobalPlayerLooper.Unregister(this, LoopTiming.PostUpdateAllRenderers);
            GlobalPlayerLooper.Unregister(this, LoopTiming.PreUpdateAllSkinnedMeshes);
            GlobalPlayerLooper.Unregister(this, LoopTiming.PostUpdateAllSkinnedMeshes);
            GlobalPlayerLooper.Unregister(this, LoopTiming.PreDirectorUpdateAnimationBegin);
            GlobalPlayerLooper.Unregister(this, LoopTiming.PostDirectorUpdateAnimationEnd);
            GlobalPlayerLooper.Unregister(this, LoopTiming.PrePlayerUpdateCanvases);
            GlobalPlayerLooper.Unregister(this, LoopTiming.PostPlayerUpdateCanvases);
            GlobalPlayerLooper.Unregister(this, LoopTiming.PreUpdateAudio);
            GlobalPlayerLooper.Unregister(this, LoopTiming.PostUpdateAudio);

            if (_cameraEventNotifier is not null)
            {
                _cameraEventNotifier.OnPreCullEvent -= OnPreCull;
                _cameraEventNotifier.OnPreRenderEvent -= OnPreRender;
                _cameraEventNotifier.OnPostRenderEvent -= OnPostRender;
            }

            _stopwatch.Reset();
            _initialized = false;
        }

        public bool TryDequeueRecord(ref CpuPerformanceRecord record)
        {
            if (_bufferHead >= _bufferTail) { return false; }

            var index = _bufferHead & _bufferMask;
            _recordBuffer[index].CopyTo(record);
            _bufferHead++;

            return true;
        }

        private void EnqueueRecord(long frameStartTicks, long frameEndTicks)
        {
            var bufferFreeCount = _bufferSize - (int)(_bufferTail - _bufferHead);
            if (bufferFreeCount > 0)
            {
                var index = _bufferTail & _bufferMask;

                var record = _recordBuffer[index];
                record.FrameCount = _frameCount;
                record.FrameStartTicks = frameStartTicks;
                record.FrameEndTicks = frameEndTicks;
                record.OnPreBehaviourFixedUpdateTicks = _onPreBehaviourFixedUpdateTicks;
                record.OnPostBehaviourFixedUpdateTicks = _onPostBehaviourFixedUpdateTicks;
                record.OnPreBehaviourUpdateTicks = _onPreBehaviourUpdateTicks;
                record.OnPostBehaviourUpdateTicks = _onPostBehaviourUpdateTicks;
                record.OnPreDelayedTasksTicks = _onPreDelayedTasksTicks;
                record.OnPostDelayedTasksTicks = _onPostDelayedTasksTicks;
                record.OnPreBehaviourLateUpdateTicks = _onPreBehaviourLateUpdateTicks;
                record.OnPostBehaviourLateUpdateTicks = _onPostBehaviourLateUpdateTicks;
                record.OnPrePhysicsFixedUpdateTicks = _onPrePhysicsFixedUpdateTicks;
                record.OnPostPhysicsFixedUpdateTicks = _onPostPhysicsFixedUpdateTicks;
                record.OnPreUpdateAllRenderersTicks = _onPreUpdateAllRenderersTicks;
                record.OnPostUpdateAllRenderersTicks = _onPostUpdateAllRenderersTicks;
                record.OnPreUpdateAllSkinnedMeshesTicks = _onPreUpdateAllSkinnedMeshesTicks;
                record.OnPostUpdateAllSkinnedMeshesTicks = _onPostUpdateAllSkinnedMeshesTicks;
                record.OnPreDirectorUpdateAnimationBeginTicks = _onPreDirectorUpdateAnimationBeginTicks;
                record.OnPostDirectorUpdateAnimationEndTicks = _onPostDirectorUpdateAnimationEndTicks;
                record.OnPrePlayerUpdateCanvasesTicks = _onPrePlayerUpdateCanvasesTicks;
                record.OnPostPlayerUpdateCanvasesTicks = _onPostPlayerUpdateCanvasesTicks;
                record.OnPreUpdateAudioTicks = _onPreUpdateAudioTicks;
                record.OnPostUpdateAudioTicks = _onPostUpdateAudioTicks;
                record.OnPreCullTicks = _onPreCullTicks;
                record.OnPreRenderTicks = _onPreRenderTicks;
                record.OnPostRenderTicks = _onPostRenderTicks;

                _bufferTail++;
            }
        }

        private void OnPreTimeUpdate()
        {
            var currentTicks = _stopwatch.ElapsedTicks;

            if (_onPostTimeUpdateTicks > 0)
            {
                _frameCount++;
                EnqueueRecord(_onPostTimeUpdateTicks, currentTicks);
            }
        }

        private void OnPostTimeUpdate()
        {
            _onPostTimeUpdateTicks = _stopwatch.ElapsedTicks;
        }

        private void OnPreBehaviourFixedUpdate()
        {
            _onPreBehaviourFixedUpdateTicks = _stopwatch.ElapsedTicks;
        }

        private void OnPostBehaviourFixedUpdate()
        {
            _onPostBehaviourFixedUpdateTicks = _stopwatch.ElapsedTicks;
        }

        private void OnPreBehaviourUpdate()
        {
            _onPreBehaviourUpdateTicks = _stopwatch.ElapsedTicks;
        }

        private void OnPostBehaviourUpdate()
        {
            _onPostBehaviourUpdateTicks = _stopwatch.ElapsedTicks;
        }

        private void OnPreDelayedTasks()
        {
            _onPreDelayedTasksTicks = _stopwatch.ElapsedTicks;
        }

        private void OnPostDelayedTasks()
        {
            _onPostDelayedTasksTicks = _stopwatch.ElapsedTicks;
        }

        private void OnPreBehaviourLateUpdate()
        {
            _onPreBehaviourLateUpdateTicks = _stopwatch.ElapsedTicks;
        }

        private void OnPostBehaviourLateUpdate()
        {
            _onPostBehaviourLateUpdateTicks = _stopwatch.ElapsedTicks;
        }

        private void OnPrePhysicsFixedUpdate()
        {
            _onPrePhysicsFixedUpdateTicks = _stopwatch.ElapsedTicks;
        }

        private void OnPostPhysicsFixedUpdate()
        {
            _onPostPhysicsFixedUpdateTicks = _stopwatch.ElapsedTicks;
        }

        private void OnPreUpdateAllRenderers()
        {
            _onPreUpdateAllRenderersTicks = _stopwatch.ElapsedTicks;
        }

        private void OnPostUpdateAllRenderers()
        {
            _onPostUpdateAllRenderersTicks = _stopwatch.ElapsedTicks;
        }

        private void OnPreUpdateAllSkinnedMeshes()
        {
            _onPreUpdateAllSkinnedMeshesTicks = _stopwatch.ElapsedTicks;
        }

        private void OnPostUpdateAllSkinnedMeshes()
        {
            _onPostUpdateAllSkinnedMeshesTicks = _stopwatch.ElapsedTicks;
        }

        private void OnPreDirectorUpdateAnimationBegin()
        {
            _onPreDirectorUpdateAnimationBeginTicks = _stopwatch.ElapsedTicks;
        }

        private void OnPostDirectorUpdateAnimationEnd()
        {
            _onPostDirectorUpdateAnimationEndTicks = _stopwatch.ElapsedTicks;
        }

        private void OnPrePlayerUpdateCanvases()
        {
            _onPrePlayerUpdateCanvasesTicks = _stopwatch.ElapsedTicks;
        }

        private void OnPostPlayerUpdateCanvases()
        {
            _onPostPlayerUpdateCanvasesTicks = _stopwatch.ElapsedTicks;
        }

        private void OnPreUpdateAudio()
        {
            _onPreUpdateAudioTicks = _stopwatch.ElapsedTicks;
        }

        private void OnPostUpdateAudio()
        {
            _onPostUpdateAudioTicks = _stopwatch.ElapsedTicks;
        }

        private void OnPreCull()
        {
            _onPreCullTicks = _stopwatch.ElapsedTicks;
        }

        private void OnPreRender()
        {
            _onPreRenderTicks = _stopwatch.ElapsedTicks;
        }

        private void OnPostRender()
        {
            _onPostRenderTicks = _stopwatch.ElapsedTicks;
        }
    }
}