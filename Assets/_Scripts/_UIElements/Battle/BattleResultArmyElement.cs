using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using System.Linq;
using Random = UnityEngine.Random;

public class BattleResultArmyElement : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";
    const string _ussCommonButtonBasic = "common__button-basic";

    const string _ussClassName = "battle-result-army__";
    const string _ussMain = _ussClassName + "main";
    const string _ussArmyGroupContainer = _ussClassName + "army-group-container";

    GameManager _gameManager;
    AudioManager _audioManager;
    BattleManager _battleManager;

    ScrollView _armyGroupContainer;
    List<CreatureCardExp> _creatureExpElements = new();

    public event Action OnFinished;
    public BattleResultArmyElement(VisualElement content)
    {
        _gameManager = GameManager.Instance;
        _audioManager = _gameManager.GetComponent<AudioManager>();
        _battleManager = BattleManager.Instance;

        var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
        if (commonStyles != null)
            styleSheets.Add(commonStyles);
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.BattleResultArmyStyles);
        if (ss != null)
            styleSheets.Add(ss);

        AddToClassList(_ussMain);
        AddToClassList(_ussCommonTextPrimary);

        _armyGroupContainer = new();
        _armyGroupContainer.AddToClassList(_ussArmyGroupContainer);
        Add(_armyGroupContainer);
        _gameManager.PlayerHero.OnCreatureAdded += AddArmyToContainer;

        ShowArmyStats();

        this.schedule.Execute(() => OnFinished?.Invoke())
                .StartingIn(_gameManager.PlayerHero.CreatureArmy.Count * 500);
    }

    void AddArmyToContainer(Creature creature)
    {
        CreatureIcon icon = new(creature);
        icon.SmallIcon();
        _armyGroupContainer.Add(icon);
    }

    void ShowArmyStats()
    {
        for (int i = 0; i < _gameManager.PlayerHero.CreatureArmy.Count; i++)
        {
            _audioManager.PlayUIDelayed("Placing Paper", 0.5f * i);

            Creature c = _gameManager.PlayerHero.CreatureArmy[i];
            CreatureCardExp creatureElement = new(c);
            _armyGroupContainer.Add(creatureElement);
            _creatureExpElements.Add(creatureElement);

            creatureElement.style.opacity = 0;
            DOTween.To(x => creatureElement.style.opacity = x, 0, 1, 0.5f)
                    .SetDelay(0.5f * i);
        }
    }

    public void RefreshArmy()
    {
        _armyGroupContainer.Clear();
        ShowArmyStats();
    }

    public void MoveAway()
    {
        DOTween.To(x => style.opacity = x, 1, 0, 0.5f);
    }
}
