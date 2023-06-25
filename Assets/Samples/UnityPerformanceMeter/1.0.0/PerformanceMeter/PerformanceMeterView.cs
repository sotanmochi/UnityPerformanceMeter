using System.Collections;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

namespace UnityPerformanceMeter.Samples
{
    // public sealed class GpuTimeView : MonoBehaviour
    public sealed class PerformanceMeterView : MonoBehaviour
    {
        [SerializeField] int _startUpDelayMilliseconds = 1000;
        [SerializeField] TMP_Text _gpuElapsedMillisecondsText;
        [SerializeField] TMP_Text _gpuTimeDetailsText;
        [SerializeField] SimpleHorizontalBar _horizontalBar;

        private readonly float MaxValueOfSliderVar = 66.66666f; // Milliseconds
        private readonly StringBuilder _stringBuilder = new();
        private GpuPerformanceMeter _gpuPerformanceMeter;

        async void Start()
        {
            _horizontalBar.SetMaxValue(MaxValueOfSliderVar);
            await Task.Delay(_startUpDelayMilliseconds);
            _gpuPerformanceMeter = new();
            _gpuPerformanceMeter.Initialize();
        }

        void Update()
        {
            if (_gpuPerformanceMeter is null) return;
            StartCoroutine(UpdateGpuPerformanceMeter());
        }

        IEnumerator UpdateGpuPerformanceMeter()
        {
            yield return new WaitForEndOfFrame();

            _gpuPerformanceMeter.UpdateActiveGpuRecorders();
            var gpuElapsedMilliseconds = _gpuPerformanceMeter.GetGpuElapsedNanoseconds() * 1e-6f;

            _gpuElapsedMillisecondsText.text = $"{gpuElapsedMilliseconds.ToString("F2")} [ms]";
            _horizontalBar.SetCurrentValue(gpuElapsedMilliseconds);

            _stringBuilder.Clear();
            _stringBuilder.AppendLine("GPU Time Details");

            var gpuRecords = _gpuPerformanceMeter.GetGpuRecords();
            for (var i = 0; i < gpuRecords.Count; i++)
            {
                _stringBuilder.AppendLine("----------");
                _stringBuilder.AppendLine($"{gpuRecords[i].RecorderName} : {gpuRecords[i].GpuElapsedNanoseconds * 1e-6f} [ms]");
            }

            _gpuTimeDetailsText.text = _stringBuilder.ToString();
        }
    }
}