using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ManaStoreyManager : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    GameManager _gameManager;
    BattleManager _battleManager;
    BattleTooltipManager _tooltipManager;

    Spire _base;
    StoreyManaElement _storeyManaElement;

    IEnumerator _manaRestorationCoroutine;
    void Start()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;

        _tooltipManager = BattleTooltipManager.Instance;

        _base = BattleSpire.Instance.Spire;

        ResetManaRestorationCoroutine();
    }

    void ResetManaRestorationCoroutine()
    {
        if (_manaRestorationCoroutine != null)
            StopCoroutine(_manaRestorationCoroutine);

        _manaRestorationCoroutine = ManaRestorationCoroutine();
        StartCoroutine(_manaRestorationCoroutine);
    }

    IEnumerator ManaRestorationCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(10);
            int manaGain = _base.StoreyMana.ManaPerTurnTree.CurrentValue.Value;
            if (!_base.StoreyMana.DirectManaRestorationUpgrade.IsPurchased)
            {
                BankMana(manaGain);
                continue;
            }

            // try restoring mana to hero if there is mana leftover bank it
            Hero hero = _gameManager.PlayerHero;
            int heroMissingMana = hero.BaseTotalMana.Value - hero.CurrentMana.Value;

            if (heroMissingMana <= 0)
            {
                BankMana(manaGain);
                continue;
            }

            if (heroMissingMana >= manaGain)
            {
                hero.CurrentMana.ApplyChange(manaGain);
                continue;
            }

            hero.CurrentMana.ApplyChange(heroMissingMana);
            BankMana(manaGain - heroMissingMana);
        }
    }

    void BankMana(int mana)
    {
        mana = Mathf.Clamp(mana, 0, _base.StoreyMana.ManaBankCapacityTree.CurrentValue.Value);
        _base.StoreyMana.ManaInBank.SetValue(mana);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        _tooltipManager.ShowInfo(new BattleInfoElement("Mana Shrine Upgrades"));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        _tooltipManager.HideInfo();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        _storeyManaElement = new(_base.StoreyMana);
        _storeyManaElement.style.opacity = 0;
        _battleManager.Root.Add(_storeyManaElement);
        _battleManager.PauseGame();
        DOTween.To(x => _storeyManaElement.style.opacity = x, 0, 1, 0.5f).SetUpdate(true);

        _storeyManaElement.OnClosed += () =>
        {
            _battleManager.ResumeGame();
            _battleManager.Root.Remove(_storeyManaElement);
            _storeyManaElement = null;
        };
    }
}
