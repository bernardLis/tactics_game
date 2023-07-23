using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class TurretStoreyManager : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    GameManager _gameManager;
    BattleManager _battleManager;
    BattleGrabManager _battleGrabManager;
    BattleTooltipManager _tooltipManager;

    StoreyTurret _storeyTurret;
    BattleTurret _battleTurret;
    StoreyTurretElement _storeyTurretElement;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;
        _battleGrabManager = BattleGrabManager.Instance;

        _tooltipManager = BattleTooltipManager.Instance;
    }

    public void Initialize(StoreyTurret storeyTurret)
    {
        _storeyTurret = storeyTurret;
        _battleTurret = GetComponent<BattleTurret>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        _tooltipManager.ShowInfo($"Click for turret upgrades");
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        _tooltipManager.HideInfo();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_battleGrabManager.IsGrabbingEnabled) return;
        Debug.Log("Turret upgrades");

        _storeyTurretElement = new(_storeyTurret, _battleTurret);
        _storeyTurretElement.style.opacity = 0;
        _battleManager.Root.Add(_storeyTurretElement);
        _battleManager.PauseGame();
        DOTween.To(x => _storeyTurretElement.style.opacity = x, 0, 1, 0.5f).SetUpdate(true);

        _storeyTurretElement.OnClosed += () =>
        {
            _battleManager.ResumeGame();
            _battleManager.Root.Remove(_storeyTurretElement);
            _storeyTurretElement = null;
        };
    }
}
