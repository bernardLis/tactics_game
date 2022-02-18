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
    public GameObject obolPrefab;

    public int numberOfChests;

    List<JourneyChestBehaviour> chestBehaviours = new();


    // UI
    UIDocument UIDocument;
    VisualElement wrapper;

    VisualElement rewardWrapper;
    Label obolAmountLabel;
    Button backToJourneyButton;

    JourneyChest selectedChest;

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
    }

    public void ChestWasSelected(JourneyChest _chest)
    {
        selectedChest = _chest;
        // TODO: there probably is a better way to do that but that's easy.
        foreach (JourneyChestBehaviour c in chestBehaviours)
            c.wasChestSelected = true;

        journeyManager.SetNodeReward(_chest.reward);
        obolAmountLabel.text = "0";
        StartCoroutine(ChangeObolsCoroutine(_chest.reward.obols));

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
