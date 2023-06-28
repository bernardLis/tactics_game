using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using DG.Tweening;

public class BattleManager : Singleton<BattleManager>
{
    GameManager _gameManager;

    [SerializeField] GameObject _obstaclePrefab;
    GameObject _obstacleInstance;

    [SerializeField] Sound _battleMusic;
    public Battle LoadedBattle { get; private set; }

    public VisualElement Root { get; private set; }

    public Transform EntityHolder;

    // skybox rotation https://forum.unity.com/threads/rotate-a-skybox.130639/
    int _rotationProperty;
    float _initRot;
    Material _skyMat;
    [SerializeField] float _skyboxRotationSpeed = 0.2f;

    [SerializeField] TextMeshProUGUI _timerText;
    [SerializeField] TextMeshProUGUI _scoreText;

    public float BattleTime { get; private set; }

    Hero _playerHero;
    Hero _opponentHero;

    int _initialPlayerEntityCount;
    int _initialOpponentEntityCount;

    [SerializeField] GameObject _playerSpawnPoint;
    [SerializeField] GameObject _enemySpawnPoint;

    public List<BattleEntity> PlayerEntities = new();
    public List<BattleEntity> OpponentEntities = new();

    public List<BattleEntity> KilledPlayerEntities = new();
    public List<BattleEntity> KilledOpponentEntities = new();

    [HideInInspector] public List<Pickup> CollectedPickups = new();

    public bool IsEndingBattleBlocked;
    bool _battleFinalized = false;

    public event Action OnBattleFinalized;

    protected override void Awake()
    {
        base.Awake();

        Root = GetComponent<UIDocument>().rootVisualElement;

        VisualElement bottomPanel = Root.Q<VisualElement>("bottomPanel");
    }

    void Start()
    {
        VFXCameraManager.Instance.gameObject.SetActive(false);

        _gameManager = GameManager.Instance;
        _gameManager.SaveJsonData();
        LoadedBattle = _gameManager.SelectedBattle;

        Root.Q<VisualElement>("vfx").pickingMode = PickingMode.Ignore;

        _rotationProperty = Shader.PropertyToID("_Rotation");
        _skyMat = RenderSettings.skybox;
        _initRot = _skyMat.GetFloat(_rotationProperty);

#if UNITY_EDITOR
        GetComponent<BattleInputManager>().OnEnterClicked += WinBattle;
#endif
        AudioManager.Instance.PlayMusic(_battleMusic);
    }

    void Update()
    {
        BattleTime += Time.deltaTime;
        TimeSpan time = TimeSpan.FromSeconds(BattleTime);
        _timerText.text = $"{time.Minutes:D2}:{time.Seconds:D2}";

        _skyMat.SetFloat(_rotationProperty, UnityEngine.Time.time * _skyboxRotationSpeed);
    }

    public void Initialize(Hero playerHero, Hero opponentHero, List<Creature> playerArmy, List<Creature> opponentArmy)
    {
        PlaceObstacle();
        _battleFinalized = false;

        GetComponent<BattleGrabManager>().Initialize();

        if (playerHero != null)
        {
            _playerHero = playerHero;
            GetComponent<BattleHeroManager>().Initialize(playerHero);
            GetComponent<BattleAbilityManager>().Initialize(playerHero);
        }

        if (opponentHero != null) _opponentHero = opponentHero;

        if (playerArmy == null) return;

        foreach (Creature c in playerArmy)
            InstantiatePlayer(c);
        foreach (Creature c in opponentArmy)
            InstantiateOpponent(c);

        _initialPlayerEntityCount = PlayerEntities.Count;
        _initialOpponentEntityCount = OpponentEntities.Count;

        _scoreText.text = $"{_initialPlayerEntityCount} : {_initialOpponentEntityCount}";

        if (_gameManager == null) _gameManager = GameManager.Instance;
        _gameManager.ToggleTimer(true);

        GetComponent<BattleLogManager>().Initialize(PlayerEntities, OpponentEntities);
    }

    void PlaceObstacle()
    {
        if (_obstacleInstance != null)
            Destroy(_obstacleInstance);

        //   if (Random.value > 0.5f) return; // 50/50 there is going to be an obstacle

        // between player and enemy
        float posX = _playerSpawnPoint.transform.position.x + (_enemySpawnPoint.transform.position.x - _playerSpawnPoint.transform.position.x) / 2;
        float posZ = _playerSpawnPoint.transform.position.z + (_enemySpawnPoint.transform.position.z - _playerSpawnPoint.transform.position.z) / 2;
        Vector3 pos = new Vector3(posX, 1, posZ);

        float sizeY = Random.Range(3, 10);
        float sizeX = Random.Range(10, 30);
        float sizeZ = Random.Range(1, 5);
        Vector3 size = new Vector3(sizeX, sizeY, sizeZ);

        Vector3 rot = new Vector3(0, Random.Range(-45, 45), 0);

        _obstacleInstance = Instantiate(_obstaclePrefab, pos, Quaternion.identity);
        _obstacleInstance.transform.localScale = size;
        _obstacleInstance.transform.Rotate(rot);
    }

