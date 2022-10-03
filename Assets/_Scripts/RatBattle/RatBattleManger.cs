using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.Threading.Tasks;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class RatBattleManger : Singleton<RatBattleManger>
{
    GameManager _gameManager;
    RunManager _runManager;
    BattleManager _battleManager;
    TurnManager _turnManager;
    BattleCameraManager _cameraManager;
    ConversationManager _conversationManager;
    BattleCutSceneManager _battleCutSceneManager;
    MovePointController _movePointController;
    BattleInputController _battleInputController;
    HighlightManager _highlightManager;
    RatBattleShapesPainter _ratBattleShapesPainter;


    [Header("General")]
    [SerializeField] Sound _ambience;
    [SerializeField] TextAsset _graphData;
    [SerializeField] Light2D _globalLight;
    [SerializeField] GameObject _waterOnTile;
    [SerializeField] GameObject _envObjectsHolder;
    [SerializeField] GameObject _boulderBlockingRats;
    [SerializeField] GameObject _boulderBlockingBoss;
    [SerializeField] GameObject _fogOfWarRats;
    [SerializeField] GameObject _fogOfWarBoss;

    [Header("Player")]
    [SerializeField] GameObject _playerPrefab;
    GameObject _playerGO;

    [Header("Friend")]
    GameObject _friendGO;
    bool _isFriendSpawned;

    [Header("Conversation")]
    [SerializeField] Conversation _friendComes;
    [SerializeField] Conversation _friendComments;

    [Header("Rats")]
    [SerializeField] GameObject _ratPrefab;
    [SerializeField] Brain[] _ratBrains;
    [SerializeField] Brain _bossBrain;
    [SerializeField] RatSpawner _ratSpawner;
    List<GameObject> _spawnedRats = new();
    GameObject _boss;
    int _ratsKilled = 0;

    protected override void Awake()
    {
        base.Awake();
        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;
    }

    void OnDestroy()
    {
        TurnManager.OnBattleStateChanged -= TurnManager_OnBattleStateChanged;
    }

    void Start()
    {
        _gameManager = GameManager.Instance;
        _runManager = RunManager.Instance;
        _battleManager = BattleManager.Instance;
        _turnManager = TurnManager.Instance;
        _cameraManager = BattleCameraManager.Instance;
        _conversationManager = ConversationManager.Instance;
        _battleCutSceneManager = BattleCutSceneManager.Instance;
        _movePointController = MovePointController.Instance;
        _battleInputController = BattleInputController.Instance;
        _highlightManager = HighlightManager.Instance;
        _ratBattleShapesPainter = GetComponent<RatBattleShapesPainter>();

        LightManager.Instance.Initialize(_globalLight);

        MapSetUp();
        SpawnFirstRats();
        SpawnBoss();

        _boulderBlockingRats.GetComponent<PushableObstacle>().OnPushed += OnBoulderBlockingRatsPushed;
    }

    void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (state == BattleState.PlayerTurn)
            HandlePlayerTurn();
    }

    async void HandlePlayerTurn()
    {
        await Task.Yield();
        if (_ratsKilled == 2 && !_isFriendSpawned)
        {
            await SpawnFriend();
            _isFriendSpawned = true;
        }
    }

    async void MapSetUp()
    {
        _battleManager.GetComponent<TileManager>().SetUp();
        AudioManager.Instance.PlayAmbience(_ambience);
        await SetupAstar();
        await SpawnPlayer();

        foreach (GameObject rat in _spawnedRats)
            rat.GetComponent<ObjectUI>().ToggleCharacterNameDisplay(false);

        await WalkPlayer();
        await _cameraManager.LerpOrthographicSize(7, 1);
        _turnManager.UpdateBattleState(BattleState.PlayerTurn);
        _turnManager.AddEnemy(_boss);
    }

    void SpawnFirstRats()
    {
        Vector3[] positions = new Vector3[]{
            new Vector3(2.5f, -3.5f),
            new Vector3(8.5f, -4.5f),
        };

        EnemyCharacter enemySO = (EnemyCharacter)ScriptableObject.CreateInstance<EnemyCharacter>();
        enemySO.CreateEnemy(1, _ratBrains[Random.Range(0, _ratBrains.Length)]);

        foreach (Vector3 pos in positions)
        {
            GameObject rat = SpawnEnemy(pos, enemySO, Vector3.one * 1.5f);
            _spawnedRats.Add(rat);
        }
    }

    GameObject SpawnEnemy(Vector3 pos, EnemyCharacter enemyCharacter, Vector3 scale)
    {
        Character instantiatedSO = Instantiate(enemyCharacter);
        GameObject enemyGO = Instantiate(_ratPrefab, pos, Quaternion.identity);
        instantiatedSO.Initialize(enemyGO);
        enemyGO.name = instantiatedSO.CharacterName;
        enemyGO.transform.parent = _envObjectsHolder.transform;
        enemyGO.GetComponent<CharacterStats>().OnCharacterDeath += OnRatDeath;

        CharacterStats stats = enemyGO.GetComponent<CharacterStats>();
        stats.SetCharacteristics(instantiatedSO);
        CharacterRendererManager characterRendererManager = enemyGO.GetComponentInChildren<CharacterRendererManager>();
        characterRendererManager.transform.localPosition = new Vector3(0f, 0.1f); // normally, characters are moved by 0.5 on y axis
        characterRendererManager.transform.localScale = scale;
        characterRendererManager.Face(Vector2.down);

        return enemyGO;
    }

    void SpawnBoss()
    {
        Vector3 bossSpawnPosition = new Vector3(18.5f, -11.5f);
        EnemyCharacter enemySO = (EnemyCharacter)ScriptableObject.CreateInstance<EnemyCharacter>();
        enemySO.CreateEnemy(1, _bossBrain);

        _boss = SpawnEnemy(bossSpawnPosition, enemySO, Vector3.one * 3f);
        _boss.GetComponentInChildren<CharacterRendererManager>().transform.localPosition = new Vector3(0, 0.4f); // moving it to be more centered
        _boss.SetActive(false);
    }

    async Task SetupAstar()
    {
        AstarData data = AstarPath.active.data;

        data.DeserializeGraphs(_graphData.bytes);
        GridGraph gg = data.gridGraph;

        // Setup a grid graph with some values
        int width = 30;
        int depth = 26;
        float nodeSize = 1;
        gg.center = new Vector3(10, -4, 0);

        // Updates internal size from the above values
        gg.SetDimensions(width, depth, nodeSize);

        // Scans all graphs
        AstarPath.active.Scan();

        await Task.Delay(10);
    }

    async Task SpawnPlayer()
    {
        Vector3 pos = new Vector3(-3.5f, 8.5f);
        _playerGO = Instantiate(_playerPrefab, pos, Quaternion.identity);
        _playerGO.SetActive(false);

        Character playerCharacter = _runManager.PlayerTroops[0];
        Character instantiatedSO = Instantiate(playerCharacter);
        instantiatedSO.Initialize(_playerGO);
        _playerGO.GetComponent<CharacterStats>().SetCharacteristics(instantiatedSO);
        _playerGO.GetComponentInChildren<CharacterRendererManager>().Face(Vector2.down);

        await Task.Delay(10);
    }

    async Task WalkPlayer()
    {
        await _battleCutSceneManager.WalkCharacterTo(_playerGO, new Vector3(-3.5f, 2.5f));
        await Task.Delay(200);
        _playerGO.GetComponentInChildren<CharacterRendererManager>().Face(Vector2.right);
    }
    void OnBoulderBlockingRatsPushed()
    {
        SpriteRenderer sr = _fogOfWarRats.GetComponentInChildren<SpriteRenderer>();
        sr.DOColor(new Color(0f, 0f, 0f, 0f), 1f);

        foreach (GameObject rat in _spawnedRats)
            rat.GetComponent<ObjectUI>().ToggleCharacterNameDisplay(true);
    }

    async void OnRatDeath(GameObject obj)
    {
        _spawnedRats.Remove(obj);
        _ratsKilled++;
        if (_ratsKilled == 2)
        {
            _boss.SetActive(true);
            await Task.Delay(10);
            _boss.GetComponent<ObjectUI>().ToggleCharacterNameDisplay(false);
        }
    }

    async Task SpawnFriend()
    {
        _playerGO.GetComponentInChildren<CharacterRendererManager>().Face(Vector2.left);

        Vector3 pos = new Vector3(-3.5f, 8.5f);
        _friendGO = Instantiate(_playerPrefab, pos, Quaternion.identity);
        _friendGO.SetActive(false);

        Character friendCharacter = _runManager.PlayerTroops[1];
        Character instantiatedSO = Instantiate(friendCharacter);
        instantiatedSO.Initialize(_friendGO);
        _friendGO.GetComponent<CharacterStats>().SetCharacteristics(instantiatedSO);
        _friendGO.GetComponentInChildren<CharacterRendererManager>().Face(Vector2.down);
        _turnManager.AddPlayer(_friendGO);

        await Task.Delay(10);
        _battleInputController.SetInputAllowed(false);
        await _battleCutSceneManager.WalkCharacterTo(_friendGO, new Vector3(-0.5f, 2.5f), 5);
        _friendGO.GetComponentInChildren<CharacterRendererManager>().Face(Vector2.right);
        await _conversationManager.PlayConversation(_friendComes);
        await _battleCutSceneManager.WalkCharacterTo(_friendGO, new Vector3(4.5f, -6.5f), 5);
        await FriendDestroysBoulder();
        await RevealThirdRoom();
        await _conversationManager.PlayConversation(_friendComments);
        _movePointController.transform.position = _ratSpawner.transform.position;
        await Task.Delay(200);
        await _ratSpawner.SpawnRat();
        await _ratSpawner.SpawnRat();
        _friendGO.GetComponent<CharacterSelection>().FinishCharacterTurn();

        _battleInputController.SetInputAllowed(true);
        _cameraManager.SetTarget(_movePointController.transform);
    }

    async Task FriendDestroysBoulder()
    {
        _playerGO.GetComponentInChildren<CharacterRendererManager>().Face(Vector2.down);

        // use ability
        CharacterStats stats = _friendGO.GetComponent<CharacterStats>();
        Vector3 attackPos = _boulderBlockingBoss.transform.position;
        await Task.Delay(300);
        _friendGO.GetComponentInChildren<CharacterRendererManager>().Face(Vector2.right);

        _movePointController.transform.position = attackPos;
        _cameraManager.SetTarget(_movePointController.transform);

        await stats.Abilities[0].HighlightAreaOfEffect(attackPos); //TODO: risky bisquits
        await Task.Delay(500);
        await stats.Abilities[0].TriggerAbility(_highlightManager.HighlightedTiles);
        await _highlightManager.ClearHighlightedTiles();
    }

    async Task RevealThirdRoom()
    {
        _ratSpawner.gameObject.SetActive(true);
        SpriteRenderer sr = _fogOfWarBoss.GetComponentInChildren<SpriteRenderer>();
        await sr.DOColor(new Color(0f, 0f, 0f, 0f), 1f).AsyncWaitForCompletion();
        _boss.GetComponent<ObjectUI>().ToggleCharacterNameDisplay(true);
    }

}
