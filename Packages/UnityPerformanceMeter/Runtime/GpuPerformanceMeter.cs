using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace UnityPerformanceMeter
{
    public sealed class GpuPerformanceMeter
    {
        public static bool IsAvailable => SystemInfo.supportsGpuRecorder;

        public bool Initialized => _initialized;

        public int ActiveGpuRecorderCount => _activeGpuRecorderCount;
        public int AllProfilingRecorderCount => _allRecorderCount;
        public int ValidRecorderCount => _validRecorderCount;

        private readonly List<GpuRecord> _activeGpuRecords = new();
        private readonly Dictionary<string, Recorder> _activeGpuRecorders = new();
        private readonly List<string> _activeGpuRecorderNames = new();
        private readonly List<Recorder> _profilingRecorders = new();
        private readonly List<string> _profilingRecorderNames = new();

        private bool _initialized;
        private int _activeGpuRecorderCount;
        private int _allRecorderCount;
        private int _validRecorderCount;

        public void Initialize()
        {
            if (_initialized) return;

            _allRecorderCount = 0;
            _validRecorderCount = 0;
            _profilingRecorders.Clear();
            _profilingRecorderNames.Clear();

            _activeGpuRecorderCount = 0;
            _activeGpuRecorders.Clear();
            _activeGpuRecorderNames.Clear();

            Sampler.GetNames(_profilingRecorderNames);
            _allRecorderCount = _profilingRecorderNames.Count;

            for (var i = 0; i < _allRecorderCount; i++)
            {
                _profilingRecorders.Add(Recorder.Get(_profilingRecorderNames[i]));
                _profilingRecorders[i].enabled = true;
                if (_profilingRecorders[i].isValid) _validRecorderCount++;
            }

            _initialized = true;
        }

        public void UpdateActiveGpuRecorders()
        {
            if (!_initialized) return;

            for (var i = 0; i < _allRecorderCount; i++)
            {
                var recorder = _profilingRecorders[i];
                var recorderName = _profilingRecorderNames[i];

                if (!recorder.isValid) continue;

                if (recorder.gpuElapsedNanoseconds > 0 && !_activeGpuRecorders.ContainsKey(recorderName))
                {
                    _activeGpuRecorderNames.Add(_profilingRecorderNames[i]);
                    _activeGpuRecorders.Add(_profilingRecorderNames[i], _profilingRecorders[i]);
                    _activeGpuRecorderCount++;
                }
                else if (recorder.gpuElapsedNanoseconds <= 0 && _activeGpuRecorders.ContainsKey(recorderName))
                {
                    _activeGpuRecorderNames.Remove(recorderName);
                    _activeGpuRecorders.Remove(recorderName);
                    _activeGpuRecorderCount--;
                }
            }
        }

        public long GetGpuElapsedNanoseconds()
        {
            var gpuElapsedNanoseconds = 0L;

            for (var i = 0; i < _activeGpuRecorderCount; i++)
            {
                var recorder = _activeGpuRecorders[_activeGpuRecorderNames[i]];
                if (recorder.isValid)
                {
                    gpuElapsedNanoseconds += recorder.gpuElapsedNanoseconds;
                }
            }

            return gpuElapsedNanoseconds;
        }

        public List<GpuRecord> GetGpuRecords()
        {
            _activeGpuRecords.Clear();

            for (var i = 0; i < _activeGpuRecorderCount; i++)
            {
                var recorder = _activeGpuRecorders[_activeGpuRecorderNames[i]];
                if (recorder.isValid)
                {
                    _activeGpuRecords.Add(new GpuRecord
                    {
                        RecorderName = _activeGpuRecorderNames[i],
                        GpuElapsedNanoseconds = recorder.gpuElapsedNanoseconds
                    });
                }
            }
            
            _activeGpuRecords.Sort((x, y) => y.GpuElapsedNanoseconds.CompareTo(x.GpuElapsedNanoseconds));;

            return _activeGpuRecords;
        }
    }
}