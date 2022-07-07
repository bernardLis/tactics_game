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
    BattleManager _battleManager;
    TurnManager _turnManager;
    CameraManager _cameraManager;
    ConversationManager _conversationManager;
    BattleCutSceneManager _battleCutSceneManager;

    [Header("General")]
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
    [SerializeField] Conversation _beginningMonologue;
    [SerializeField] Conversation _friendComes;


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
        _battleManager = BattleManager.Instance;
        _turnManager = TurnManager.Instance;
        _cameraManager = CameraManager.Instance;
        _conversationManager = ConversationManager.Instance;
        _battleCutSceneManager = BattleCutSceneManager.Instance;
        LightManager.Instance.Initialize(_globalLight);

        MapSetUp();
    }

    void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (TurnManager.BattleState == BattleState.PlayerTurn)
            HandlePlayerTurn();
    }

    async void HandlePlayerTurn()
    {
        // water spreads every turn
        GameObject[] water = GameObject.FindGameObjectsWithTag(Tags.WaterOnTile);
        foreach (var w in water)
            for (int x = -1; x <= 1; x++)
                for (int y = -1; y < 1; y++)
                    InstantiateWater(new Vector3(w.transform.position.x + x, w.transform.position.y + y));

        if (TurnManager.CurrentTurn == 2) // TODO: normally 5th turn? 
            await SpawnFriend();
    }

    async void MapSetUp()
    {
        _battleManager.GetComponent<TileManager>().SetUp();

        await SetupAstar();
        await SpawnPlayer();
        await WalkPlayer();
        await _cameraManager.LerpOrthographicSize(7, 1);
        await _conversationManager.PlayConversation(_beginningMonologue);
        _turnManager.UpdateBattleState(BattleState.PlayerTurn);
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

        Character playerCharacter = _gameManager.PlayerTroops[0];
        Character instantiatedSO = Instantiate(playerCharacter);
        instantiatedSO.Initialize(_playerGO);
        _playerGO.GetComponent<CharacterStats>().SetCharacteristics(instantiatedSO);
        _playerGO.GetComponentInChildren<CharacterRendererManager>().Face(Vector2.down);

        await Task.Delay(10);
    }

    async Task WalkPlayer()
    {
        await _battleCutSceneManager.WalkCharacterTo(_playerGO, new Vector3(-3.5f, 3.5f));
        await Task.Delay(10);
        _playerGO.GetComponentInChildren<CharacterRendererManager>().Face(Vector2.right);
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
        if (w != null)
            sr.DOColor(targetColor, 10f);

    }

    async Task SpawnFriend()
    {
        Vector3 pos = new Vector3(-3.5f, 8.5f);
        _friendGO = Instantiate(_playerPrefab, pos, Quaternion.identity);
        _friendGO.SetActive(false);

        Character playerCharacter = _gameManager.PlayerTroops[1];
        Character instantiatedSO = Instantiate(playerCharacter);
        instantiatedSO.Initialize(_friendGO);
        _friendGO.GetComponent<CharacterStats>().SetCharacteristics(instantiatedSO);
        _friendGO.GetComponentInChildren<CharacterRendererManager>().Face(Vector2.down);

        await Task.Delay(10);
        BattleInputController.Instance.SetInputAllowed(false);
        await _battleCutSceneManager.WalkCharacterTo(_friendGO, new Vector3(-3.5f, 6.5f));
        await _conversationManager.PlayConversation(_friendComes);
        Debug.Log("after convo");
        BattleInputController.Instance.SetInputAllowed(true);
        _cameraManager.SetTarget(MovePointController.Instance.transform);
    }

}
