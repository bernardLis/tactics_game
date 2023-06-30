using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleInitializer : MonoBehaviour
{
    GameManager _gameManager;
    BattleManager _battleManager;

    BattleCameraManager _battleCameraManager;
    BattleInputManager _battleInputManager;

    Transform _entityHolder;

    [SerializeField] Transform _playerSpawnPoint;
    [SerializeField] Transform _enemySpawnPoint;

    [SerializeField] GameObject _obstaclePrefab;
    GameObject _obstacleInstance;

    [SerializeField] GameObject _creatureSpawnerPrefab;

    Hero _playerHero;
    Hero _opponentHero;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;
        _battleCameraManager = _battleManager.GetComponent<BattleCameraManager>();
        _battleInputManager = _battleManager.GetComponent<BattleInputManager>();

        _entityHolder = _battleManager.EntityHolder;

        _battleCameraManager = Camera.main.GetComponentInParent<BattleCameraManager>();

        _playerHero = _gameManager.PlayerHero;
        _opponentHero = _gameManager.SelectedBattle.Opponent;

        StartCoroutine(BattleStartShow());
    }


    IEnumerator BattleStartShow()
    {
        _battleInputManager.enabled = false;

        yield return new WaitForSeconds(0.5f);

        _battleCameraManager.MoveCameraToDefaultPosition(5f);

        PlaceObstacle();

        yield return new WaitForSeconds(1f);

        GameObject playerSpawnerInstance = Instantiate(_creatureSpawnerPrefab, _playerSpawnPoint.position,
                Quaternion.identity);
        CreatureSpawner playerSpawner = playerSpawnerInstance.GetComponent<CreatureSpawner>();
        playerSpawner.SpawnHeroArmy(_playerHero, 1.5f);

        Vector3 oppPortalRotation = new(0, 180, 0);
        GameObject opponentSpawnerInstance = Instantiate(_creatureSpawnerPrefab, _enemySpawnPoint.position,
                 Quaternion.Euler(oppPortalRotation));
        CreatureSpawner opponentSpawner = opponentSpawnerInstance.GetComponent<CreatureSpawner>();
        opponentSpawner.SpawnHeroArmy(_opponentHero, 1.5f);

        yield return new WaitForSeconds(2f);

        List<BattleEntity> playerArmy = new(playerSpawner.SpawnedEntities);
        List<BattleEntity> opponentArmy = new(opponentSpawner.SpawnedEntities);

        playerSpawnerInstance.transform.DOScale(0, 0.5f).SetEase(Ease.InBack);
        opponentSpawnerInstance.transform.DOScale(0, 0.5f).SetEase(Ease.InBack);

        yield return new WaitForSeconds(1f);

        Destroy(playerSpawnerInstance);
        Destroy(opponentSpawnerInstance);

        _battleInputManager.enabled = true;

        _battleManager.Initialize(_playerHero, playerArmy, opponentArmy);
        yield return null;

    }


    void PlaceObstacle()
    {
        if (_obstacleInstance != null)
            Destroy(_obstacleInstance);

        //   if (Random.value > 0.5f) return; // 50/50 there is going to be an obstacle

        // between player and enemy
        float posX = _playerSpawnPoint.transform.position.x + (_enemySpawnPoint.transform.position.x - _playerSpawnPoint.transform.position.x) / 2;
        float posZ = _playerSpawnPoint.transform.position.z + (_enemySpawnPoint.transform.position.z - _playerSpawnPoint.transform.position.z) / 2;
        Vector3 pos = new Vector3(posX, 10, posZ);

        float sizeY = Random.Range(3, 10);
        float sizeX = Random.Range(10, 30);
        float sizeZ = Random.Range(1, 5);
        Vector3 size = new Vector3(sizeX, sizeY, sizeZ);

        Vector3 rot = new Vector3(0, Random.Range(-45, 45), 0);

        _obstacleInstance = Instantiate(_obstaclePrefab, pos, Quaternion.identity);
        _obstacleInstance.transform.localScale = size;
        _obstacleInstance.transform.Rotate(rot);
    }


}
