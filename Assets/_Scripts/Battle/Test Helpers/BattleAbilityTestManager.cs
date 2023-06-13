using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if (UNITY_EDITOR) 

public class BattleAbilityTestManager : MonoBehaviour
{
    BattleManager _battleManager;
    Hero _hero;

    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        root.Q<Button>("restoreMana").clickable.clicked += () => _hero.CurrentMana.SetValue(1000);
        root.Q<Button>("levelUpAbilities").clickable.clicked += () =>
        {
            foreach (Ability a in _hero.Abilities)
                a.LevelUp();
        };
        root.Q<Button>("levelDownAbilities").clickable.clicked += () =>
        {
            foreach (Ability a in _hero.Abilities)
                a.LevelDown();
        };

        _battleManager = BattleManager.Instance;

        _hero = ScriptableObject.CreateInstance<Hero>();
        _hero.CreateRandom(1);
        _hero.Abilities = new();
        _hero.BaseMana.SetValue(1200);


        foreach (Ability a in GameManager.Instance.HeroDatabase.GetAllAbilities())
        {
            Ability instance = Instantiate<Ability>(a);
            _hero.Abilities.Add(instance);
        }
        // HERE: pickup testing
        for (int i = 0; i < 7; i++)
        {
            foreach (Ability a in _hero.Abilities)
                a.LevelUp();
        }

        _battleManager.Initialize(_hero, null, null, null);
        GameManager.Instance.ToggleTimer(true);

    }
}
#endif
