using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;

public class BattleInitializer : MonoBehaviour
{
    GameManager _gameManager;
    BattleManager _battleManager;

    BattleCameraManager _battleCameraManager;
    BattleInputManager _battleInputManager;
    PlayerArmyDeployer _playerArmyDeployer;

    Transform _entityHolder;

    Hero _playerHero;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;
        _battleCameraManager = _battleManager.GetComponent<BattleCameraManager>();
        _battleInputManager = _battleManager.GetComponent<BattleInputManager>();
        _playerArmyDeployer = _battleManager.GetComponent<PlayerArmyDeployer>();

        _entityHolder = _battleManager.EntityHolder;

        _battleCameraManager = Camera.main.GetComponentInParent<BattleCameraManager>();

        _playerHero = _gameManager.PlayerHero;

        StartCoroutine(BattleStartShow());
    }

    IEnumerator BattleStartShow()
    {
        _battleInputManager.enabled = false;

        yield return new WaitForSeconds(0.5f);

        _battleCameraManager.MoveCameraToDefaultPosition(3f);

        yield return new WaitForSeconds(1f);

        _battleManager.Initialize(_playerHero);
        GetComponent<BattleWaveManager>().Initialize();

        yield return new WaitForSeconds(2f);

        _playerArmyDeployer.Initialize();
        _battleInputManager.enabled = true;
    }

    // HERE: obstacle code
    [SerializeField] GameObject _obstaclePrefab;
    GameObject _obstacleInstance;
    void PlaceObstacle()
    {
        if (_obstacleInstance != null)
            Destroy(_obstacleInstance);

        // between player and enemy
        //   float posX = _playerSpawnPoint.transform.position.x + (_enemySpawnPoint.transform.position.x - _playerSpawnPoint.transform.position.x) / 2;
        //   float posZ = _playerSpawnPoint.transform.position.z + (_enemySpawnPoint.transform.position.z - _playerSpawnPoint.transform.position.z) / 2;
        Vector3 pos = new Vector3(Random.Range(-15, 15), 10, Random.Range(-15, 15));

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
