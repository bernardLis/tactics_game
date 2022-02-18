using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class JourneyChestManager : MonoBehaviour
{
    JourneyManager journeyManager;
    LevelLoader levelLoader;

    [Header("Unity Setup")]
    public JourneyChest[] chests;
    public GameObject chestPrefab;
    public GameObject cursorPrefab;
    public GameObject obolPrefab;

    public int numberOfChests;

    // UI
    UIDocument UIDocument;
    VisualElement wrapper;

    Label result;
    VisualElement rewardWrapper;
    Label obolAmountLabel;
    Button backToJourneyButton;

    // mini game
    List<JourneyChestBehaviour> chestBehaviours = new();
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

        journeyManager = JourneyManager.instance;
        levelLoader = journeyManager.GetComponent<LevelLoader>();

        UIDocument = GetComponent<UIDocument>();
        var root = UIDocument.rootVisualElement;
        wrapper = root.Q<VisualElement>("wrapper");

        result = root.Q<Label>("result");
        rewardWrapper = root.Q<VisualElement>("rewardWrapper");
        obolAmountLabel = root.Q<Label>("obolAmount");
        backToJourneyButton = root.Q<Button>("backToJourney");

        backToJourneyButton.clickable.clicked += BackToJourney;
    }

    void Start()
    {
        int x = -5;
        int y = 2;
        List<JourneyChest> availableChests = new(chests);
        for (int i = 0; i < numberOfChests; i++)
        {
            GameObject obj = Instantiate(chestPrefab, new Vector3(x, y), Quaternion.identity);
            JourneyChest chestScriptable = Instantiate(availableChests[Random.Range(0, availableChests.Count)]);

            JourneyChestBehaviour chestBehaviour = obj.GetComponent<JourneyChestBehaviour>();
            chestBehaviour.chest = chestScriptable;
            chestBehaviours.Add(chestBehaviour);

            availableChests.Remove(chestScriptable); // making sure chests are different
            chestScriptable.Initialize(obj);
            x += 5;
        }
        ResponsiveCamera cam = Camera.main.GetComponent<ResponsiveCamera>();
        cam.SetOrthoSize();

        StartGame();
    }

    async void StartGame()
    {
        allowedChests = new(chestBehaviours);
        cursor = Instantiate(cursorPrefab, transform.position, Quaternion.identity);
        // every second change pointed chest
        while (selectedChest == null)
        {
            MoveCursor();
            await Task.Delay(400);
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
        // TODO: there probably is a better way to do that but that's easy.
        foreach (JourneyChestBehaviour c in chestBehaviours)
            c.wasChestSelected = true;

        JourneyNodeReward reward;
        if (_chestBehaviour == cursorChest)
        {
            result.text = "Winner winner chicken dinner";
            result.style.color = Color.white;
            reward = _chestBehaviour.chest.Won();
        }
        else
        {
            result.text = "Better luck next time";
            result.style.color = Color.red;
            reward = _chestBehaviour.chest.Lost();
        }

        obolAmountLabel.text = "0";

        journeyManager.SetNodeReward(reward);
        StartCoroutine(ChangeObolsCoroutine(reward.obols));

        wrapper.style.opacity = 0;
        wrapper.style.display = DisplayStyle.Flex;
        DOTween.To(() => wrapper.style.opacity.value, x => wrapper.style.opacity = x, 1f, 1f)
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

    void BackToJourney()
    {
        levelLoader.ChangeScene("Journey");
    }

}
