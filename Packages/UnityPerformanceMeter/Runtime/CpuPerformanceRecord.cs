using System;
using System.Diagnostics;

namespace UnityPerformanceMeter
{
    [Serializable]
    public class CpuPerformanceRecord
    {
        public long FrameCount;

        public long FrameStartTicks;
        public long FrameEndTicks;

        public long OnPreBehaviourFixedUpdateTicks;
        public long OnPostBehaviourFixedUpdateTicks;

        public long OnPreBehaviourUpdateTicks;
        public long OnPostBehaviourUpdateTicks;

        public long OnPreDelayedTasksTicks;
        public long OnPostDelayedTasksTicks;

        public long OnPreBehaviourLateUpdateTicks;
        public long OnPostBehaviourLateUpdateTicks;

        public long OnPrePhysicsFixedUpdateTicks;
        public long OnPostPhysicsFixedUpdateTicks;

        public long OnPreUpdateAllRenderersTicks;
        public long OnPostUpdateAllRenderersTicks;

        public long OnPreUpdateAllSkinnedMeshesTicks;
        public long OnPostUpdateAllSkinnedMeshesTicks;

        public long OnPreDirectorUpdateAnimationBeginTicks;
        public long OnPostDirectorUpdateAnimationEndTicks;

        public long OnPrePlayerUpdateCanvasesTicks;
        public long OnPostPlayerUpdateCanvasesTicks;

        public long OnPreUpdateAudioTicks;
        public long OnPostUpdateAudioTicks;

        public long OnPreCullTicks;
        public long OnPreRenderTicks;
        public long OnPostRenderTicks;

        public static float GetTimeMilliseconds(long beginTicks, long endTicks)
        {
            return (endTicks - beginTicks) / (float)Stopwatch.Frequency * 1000f;
        }

        public void CopyTo(CpuPerformanceRecord dst)
        {
            dst.FrameCount = FrameCount;
            dst.FrameStartTicks = FrameStartTicks;
            dst.FrameEndTicks = FrameEndTicks;
            dst.OnPreBehaviourFixedUpdateTicks = OnPreBehaviourFixedUpdateTicks;
            dst.OnPostBehaviourFixedUpdateTicks = OnPostBehaviourFixedUpdateTicks;
            dst.OnPreBehaviourUpdateTicks = OnPreBehaviourUpdateTicks;
            dst.OnPostBehaviourUpdateTicks = OnPostBehaviourUpdateTicks;
            dst.OnPreDelayedTasksTicks = OnPreDelayedTasksTicks;
            dst.OnPostDelayedTasksTicks = OnPostDelayedTasksTicks;
            dst.OnPreBehaviourLateUpdateTicks = OnPreBehaviourLateUpdateTicks;
            dst.OnPostBehaviourLateUpdateTicks = OnPostBehaviourLateUpdateTicks;
            dst.OnPrePhysicsFixedUpdateTicks = OnPrePhysicsFixedUpdateTicks;
            dst.OnPostPhysicsFixedUpdateTicks = OnPostPhysicsFixedUpdateTicks;
            dst.OnPreUpdateAllRenderersTicks = OnPreUpdateAllRenderersTicks;
            dst.OnPostUpdateAllRenderersTicks = OnPostUpdateAllRenderersTicks;
            dst.OnPreUpdateAllSkinnedMeshesTicks = OnPreUpdateAllSkinnedMeshesTicks;
            dst.OnPostUpdateAllSkinnedMeshesTicks = OnPostUpdateAllSkinnedMeshesTicks;
            dst.OnPreDirectorUpdateAnimationBeginTicks = OnPreDirectorUpdateAnimationBeginTicks;
            dst.OnPostDirectorUpdateAnimationEndTicks = OnPostDirectorUpdateAnimationEndTicks;
            dst.OnPrePlayerUpdateCanvasesTicks = OnPrePlayerUpdateCanvasesTicks;
            dst.OnPostPlayerUpdateCanvasesTicks = OnPostPlayerUpdateCanvasesTicks;
            dst.OnPreUpdateAudioTicks = OnPreUpdateAudioTicks;
            dst.OnPostUpdateAudioTicks = OnPostUpdateAudioTicks;
            dst.OnPreCullTicks = OnPreCullTicks;
            dst.OnPreRenderTicks = OnPreRenderTicks;
            dst.OnPostRenderTicks = OnPostRenderTicks;
        }

        public float GetFrameTimeMilliseconds()
        {
            return GetTimeMilliseconds(FrameStartTicks, FrameEndTicks);
        }

        public float GetFixedUpdateTimeMilliseconds()
        {
            return GetTimeMilliseconds(OnPreBehaviourFixedUpdateTicks, OnPostBehaviourFixedUpdateTicks);
        }

        public float GetUpdateTimeMilliseconds()
        {
            return GetTimeMilliseconds(OnPreBehaviourUpdateTicks, OnPostBehaviourUpdateTicks);
        }

        public float GetLateUpdateTimeMilliseconds()
        {
            return GetTimeMilliseconds(OnPreBehaviourLateUpdateTicks, OnPostBehaviourLateUpdateTicks);
        }

        public float GetDelayedTasksTimeMilliseconds()
        {
            return GetTimeMilliseconds(OnPreDelayedTasksTicks, OnPostDelayedTasksTicks);
        }

        public float GetPhysicsFixedUpdateTimeMilliseconds()
        {
            return GetTimeMilliseconds(OnPrePhysicsFixedUpdateTicks, OnPostPhysicsFixedUpdateTicks);
        }

        public float GetUpdateAllRenderersTimeMilliseconds()
        {
            return GetTimeMilliseconds(OnPreUpdateAllRenderersTicks, OnPostUpdateAllRenderersTicks);
        }

        public float GetUpdateAllSkinnedMeshesTimeMilliseconds()
        {
            return GetTimeMilliseconds(OnPreUpdateAllSkinnedMeshesTicks, OnPostUpdateAllSkinnedMeshesTicks);
        }

        public float GetDirectorUpdateAnimationTimeMilliseconds()
        {
            return GetTimeMilliseconds(OnPreDirectorUpdateAnimationBeginTicks, OnPostDirectorUpdateAnimationEndTicks);
        }

        public float GetPlayerUpdateCanvasesTimeMilliseconds()
        {
            return GetTimeMilliseconds(OnPrePlayerUpdateCanvasesTicks, OnPostPlayerUpdateCanvasesTicks);
        }

        public float GetPlayerUpdateAudioTimeMilliseconds()
        {
            return GetTimeMilliseconds(OnPreUpdateAudioTicks, OnPostUpdateAudioTicks);
        }

        public float GetCameraRenderTimeMilliseconds()
        {
            return GetTimeMilliseconds(OnPreCullTicks, OnPostRenderTicks);
        }
    }
}