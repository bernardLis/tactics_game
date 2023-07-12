using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using MoreMountains.Feedbacks;
using UnityEngine.EventSystems;

public class BattleBase : Singleton<BattleBase>, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    GameManager _gameManager;
    BattleManager _battleManager;
    BattleTooltipManager _tooltipManager;

    Base _base;

    MMF_Player _feelPlayer;

    TroopsLimitElement _troopsLimitElement;
    Label _livesCountLabel;
    public int Lives { get; private set; }

    void Start()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;
        _livesCountLabel = _battleManager.Root.Q<Label>("livesCount");

        _tooltipManager = BattleTooltipManager.Instance;
        _feelPlayer = GetComponent<MMF_Player>();

        _base = _gameManager.SelectedBattle.Base;

        AddTroopsLimitElement();

        Lives = 10;
        _livesCountLabel.style.display = DisplayStyle.Flex;
        _livesCountLabel.text = $"Lives: {Lives}";
    }

    void AddTroopsLimitElement()
    {
        VisualElement TroopsLimitContainer = _battleManager.Root.Q<VisualElement>("troopsLimitContainer");
        TroopsLimitContainer.style.display = DisplayStyle.Flex;
        _troopsLimitElement = new($"{_gameManager.PlayerHero.CreatureArmy.Count} / {_base.TroopsLimit.Value}");

        TroopsLimitContainer.Add(_troopsLimitElement);

        _gameManager.PlayerHero.OnCreatureAdded += (c) => UpdateTroopsLimitElement();
        _base.TroopsLimit.OnValueChanged += (v) => UpdateTroopsLimitElement();
    }

    void UpdateTroopsLimitElement()
    {
        _troopsLimitElement.UpdateCountContainer($"{_gameManager.PlayerHero.CreatureArmy.Count} / {_base.TroopsLimit}", Color.white);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<BattleEntity>(out BattleEntity battleEntity))
        {
            if (battleEntity.Team == 0) return;

            Lives--;
            DisplayFloatingText($"Lives: {Lives}", Color.white);
            StartCoroutine(battleEntity.Die(hasPickup: false));

            _livesCountLabel.text = $"Lives: {Lives}";

            if (Lives <= 0)
                _battleManager.LoseBattle();
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"click click");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        _tooltipManager.ShowInfo($"{Lives} lives left");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        _tooltipManager.HideInfo();
    }

    public void DisplayFloatingText(string text, Color color)
    {
        if (_feelPlayer == null) return;
        MMF_FloatingText floatingText = _feelPlayer.GetFeedbackOfType<MMF_FloatingText>();
        floatingText.Value = text;
        floatingText.ForceColor = true;
        floatingText.AnimateColorGradient = Helpers.GetGradient(color);
        _feelPlayer.PlayFeedbacks(transform.position);
    }


}
