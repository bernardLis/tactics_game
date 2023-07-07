using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class BattleArmyHotkeys : MonoBehaviour
{
    VisualElement _armyHotkeysContainer;
    BattleCameraManager _cameraManager;
    BattleManager _battleManager;
    BattleEntityTooltipManager _tooltipManager;

    void Start()
    {
        _battleManager = BattleManager.Instance;
        _battleManager.OnPlayerEntityAdded += AddPlayerArmyEntityHotkey;
        _tooltipManager = BattleEntityTooltipManager.Instance;

        _cameraManager = Camera.main.GetComponentInParent<BattleCameraManager>();

        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        _armyHotkeysContainer = root.Q<VisualElement>("armyHotkeysContainer");
    }

    void AddPlayerArmyEntityHotkey(BattleEntity be)
    {
        CreatureIcon icon = new(be.Creature);
        _armyHotkeysContainer.Add(icon);

        icon.BlockAnimation();
        icon.BlockTooltip();
        icon.style.opacity = 0;
        DOTween.To(x => icon.style.opacity = x, 0, 0.8f, 0.5f);

        icon.RegisterCallback<MouseUpEvent>((e) =>
        {
            if (e.button != 0) return;
            _cameraManager.CenterCameraOnBattleEntity(be);
            _tooltipManager.DisplayTooltip(be);
        });

        icon.RegisterCallback<MouseEnterEvent>((e) =>
        {
            icon.style.opacity = 1;
            be.ShowHighlightDiamond();
        });

        icon.RegisterCallback<MouseLeaveEvent>((e) =>
        {
            icon.style.opacity = 0.8f;
            be.HideHighlightDiamond();
        });

    }

}
