using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class BattleEndManager : MonoBehaviour
{
    GameManager _gameManager;
    Camera _cam;
    BattleManager _battleManager;

    [SerializeField] Canvas _battleCanvas;

    VisualElement _root;

    List<ArmyGroup> _playerArmy;

    BattleEntity _currentEntity;

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
        ag.KillCount = Random.Range(0, 10);

        // for each of player army, I want to spawn one entity
        ArmyEntity entityInstance = Instantiate(ag.ArmyEntity);
        entityInstance.HeroInfluence(_gameManager.PlayerHero);

        Vector3 pos = Vector3.zero;
        pos.y = 1f;
        GameObject instance = Instantiate(ag.ArmyEntity.Prefab, pos, Quaternion.identity);
        _currentEntity = instance.GetComponent<BattleEntity>();
        List<BattleEntity> emptyList = new();
        _currentEntity.Initialize(true, entityInstance, ref emptyList);
        _currentEntity.StopRunEntityCoroutine();
        // does not wokr  _currentEntity.Animator.SetBool("Move", true); 
        // TODO: ideally there is a cool animation list that is going on

        yield return new WaitForSeconds(1f);
        _currentEntity.Animator.SetBool("Move", true);

        // HERE: you can make an element out of it, and every x call next method
        VisualElement container = new();
        container.style.position = Position.Absolute;
        container.style.alignItems = Align.Center;
        container.style.alignContent = Align.Center;
        container.style.justifyContent = Justify.FlexStart;
        container.style.left = 50;
        container.style.top = 20;
        container.style.height = Length.Percent(100);
        container.style.minWidth = 300;

        _root.Add(container);

        EntityElement el = new(ag.ArmyEntity);
        el.style.backgroundColor = Color.white;
        container.Add(el);
        el.style.opacity = 0;
        DOTween.To(x => el.style.opacity = x, 0, 1, 0.5f);

        yield return new WaitForSeconds(2f);

        container.Add(new Label("Kills to evolve:"));

        IntVariable kills = ScriptableObject.CreateInstance<IntVariable>();
        kills.SetValue(ag.OldKillCount);

        IntVariable killToEvolve = ScriptableObject.CreateInstance<IntVariable>();
        killToEvolve.SetValue(ag.NumberOfKillsToEvolve());

        ResourceBarElement killBar = new(Color.white, "Kills", kills, killToEvolve);
        killBar.style.width = 200;
        container.Add(killBar);
        killBar.style.opacity = 0;
        DOTween.To(x => killBar.style.opacity = x, 0, 1, 0.5f);

        yield return new WaitForSeconds(2f);

        VisualElement killCountContainer = new();
        container.Add(killCountContainer);

        killCountContainer.Add(new Label("Kills this battle:"));
        ChangingValueElement cv = new();
        cv.Initialize(ag.KillCount - ag.OldKillCount, 24);

        killCountContainer.Add(cv);

        yield return new WaitForSeconds(2f);
        int killChange = Mathf.Clamp(ag.KillCount - ag.OldKillCount, 0, ag.NumberOfKillsToEvolve());

        cv.ChangeAmount(killChange);

        kills.ApplyChange(killChange);

        yield return new WaitForSeconds(2f);

        if (ag.ShouldEvolve())
            yield return Evolve();


        // center camera on it
        while (true)
            yield return null;
    }

    IEnumerator Evolve()
    {
        // fade out the 
        yield return null;
    }

    IEnumerator RotateCameraAroundEntity()
    {
        while (true)
        {
            if (_currentEntity != null)
            {
                _cam.transform.LookAt(_currentEntity.transform.position);
                _cam.transform.parent.RotateAround(_currentEntity.transform.position,
                        Vector3.up, 0.5f * Time.deltaTime);
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
