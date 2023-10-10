using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHeroHealthBar : MonoBehaviour
{
    Slider _healthBarSlider;
    [SerializeField] Image _fill;

    Hero _hero;

    public void Initialize(Hero hero)
    {
        _healthBarSlider = GetComponent<Slider>();
        _fill.color = GameManager.Instance.GameDatabase.GetColorByName("Health").Color;

        _hero = hero;
        _hero.MaxHealth.OnValueChanged += UpdateHealthBar;
        _hero.CurrentHealth.OnValueChanged += UpdateHealthBar;

        UpdateHealthBar(default);
    }

    void OnDestroy()
    {
        _hero.MaxHealth.OnValueChanged -= UpdateHealthBar;
        _hero.CurrentHealth.OnValueChanged -= UpdateHealthBar;
    }

    void UpdateHealthBar(int _)
    {
        float newValue = (float)_hero.CurrentHealth.Value / _hero.MaxHealth.GetValue();
        newValue = Mathf.Clamp(newValue, 0, 1);
        _healthBarSlider.value = newValue;

        if (_healthBarSlider.value == 1)
            _healthBarSlider.gameObject.SetActive(false);
        else
            _healthBarSlider.gameObject.SetActive(true);
    }
}
