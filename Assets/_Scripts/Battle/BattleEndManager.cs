using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using Random = UnityEngine.Random;
using Cursor = UnityEngine.Cursor;

public class BattleEndManager : MonoBehaviour
{
    const string _ussClassName = "battle__";
    const string _ussEvolutionInfoContainer = _ussClassName + "evolution-info-container";
    const string _ussEvolutionContinueButton = _ussClassName + "evolution-continue-button";

    GameManager _gameManager;
    Camera _cam;
    BattleCameraManager _cameraManager;
    BattleManager _battleManager;

    [SerializeField] Shader _dissolveShader;

    VisualElement _root;

    VisualElement _infoContainer;
    CreatureCardEvolution _evolutionElement;

    MyButton _continueButton;

    BattleEntity _currentEntity;
    BattleEntity _evolvedEntity;

    public BattleResultElement BattleResult { get; private set; }

    Creature _creatureToEvolve;

    IEnumerator _cameraRotationCoroutine;

    public event Action OnBattleResultShown;
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
        Debug.Log($"Begin End Battle Show");

        DisableBattle();
        ShowUI();

        foreach (Creature c in _gameManager.PlayerHero.Army)
        {
            c.OnLevelUp += () =>
            {
                if (c.ShouldEvolve()) Evolve(c);
            };
        }
    }

    void DisableBattle()
    {
        GetComponent<BattleGrabManager>().enabled = false;
        GetComponent<BattleAbilityManager>().enabled = false;
        GetComponent<BattleEntityTooltipManager>().enabled = false;
        GetComponent<BattleInputManager>().enabled = false;

        _root.Q<VisualElement>("infoPanel").style.display = DisplayStyle.None;
        _root.Q<VisualElement>("bottomPanel").Clear();
        _root.Q<VisualElement>("topPanel").Clear();
        _root.Q<VisualElement>("bottomPanel").style.display = DisplayStyle.None;
        _root.Q<VisualElement>("topPanel").style.display = DisplayStyle.None;

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    void ShowUI()
    {
        _cameraManager.MoveCameraTo(Vector3.zero, Vector3.zero, 5);
        VFXCameraManager.Instance.gameObject.SetActive(true);

        StopAllCoroutines();
        if (_gameManager.PlayerHero != null && _battleManager.LoadedBattle.Won)
        {
            BattleResult = new(_root);
            OnBattleResultShown?.Invoke();
        }
    }

    void Evolve(Creature creature)
    {
        Debug.Log($"Evolving {creature.name}");
        BattleResult.Hide();
        _creatureToEvolve = creature;
        StartCoroutine(RunShow());
    }

    IEnumerator RunShow()
    {
        Vector3 pos = new Vector3(14f, 0, 0); // CAM
        Vector3 rot = new Vector3(30f, -90f, 0);
        _cameraManager.MoveCameraTo(pos, rot, 5);
        _cameraRotationCoroutine = RotateCameraAroundEntity();
        StartCoroutine(_cameraRotationCoroutine);

        _infoContainer = new();
        _infoContainer.AddToClassList(_ussEvolutionInfoContainer);
        _infoContainer.style.display = DisplayStyle.None;
        _root.Add(_infoContainer);

        yield return ShowCreature();
    }

    IEnumerator ShowCreature()
    {
        BattleEntity be = InstantiateEntity(_creatureToEvolve, Vector3.zero);
        _currentEntity = be;

        yield return new WaitForSeconds(1f);
        // TODO: ideally there is a cool animation list that is going on
        be.Animator.SetBool("Move", true);

        _evolutionElement = new(_creatureToEvolve);
        _infoContainer.Add(_evolutionElement);
        _infoContainer.style.opacity = 0;
        _infoContainer.style.display = DisplayStyle.Flex;
        DOTween.To(x => _infoContainer.style.opacity = x, 0, 1, 0.5f);

        yield return new WaitForSeconds(2f);
        yield return EvolutionShow();

        yield return new WaitForSeconds(0.5f);
        ShowContinueButton();
    }

    IEnumerator EvolutionShow()
    {
        // need to swap the creature in the players army
        int index = _gameManager.PlayerHero.Army.IndexOf(_creatureToEvolve);
        Creature evolvedCreature = Instantiate(_gameManager.HeroDatabase.GetCreatureById(_creatureToEvolve.EvolvedCreature.Id));
        evolvedCreature.Level = _creatureToEvolve.Level;
        _gameManager.PlayerHero.Army[index] = evolvedCreature;

        _evolutionElement.Evolve(evolvedCreature);

        _currentEntity.DisplayFloatingText("Evolving!", Color.magenta);
        Material mat = _currentEntity.GetComponentInChildren<Renderer>().material;
        Texture2D tex = mat.mainTexture as Texture2D;
        mat.shader = _dissolveShader;
        mat.SetTexture("_Base_Texture", tex);
        DOTween.To(x => mat.SetFloat("_Dissolve_Value", x), 0, 1, 5f)
                .OnComplete(() => _currentEntity.gameObject.SetActive(false));

        yield return new WaitForSeconds(2f);

        _evolvedEntity = InstantiateEntity(_creatureToEvolve.EvolvedCreature,
                                                         _currentEntity.transform.position);
        _evolvedEntity.Collider.enabled = false;

        Material evolvedMat = _evolvedEntity.GetComponentInChildren<Renderer>().material;
        Texture2D evolvedTex = evolvedMat.mainTexture as Texture2D;
        evolvedMat.shader = _dissolveShader;
        evolvedMat.SetTexture("_Base_Texture", evolvedTex);
        mat.SetFloat("_Dissolve_Value", 1f);
        DOTween.To(x => evolvedMat.SetFloat("_Dissolve_Value", x), 1, 0, 5f)
                .OnComplete(() => _evolvedEntity.Animator.SetBool("Move", true));
    }

    BattleEntity InstantiateEntity(Creature creature, Vector3 pos)
    {
        Creature entityInstance = Instantiate(creature);

        GameObject instance = Instantiate(creature.Prefab, pos, Quaternion.identity);
        BattleEntity be = instance.GetComponent<BattleEntity>();
        be.SetDead();
        List<BattleEntity> emptyList = new();
        be.InitializeCreature(creature);
        be.transform.DODynamicLookAt(_cam.transform.position, 0.5f, AxisConstraint.Y);

        return be;
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
        DOTween.To(x => _infoContainer.style.opacity = x, 1, 0, 0.5f)
            .OnComplete(() => _infoContainer.RemoveFromHierarchy());

        DOTween.To(x => _continueButton.style.opacity = x, 1, 0, 0.5f)
            .OnComplete(() =>
            {
                _continueButton.style.display = DisplayStyle.None;
                BattleResult.Show();
                CleanUp();
                StopCoroutine(_cameraRotationCoroutine);
            });
    }

    void CleanUp()
    {
        _infoContainer.Clear();

        _currentEntity.Animator.DOKill();
        Destroy(_currentEntity.gameObject);
        _evolvedEntity.Animator.DOKill();
        Destroy(_evolvedEntity.gameObject);
    }

    IEnumerator RotateCameraAroundEntity()
    {
        while (true)
        {
            _cam.transform.LookAt(Vector3.zero);
            _cam.transform.parent.RotateAround(Vector3.zero, Vector3.up, -0.5f * Time.deltaTime);
            yield return null;
        }
    }
}
