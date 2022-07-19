using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RatWinScreenManager : MonoBehaviour
{


    BattleUI _battleUI;

    // TODO: maybe battle UI has a method where you can add text to battle end container...
    void Awake()
    {
        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;
        _battleUI = BattleUI.Instance;
    }

    void OnDestroy()
    {
        TurnManager.OnBattleStateChanged -= TurnManager_OnBattleStateChanged;
    }

    void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (TurnManager.BattleState == BattleState.PlayerTurn)
            HandlePlayerTurn();
        if (state == BattleState.Won)
            HandleWinning();
    }

    void HandlePlayerTurn()
    {

    }

    void HandleWinning()
    {
        Debug.Log("in rat win screen manager");
        WasElectrified();
        CoveredRatSpawners();
        FoundCollectible();
    }

    void WasElectrified()
    {
        GameObject player = TurnManager.Instance.GetPlayerCharacters()[0];
        CharacterStats stats = player.GetComponent<CharacterStats>();

        Label l = new Label();
        l.AddToClassList("primaryText");
        l.text = "Unpowered!";
        foreach (Status s in stats.Statuses)
            if (s.ReferenceID == "ElectrifyStatus")
                l.text = "Power nap!";

        _battleUI.AddElementToBattleEnd(l);
    }

    void CoveredRatSpawners()
    {
        RatSpawner[] ratSpawners = GameObject.FindObjectsOfType<RatSpawner>();
        Label l = new Label();
        l.AddToClassList("primaryText");

        int gratesCovered = 0;

        foreach (RatSpawner s in ratSpawners)
        {
            if (s.IsSpawnCoveredWithBoulder())
                gratesCovered++;

        }

        l.text = $"Grates covered: {gratesCovered}!";
        if (gratesCovered == 2)
            l.text += " Strongman!";

        _battleUI.AddElementToBattleEnd(l);
    }

    void FoundCollectible()
    {
        Collectible c = GameObject.FindObjectOfType<Collectible>();

        Label l = new Label();
        l.AddToClassList("primaryText");
        l.text = "Fox is missing.";

        if (c.IsCollected)
            l.text = "Fox collector.";


        _battleUI.AddElementToBattleEnd(l);


    }



}
