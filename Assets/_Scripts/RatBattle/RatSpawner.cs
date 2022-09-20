using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class RatSpawner : MonoBehaviour, IUITextDisplayable
{
    RatBattleManger _ratBattleManger;
    TurnManager _turnManager;

    [SerializeField] GameObject _spawnerEffect;

    [Header("Rats")]
    [SerializeField] GameObject _envObjectsHolder;
    [SerializeField] Sound _ratSpawnSound;
    [SerializeField] GameObject _ratSpawnEffect;
    [SerializeField] GameObject _ratPrefab;
    [SerializeField] Brain[] _ratBrains;

    void Awake()
    {
        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;
    }

    void Start()
    {
        _ratBattleManger = RatBattleManger.Instance;
        _turnManager = TurnManager.Instance;
    }

    void OnDestroy()
    {
        TurnManager.OnBattleStateChanged -= TurnManager_OnBattleStateChanged;
    }

    void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (state == BattleState.PlayerTurn)
            HandlePlayerTurn();
    }

    void Update()
    {
        // TODO: is it too much to check it in update?
        if (IsSpawnCovered() && _spawnerEffect.activeInHierarchy)
            _spawnerEffect.SetActive(false);
        if (!IsSpawnCovered() && !_spawnerEffect.activeInHierarchy)
            _spawnerEffect.SetActive(true);
    }

    void HandlePlayerTurn()
    {
        // get all with tag enemy, if there are less than 4, spawn rats
        GameObject[] rats = GameObject.FindGameObjectsWithTag("Enemy");
        if (rats.Length < 4 && !IsSpawnCovered())
            SpawnRat();
    }

    bool IsSpawnCovered()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 0.2f);
        foreach (Collider2D c in cols)
            if (c.CompareTag(Tags.Player) || c.CompareTag(Tags.Enemy) || c.CompareTag(Tags.PushableObstacle))
                return true;

        return false;
    }

    public bool IsSpawnCoveredWithBoulder()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 0.2f);
        foreach (Collider2D c in cols)
            if (c.CompareTag(Tags.PushableObstacle))
                return true;

        return false;
    }

    void SpawnRat()
    {
        Vector3 pos = ChooseSpawnPosition();
        if (pos == Vector3.zero)
            return;

        EnemyCharacter enemySO = (EnemyCharacter)ScriptableObject.CreateInstance<EnemyCharacter>();
        enemySO.CreateEnemy(1, _ratBrains[Random.Range(0, _ratBrains.Length)]);
        Character instantiatedSO = Instantiate(enemySO);
        GameObject enemyGO = Instantiate(_ratPrefab, transform.position, Quaternion.identity);
        instantiatedSO.Initialize(enemyGO);
        enemyGO.name = instantiatedSO.CharacterName;
        enemyGO.transform.parent = _envObjectsHolder.transform;

        // rat specific stat machinations
        CharacterStats stats = enemyGO.GetComponent<CharacterStats>();
        stats.SetCharacteristics(instantiatedSO);
        /*
        stats.MovementRange.BaseValue = 1;
        stats.MaxHealth.BaseValue = 10;
        stats.MaxMana.BaseValue = 0;
        stats.SetCurrentHealth(10);
*/
        CharacterRendererManager characterRendererManager = enemyGO.GetComponentInChildren<CharacterRendererManager>();
        characterRendererManager.transform.localPosition = Vector3.zero; // normally, characters are moved by 0.5 on y axis
        characterRendererManager.Face(Vector2.down);

        _turnManager.AddEnemy(enemyGO);
        AudioManager.Instance.PlaySFX(_ratSpawnSound, transform.position);
        GameObject spawnEffect = Instantiate(_ratSpawnEffect, transform.position, Quaternion.identity);

        enemyGO.transform.DOMove(pos, 1f);
        spawnEffect.transform.DOMove(pos, 1f).OnComplete(() => Destroy(spawnEffect));
    }

    public Vector3 ChooseSpawnPosition()
    {
        //spawn in position adjecent to transorm that is not taken
        Vector3[] positions = new Vector3[4] {
            new Vector3(transform.position.x + 1, transform.position.y),
            new Vector3(transform.position.x - 1, transform.position.y),
            new Vector3(transform.position.x, transform.position.y + 1),
            new Vector3(transform.position.x, transform.position.y - 1)
        };

        List<Vector3> openPositions = new();
        foreach (Vector3 pos in positions)
        {
            bool isPosTaken = false;

            Collider2D[] cols = Physics2D.OverlapCircleAll(pos, 0.2f);
            foreach (Collider2D c in cols)
                if (c.CompareTag(Tags.Player) || c.CompareTag(Tags.Enemy) || c.CompareTag(Tags.PushableObstacle))
                    isPosTaken = true;

            if (!isPosTaken)
                openPositions.Add(pos);
        }

        if (openPositions.Count == 0)
            return Vector3.zero;

        return openPositions[Random.Range(0, openPositions.Count)];
    }

    public VisualElement DisplayText()
    {
        return new Label("Grate. Perhaps that's were the rats come from?");
    }
}
