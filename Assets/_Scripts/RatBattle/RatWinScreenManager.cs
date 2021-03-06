using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RatWinScreenManager : MonoBehaviour
{
    BattleUI _battleUI;

    [SerializeField] Texture2D _check;

    void Start()
    {
        _battleUI = BattleUI.Instance;
        _battleUI.OnBattleEndScreenShown += BattleUI_OnBattleEndScreenShown;
    }

    void OnDestroy()
    {
        _battleUI.OnBattleEndScreenShown -= BattleUI_OnBattleEndScreenShown;
    }

    void BattleUI_OnBattleEndScreenShown()
    {
        DefeatedAllEnemies();
        WasElectrified();
        CoveredRatSpawners();
        FoundCollectible();
    }

    VisualElement GetCheckElement(bool isCompleted)
    {
        VisualElement el = new VisualElement();
        if (isCompleted)
            el.style.backgroundImage = _check;

        el.AddToClassList("battleGoalCheck");
        return el;
    }

    VisualElement GetGoalContainer()
    {
        VisualElement el = new VisualElement();
        el.AddToClassList("primaryText");
        el.style.flexDirection = FlexDirection.Row;
        return el;
    }

    void DefeatedAllEnemies()
    {
        VisualElement container = GetGoalContainer();
        container.Add(GetCheckElement(true));
        TextWithTooltip l = new TextWithTooltip("Defeat all enemies.", "Rats!");
        container.Add(l);
        _battleUI.AddGoalToBattleEndScreen(container);
    }

    void WasElectrified()
    {
        GameObject player = TurnManager.Instance.GetPlayerCharacters()[0];
        CharacterStats stats = player.GetComponent<CharacterStats>();

        VisualElement container = GetGoalContainer();

        bool isElectrified = false;
        foreach (Status s in stats.Statuses)
            if (s.ReferenceID == "ElectrifyStatus")
                isElectrified = true;

        if (!isElectrified)
            container.Add(GetCheckElement(true));
        else
            container.Add(GetCheckElement(false));

        TextWithTooltip l = new TextWithTooltip("I don't need a power nap.", "Don't get electrified.");
        container.Add(l);
        _battleUI.AddGoalToBattleEndScreen(container);
    }

    void CoveredRatSpawners()
    {
        VisualElement container = GetGoalContainer();

        RatSpawner[] ratSpawners = GameObject.FindObjectsOfType<RatSpawner>();
        int gratesCovered = 0;
        foreach (RatSpawner s in ratSpawners)
            if (s.IsSpawnCoveredWithBoulder())
                gratesCovered++;

        if (gratesCovered == 2)
            container.Add(GetCheckElement(true));
        else
            container.Add(GetCheckElement(false));

        TextWithTooltip l = new TextWithTooltip("Strongman!", "Cover both grates with boulders.");
        container.Add(l);

        _battleUI.AddGoalToBattleEndScreen(container);
    }

    void FoundCollectible()
    {
        Collectible c = GameObject.FindObjectOfType<Collectible>();

        VisualElement container = GetGoalContainer();
        TextWithTooltip l = new TextWithTooltip("Fox collector!", "Find a fox collectible.");

        if (c.IsCollected)
            container.Add(GetCheckElement(true));
        else
            container.Add(GetCheckElement(false));

        container.Add(l);
        _battleUI.AddGoalToBattleEndScreen(container);
    }
}
