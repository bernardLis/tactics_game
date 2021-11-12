using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;

public class BattleUI : MonoBehaviour
{

    // global utility
    BattleInputController battleInputController;
    CharacterBattleController characterBattleController;
    Highlighter highlighter;

    // UI Elements
    UIDocument UIDocument;

    VisualElement characterUIContainer;

    VisualElement characterUITooltipContainer;
    Label characterUITooltipAbilityName;
    Label characterUITooltipAbilityDescription;
    Label characterUITooltipAbilityManaCost;

    Label characterName;
    VisualElement characterPortrait;

    Label characterHealth;
    Label characterMana;

    Button characterAButton;
    Button characterSButton;

    Button characterQButton;
    Button characterWButton;
    Button characterEButton;
    Button characterRButton;

    VisualElement characterQSkillIcon;
    VisualElement characterWSkillIcon;
    VisualElement characterESkillIcon;
    VisualElement characterRSkillIcon;

    // local
    PlayerStats selectedPlayerStats;

    // animate ui up/down on show/hide
    float topPercent = 20;
    bool isAnimationOn;
    IVisualElementScheduledItem scheduler;

    // buttons management
    Queue<IEnumerator> buttonClickQueue = new();
    bool wasClickEnqueued;

    #region Singleton
    public static BattleUI instance;
    void Awake()
    {
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

        characterUIContainer = root.Q<VisualElement>("characterUIContainer");

        characterUITooltipContainer = root.Q<VisualElement>("characterUITooltipContainer");
        characterUITooltipAbilityName = root.Q<Label>("characterUITooltipAbilityName");
        characterUITooltipAbilityDescription = root.Q<Label>("characterUITooltipAbilityDescription");
        characterUITooltipAbilityManaCost = root.Q<Label>("characterUITooltipAbilityManaCost");

        characterName = root.Q<Label>("characterName");
        characterPortrait = root.Q<VisualElement>("characterPortrait");
        characterHealth = root.Q<Label>("characterHealth");
        characterMana = root.Q<Label>("characterMana");

        characterAButton = root.Q<Button>("characterAButton");
        characterSButton = root.Q<Button>("characterSButton");

        characterQButton = root.Q<Button>("characterQButton");
        characterWButton = root.Q<Button>("characterWButton");
        characterEButton = root.Q<Button>("characterEButton");
        characterRButton = root.Q<Button>("characterRButton");

        // register interaction callbacks (buttons)
        characterAButton.clickable.clicked += AButtonClicked;
        characterSButton.clickable.clicked += SButtonClicked;

        characterQButton.clickable.clicked += QButtonClicked;
        characterWButton.clickable.clicked += WButtonClicked;
        characterEButton.clickable.clicked += EButtonClicked;
        characterRButton.clickable.clicked += RButtonClicked;

        characterQSkillIcon = root.Q<VisualElement>("characterQSkillIcon");
        characterWSkillIcon = root.Q<VisualElement>("characterWSkillIcon");
        characterESkillIcon = root.Q<VisualElement>("characterESkillIcon");
        characterRSkillIcon = root.Q<VisualElement>("characterRSkillIcon");
    }

    void Start()
    {
        battleInputController = BattleInputController.instance;
        characterBattleController = CharacterBattleController.instance;
        highlighter = Highlighter.instance;

        //https://answers.unity.com/questions/1590871/how-to-stack-coroutines-and-call-each-one-till-all.html
        StartCoroutine(CoroutineCoordinator());
    }

    // TODO: 
    // * hook up buttons and interactions with character battle controller
    // * on hover or on selection show ability tooltip with some info
    // * update character UI on interaction


    // allow clicks only when not moving and character is selected & did not finish its turn
    // first 2 abilities should always be 
    void AButtonClicked()
    {
        Debug.Log("A button clicked - basic attack");
        if (!characterBattleController.CanInteract())
            return;

        buttonClickQueue.Enqueue(HandleButtonClick(selectedPlayerStats.abilities[0]));
    }

    void SButtonClicked()
    {
        Debug.Log(" button clicked - basic defend");
        if (!characterBattleController.CanInteract())
            return;

        buttonClickQueue.Enqueue(HandleButtonClick(selectedPlayerStats.abilities[1]));
    }



    void QButtonClicked()
    {
        if (!characterBattleController.CanInteract())
            return;

        buttonClickQueue.Enqueue(HandleButtonClick(selectedPlayerStats.abilities[2]));
    }

    void WButtonClicked()
    {
        if (!characterBattleController.CanInteract())
            return;

        buttonClickQueue.Enqueue(HandleButtonClick(selectedPlayerStats.abilities[3]));
    }

    void EButtonClicked()
    {
        if (!characterBattleController.CanInteract())
            return;

        buttonClickQueue.Enqueue(HandleButtonClick(selectedPlayerStats.abilities[4]));
    }

    void RButtonClicked()
    {
        if (!characterBattleController.CanInteract())
            return;

        buttonClickQueue.Enqueue(HandleButtonClick(selectedPlayerStats.abilities[5]));
    }


    // TODO: Hey future Bernard, I know you are looking at this and thinking: "damn... mixing coroutines and async await sucks, what was I thinking"
    // I, past Bernard would like to tell you that I tried hard to make it work and I left coroutines and async await not because I don't like you but
    // because I am not skilled enough to rewrite everything to use only async await. 
    IEnumerator HandleButtonClick(Ability ability)
    {
        ShowAbilityTooltip(ability);

        Task task = ability.HighlightTargetable(); // for some reason this works, but it has to be written exactly like that with Task task = ;
        yield return new WaitUntil(() => task.IsCompleted);

        characterBattleController.SetSelectedAbility(ability);
    }


