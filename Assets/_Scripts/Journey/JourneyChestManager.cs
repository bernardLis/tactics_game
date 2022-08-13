using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class JourneyChestManager : Singleton<JourneyChestManager>
{
    // global
    GameManager _gameManager;
    RunManager _runManger;

    ResponsiveCamera _cam;

    [Header("Unity Setup")]
    [SerializeField] JourneyChest[] _chests;
    [SerializeField] GameObject _chestPrefab;
    [SerializeField] GameObject _cursorPrefab;
    [SerializeField] GameObject _obolPrefab;
    [SerializeField] JourneyNodeReward _reward;

    // UI
    VisualElement _difficultyAdjustWrapper;
    Button _increaseDifficultyButton;
    Button _decreaseDifficultyButton;
    Label _playingForGoldLabel;

    VisualElement _resultWrapper;
    Label _result;
    VisualElement _rewardWrapper;
    Label _obolAmountLabel;
    Button _backToJourneyButton;

    // mini game
    int _numberOfChests = 3;
    int _goldReward = 3;
    int _cursorDelay = 500;

    List<JourneyChestBehaviour> _chestBehaviours = new();
    List<JourneyChest> _availableChests; // to make sure they are all visiually differnet
    List<JourneyChestBehaviour> _allowedChests;
    GameObject _cursor;
    public JourneyChestBehaviour CurrentCursorChest { get; private set; }
    JourneyChestBehaviour _selectedChest;

    protected override void Awake()
    {
        base.Awake();

        // global
        _gameManager = GameManager.Instance;
        _runManger = RunManager.Instance;
        _cam = Camera.main.GetComponent<ResponsiveCamera>();

        // UI
        var root = GetComponent<UIDocument>().rootVisualElement;

        _difficultyAdjustWrapper = root.Q<VisualElement>("difficultyAdjustWrapper");
        _playingForGoldLabel = root.Q<Label>("playingForGold");
        _increaseDifficultyButton = root.Q<Button>("increaseDifficulty");
        _decreaseDifficultyButton = root.Q<Button>("decreaseDifficulty");

        _increaseDifficultyButton.clickable.clicked += IncreaseDifficulty;
        _decreaseDifficultyButton.clickable.clicked += DecreaseDifficulty;

        _resultWrapper = root.Q<VisualElement>("resultWrapper");
        _result = root.Q<Label>("result");
        _rewardWrapper = root.Q<VisualElement>("rewardWrapper");
        _obolAmountLabel = root.Q<Label>("obolAmount");
        _backToJourneyButton = root.Q<Button>("backToJourney");

        _backToJourneyButton.clickable.clicked += BackToJourney;

        // other
        _availableChests = new(_chests);
    }

    void Start()
    {
        int x = -5;
        for (int i = 0; i < _numberOfChests; i++)
        {
            GameObject obj = Instantiate(_chestPrefab, new Vector3(x, 2), Quaternion.identity); // TOOD: magic 2
            JourneyChest chestScriptable = Instantiate(_availableChests[Random.Range(0, _availableChests.Count)]);

            JourneyChestBehaviour chestBehaviour = obj.GetComponent<JourneyChestBehaviour>();
            chestBehaviour.Chest = chestScriptable;
            _chestBehaviours.Add(chestBehaviour);

            _availableChests.Remove(chestScriptable); // making sure chests are different
            chestScriptable.Initialize(obj);
            x += 5;
        }
        _cam.SetOrthoSize();

        StartGame();
    }

    async void StartGame()
    {
        _allowedChests = new(_chestBehaviours);
        _cursor = Instantiate(_cursorPrefab, transform.position, Quaternion.identity);

        while (_selectedChest == null)
        {
            MoveCursor();
            await Task.Delay(_cursorDelay);
        }
    }

    public void AddForbiddenChest(JourneyChestBehaviour _chest)
    {
        _allowedChests.Remove(_chest);
    }

    public void RemoveForbiddenChest(JourneyChestBehaviour _chest)
    {
        _allowedChests.Add(_chest);
    }

    void MoveCursor()
    {
        CurrentCursorChest = _allowedChests[Random.Range(0, _allowedChests.Count)];
        Vector3 pos = new Vector3(CurrentCursorChest.transform.position.x, CurrentCursorChest.transform.position.y + 0.5f);
        _cursor.transform.position = pos;
    }

    public void ChestWasSelected(JourneyChestBehaviour _chestBehaviour)
    {
        _selectedChest = _chestBehaviour;

        foreach (JourneyChestBehaviour c in _chestBehaviours)
            c.WasChestSelected = true;

        JourneyNodeReward rewardInstance = Instantiate(_reward);
        if (_chestBehaviour == CurrentCursorChest)
        {
            _result.text = "Winner winner chicken dinner";
            _result.style.color = Color.white;
            _chestBehaviour.Chest.Won();
            rewardInstance.Gold = _goldReward;
        }
        else
        {
            _result.text = "Better luck next time";
            _result.style.color = Color.red;
            _chestBehaviour.Chest.Lost();
            rewardInstance.Gold = 0;
        }

        _obolAmountLabel.text = "0";

        _runManger.SetNodeReward(rewardInstance);
        StartCoroutine(ChangeGoldCoroutine(rewardInstance.Gold));

        _difficultyAdjustWrapper.style.display = DisplayStyle.None;

        _resultWrapper.style.opacity = 0;
        _resultWrapper.style.display = DisplayStyle.Flex;
        DOTween.To(() => _resultWrapper.style.opacity.value, x => _resultWrapper.style.opacity = x, 1f, 1f)
            .SetEase(Ease.InSine);
    }

    IEnumerator ChangeGoldCoroutine(int _amount)
    {
        int current = 0;
        for (int i = 0; i < _amount; i++)
        {
            current++;
            SpawnGold();
            yield return new WaitForSeconds(0.2f);
            _obolAmountLabel.text = current.ToString();
        }
    }

    void SpawnGold()
    {
        Vector3 spawnPos = _selectedChest.gameObject.transform.position;
        Vector3 randomVector = new Vector3(Random.Range(0f, 3f), Random.Range(0f, 3f));

        GameObject g = Instantiate(_obolPrefab, spawnPos, Quaternion.identity);
        g.transform.DOJump(spawnPos + randomVector, 1f, 1, 1f);

        Destroy(g, 1.1f);
    }

    /* BUTTONS */
    void IncreaseDifficulty()
    {
        if (_goldReward == 10)
            return;

        _goldReward++;
        _playingForGoldLabel.text = _goldReward.ToString();
        // we add a chest or decrease delay every second obol, starting with adding a chest
        if (_goldReward % 2 == 0)
            AddChest();
        else
            DecreaseDelay();
    }

    void DecreaseDifficulty()
    {
        if (_goldReward == 1)
            return;

        _goldReward--;
        _playingForGoldLabel.text = _goldReward.ToString();
        // we add a chest or decrease delay every second obol, starting with adding a chest
        if (_goldReward % 2 == 0)
            IncreaseDelay();
        else
            RemoveChest();
    }

    void AddChest()
    {
        // start over if you use all visuals
        if (_availableChests.Count == 0)
            _availableChests = new(_chests);

        JourneyChestBehaviour lastChest = _chestBehaviours[_chestBehaviours.Count - 1];
        Vector3 pos = new Vector3(lastChest.transform.position.x + 5f, lastChest.transform.position.y);

        GameObject obj = Instantiate(_chestPrefab, pos, Quaternion.identity);
        JourneyChest chestScriptable = Instantiate(_availableChests[Random.Range(0, _availableChests.Count)]);

        JourneyChestBehaviour chestBehaviour = obj.GetComponent<JourneyChestBehaviour>();
        chestBehaviour.Chest = chestScriptable;
        _chestBehaviours.Add(chestBehaviour);
        _allowedChests.Add(chestBehaviour);

        _availableChests.Remove(chestScriptable); // making sure chests are different
        chestScriptable.Initialize(obj);

        _cam.SetOrthoSize();
    }

    void RemoveChest()
    {
        JourneyChestBehaviour chestToRemove = _chestBehaviours[_chestBehaviours.Count - 1];
        _chestBehaviours.Remove(chestToRemove);
        _allowedChests.Remove(chestToRemove);
        Destroy(chestToRemove.gameObject);

        _cam.SetOrthoSize();
    }

    void DecreaseDelay()
    {
        // TODO: this should be usnig a curve
        _cursorDelay -= 100;
    }

    void IncreaseDelay()
    {
        _cursorDelay += 100;
    }

    void BackToJourney()
    {
        _gameManager.LoadLevel(Scenes.Journey);
    }

}
