using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class OpponentPortalCard : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "opponent-portal-card__";
    const string _ussMain = _ussClassName + "main";
    const string _ussDifficultyIcon = _ussClassName + "difficulty-icon";

    GameManager _gameManager;
    BattleManager _battleManager;

    Element _element;
    public OpponentPortalCard(Element element)
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;

        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.OpponentPortalCardStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddToClassList(_ussCommonTextPrimary);
        AddToClassList(_ussMain);

        _element = element;

        Battle battle = _gameManager.CurrentBattle;
        List<BattleWave> waves = new(battle.Waves.Where(w => w.Element == _element));

        // get next wave
        BattleWave nextWave = waves.FirstOrDefault(w => w.StartTime > Time.time);
        if (nextWave == null)
        {
            Add(new Label("No more waves"));
            return;
        }

        AddTimerToNextWave(nextWave);
        AddWaveDifficulty(nextWave);
    }

    void AddTimerToNextWave(BattleWave wave)
    {
        float timeUntilWave = wave.StartTime - Time.time;
        TimerElement nextWaveTimer = new(timeUntilWave, timeUntilWave, false, "Next wave in: ");
        Add(nextWaveTimer);
    }

    void AddWaveDifficulty(BattleWave wave)
    {
        VisualElement container = new();
        container.style.flexDirection = FlexDirection.Row;
        Add(container);

        Label diff = new("Difficulty:");
        container.Add(diff);

        for (int i = 0; i < wave.Difficulty; i++)
        {
            Label icon = new();
            icon.AddToClassList(_ussDifficultyIcon);
            container.Add(icon);
        }
    }

}
