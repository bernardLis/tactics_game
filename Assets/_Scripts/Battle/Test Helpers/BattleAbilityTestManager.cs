using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if (UNITY_EDITOR) 

public class BattleAbilityTestManager : MonoBehaviour
{
    GameManager _gameManager;
    BattleManager _battleManager;

    Hero _hero;

    [SerializeField] int _creaturesToSpawn = 10;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;
        _battleManager.BlockBattleEnd = true;

        SetupUI();
        CreateHero();
        LevelUpAbilities(7);

        _gameManager.PlayerHero = _hero;

        Battle b = ScriptableObject.CreateInstance<Battle>();
        b.CreateRandomDuel(1);
        _gameManager.SelectedBattle = b;

        _battleManager.Initialize(_hero);
        _battleManager.GetComponent<BattleAbilityManager>().Initialize(_hero);
        _battleManager.GetComponent<BattleGrabManager>().Initialize();

        for (int i = 0; i < _creaturesToSpawn; i++)
            StartCoroutine(SpawnCreature());
    }

    void SetupUI()
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
    }

    void CreateHero()
    {
        _hero = ScriptableObject.CreateInstance<Hero>();
        _hero.CreateRandom(1);
        _hero.Abilities = new();
        _hero.BaseMana.SetValue(1200);

        _gameManager.PlayerHero = _hero;

        foreach (Ability a in _gameManager.HeroDatabase.GetAllAbilities())
        {
            Ability instance = Instantiate<Ability>(a);
            _hero.Abilities.Add(instance);
        }
    }

    void LevelUpAbilities(int level)
    {
        for (int i = 0; i < level; i++)
            foreach (Ability a in _hero.Abilities)
                a.LevelUp();
    }

    IEnumerator SpawnCreature()
    {
        Creature creature = _gameManager.HeroDatabase.GetRandomCreatureByUpgradeTier(0);

        Vector3 pos = new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f));
        GameObject instance = Instantiate(creature.Prefab, pos, transform.localRotation);
        BattleEntity be = instance.GetComponent<BattleEntity>();
        be.InitializeCreature(creature);
        be.OnDeath += (a, b, c) => StartCoroutine(SpawnCreature());
        be.OnDeath += (a, b, c) => Destroy(be.gameObject, 2f);
        yield return new WaitForSeconds(0.1f);
        _battleManager.AddOpponentArmyEntity(be);
    }

}
#endif
