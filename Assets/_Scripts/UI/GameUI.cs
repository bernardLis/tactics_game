using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameUI : MonoBehaviour
{
    // UI
    UIDocument UIDocument;

    VisualElement logContainer;
    Label logText;

    Queue<string> logQueue;
    string currentLog;
    bool showLogIsRunning;

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
        UIDocument = GetComponent<UIDocument>();
        var root = UIDocument.rootVisualElement;
 
        // log
        logContainer = root.Q<VisualElement>("logContainer");
        logText = root.Q<Label>("logText");

        logQueue = new Queue<string>();
    }

    public void HideAllUIPanels()
    {
        UIDocument.rootVisualElement.Q<VisualElement>("inventoryContainer").style.display = DisplayStyle.None;
        UIDocument.rootVisualElement.Q<VisualElement>("questUI").style.display = DisplayStyle.None;
        UIDocument.rootVisualElement.Q<VisualElement>("conversationContainer").style.display = DisplayStyle.None;
        UIDocument.rootVisualElement.Q<VisualElement>("tooltipUI").style.display = DisplayStyle.None;
        logContainer.style.display = DisplayStyle.None;
    }

    public void DisplayLogText(string newText)
    {
        // add log text to the queue
        logQueue.Enqueue(newText);

        // make sure only one ui panel is active
        if (!showLogIsRunning)
            StartCoroutine(ShowLogText());
    }

    IEnumerator ShowLogText()
    {
        showLogIsRunning = true;

        // only one can be visible.
        HideAllUIPanels();
        logContainer.style.display = DisplayStyle.Flex;

        while (logQueue.Count > 0)
        {
            currentLog = logQueue.Dequeue();
            logText.text = currentLog;
            yield return new WaitForSeconds(2f);
        }

        showLogIsRunning = false;
        logContainer.style.display = DisplayStyle.None;
        yield break;
    }
}
