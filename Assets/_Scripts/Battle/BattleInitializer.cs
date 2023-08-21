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
    BattleDeploymentManager _battleDeploymentManager;

    Hero _playerHero;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;
        _battleCameraManager = _battleManager.GetComponent<BattleCameraManager>();
        _battleInputManager = _battleManager.GetComponent<BattleInputManager>();
        _battleDeploymentManager = _battleManager.GetComponent<BattleDeploymentManager>();

        _battleCameraManager = BattleCameraManager.Instance;

        _playerHero = _gameManager.PlayerHero;

        StartCoroutine(BattleStartShow());
    }

    IEnumerator BattleStartShow()
    {
        _battleInputManager.enabled = false;

        yield return new WaitForSeconds(0.5f);

        _battleCameraManager.MoveCameraToDefaultPosition(3f);

        yield return new WaitForSeconds(1f);

        _battleManager.Initialize(_playerHero, Vector3.zero);
        GetComponent<BattleWaveManager>().Initialize();

        yield return new WaitForSeconds(2f);

        _battleDeploymentManager.HandlePlayerArmyDeployment(_playerHero.CreatureArmy);
        _battleInputManager.enabled = true;
    }
}
