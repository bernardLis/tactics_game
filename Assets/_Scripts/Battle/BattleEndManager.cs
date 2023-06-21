using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using Random = UnityEngine.Random;

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
    CreatureEvolutionElement _evolutionElement;

    MyButton _continueButton;

    List<Creature> _playerArmy;

    int _currentArmyGroupIndex;
    List<BattleEntity> _currentEntities = new();
    List<BattleEntity> _evolvedEntities = new();

    public BattleResultElement BattleResult { get; private set; }

    public event Action OnBattleResultShown;
    void Start()
    {
        _gameManager = GameManager.Instance;
        _cam = Camera.main;
        _cameraManager = _cam.GetComponentInParent<BattleCameraManager>();
        _battleManager = BattleManager.Instance;
        _root = _battleManager.Root;

        _battleManager.OnBattleFinalized += BeginEndBattleShow;

        // _continueButton = new("CONTINUE", _ussEvolutionContinueButton, ContinueButtonClick);
        //      _root.Add(_continueButton);
        //        _continueButton.style.display = DisplayStyle.None;
    }

    void BeginEndBattleShow()
    {
        Debug.Log($"Begin End Battle Show");

        _root.Q<VisualElement>("bottomPanel").Clear();
        _root.Q<VisualElement>("topPanel").Clear();

        ShowUI();
        /* TODO: pickup testing
        _playerArmy = _gameManager.PlayerHero.Army;
        StartCoroutine(RunShow());
        StartCoroutine(RotateCameraAroundEntity());
*/
    }
    /*
        IEnumerator RunShow()
        {
            _battleCanvas.enabled = false;
            GetComponent<BattleGrabManager>().enabled = false;
            GetComponent<BattleAbilityManager>().enabled = false;
            GetComponent<BattleEntityTooltipManager>().enabled = false;
            GetComponent<BattleInputManager>().enabled = false;

            Vector3 pos = new Vector3(14f, 0, 0); // CAM
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

        void ClearEntities()
        {
            for (int i = _currentEntities.Count - 1; i >= 0; i--)
            {
                _currentEntities[i].Animator.DOKill();
                Destroy(_currentEntities[i].gameObject);
            }

            for (int i = _evolvedEntities.Count - 1; i >= 0; i--)
            {
                _evolvedEntities[i].Animator.DOKill();
                Destroy(_evolvedEntities[i].gameObject);
            }

            _currentEntities.Clear();

            _evolvedEntities.Clear();
        }

        IEnumerator ShowArmyGroup(ArmyGroup ag)
        {
            _infoContainer.Clear();
            ClearEntities();

            for (int i = 0; i < ag.NumberOfUnits; i++)
            {
                Vector3 pos = new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
                BattleEntity be = InstantiateEntity(ag.Creature, pos);
                _currentEntities.Add(be);
            }

            yield return new WaitForSeconds(1f);
            // TODO: ideally there is a cool animation list that is going on
            foreach (BattleEntity be in _currentEntities)
                be.Animator.SetBool("Move", true);

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

            Evolve();
            yield return new WaitForSeconds(0.5f);

            _evolutionElement.ResetKillsToEvolveBar();
            yield return new WaitForSeconds(0.5f);
            _evolutionElement.AddKills();

            yield return new WaitForSeconds(0.5f);
            yield return HandleEvolution(ag); // it should go in circles in case of multiple evolutions
        }

        void Evolve()
        {
            _playerArmy[_currentArmyGroupIndex].Evolve();
            _evolutionElement.Evolve(_playerArmy[_currentArmyGroupIndex].Creature);

            foreach (BattleEntity be in _currentEntities)
            {
                _currentEntities[0].DisplayFloatingText("Evolving!", Color.magenta);
                Material mat = be.GetComponentInChildren<Renderer>().material;
                Texture2D tex = mat.mainTexture as Texture2D;
                mat.shader = _dissolveShader;
                mat.SetTexture("_Base_Texture", tex);
                DOTween.To(x => mat.SetFloat("_Dissolve_Value", x), 0, 1, 5f)
                        .OnComplete(() => be.gameObject.SetActive(false));

                BattleEntity evolvedEntity = InstantiateEntity(_playerArmy[_currentArmyGroupIndex].Creature,
                                                                 be.transform.position);
                _evolvedEntities.Add(evolvedEntity);
                evolvedEntity.Collider.enabled = false;

                Material evolvedMat = evolvedEntity.GetComponentInChildren<Renderer>().material;
                Texture2D evolvedTex = evolvedMat.mainTexture as Texture2D;
                evolvedMat.shader = _dissolveShader;
                evolvedMat.SetTexture("_Base_Texture", evolvedTex);
                mat.SetFloat("_Dissolve_Value", 1f);
                DOTween.To(x => evolvedMat.SetFloat("_Dissolve_Value", x), 1, 0, 5f)
                        .OnComplete(() => evolvedEntity.Animator.SetBool("Move", true));
            }
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
                _gameManager.PlayerHero.TryJoiningArmies();
                ShowUI();
                return;
            }

            StartCoroutine(ShowArmyGroup(_playerArmy[_currentArmyGroupIndex]));
        }

        BattleEntity InstantiateEntity(Creature armyEntity, Vector3 pos)
        {
            // for each of player army, I want to spawn one entity
            Creature entityInstance = Instantiate(armyEntity);
            entityInstance.HeroInfluence(_gameManager.PlayerHero);

            GameObject instance = Instantiate(armyEntity.Prefab, pos, Quaternion.identity);
            BattleEntity be = instance.GetComponent<BattleEntity>();
            be.SetDead();
            List<BattleEntity> emptyList = new();
            be.Initialize(0, entityInstance, ref emptyList);
            be.transform.DODynamicLookAt(_cam.transform.position, 0.5f, AxisConstraint.Y);

            return be;
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
        */

    void ShowUI()
    {
        _cameraManager.MoveCameraTo(Vector3.zero, Vector3.zero, 5);

        StopAllCoroutines();
        if (_gameManager.PlayerHero != null && _battleManager.LoadedBattle.Won)
        {
            BattleResult = new(_root);
            OnBattleResultShown?.Invoke();
        }
    }
}
