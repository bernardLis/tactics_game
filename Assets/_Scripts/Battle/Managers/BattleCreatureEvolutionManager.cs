using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleCreatureEvolutionManager : MonoBehaviour
{

    BattleManager _battleManager;

    [SerializeField] GameObject _effectPrefab;
    GameObject _effectInstance;

    void Start()
    {
        _battleManager = GetComponent<BattleManager>();
        _battleManager.OnPlayerCreatureAdded += OnPlayerCreatureAdded;

        foreach (BattleEntity be in _battleManager.PlayerCreatures)
        {
            BattleCreature bc = be.GetComponent<BattleCreature>();
            bc.OnEvolving += EvolveCreature;
        }
    }

    void OnPlayerCreatureAdded(BattleCreature bc)
    {
        bc.OnEvolving += EvolveCreature;
    }

    void EvolveCreature(BattleCreature bc)
    {
        StartCoroutine(EvolutionCoroutine(bc));
    }

    IEnumerator EvolutionCoroutine(BattleCreature originalEntity)
    {
        Vector3 effectPos = new Vector3(originalEntity.transform.position.x,
                0f, originalEntity.transform.position.z);
        _effectInstance = Instantiate(_effectPrefab, effectPos, Quaternion.identity);

        yield return originalEntity.transform.DOMoveY(5f, 2f)
            .SetEase(Ease.InFlash)
            .WaitForCompletion();

        originalEntity.DisplayFloatingText("Evolving!", Color.magenta);
        ObjectShaders sh = originalEntity.GetComponent<ObjectShaders>();

        sh.Dissolve(5f, false);
        sh.OnDissolveComplete += () =>
        {
            originalEntity.Entity.Hero.RemoveCreature(originalEntity.Entity as Creature);
            originalEntity.TriggerDieCoroutine();
        };

        InstantiateEvolvedCreature(originalEntity);
    }

    void InstantiateEvolvedCreature(BattleCreature originalCreature)
    {
        Creature oldCreature = originalCreature.Entity as Creature;
        Hero hero = oldCreature.Hero;

        Creature newCreature = Instantiate(oldCreature.EvolvedCreature);

        hero.AddCreature(newCreature, true);

        newCreature.InitializeBattle(hero);
        newCreature.ImportCreatureStats(oldCreature);

        Vector3 pos = originalCreature.transform.position;
        GameObject instance = Instantiate(newCreature.Prefab, pos, transform.localRotation);
        BattleEntity be = instance.GetComponent<BattleEntity>();
        be.InitializeEntity(newCreature);
        be.Collider.enabled = false;

        be.GetComponent<ObjectShaders>().Dissolve(5f, true);

        be.transform.DOMoveY(1f, 2f)
            .SetDelay(2f)
            .OnComplete(() =>
            {
                be.Collider.enabled = true;
                _battleManager.AddPlayerArmyEntity(be);

                _effectInstance.transform.DOScale(0f, 1f)
                    .OnComplete(() => Destroy(_effectInstance));
            });
    }
}
