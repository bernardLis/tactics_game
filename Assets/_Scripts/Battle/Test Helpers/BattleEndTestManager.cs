using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

#if (UNITY_EDITOR) 

public class BattleEndTestManager : MonoBehaviour
{
    GameManager _gameManager;
    BattleManager _battleManager;
    Hero _hero;
    Hero _opponent;

    [SerializeField] List<Creature> _army;

    [SerializeField] List<Pickup> _pickups;

    void Start()
    {
        _battleManager = BattleManager.Instance;

        InstantiateHeroes();
        InstantiatePickups();
        InstantiateArmies();

        Invoke("WinBattle", 1f);
    }

    void InstantiateHeroes()
    {
        _gameManager = GameManager.Instance;
        List<Creature> armyInstance = new();
        foreach (Creature c in _army)
        {
            Creature instance = Instantiate<Creature>(c);
            armyInstance.Add(instance);
        }

        _hero = ScriptableObject.CreateInstance<Hero>();
        _hero.CreateRandom(1);
        _hero.Abilities = new();
        _hero.Army = armyInstance;
        _gameManager.PlayerHero = _hero;

        foreach (Ability a in GameManager.Instance.HeroDatabase.GetAllAbilities())
        {
            Ability instance = Instantiate<Ability>(a);
            _hero.Abilities.Add(instance);
        }

        Battle battle = ScriptableObject.CreateInstance<Battle>();
        battle.CreateRandomDuel(1);
        _gameManager.SelectedBattle = battle;

        _gameManager.BattleNumber = 0;
    }

    void InstantiatePickups()
    {
        for (int i = 0; i < 10; i++)
        {
            List<Pickup> ordered = new(_pickups.OrderBy(o => o.PickupChance).ToList());
            float roll = Random.value;

            foreach (Pickup p in ordered)
            {
                if (roll <= p.PickupChance)
                {
                    Pickup instance = Instantiate(p);
                    instance.Initialize();
                    _battleManager.CollectPickup(instance);
                    break;
                }
                roll -= p.PickupChance;
            }
        }
    }

    void InstantiateArmies()
    {
        List<BattleEntity> heroArmy = new();
        List<BattleEntity> opponentArmy = new();

        foreach (Creature c in _hero.Army)
        {
            Vector3 pos = new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f));
            GameObject instance = Instantiate(c.Prefab, pos, transform.localRotation);
            BattleEntity be = instance.GetComponent<BattleEntity>();
            be.InitializeCreature(c);
            heroArmy.Add(be);
        }

        foreach (Creature c in _gameManager.SelectedBattle.Opponent.Army)
        {
            Vector3 pos = new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f))
                    + new Vector3(10f, 0f, 10f);
            GameObject instance = Instantiate(c.Prefab, pos, transform.localRotation);
            BattleEntity be = instance.GetComponent<BattleEntity>();
            be.InitializeCreature(c);
            opponentArmy.Add(be);
        }

        _battleManager.Initialize(_hero);
        _battleManager.AddPlayerArmyEntities(heroArmy);
        _battleManager.AddOpponentArmyEntities(opponentArmy);
    }

    void WinBattle()
    {
        _battleManager.WinBattle();
    }
}
#endif
