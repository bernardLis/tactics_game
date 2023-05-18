using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class BattleEndManager : MonoBehaviour
{
    const string _ussClassName = "battle__";
    const string _ussEvolutionInfoContainer = _ussClassName + "evolution-info-container";

    GameManager _gameManager;
    Camera _cam;
    BattleManager _battleManager;

    [SerializeField] Canvas _battleCanvas;

    [SerializeField] Shader _dissolveShader;

    VisualElement _root;

    VisualElement _infoContainer;
    VisualElement _entityElementContainer;

    EntityElement _entityElement;

    List<ArmyGroup> _playerArmy;


    ArmyGroup _currentArmyGroup;
    BattleEntity _currentEntity;
    BattleEntity _evolvedEntity;


    void Start()
    {

        _gameManager = GameManager.Instance;
        _cam = Camera.main;
        _battleManager = BattleManager.Instance;
        _root = _battleManager.Root;

        _battleManager.OnBattleFinalized += BeginEndBattleShow;
    }

    void BeginEndBattleShow()
    {
        _playerArmy = _gameManager.PlayerHero.Army;
        StartCoroutine(RunShow());
        StartCoroutine(RotateCameraAroundEntity());
    }

    IEnumerator RunShow()
    {
        _battleCanvas.enabled = false;
        GetComponent<BattleGrabManager>().enabled = false;
        GetComponent<BattleAbilityManager>().enabled = false;
        GetComponent<BattleEntityTooltipManager>().enabled = false;
        GetComponent<BattleInputManager>().enabled = false;

        Vector3 pos = new Vector3(9.5f, 0, 0);
        Vector3 rot = new Vector3(30, -90, 0);

        Camera.main.GetComponentInParent<BattleCameraManager>().MoveCameraTo(pos, rot, 5);

        _root.Q<VisualElement>("bottomPanel").style.display = DisplayStyle.None;
        _root.Q<VisualElement>("topPanel").style.display = DisplayStyle.None;


        foreach (ArmyGroup ag in _playerArmy)
        {
            _currentArmyGroup = ag;
            yield return ShowArmyGroup(ag);
        }
        ShowUI();

        // make it run or do some animation
        // show its stats
        // add kills that the army made
        // evolve it if it can
        // move to next entity
    }

    IEnumerator ShowArmyGroup(ArmyGroup ag)
    {
        // HERE: testing
        ag.KillCount = Random.Range(5, 10);

        _currentEntity = InstantiateEntity(ag.ArmyEntity);
        // does not wokr  _currentEntity.Animator.SetBool("Move", true); 
        // TODO: ideally there is a cool animation list that is going on

        yield return new WaitForSeconds(1f);
        _currentEntity.Animator.SetBool("Move", true);

        // HERE: you can make an element out of it, and every x call next method
        _infoContainer = new();
        _infoContainer.AddToClassList(_ussEvolutionInfoContainer);
        _root.Add(_infoContainer);

        _entityElementContainer = new();
        _entityElementContainer.style.flexDirection = FlexDirection.Row;
        _infoContainer.Add(_entityElementContainer);


        _entityElement = new(ag.ArmyEntity);
        _entityElementContainer.Add(_entityElement);
        _entityElement.style.opacity = 0;
        DOTween.To(x => _entityElement.style.opacity = x, 0, 1, 0.5f);

        yield return new WaitForSeconds(1f);

        _infoContainer.Add(new Label("Kills to evolve:"));

        IntVariable kills = ScriptableObject.CreateInstance<IntVariable>();
        kills.SetValue(ag.OldKillCount);

        IntVariable killToEvolve = ScriptableObject.CreateInstance<IntVariable>();
        killToEvolve.SetValue(ag.NumberOfKillsToEvolve());
        Color barColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        ResourceBarElement killBar = new(barColor, "Kills", kills, killToEvolve);
        killBar.style.width = 200;
        _infoContainer.Add(killBar);
        killBar.style.opacity = 0;
        DOTween.To(x => killBar.style.opacity = x, 0, 1, 0.5f);

        yield return new WaitForSeconds(1f);

        VisualElement killCountContainer = new();
        _infoContainer.Add(killCountContainer);

        killCountContainer.Add(new Label("Kills this battle:"));
        ChangingValueElement cv = new();
        cv.Initialize(ag.KillCount - ag.OldKillCount, 24);

        killCountContainer.Add(cv);

        yield return new WaitForSeconds(1f);
        int killChange = Mathf.Clamp(ag.KillCount - ag.OldKillCount, 0, ag.NumberOfKillsToEvolve());

        cv.ChangeAmount(ag.KillCount - ag.OldKillCount - killChange);

        kills.ApplyChange(killChange);

        yield return new WaitForSeconds(1f);

        if (ag.ShouldEvolve())
            yield return Evolve();

        // center camera on it
        while (true)
            yield return null;
    }

    IEnumerator Evolve()
    {
        Material mat = _currentEntity.GetComponentInChildren<Renderer>().material;
        Texture2D tex = mat.mainTexture as Texture2D;

        mat.shader = _dissolveShader;
        mat.SetTexture("_Base_Texture", tex);

        DOTween.To(x => mat.SetFloat("_Dissolve_Value", x), 0, 1, 5f);
        yield return new WaitForSeconds(1f);

        _currentArmyGroup.Evolve();

        _evolvedEntity = InstantiateEntity(_currentArmyGroup.ArmyEntity);
        _evolvedEntity.Collider.enabled = false;

        Material evolvedMat = _evolvedEntity.GetComponentInChildren<Renderer>().material;
        Texture2D evolvedTex = evolvedMat.mainTexture as Texture2D;

        evolvedMat.shader = _dissolveShader;
        evolvedMat.SetTexture("_Base_Texture", evolvedTex);

        mat.SetFloat("_Dissolve_Value", 1f);
        DOTween.To(x => evolvedMat.SetFloat("_Dissolve_Value", x), 1, 0, 5f)
                .OnComplete(() => _evolvedEntity.Animator.SetBool("Move", true));

        _infoContainer.Add(new Label("Evolved!"));
        //   EntityElement evolvedElement = new(_currentArmyGroup.ArmyEntity);
        _entityElement.SetValues(_currentArmyGroup.ArmyEntity);
        //   _entityElementContainer.Add(evolvedElement);
        //   evolvedElement.style.opacity = 0;
        //    DOTween.To(x => evolvedElement.style.opacity = x, 0, 1, 0.5f);

        yield return new WaitForSeconds(5f);

        yield return null;
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