    void ShowAbilityTooltip(Ability ability)
    {
        characterUITooltipContainer.style.display = DisplayStyle.Flex;

        characterUITooltipAbilityName.text = ability.aName;
        characterUITooltipAbilityDescription.text = ability.aDescription;
        characterUITooltipAbilityManaCost.text = "Mana cost: " + ability.manaCost;
    }

    public void HideAbilityTooltip()
    {
        characterUITooltipContainer.style.display = DisplayStyle.None;
    }

    public void ShowCharacterUI(PlayerStats playerStats)
    {
        if (isAnimationOn)
            return;

        // current character is not in the scene, keep that in mind. It's a static scriptable object.
        selectedPlayerStats = playerStats;

        characterUIContainer.style.display = DisplayStyle.Flex;

        characterName.text = selectedPlayerStats.character.characterName;
        characterPortrait.style.backgroundImage = selectedPlayerStats.character.portrait.texture;
        characterHealth.text = selectedPlayerStats.currentHealth + "/" + selectedPlayerStats.maxHealth.GetValue();
        characterMana.text = selectedPlayerStats.currentMana + "/" + selectedPlayerStats.maxMana.GetValue();

        characterQSkillIcon.style.backgroundImage = selectedPlayerStats.abilities[2].aIcon.texture;
        characterWSkillIcon.style.backgroundImage = selectedPlayerStats.abilities[3].aIcon.texture;
        characterESkillIcon.style.backgroundImage = selectedPlayerStats.abilities[4].aIcon.texture;
        characterRSkillIcon.style.backgroundImage = selectedPlayerStats.abilities[5].aIcon.texture;

        DisableSkillButtons();
        EnableSkillButtons();

        // 'animate' it to come up 
        characterUIContainer.style.top = Length.Percent(topPercent);
        isAnimationOn = true;
        scheduler = characterUIContainer.schedule.Execute(() => AnimateConversationBoxUp()).Every(10); // ms
    }

    public void DisableSkillButtons()
    {
        characterAButton.SetEnabled(false);
        characterSButton.SetEnabled(false);

        characterQButton.SetEnabled(false);
        characterWButton.SetEnabled(false);
        characterEButton.SetEnabled(false);
        characterRButton.SetEnabled(false);
    }

    public void EnableSkillButtons()
    {
        // costless actions
        characterAButton.SetEnabled(true);
        characterSButton.SetEnabled(true);

        // enable skill buttons if you can afford mana cost;
        if (selectedPlayerStats.abilities[2].manaCost <= selectedPlayerStats.currentMana)
            characterQButton.SetEnabled(true);
        if (selectedPlayerStats.abilities[3].manaCost <= selectedPlayerStats.currentMana)
            characterWButton.SetEnabled(true);
        if (selectedPlayerStats.abilities[4].manaCost <= selectedPlayerStats.currentMana)
            characterEButton.SetEnabled(true);
        if (selectedPlayerStats.abilities[5].manaCost <= selectedPlayerStats.currentMana)
            characterRButton.SetEnabled(true);

    }

    public void HideCharacterUI()
    {
        if (isAnimationOn)
            return;

        selectedPlayerStats = null;
        isAnimationOn = true;
        scheduler = characterUIContainer.schedule.Execute(() => AnimateConversationBoxDown()).Every(10); // ms
    }

    void AnimateConversationBoxUp()
    {
        if (topPercent > -1f)
        {
            characterUIContainer.style.top = Length.Percent(topPercent);
            topPercent--;
            return;
        }
        isAnimationOn = false;

        scheduler.Pause();
    }

    void AnimateConversationBoxDown()
    {
        if (topPercent < 21f)
        {
            characterUIContainer.style.top = Length.Percent(topPercent);
            topPercent++;
            return;
        }

        // TODO: idk how to destroy scheduler...
        isAnimationOn = false;
        scheduler.Pause();

        characterUIContainer.style.display = DisplayStyle.None;
        HideAbilityTooltip();
    }


    // https://answers.unity.com/questions/1590871/how-to-stack-coroutines-and-call-each-one-till-all.html
    IEnumerator CoroutineCoordinator()
    {
        while (true)
        {
            while (buttonClickQueue.Count > 0)
            {
                // block input when the queue is not 0;
                wasClickEnqueued = true;
                battleInputController.SetInputAllowed(false);

                yield return StartCoroutine(buttonClickQueue.Dequeue());
            }
            // set input allowed to true only once when the queue is emptied;
            if (wasClickEnqueued)
            {
                wasClickEnqueued = false;
                battleInputController.SetInputAllowed(true);
            }

            yield return null;
        }
    }
    // for keyboard input

    public void SimulateAButtonClicked()
    {
        using (var e = new NavigationSubmitEvent() { target = characterAButton })
            characterAButton.SendEvent(e);
    }

    public void SimulateSButtonClicked()
    {
        using (var e = new NavigationSubmitEvent() { target = characterSButton })
            characterSButton.SendEvent(e);
    }

    public void SimulateQButtonClicked()
    {
        // https://forum.unity.com/threads/trigger-button-click-from-code.1124356/
        using (var e = new NavigationSubmitEvent() { target = characterQButton })
            characterQButton.SendEvent(e);
    }

    public void SimulateWButtonClicked()
    {
        using (var e = new NavigationSubmitEvent() { target = characterWButton })
            characterWButton.SendEvent(e);
    }

    public void SimulateEButtonClicked()
    {
        using (var e = new NavigationSubmitEvent() { target = characterEButton })
            characterEButton.SendEvent(e);
    }

    public void SimulateRButtonClicked()
    {
        using (var e = new NavigationSubmitEvent() { target = characterRButton })
            characterRButton.SendEvent(e);
    }

}