    public void InstantiatePlayer(Creature creature)
    {
        creature.InitializeBattle(_playerHero);

        // Creature creatureInstance = Instantiate(creature);

        Vector3 pos = _playerSpawnPoint.transform.position + new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
        GameObject instance = Instantiate(creature.Prefab, pos, Quaternion.identity);
        instance.layer = 10;
        instance.transform.parent = EntityHolder;
        BattleEntity be = instance.GetComponent<BattleEntity>();
        be.Initialize(0, creature, ref OpponentEntities);
        PlayerEntities.Add(be);
        be.OnEnemyKilled += creature.AddKill;
        be.OnDamageDealt += creature.AddDmgDealt;
        be.OnDamageTaken += creature.AddDmgTaken;
        be.OnDeath += OnPlayerDeath;
    }

    void InstantiateOpponent(Creature creature)
    {
        Creature entityInstance = Instantiate(creature);
        creature.InitializeBattle(_opponentHero);

        Vector3 pos = _enemySpawnPoint.transform.position + new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
        Quaternion rotation = Quaternion.Euler(0, 180, 0);
        GameObject instance = Instantiate(creature.Prefab, pos, rotation);
        instance.layer = 11;
        instance.transform.parent = EntityHolder;
        BattleEntity be = instance.GetComponent<BattleEntity>();
        be.Initialize(1, creature, ref PlayerEntities);
        OpponentEntities.Add(be);
        be.OnDeath += OnEnemyDeath;
    }

    void OnPlayerDeath(BattleEntity be, BattleEntity killer, Ability killerAbility)
    {
        KilledPlayerEntities.Add(be);
        PlayerEntities.Remove(be);
        _scoreText.text = $"{PlayerEntities.Count} : {OpponentEntities.Count}";

        if (PlayerEntities.Count == 0)
            StartCoroutine(BattleLost());
    }

    void OnEnemyDeath(BattleEntity be, BattleEntity killer, Ability killerAbility)
    {
        KilledOpponentEntities.Add(be);
        OpponentEntities.Remove(be);
        _scoreText.text = $"{PlayerEntities.Count} : {OpponentEntities.Count}";

        if (OpponentEntities.Count == 0)
            StartCoroutine(BattleWon());
    }

    public List<BattleEntity> GetAllies(BattleEntity battleEntity)
    {
        if (battleEntity.Team == 0) return PlayerEntities;
        //if (battleEntity.Team == 1) 
        return OpponentEntities;
    }

    public void CollectPickup(Pickup p)
    {
        CollectedPickups.Add(p);
    }

    IEnumerator BattleLost()
    {
        LoadedBattle.Won = false;

        if (IsEndingBattleBlocked)
        {
            yield return FinalizeBattle();
            yield break;
        }

        ConfirmPopUp popUp = new();
        popUp.Initialize(Root, () => _gameManager.ClearSaveData(),
                "Oh... you lost, for now the only choice is to go to main menu, and try again. Do you want do it?");
        popUp.HideCancelButton();
        yield return null;
    }

    IEnumerator BattleWon()
    {
        LoadedBattle.Won = true;
        if (IsEndingBattleBlocked)
        {
            yield return FinalizeBattle();
            yield break;
        }

        VisualElement topPanel = Root.Q<VisualElement>("topPanel");
        topPanel.Clear();

        Label label = new("Battle won!");
        label.AddToClassList("battle__won-label");
        label.style.opacity = 0;
        DOTween.To(x => label.style.opacity = x, 0, 1, 0.5f);

        topPanel.Add(label);

        if (_gameManager.BattleNumber == 8)
        {
            ConfirmPopUp popUp = new();
            popUp.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0.5f));
            popUp.Initialize(Root, () => StartCoroutine(FinalizeBattle()),
                    "You won the game! I owe you a beer for winning this prototype. You can continue playing or you can try another element. Btw. let me know what you think about this experience!.");
            popUp.HideCancelButton();
            yield break;
        }

        yield return FinalizeBattle();
    }

    IEnumerator FinalizeBattle()
    {
        _gameManager.BattleNumber++; // TODO: hihihihihi
        // if entities die "at the same time" it triggers twice
        if (_battleFinalized) yield break;
        _battleFinalized = true;

        yield return new WaitForSeconds(3f);

        ClearAllEntities();

        OnBattleFinalized?.Invoke();
    }

    void ClearAllEntities()
    {
        Destroy(_obstacleInstance);

        PlayerEntities.Clear();
        OpponentEntities.Clear();
        foreach (Transform child in EntityHolder.transform)
        {
            child.transform.DOKill(child.transform);
            GameObject.Destroy(child.gameObject);
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Win Battle")]
    public void WinBattle()
    {
        List<BattleEntity> copy = new(OpponentEntities);
        foreach (BattleEntity be in copy)
        {
            StartCoroutine(be.Die());
        }
    }
    [ContextMenu("Alternative Win Battle")]
    public void WinBattleAlternative()
    {
        StartCoroutine(BattleWon());
    }
#endif

}
