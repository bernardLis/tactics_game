using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HealthDisplay : MonoBehaviour
{
    BattleEntity _entity;

    Slider _slider;

    float _totalHealth;
    float _currentHealth;

    void Start()
    {
        _entity = transform.parent.parent.GetComponent<BattleEntity>();
        _entity.OnHealthChanged += OnHealthChanged;
        _totalHealth = _entity.GetTotalHealth();
        _currentHealth = _entity.GetCurrentHealth();

        _slider = GetComponent<Slider>();
        _slider.value = _currentHealth / _totalHealth;
    }

    void OnHealthChanged(float currentHealth)
    {
        DOTween.To(x => _slider.value = x, _currentHealth / _totalHealth, currentHealth / _totalHealth, 0.3f)
                .SetEase(Ease.InExpo)
                .OnComplete(() => _currentHealth = currentHealth);
    }

}
