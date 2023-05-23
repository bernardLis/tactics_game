using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class BattleEndManager : MonoBehaviour
{
    const string _ussClassName = "battle__";
    const string _ussEvolutionInfoContainer = _ussClassName + "evolution-info-container";
    const string _ussEvolutionContinueButton = _ussClassName + "evolution-continue-button";

    GameManager _gameManager;
    Camera _cam;
    BattleCameraManager _cameraManager;
    BattleManager _battleManager;

    [SerializeField] Canvas _battleCanvas;

    [SerializeField] Shader _dissolveShader;

    VisualElement _root;

    VisualElement _infoContainer;
    ArmyEvolutionElement _evolutionElement;

    MyButton _continueButton;

    List<ArmyGroup> _playerArmy;

    int _currentArmyGroupIndex;
    BattleEntity _currentEntity;
    BattleEntity _evolvedEntity;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _cam = Camera.main;
        _cameraManager = _cam.GetComponentInParent<BattleCameraManager>();
        _battleManager = BattleManager.Instance;
        _root = _battleManager.Root;

        _battleManager.OnBattleFinalized += BeginEndBattleShow;

        _continueButton = new("CONTINUE", _ussEvolutionContinueButton, ContinueButtonClick);
        _root.Add(_continueButton);
        _continueButton.style.display = DisplayStyle.None;
    }

    void BeginEndBattleShow()
    {
        ShowUI();

        /*  // HERE: testing battle end UI
        _playerArmy = _gameManager.PlayerHero.Army;
        StartCoroutine(RunShow());
        StartCoroutine(RotateCameraAroundEntity());
        */
    }

    IEnumerator RunShow()
    {
        _battleCanvas.enabled = false;
        GetComponent<BattleGrabManager>().enabled = false;
        GetComponent<BattleAbilityManager>().enabled = false;
        GetComponent<BattleEntityTooltipManager>().enabled = false;
        GetComponent<BattleInputManager>().enabled = false;

        Vector3 pos = new Vector3(9.5f, 0, 0);
        Vector3 rot = new Vector3(30f, -90f, 0);

        _cameraManager.MoveCameraTo(pos, rot, 5);

        _root.Q<VisualElement>("bottomPanel").style.display = DisplayStyle.None;
        _root.Q<VisualElement>("topPanel").style.display = DisplayStyle.None;

        _infoContainer = new();
        _infoContainer.AddToClassList(_ussEvolutionInfoContainer);
        _root.Add(_infoContainer);

        _currentArmyGroupIndex = 0;
        yield return ShowArmyGroup(_playerArmy[_currentArmyGroupIndex]);
    }

    IEnumerator ShowArmyGroup(ArmyGroup ag)
    {
        _infoContainer.Clear();
        if (_currentEntity != null)
            Destroy(_currentEntity.gameObject);
        if (_evolvedEntity != null)
            Destroy(_evolvedEntity.gameObject);

        // HERE: testing
        ag.TotalKillCount = Random.Range(5, 10);

        _currentEntity = InstantiateEntity(ag.ArmyEntity);

        yield return new WaitForSeconds(1f);
        // TODO: ideally there is a cool animation list that is going on
        _currentEntity.Animator.SetBool("Move", true);

        _evolutionElement = new(ag);
        _infoContainer.Add(_evolutionElement);
        _evolutionElement.style.opacity = 0;
        DOTween.To(x => _evolutionElement.style.opacity = x, 0, 1, 0.5f);

        yield return new WaitForSeconds(0.5f);
        _evolutionElement.ShowKillsThisBattle();

        yield return new WaitForSeconds(0.5f);
        _evolutionElement.ShowKillsToEvolveBar();

        yield return new WaitForSeconds(0.5f);
        _evolutionElement.AddKills();

        yield return new WaitForSeconds(0.5f);
        yield return HandleEvolution(ag);

        yield return new WaitForSeconds(0.5f);
        ShowContinueButton();
    }

    IEnumerator HandleEvolution(ArmyGroup ag)
    {
        if (!ag.ShouldEvolve()) yield break;

        yield return Evolve();
        yield return new WaitForSeconds(0.5f);

        _evolutionElement.ResetKillsToEvolveBar();
        yield return new WaitForSeconds(0.5f);
        _evolutionElement.AddKills();

        yield return new WaitForSeconds(0.5f);
        yield return HandleEvolution(ag); // it should go in circles in case of multiple evolutions
    }

    IEnumerator Evolve()
    {
        _currentEntity.DisplayFloatingText("Evolving!", Color.magenta);

        Material mat = _currentEntity.GetComponentInChildren<Renderer>().material;
        Texture2D tex = mat.mainTexture as Texture2D;
        mat.shader = _dissolveShader;
        mat.SetTexture("_Base_Texture", tex);
        DOTween.To(x => mat.SetFloat("_Dissolve_Value", x), 0, 1, 5f);
        yield return new WaitForSeconds(1f);

        _playerArmy[_currentArmyGroupIndex].Evolve();
        _evolutionElement.Evolve(_playerArmy[_currentArmyGroupIndex].ArmyEntity);

        _evolvedEntity = InstantiateEntity(_playerArmy[_currentArmyGroupIndex].ArmyEntity);
        _evolvedEntity.Collider.enabled = false;

        Material evolvedMat = _evolvedEntity.GetComponentInChildren<Renderer>().material;
        Texture2D evolvedTex = evolvedMat.mainTexture as Texture2D;
        evolvedMat.shader = _dissolveShader;
        evolvedMat.SetTexture("_Base_Texture", evolvedTex);
        mat.SetFloat("_Dissolve_Value", 1f);
        DOTween.To(x => evolvedMat.SetFloat("_Dissolve_Value", x), 1, 0, 5f)
                .OnComplete(() => _evolvedEntity.Animator.SetBool("Move", true));

        yield return new WaitForSeconds(1f);
        if (_currentEntity != null)
            Destroy(_currentEntity.gameObject);
        _currentEntity = _evolvedEntity;
    }

    void ShowContinueButton()
    {
        _continueButton.SetEnabled(true);
        _continueButton.style.display = DisplayStyle.Flex;
        _continueButton.style.opacity = 0;
        DOTween.To(x => _continueButton.style.opacity = x, 0, 1, 0.5f);
    }

    void ContinueButtonClick()
    {
        _continueButton.SetEnabled(false);
        DOTween.To(x => _continueButton.style.opacity = x, 1, 0, 0.5f)
            .OnComplete(() => _continueButton.style.display = DisplayStyle.Flex);

        _currentArmyGroupIndex++;
        if (_currentArmyGroupIndex >= _playerArmy.Count)
        {
            ShowUI();
            return;
        }

        StartCoroutine(ShowArmyGroup(_playerArmy[_currentArmyGroupIndex]));
    }

    BattleEntity InstantiateEntity(ArmyEntity armyEntity)
    {
        // for each of player army, I want to spawn one entity
        ArmyEntity entityInstance = Instantiate(armyEntity);
        entityInstance.HeroInfluence(_gameManager.PlayerHero);

        Vector3 pos = Vector3.zero;
        pos.y = 1f;
        GameObject instance = Instantiate(armyEntity.Prefab, pos, Quaternion.identity);
        BattleEntity be = instance.GetComponent<BattleEntity>();
        List<BattleEntity> emptyList = new();
        be.Initialize(true, entityInstance, ref emptyList);
        be.StopRunEntityCoroutine();

        return be;
    }

    IEnumerator RotateCameraAroundEntity()
    {
        while (true)
        {
            if (_currentEntity != null)
            {
                _cam.transform.LookAt(_currentEntity.transform.position);
                _cam.transform.parent.RotateAround(_currentEntity.transform.position,
                        Vector3.up, -0.5f * Time.deltaTime);
            }
            yield return null;
        }
    }

    void ShowUI()
    {
        StopAllCoroutines();
        if (_gameManager.PlayerHero != null && _battleManager.LoadedBattle.Won)
        {
            BattleResult r = new(_root);
        }
    }
}
