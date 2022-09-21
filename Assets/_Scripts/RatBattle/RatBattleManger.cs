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

    [Header("Player")]
    [SerializeField] GameObject _playerPrefab;
    GameObject _playerGO;

    [Header("Friend")]
    GameObject _friendGO;

    [Header("Conversation")]
    [SerializeField] Conversation _friendComes;
    [SerializeField] Conversation _friendElectrifies;


    [Header("Rats")]
    [SerializeField] GameObject _ratPrefab;
    [SerializeField] Brain[] _ratBrains;

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
    }

    void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (state == BattleState.PlayerTurn)
            HandlePlayerTurn();
    }

    async void HandlePlayerTurn()
    {
        await Task.Yield();
        // water spreads every turn
        // await SpreadWater();

        // if (TurnManager.CurrentTurn == 1)
        //     InstantiateWater(new Vector3(4.5f, 4.5f));

        // if (TurnManager.CurrentTurn == 4) // TODO: normally 5th turn? 
        //     await SpawnFriend();

        //  if (TurnManager.CurrentTurn == 7) // TODO: normally 7th turn? 
        //        await FriendElectrifies();
    }

    async void MapSetUp()
    {
        _battleManager.GetComponent<TileManager>().SetUp();
        AudioManager.Instance.PlayAmbience(_ambience);
        await SetupAstar();
        await SpawnPlayer();
        await WalkPlayer();
        await _cameraManager.LerpOrthographicSize(7, 1);
        _turnManager.UpdateBattleState(BattleState.PlayerTurn);
    }

    void SpawnFirstRats()
    {
        Vector3[] positions = new Vector3[]{
            new Vector3(2.5f, -3.5f),
            new Vector3(8.5f, -4.5f),
        };

        foreach (Vector3 pos in positions)
            SpawnRat(pos);
    }

    void SpawnRat(Vector3 pos)
    {
        EnemyCharacter enemySO = (EnemyCharacter)ScriptableObject.CreateInstance<EnemyCharacter>();
        enemySO.CreateEnemy(1, _ratBrains[Random.Range(0, _ratBrains.Length)]);
        Character instantiatedSO = Instantiate(enemySO);
        GameObject enemyGO = Instantiate(_ratPrefab, pos, Quaternion.identity);
        instantiatedSO.Initialize(enemyGO);
        enemyGO.name = instantiatedSO.CharacterName;
        enemyGO.transform.parent = _envObjectsHolder.transform;

        // rat specific stat machinations
        CharacterStats stats = enemyGO.GetComponent<CharacterStats>();
        stats.SetCharacteristics(instantiatedSO);
        CharacterRendererManager characterRendererManager = enemyGO.GetComponentInChildren<CharacterRendererManager>();
        characterRendererManager.transform.localPosition = Vector3.zero; // normally, characters are moved by 0.5 on y axis
        characterRendererManager.Face(Vector2.down);
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

    async Task SpreadWater()
    {
        GameObject[] water = GameObject.FindGameObjectsWithTag(Tags.WaterOnTile);
        if (water.Length == 0)
            return;

        foreach (GameObject w in water)
            for (int x = -1; x <= 1; x++)
                for (int y = -1; y < 1; y++)
                    InstantiateWater(new Vector3(w.transform.position.x + x, w.transform.position.y + y));

        await Task.Yield(); // to silnce the warning
    }

    async void InstantiateWater(Vector3 pos)
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(pos, 0.2f);
        foreach (Collider2D c in cols)
            if (c.CompareTag(Tags.WaterOnTile) || c.CompareTag(Tags.BoundCollider))
                return;

        GameObject w = Instantiate(_waterOnTile, pos, Quaternion.identity);
        await w.GetComponent<WaterOnTile>().Initialize(pos, null, Tags.Player);
        SpriteRenderer sr = w.GetComponent<SpriteRenderer>();
        sr.color = new Color(1f, 1f, 1f, 0f);
        w.transform.parent = _envObjectsHolder.transform;

        Color targetColor = new Color(1f, 1f, 1f, 0.5f);
        await Task.Delay(50);
        if (w != null && sr != null)
            sr.DOColor(targetColor, 10f);
    }

    async Task SpawnFriend()
    {
        _playerGO.GetComponentInChildren<CharacterRendererManager>().Face(Vector2.up);

        Vector3 pos = new Vector3(-3.5f, 8.5f);
        _friendGO = Instantiate(_playerPrefab, pos, Quaternion.identity);
        _friendGO.SetActive(false);

        Character friendCharacter = _runManager.PlayerTroops[1];
        Character instantiatedSO = Instantiate(friendCharacter);
        instantiatedSO.Initialize(_friendGO);
        _friendGO.GetComponent<CharacterStats>().SetCharacteristics(instantiatedSO);
        _friendGO.GetComponentInChildren<CharacterRendererManager>().Face(Vector2.down);
        _friendGO.tag = Tags.IdleCharacter;

        await Task.Delay(10);
        _battleInputController.SetInputAllowed(false);
        await _battleCutSceneManager.WalkCharacterTo(_friendGO, new Vector3(-3.5f, 6.5f));
        await _conversationManager.PlayConversation(_friendComes);
        _battleInputController.SetInputAllowed(true);
        _cameraManager.SetTarget(_movePointController.transform);
    }

    async Task FriendElectrifies()
    {
        _playerGO.GetComponentInChildren<CharacterRendererManager>().Face(Vector2.left);

        Debug.Log("friend electrifies");
        _battleInputController.SetInputAllowed(false);
        await _battleCutSceneManager.WalkCharacterTo(_friendGO, new Vector3(-3.5f, 3.5f));
        await _conversationManager.PlayConversation(_friendElectrifies);

        _cameraManager.SetTarget(_movePointController.transform);

        // use ability
        CharacterStats stats = _friendGO.GetComponent<CharacterStats>();
        Vector3 attackPos = new Vector3(4.5f, 3.5f);
        await Task.Delay(200);
        _friendGO.GetComponentInChildren<CharacterRendererManager>().Face(Vector2.right);

        _movePointController.transform.position = attackPos;
        await stats.Abilities[0].HighlightAreaOfEffect(attackPos);
        await Task.Delay(200);
        await stats.Abilities[0].TriggerAbility(_highlightManager.HighlightedTiles);
    }
}
