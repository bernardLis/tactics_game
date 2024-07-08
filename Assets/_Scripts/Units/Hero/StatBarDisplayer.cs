using Lis.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Lis.Units.Hero
{
    public class StatBarDisplayer : MonoBehaviour
    {
        [SerializeField] Image _fill;
        Slider _barSlider;

        Stat _maxValue;
        FloatVariable _currentValue;

        void OnDestroy()
        {
            _maxValue.OnValueChanged -= UpdateBar;
            _currentValue.OnValueChanged -= UpdateBar;
        }

        public void Initialize(Stat maxValue, FloatVariable currentValue, Color displayColor)
        {
            _maxValue = maxValue;
            _currentValue = currentValue;

            _barSlider = GetComponent<Slider>();
            _fill.color = displayColor;

            _maxValue.OnValueChanged += UpdateBar;
            _currentValue.OnValueChanged += UpdateBar;

            UpdateBar(default);
        }

        void UpdateBar(float _)
        {
            float newValue = _currentValue.Value / _maxValue.GetValue();
            newValue = Mathf.Clamp(newValue, 0, 1);
            _barSlider.value = newValue;

            _barSlider.gameObject.SetActive(!Mathf.Approximately(_barSlider.value, 1f));
        }
    }
}