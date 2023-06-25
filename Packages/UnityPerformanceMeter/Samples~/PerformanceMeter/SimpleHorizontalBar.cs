using UnityEngine;
using UnityEngine.UI;

namespace UnityPerformanceMeter.Samples
{
    public class SimpleHorizontalBar : MonoBehaviour
    {
        [SerializeField] Slider _barSlider;
        [SerializeField] Image _fillImage;

        public void SetMaxValue(float value)
        {
            _barSlider.maxValue = value;
        }

        public void SetCurrentValue(float value)
        {
            _barSlider.value = value;
        }

        public void SetColor(Color color)
        {
            _fillImage.color = color;
        }
    }
}
