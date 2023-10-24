using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class BattleWolfLair : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    BattleManager _battleManager;
    BattleTooltipManager _tooltipManager;

    [SerializeField] Creature _wolf;
    [SerializeField] int _wolfCount;
    [SerializeField] BattleEntitySpawner _spawnerPrefab;
    [SerializeField] Transform _spawnPoint;

    [Header("Friendly Wolf Lair")]
    [SerializeField] float _delayBetweenWolfSpawns;
    [SerializeField] int _maxFriendlyWolves;


    IEnumerator _friendlyWolfSpawnCoroutine;

    public List<BattleCreature> _friendlyWolves = new();


    public void Initialize()
    {
        _battleManager = BattleManager.Instance;
        _tooltipManager = BattleTooltipManager.Instance;

        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 1f)
                            .SetEase(Ease.OutBack)
                            .SetDelay(2.5f);

        transform.LookAt(_battleManager.GetComponent<BattleHeroManager>().BattleHero.transform.position);
    }

    public void SpawnWave(int difficulty)
    {
        // TODO: difficulty
        List<Entity> entitiesToSpawn = new();
        for (int i = 0; i < _wolfCount; i++)
            entitiesToSpawn.Add(Instantiate(_wolf));

        BattleEntitySpawner spawner = Instantiate(_spawnerPrefab, _spawnPoint.position, transform.rotation);
        spawner.SpawnEntities(entitiesToSpawn);
        spawner.OnSpawnComplete += (l) =>
        {
            _battleManager.AddOpponentArmyEntities(l);
            spawner.DestroySelf();
        };
    }

    public void Secured()
    {

        StartFriendlyWolfSpawnCoroutine();

    }

    void StartFriendlyWolfSpawnCoroutine()
    {
        Debug.Log($"StartFriendlyWolfSpawnCoroutine {_friendlyWolfSpawnCoroutine}");
        if (_friendlyWolfSpawnCoroutine != null) return;
        Debug.Log($"StartFriendlyWolfSpawnCoroutine after if");

        _friendlyWolfSpawnCoroutine = SpawnFriendlyWolves();
        StartCoroutine(_friendlyWolfSpawnCoroutine);
    }

    IEnumerator SpawnFriendlyWolves()
    {
        while (_friendlyWolves.Count < _maxFriendlyWolves)
        {
            SpawnFriendlyWolf();
            yield return new WaitForSeconds(_delayBetweenWolfSpawns);
        }
        _friendlyWolfSpawnCoroutine = null;
    }

    void SpawnFriendlyWolf()
    {
        BattleEntitySpawner spawner = Instantiate(_spawnerPrefab,
                                _spawnPoint.position, transform.rotation);

        Creature wolf = Instantiate(_wolf);
        spawner.SpawnEntities(new List<Entity>() { wolf });
        spawner.OnSpawnComplete += (l) =>
        {
            // now I need to track the spawned wolf
            BattleCreature bc = l[0] as BattleCreature;
            _friendlyWolves.Add(bc);
            // if it dies, and coroutine is inactive - restart coroutine
            bc.OnDeath += (_, __) =>
            {
                Debug.Log($"on death");
                _friendlyWolves.Remove(bc);
                StartFriendlyWolfSpawnCoroutine();
            };

            _battleManager.AddPlayerArmyEntities(l);
            spawner.DestroySelf();
        };
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;

        _tooltipManager.ShowHoverInfo(new BattleInfoElement("Woof Lair"));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        _tooltipManager.HideHoverInfo();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        Debug.Log($"woof lair click");
    }
}
