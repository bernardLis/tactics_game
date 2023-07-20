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

        _base = _gameManager.SelectedBattle.Spire;


        ResetManaRestorationCoroutine(0);

        _base.StoreyMana.ManaPerTurnTree.CurrentValue.OnValueChanged += ResetManaRestorationCoroutine;
        _base.StoreyMana.ManaTurnLengthTree.CurrentValue.OnValueChanged += ResetManaRestorationCoroutine;
    }

    void ResetManaRestorationCoroutine(int ignore)
    {
        if (_manaRestorationCoroutine != null)
            StopCoroutine(_manaRestorationCoroutine);

        _manaRestorationCoroutine = ManaRestorationCoroutine();
        StartCoroutine(_manaRestorationCoroutine);
    }

    IEnumerator ManaRestorationCoroutine()
    {
        Debug.Log($"Starting mana restoration coroutine");
        while (true)
        {
            yield return new WaitForSeconds(_base.StoreyMana.ManaTurnLengthTree.CurrentValue.Value);
            int totalMana = _base.StoreyMana.ManaPerTurnTree.CurrentValue.Value;
            totalMana = Mathf.Clamp(totalMana, 0, _base.StoreyMana.ManaBankCapacityTree.CurrentValue.Value);
            _base.StoreyMana.ManaInBank.SetValue(totalMana);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        _tooltipManager.ShowInfo($"Click for mana shrine upgrades");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        _tooltipManager.HideInfo();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Mana shrine upgrade");

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
