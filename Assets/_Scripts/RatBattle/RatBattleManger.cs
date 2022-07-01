using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.Threading.Tasks;

public class RatBattleManger : Singleton<RatBattleManger>
{
    GameManager _gameManager;
    BattleManager _battleManager;
    TurnManager _turnManager;

    [Header("General")]
    [SerializeField] GameObject _envObjectsHolder;
    [SerializeField] TextAsset _graphData;

    [Header("Rats")]
    [SerializeField] GameObject _ratPrefab;
    [SerializeField] Brain[] _ratBrains;

    [Header("Player")]
    [SerializeField] GameObject _playerPrefab;

    protected override void Awake()
    {
        base.Awake();
        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;

    }

    void OnDestroy()
    {
        TurnManager.OnBattleStateChanged -= TurnManager_OnBattleStateChanged;
    }

    void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (TurnManager.BattleState == BattleState.Deployment)
            HandleDeployment();
    }

    void HandleDeployment()
    {
        _turnManager.UpdateBattleState(BattleState.PlayerTurn);
    }

    void Start()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;
        _turnManager = TurnManager.Instance;
        MapSetUp();
    }

    async void MapSetUp()
    {
        _battleManager.GetComponent<TileManager>().SetUp();

        await SetupAstar();
        await SpawnEnemies();
        await SpawnPlayer();
    }

    async Task SetupAstar()
    {
        AstarData data = AstarPath.active.data;

        data.DeserializeGraphs(_graphData.bytes);
        GridGraph gg = data.gridGraph;

        // Setup a grid graph with some values
        int width = 20;
        int depth = 20;
        float nodeSize = 1;
        gg.center = new Vector3(5, 5, 0);

        // Updates internal size from the above values
        gg.SetDimensions(width, depth, nodeSize);

        // Scans all graphs
        AstarPath.active.Scan();

        await Task.Delay(10);
    }

    async Task SpawnEnemies()
    {
        Vector3[] ratSpawnPositions = new Vector3[]{
            new Vector3(2.5f,1.5f,0),
            new Vector3(3.5f,2.5f,0),
            new Vector3(7.5f,2.5f,0),
            new Vector3(7.5f,4.5f,0)
        };

        for (int i = 0; i < 4; i++)
            SpawnRat(ratSpawnPositions[i]);

        await Task.Delay(10);
    }

    public void SpawnRat(Vector3 pos)
    {
        EnemyCharacter enemySO = (EnemyCharacter)ScriptableObject.CreateInstance<EnemyCharacter>();

        enemySO.CreateEnemy(1, _ratBrains[Random.Range(0, _ratBrains.Length)]);

        Character instantiatedSO = Instantiate(enemySO);
        GameObject newCharacter = Instantiate(_ratPrefab, pos, Quaternion.identity);

        instantiatedSO.Initialize(newCharacter);
        newCharacter.name = instantiatedSO.CharacterName;
        newCharacter.transform.parent = _envObjectsHolder.transform;

        // rat specific stat machinations
        CharacterStats stats = newCharacter.GetComponent<CharacterStats>();
        stats.SetCharacteristics(instantiatedSO);
        stats.MovementRange.BaseValue = 1;
        stats.MaxHealth.BaseValue = 10;
        stats.MaxMana.BaseValue = 0;
        stats.SetCurrentHealth(10);

        CharacterRendererManager characterRendererManager = newCharacter.GetComponentInChildren<CharacterRendererManager>();

        characterRendererManager.transform.localPosition = Vector3.zero; // normally, characters are moved by 0.5 on y axis
        characterRendererManager.Face(Vector2.left);
    }

    async Task SpawnPlayer()
    {
        Character playerCharacter = _gameManager.PlayerTroops[0];
        Vector3 placementPosition = new Vector3(-3.5f, 5.5f);
        GameObject playerGO = Instantiate(_playerPrefab, placementPosition, Quaternion.identity);

        playerGO.name = playerCharacter.CharacterName;
        Character instantiatedSO = Instantiate(playerCharacter);
        instantiatedSO.Initialize(playerGO);
        playerGO.GetComponent<CharacterStats>().SetCharacteristics(instantiatedSO);

        playerGO.GetComponentInChildren<CharacterRendererManager>().Face(Vector2.right);

        await Task.Delay(10);

        _turnManager.UpdateBattleState(BattleState.Deployment); // 'hack' coz I don't have deployment here.
    }


}
