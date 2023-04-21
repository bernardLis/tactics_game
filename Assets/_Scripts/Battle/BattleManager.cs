using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class BattleManager : MonoBehaviour
{
    GameManager _gameManager;
    Battle _loadedBattle;

    VisualElement _root;

    [SerializeField] Transform _entityHolder;

    // skybox rotation https://forum.unity.com/threads/rotate-a-skybox.130639/
    int _rotationProperty;
    float _initRot;
    Material _skyMat;
    [SerializeField] float _skyboxRotationSpeed = 0.2f;

    [SerializeField] TextMeshProUGUI _textMesh; // HERE: something smarter

    [SerializeField] Material _playerMaterial;
    [SerializeField] Material _enemyMaterial;

    int _initialPlayerEntityCount;
    int _initialEnemyEntityCount;

    [SerializeField] GameObject _playerSpawnPoint;
    [SerializeField] GameObject _enemySpawnPoint;

    Hero _opponent;

    public List<BattleEntity> PlayerEntities = new();
    public List<BattleEntity> EnemyEntities = new();

    void Start()
    {
        _gameManager = GameManager.Instance;
        _loadedBattle = _gameManager.SelectedBattle;

        _root = GetComponent<UIDocument>().rootVisualElement;

        _initialEnemyEntityCount = _loadedBattle.GetTotalNumberOfEnemies();
        _initialPlayerEntityCount = _gameManager.PlayerHero.GetTotalNumberOfArmyEntities();

        _rotationProperty = Shader.PropertyToID("_Rotation");
        _skyMat = RenderSettings.skybox;
        _initRot = _skyMat.GetFloat(_rotationProperty);

        _textMesh.text = $"{_initialPlayerEntityCount} : {_initialEnemyEntityCount}";

        _opponent = ScriptableObject.CreateInstance<Hero>();
        _opponent.CreateRandom(1);

        foreach (ArmyGroup ag in _gameManager.PlayerHero.Army)
            InstantiatePlayer(ag.ArmyEntity, ag.EntityCount);
        foreach (ArmyGroup ag in _loadedBattle.Army)
            InstantiateEnemy(ag.ArmyEntity, ag.EntityCount);

        _gameManager.ToggleTimer(true);
    }

    void Update() => _skyMat.SetFloat(_rotationProperty, Time.time * _skyboxRotationSpeed);

    void InstantiatePlayer(ArmyEntity entity, int count)
    {
        ArmyEntity entityInstance = Instantiate(entity);
        entityInstance.HeroInfluence(_gameManager.PlayerHero);

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = _playerSpawnPoint.transform.position + new Vector3(Random.Range(-5, 5), Random.Range(-5, 5));
            GameObject instance = Instantiate(entity.Prefab, pos, Quaternion.identity);
            instance.layer = 8;
            instance.transform.parent = _entityHolder;
            BattleEntity be = instance.GetComponent<BattleEntity>();
            be.Initialize(_playerMaterial, _gameManager.PlayerHero, entityInstance, ref EnemyEntities);
            PlayerEntities.Add(be);
            be.OnDeath += OnPlayerDeath;
        }
    }

    void InstantiateEnemy(ArmyEntity entity, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = _enemySpawnPoint.transform.position + new Vector3(Random.Range(-5, 5), Random.Range(-5, 5));
            GameObject instance = Instantiate(entity.Prefab, pos, Quaternion.identity);
            instance.transform.parent = _entityHolder;
            BattleEntity be = instance.GetComponent<BattleEntity>();
            be.Initialize(_enemyMaterial, _opponent, entity, ref PlayerEntities);
            EnemyEntities.Add(be);
            be.OnDeath += OnEnemyDeath;
        }
    }

    void OnPlayerDeath(BattleEntity be)
    {
        PlayerEntities.Remove(be);
        _textMesh.text = $"{PlayerEntities.Count} : {EnemyEntities.Count}";

        if (PlayerEntities.Count == 0)
            BattleLost();
    }

    void OnEnemyDeath(BattleEntity be)
    {
        EnemyEntities.Remove(be);
        _textMesh.text = $"{PlayerEntities.Count} : {EnemyEntities.Count}";

        if (EnemyEntities.Count == 0)
            BattleWon();
    }

    void BattleLost()
    {
        StartCoroutine(FinalizeBattle(EnemyEntities));
    }

    void BattleWon()
    {
        _loadedBattle.Won = true;
        StartCoroutine(FinalizeBattle(PlayerEntities));
    }

    IEnumerator FinalizeBattle(List<BattleEntity> entities)
    {
        yield return new WaitForSeconds(2f);
        BattleResult r = new(_root, _loadedBattle, entities);
    }

#if UNITY_EDITOR
    [ContextMenu("Win Battle")]
    void WinBattle()
    {
        BattleWon();
    }
#endif
}
