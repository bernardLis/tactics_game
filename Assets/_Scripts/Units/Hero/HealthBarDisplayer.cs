using Lis.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Lis.Units.Hero
{
    public class HealthBarDisplayer : MonoBehaviour
    {
        [SerializeField] Image _fill;
        Slider _healthBarSlider;

        Hero _hero;

        void OnDestroy()
        {
            _hero.MaxHealth.OnValueChanged -= UpdateHealthBar;
            _hero.CurrentHealth.OnValueChanged -= UpdateHealthBar;
        }

        public void Initialize(Hero hero)
        {
            _healthBarSlider = GetComponent<Slider>();
            _fill.color = GameManager.Instance.GameDatabase.GetColorByName("Health").Primary;

            _hero = hero;
            _hero.MaxHealth.OnValueChanged += UpdateHealthBar;
            _hero.CurrentHealth.OnValueChanged += UpdateHealthBar;

            UpdateHealthBar(default);
        }

        void UpdateHealthBar(float _)
        {
            float newValue = _hero.CurrentHealth.Value / _hero.MaxHealth.GetValue();
            newValue = Mathf.Clamp(newValue, 0, 1);
            _healthBarSlider.value = newValue;

            _healthBarSlider.gameObject.SetActive(_healthBarSlider.value != 1f);
        }
    }
}