using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameUI : MonoBehaviour
{
    // UI
    UIDocument _UIDocument;
    VisualElement _logContainer;
    Label _logText;

    Queue<string> _logQueue;
    string _currentLog;
    bool isShowLogRunning;

    public static GameUI instance;
    void Awake()
    {
        #region Singleton
        // singleton
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of UIDocument found");
            return;
        }
        instance = this;

        #endregion

        // getting ui elements
        _UIDocument = GetComponent<UIDocument>();
        var root = _UIDocument.rootVisualElement;

        // log
        _logContainer = root.Q<VisualElement>("logContainer");
        _logText = root.Q<Label>("logText");

        _logQueue = new Queue<string>();
    }

    public void HideAllUIPanels()
    {
        CharacterUI.instance.HideCharacterUI();
        InfoCardUI.instance.HideCharacterCard();
        InfoCardUI.instance.HideInteractionSummary();
        InfoCardUI.instance.HideTileInfo();

        _UIDocument.rootVisualElement.Q<VisualElement>("inventoryContainer").style.display = DisplayStyle.None;
        _UIDocument.rootVisualElement.Q<VisualElement>("questUI").style.display = DisplayStyle.None;
        _UIDocument.rootVisualElement.Q<VisualElement>("conversationContainer").style.display = DisplayStyle.None;
        _UIDocument.rootVisualElement.Q<VisualElement>("tooltipUI").style.display = DisplayStyle.None;
        _logContainer.style.display = DisplayStyle.None;
    }

    public void DisplayLogText(string newText)
    {
        // add log text to the queue
        _logQueue.Enqueue(newText);

        // make sure only one ui panel is active
        if (!isShowLogRunning)
            StartCoroutine(ShowLogText());
    }

    IEnumerator ShowLogText()
    {
        isShowLogRunning = true;

        // only one can be visible.
        HideAllUIPanels();
        _logContainer.style.display = DisplayStyle.Flex;

        while (_logQueue.Count > 0)
        {
            _currentLog = _logQueue.Dequeue();
            _logText.text = _currentLog;
            yield return new WaitForSeconds(2f);
        }

        isShowLogRunning = false;
        _logContainer.style.display = DisplayStyle.None;
        yield break;
    }
}
