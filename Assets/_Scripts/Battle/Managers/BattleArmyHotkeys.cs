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
    BattleTooltipManager _tooltipManager;

    void Start()
    {
        _battleManager = BattleManager.Instance;
        _battleManager.OnPlayerCreatureAdded += AddPlayerArmyEntityHotkey;
        _tooltipManager = BattleTooltipManager.Instance;

        _cameraManager = Camera.main.GetComponentInParent<BattleCameraManager>();

        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        _armyHotkeysContainer = root.Q<VisualElement>("armyHotkeysContainer");
    }

    void AddPlayerArmyEntityHotkey(BattleCreature be)
    {
        EntityIcon icon = new(be.Creature);
        icon.SmallIcon();
        _armyHotkeysContainer.Add(icon);

        be.OnDeath += (a, b, c) =>
        {
            DOTween.To(x => icon.style.opacity = x, 0, 1, 0.5f).OnComplete(() =>
            {
                _armyHotkeysContainer.Remove(icon);
                icon.RemoveFromHierarchy();
            });
        };

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
            be.ShowHighlightDiamond(Color.white);
        });

        icon.RegisterCallback<MouseLeaveEvent>((e) =>
        {
            icon.style.opacity = 0.8f;
            be.HideHighlightDiamond();
        });

    }

}
