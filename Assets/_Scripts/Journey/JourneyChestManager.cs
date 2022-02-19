using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class JourneyChestManager : MonoBehaviour
{
    // global
    JourneyManager journeyManager;
    LevelLoader levelLoader;

    ResponsiveCamera cam;


    [Header("Unity Setup")]
    public JourneyChest[] chests;
    public GameObject chestPrefab;
    public GameObject cursorPrefab;
    public GameObject obolPrefab;
    public JourneyNodeReward reward;


    // UI
    UIDocument UIDocument;
    VisualElement difficultyAdjustWrapper;
    Button increaseDifficultyButton;
    Button decreaseDifficultyButton;
    Label playingForObolsLabel;

    VisualElement resultWrapper;
    Label result;
    VisualElement rewardWrapper;
    Label obolAmountLabel;
    Button backToJourneyButton;

    // mini game
    int numberOfChests = 3;
    int obolsReward = 3;
    int cursorDelay = 500;

    List<JourneyChestBehaviour> chestBehaviours = new();
    List<JourneyChest> availableChests; // to make sure they are all visiually differnet
    List<JourneyChestBehaviour> allowedChests;
    GameObject cursor;
    public JourneyChestBehaviour cursorChest { get; private set; }
    JourneyChestBehaviour selectedChest;

    public static JourneyChestManager instance;
    void Awake()
    {
        #region Singleton
        // singleton
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of JourneyChestManager found");
            return;
        }
        instance = this;
        #endregion

        // global
        journeyManager = JourneyManager.instance;
        levelLoader = journeyManager.GetComponent<LevelLoader>();

        cam = Camera.main.GetComponent<ResponsiveCamera>();

        // UI
        UIDocument = GetComponent<UIDocument>();
        var root = UIDocument.rootVisualElement;

        difficultyAdjustWrapper = root.Q<VisualElement>("difficultyAdjustWrapper");
        playingForObolsLabel = root.Q<Label>("playingForObols");
        increaseDifficultyButton = root.Q<Button>("increaseDifficulty");
        decreaseDifficultyButton = root.Q<Button>("decreaseDifficulty");

        increaseDifficultyButton.clickable.clicked += IncreaseDifficulty;
        decreaseDifficultyButton.clickable.clicked += DecreaseDifficulty;

        resultWrapper = root.Q<VisualElement>("resultWrapper");
        result = root.Q<Label>("result");
        rewardWrapper = root.Q<VisualElement>("rewardWrapper");
        obolAmountLabel = root.Q<Label>("obolAmount");
        backToJourneyButton = root.Q<Button>("backToJourney");

        backToJourneyButton.clickable.clicked += BackToJourney;

        // other
        availableChests = new(chests);
    }

    void Start()
    {
        int x = -5;
        for (int i = 0; i < numberOfChests; i++)
        {
            GameObject obj = Instantiate(chestPrefab, new Vector3(x, 2), Quaternion.identity); // TOOD: magic 2
            JourneyChest chestScriptable = Instantiate(availableChests[Random.Range(0, availableChests.Count)]);

            JourneyChestBehaviour chestBehaviour = obj.GetComponent<JourneyChestBehaviour>();
            chestBehaviour.chest = chestScriptable;
            chestBehaviours.Add(chestBehaviour);

            availableChests.Remove(chestScriptable); // making sure chests are different
            chestScriptable.Initialize(obj);
            x += 5;
        }
        cam.SetOrthoSize();

        StartGame();
    }

    async void StartGame()
    {
        allowedChests = new(chestBehaviours);
        cursor = Instantiate(cursorPrefab, transform.position, Quaternion.identity);

        while (selectedChest == null)
        {
            MoveCursor();
            await Task.Delay(cursorDelay);
        }
    }

    public void AddForbiddenChest(JourneyChestBehaviour _chest)
    {
        allowedChests.Remove(_chest);
    }

    public void RemoveForbiddenChest(JourneyChestBehaviour _chest)
    {
        allowedChests.Add(_chest);
    }

    void MoveCursor()
    {
        cursorChest = allowedChests[Random.Range(0, allowedChests.Count)];
        Vector3 pos = new Vector3(cursorChest.transform.position.x, cursorChest.transform.position.y + 0.5f);
        cursor.transform.position = pos;
    }

    public void ChestWasSelected(JourneyChestBehaviour _chestBehaviour)
    {
        selectedChest = _chestBehaviour;

        foreach (JourneyChestBehaviour c in chestBehaviours)
            c.wasChestSelected = true;

        JourneyNodeReward rewardInstance = Instantiate(reward);
        if (_chestBehaviour == cursorChest)
        {
            result.text = "Winner winner chicken dinner";
            result.style.color = Color.white;
            _chestBehaviour.chest.Won();
            rewardInstance.obols = obolsReward;
        }
        else
        {
            result.text = "Better luck next time";
            result.style.color = Color.red;
            _chestBehaviour.chest.Lost();
            rewardInstance.obols = 0;
        }

        obolAmountLabel.text = "0";

        journeyManager.SetNodeReward(rewardInstance);
        StartCoroutine(ChangeObolsCoroutine(rewardInstance.obols));

        difficultyAdjustWrapper.style.display = DisplayStyle.None;

        resultWrapper.style.opacity = 0;
        resultWrapper.style.display = DisplayStyle.Flex;
        DOTween.To(() => resultWrapper.style.opacity.value, x => resultWrapper.style.opacity = x, 1f, 1f)
            .SetEase(Ease.InSine);
    }

    IEnumerator ChangeObolsCoroutine(int _amount)
    {
        int current = 0;
        for (int i = 0; i < _amount; i++)
        {
            current++;
            SpawnObol();
            yield return new WaitForSeconds(0.2f);
            obolAmountLabel.text = current.ToString();
        }
    }

    void SpawnObol()
    {
        Vector3 spawnPos = selectedChest.gameObject.transform.position;
        Vector3 randomVector = new Vector3(Random.Range(0f, 3f), Random.Range(0f, 3f));

        GameObject g = Instantiate(obolPrefab, spawnPos, Quaternion.identity);
        g.transform.DOJump(spawnPos + randomVector, 1f, 1, 1f);

        Destroy(g, 1.1f);
    }

    /* BUTTONS */
    void IncreaseDifficulty()
    {
        if (obolsReward == 10)
            return;

        obolsReward++;
        playingForObolsLabel.text = obolsReward.ToString();
        // we add a chest or decrease delay every second obol, starting with adding a chest
        if (obolsReward % 2 == 0)
            AddChest();
        else
            DecreaseDelay();
    }

    void DecreaseDifficulty()
    {
        if (obolsReward == 1)
            return;

        obolsReward--;
        playingForObolsLabel.text = obolsReward.ToString();
        // we add a chest or decrease delay every second obol, starting with adding a chest
        if (obolsReward % 2 == 0)
            IncreaseDelay();
        else
            RemoveChest();
    }

    void AddChest()
    {
        // start over if you use all visuals
        if (availableChests.Count == 0)
            availableChests = new(chests);

        JourneyChestBehaviour lastChest = chestBehaviours[chestBehaviours.Count - 1];
        Vector3 pos = new Vector3(lastChest.transform.position.x + 5f, lastChest.transform.position.y);

        GameObject obj = Instantiate(chestPrefab, pos, Quaternion.identity);
        JourneyChest chestScriptable = Instantiate(availableChests[Random.Range(0, availableChests.Count)]);

        JourneyChestBehaviour chestBehaviour = obj.GetComponent<JourneyChestBehaviour>();
        chestBehaviour.chest = chestScriptable;
        chestBehaviours.Add(chestBehaviour);
        allowedChests.Add(chestBehaviour);

        availableChests.Remove(chestScriptable); // making sure chests are different
        chestScriptable.Initialize(obj);

        cam.SetOrthoSize();
    }

    void RemoveChest()
    {
        JourneyChestBehaviour chestToRemove = chestBehaviours[chestBehaviours.Count - 1];
        chestBehaviours.Remove(chestToRemove);
        allowedChests.Remove(chestToRemove);
        Destroy(chestToRemove.gameObject);

        cam.SetOrthoSize();
    }

    void DecreaseDelay()
    {
        // TODO: this should be usnig a curve
        cursorDelay -= 100;
    }

    void IncreaseDelay()
    {
        cursorDelay += 100;
    }

    void BackToJourney()
    {
        levelLoader.ChangeScene("Journey");
    }

}
