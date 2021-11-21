using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;

public class BattleUI : MonoBehaviour
{

    // global utility
    BattleInputController battleInputController;
    BattleCharacterController battleCharacterController;

    // UI Elements
    UIDocument UIDocument;

    VisualElement characterUIContainer;
    VisualElement characterUI;

    VisualElement characterUITooltipContainer;
    Label characterUITooltipAbilityName;
    Label characterUITooltipAbilityDescription;
    Label characterUITooltipAbilityManaCost;

    Label characterName;
    VisualElement characterPortrait;

    Label characterHealth;
    Label characterMana;

    VisualElement characterHealthBarMissingHealth;
    VisualElement characterManaBarMissingMana;

    Label characterStrengthAmount;
    Label characterIntelligenceAmount;
    Label characterAgilityAmount;
    Label characterStaminaAmount;
    Label characterArmorAmount;
    Label characterRangeAmount;

    Button characterAButton;
    Button characterSButton;

    Button characterQButton;
    Button characterWButton;
    Button characterEButton;
    Button characterRButton;
    List<Button> abilityButtons;

    VisualElement characterASkillIcon;
    VisualElement characterSSkillIcon;

    VisualElement characterQSkillIcon;
    VisualElement characterWSkillIcon;
    VisualElement characterESkillIcon;
    VisualElement characterRSkillIcon;
    List<VisualElement> abilityIcons;

    // character card
    VisualElement characterCardContainer;
    Label characterCardName;
    VisualElement characterCardPortrait;

    Label characterCardHealth;
    Label characterCardMana;

    VisualElement characterCardHealthBarMissingHealth;
    VisualElement characterCardManaBarMissingMana;

    Label characterCardStrengthAmount;
    Label characterCardIntelligenceAmount;
    Label characterCardAgilityAmount;
    Label characterCardStaminaAmount;
    Label characterCardArmorAmount;
    Label characterCardRangeAmount;

    // local
    PlayerStats selectedPlayerStats;

    // animate ui up/down on show/hide
    float characterUITopPercent = 20f;
    float characterUIShowValue = 0f;
    float characterUIHideValue = 20f;
    bool isCharacterUIAnimationOn;
    IVisualElementScheduledItem scheduler;

    // animate card left right on show/hide
    float characterCardLeftPercent = -30f;
    float characterCardShowValue = 0f;
    float characterCardHideValue = -30f;

    bool isCharacterCardAnimationOn;

    IVisualElementScheduledItem characterCardScheduler;

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
        characterUI = root.Q<VisualElement>("characterUI");
        // otherwise it is null
        characterUI.style.top = Length.Percent(characterUIHideValue);

        characterUITooltipContainer = root.Q<VisualElement>("characterUITooltipContainer");
        characterUITooltipAbilityName = root.Q<Label>("characterUITooltipAbilityName");
        characterUITooltipAbilityDescription = root.Q<Label>("characterUITooltipAbilityDescription");
        characterUITooltipAbilityManaCost = root.Q<Label>("characterUITooltipAbilityManaCost");

        characterName = root.Q<Label>("characterName");
        characterPortrait = root.Q<VisualElement>("characterPortrait");
        characterHealth = root.Q<Label>("characterHealth");
        characterMana = root.Q<Label>("characterMana");

        characterHealthBarMissingHealth = root.Q<VisualElement>("characterHealthBarMissingHealth");
        characterManaBarMissingMana = root.Q<VisualElement>("characterManaBarMissingMana");

        characterStrengthAmount = root.Q<Label>("characterStrengthAmount");
        characterIntelligenceAmount = root.Q<Label>("characterIntelligenceAmount");
        characterAgilityAmount = root.Q<Label>("characterAgilityAmount");
        characterStaminaAmount = root.Q<Label>("characterStaminaAmount");
        characterArmorAmount = root.Q<Label>("characterArmorAmount");
        characterRangeAmount = root.Q<Label>("characterRangeAmount");

        characterAButton = root.Q<Button>("characterAButton");
        characterSButton = root.Q<Button>("characterSButton");

        characterQButton = root.Q<Button>("characterQButton");
        characterWButton = root.Q<Button>("characterWButton");
        characterEButton = root.Q<Button>("characterEButton");
        characterRButton = root.Q<Button>("characterRButton");

        // TODO: this could be probably improved
        abilityButtons = new();
        abilityButtons.Add(characterQButton);
        abilityButtons.Add(characterWButton);
        abilityButtons.Add(characterEButton);
        abilityButtons.Add(characterRButton);

        // register interaction callbacks (buttons)
        characterAButton.clickable.clicked += AButtonClicked;
        characterSButton.clickable.clicked += SButtonClicked;

        characterQButton.clickable.clicked += QButtonClicked;
        characterWButton.clickable.clicked += WButtonClicked;
        characterEButton.clickable.clicked += EButtonClicked;
        characterRButton.clickable.clicked += RButtonClicked;

        characterASkillIcon = root.Q<VisualElement>("characterASkillIcon");
        characterSSkillIcon = root.Q<VisualElement>("characterSSkillIcon");

        characterQSkillIcon = root.Q<VisualElement>("characterQSkillIcon");
        characterWSkillIcon = root.Q<VisualElement>("characterWSkillIcon");
        characterESkillIcon = root.Q<VisualElement>("characterESkillIcon");
        characterRSkillIcon = root.Q<VisualElement>("characterRSkillIcon");

        // TODO: this could be probably improved
        abilityIcons = new();
        abilityIcons.Add(characterQSkillIcon);
        abilityIcons.Add(characterWSkillIcon);
        abilityIcons.Add(characterESkillIcon);
        abilityIcons.Add(characterRSkillIcon);

        // character card
        characterCardContainer = root.Q<VisualElement>("characterCardContainer");
        characterCardContainer.style.left = Length.Percent(characterCardHideValue);

        characterCardName = root.Q<Label>("characterCardName");
        characterCardPortrait = root.Q<VisualElement>("characterCardPortrait");

        characterCardHealth = root.Q<Label>("characterCardHealth");
        characterCardMana = root.Q<Label>("characterCardMana");

        characterCardHealthBarMissingHealth = root.Q<VisualElement>("characterCardHealthBarMissingHealth");
        characterCardManaBarMissingMana = root.Q<VisualElement>("characterCardManaBarMissingMana");

        characterCardStrengthAmount = root.Q<Label>("characterCardStrengthAmount");
        characterCardIntelligenceAmount = root.Q<Label>("characterCardIntelligenceAmount");
        characterCardAgilityAmount = root.Q<Label>("characterCardAgilityAmount");
        characterCardStaminaAmount = root.Q<Label>("characterCardStaminaAmount");
        characterCardArmorAmount = root.Q<Label>("characterCardArmorAmount");
        characterCardRangeAmount = root.Q<Label>("characterCardRangeAmount");
    }

    void Start()
    {
        battleInputController = BattleInputController.instance;
        battleCharacterController = BattleCharacterController.instance;

        //https://answers.unity.com/questions/1590871/how-to-stack-coroutines-and-call-each-one-till-all.html
        StartCoroutine(CoroutineCoordinator());
    }

    // allow clicks only when not moving and character is selected & did not finish its turn
    // first 2 abilities should always be 
    void AButtonClicked()
    {
        if (!battleCharacterController.CanInteract())
            return;

        // TODO: hardcoded ability indexes
        buttonClickQueue.Enqueue(HandleButtonClick(selectedPlayerStats.basicAbilities[0]));
    }

    void SButtonClicked()
    {
        if (!battleCharacterController.CanInteract())
            return;

        // TODO: hardcoded ability indexes
        buttonClickQueue.Enqueue(HandleButtonClick(selectedPlayerStats.basicAbilities[1]));
    }

    void QButtonClicked()
    {
        if (!battleCharacterController.CanInteract())
            return;

        // TODO: hardcoded ability indexes
        if (selectedPlayerStats.abilities[0] == null)
            return;

        buttonClickQueue.Enqueue(HandleButtonClick(selectedPlayerStats.abilities[0]));
    }

    void WButtonClicked()
    {
        if (!battleCharacterController.CanInteract())
            return;

        // TODO: hardcoded ability indexes
        if (selectedPlayerStats.abilities[1] == null)
            return;

        buttonClickQueue.Enqueue(HandleButtonClick(selectedPlayerStats.abilities[1]));
    }

    void EButtonClicked()
    {
        if (!battleCharacterController.CanInteract())
            return;

        // TODO: hardcoded ability indexes
        if (selectedPlayerStats.abilities[2] == null)
            return;

        buttonClickQueue.Enqueue(HandleButtonClick(selectedPlayerStats.abilities[2]));
    }

    void RButtonClicked()
    {
        if (!battleCharacterController.CanInteract())
            return;

        // TODO: hardcoded ability indexes
        if (selectedPlayerStats.abilities[3] == null)
            return;

        buttonClickQueue.Enqueue(HandleButtonClick(selectedPlayerStats.abilities[3]));
    }

    // TODO: Hey future Bernard, I know you are looking at this and thinking: "damn... mixing coroutines and async await sucks, what was I thinking"
    // I, past Bernard would like to tell you that I tried hard to make it work and I left coroutines and async await not because I don't like you but
    // because I am not skilled enough to rewrite everything to use only async await. 
    IEnumerator HandleButtonClick(Ability ability)
    {
        ShowAbilityTooltip(ability);

        Task task = ability.HighlightTargetable(); // for some reason this works, but it has to be written exactly like that with Task task = ;
        yield return new WaitUntil(() => task.IsCompleted);

        battleCharacterController.SetSelectedAbility(ability);
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
        // current character is not in the scene, keep that in mind. It's a static scriptable object.
        selectedPlayerStats = playerStats;

        //characterUIContainer.style.display = DisplayStyle.Flex;
        //characterUI.style.display = DisplayStyle.Flex;

        characterName.text = selectedPlayerStats.character.characterName;
        characterPortrait.style.backgroundImage = selectedPlayerStats.character.portrait.texture;

        SetCharacterHealthMana(playerStats);
        SetCharacteristics(playerStats);
        HandleAbilityButtons();
        DisableSkillButtons();
        EnableSkillButtons();

        // skip showing it, if it is already shown;
        if (characterUI.style.top.value.value == characterUIShowValue)
            return;

        if (isCharacterUIAnimationOn)
            return;

        // 'animate' it to come up 
        characterUI.style.top = Length.Percent(characterUITopPercent);
        isCharacterUIAnimationOn = true;
        scheduler = characterUI.schedule.Execute(() => AnimateCharacterUIBoxUp()).Every(10); // ms
    }

    public void HideCharacterUI()
    {
        // skip hiding it, if it is already hidden;
        if (characterUI.style.top.value.value == characterUIHideValue)
            return;

        if (isCharacterUIAnimationOn)
            return;

        selectedPlayerStats = null;
        isCharacterUIAnimationOn = true;
        scheduler = characterUI.schedule.Execute(() => AnimateCharacterUIBoxDown()).Every(10); // ms
    }


    void SetCharacterHealthMana(PlayerStats playerStats)
    {
        characterHealth.text = playerStats.currentHealth + "/" + playerStats.maxHealth.GetValue();
        characterMana.text = playerStats.currentMana + "/" + playerStats.maxMana.GetValue();

        // (float) casts are not redundant, without them it does not work
        float missingHealthPerc = ((float)playerStats.maxHealth.GetValue() - (float)playerStats.currentHealth) / (float)playerStats.maxHealth.GetValue();
        float missingManaPerc = ((float)playerStats.maxMana.GetValue() - (float)playerStats.currentMana) / (float)playerStats.maxMana.GetValue();

        characterHealthBarMissingHealth.style.width = Length.Percent(missingHealthPerc * 100);
        characterManaBarMissingMana.style.width = Length.Percent(missingManaPerc * 100);
    }

    void SetCharacteristics(PlayerStats playerStats)
    {
        characterStrengthAmount.text = "" + playerStats.strength.GetValue();
        characterIntelligenceAmount.text = "" + playerStats.intelligence.GetValue();
        characterAgilityAmount.text = "" + playerStats.agility.GetValue();
        characterStaminaAmount.text = "" + playerStats.stamina.GetValue();
        characterArmorAmount.text = "" + playerStats.armor.GetValue();
        characterRangeAmount.text = "" + playerStats.movementRange.GetValue();
    }

    void HandleAbilityButtons()
    {
        // TODO: hardcoded ability indexes
        characterASkillIcon.style.backgroundImage = selectedPlayerStats.basicAbilities[0].aIcon.texture;
        characterSSkillIcon.style.backgroundImage = selectedPlayerStats.basicAbilities[1].aIcon.texture;

        int count = 0; // TODO: ugh...-1
        foreach (Button b in abilityButtons)
        {
            // hide buttons if there are not enough abilities to populate them;
            if (selectedPlayerStats.abilities.Count <= count)
            {
                abilityButtons[count].style.display = DisplayStyle.None;
                return;
            }

            // show buttons for each ability
            abilityButtons[count].style.display = DisplayStyle.Flex;
            abilityIcons[count].style.backgroundImage = selectedPlayerStats.abilities[count].aIcon.texture;

            count++;
        }
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

        // enable buttons if they are populated
        // && player has enough mana to cast ability;
        // && weapon type matches
        for (int i = 0; i < abilityButtons.Count; i++)
        {
            if (abilityButtons[i].style.display == DisplayStyle.None)
                continue;
            if (selectedPlayerStats.abilities[i].manaCost >= selectedPlayerStats.currentMana)
                continue;
            if (selectedPlayerStats.abilities[i].weaponType != selectedPlayerStats.character.weapon.weaponType && selectedPlayerStats.abilities[i].weaponType != WeaponType.ANY)
                continue;

            abilityButtons[i].SetEnabled(true);
        }
    }


    void AnimateCharacterUIBoxUp()
    {
        if (characterUITopPercent >= characterUIShowValue)
        {
            characterUI.style.top = Length.Percent(characterUITopPercent);
            characterUITopPercent--;
            return;
        }
        // TODO: idk how to destroy scheduler...
        isCharacterUIAnimationOn = false;

        scheduler.Pause();
    }

    void AnimateCharacterUIBoxDown()
    {
        if (characterUITopPercent <= characterUIHideValue)
        {
            characterUI.style.top = Length.Percent(characterUITopPercent);
            characterUITopPercent++;
            return;
        }

        // TODO: idk how to destroy scheduler...
        isCharacterUIAnimationOn = false;

        scheduler.Pause();

        //characterUI.style.display = DisplayStyle.None;
        HideAbilityTooltip();
    }

    // character card

    public void ShowCharacterCard(CharacterStats chStats)
    {
        PopulateCharacterCard(chStats);

        // skip showing it, if it is already shown;
        if (characterCardContainer.style.left.value.value == characterCardShowValue)
            return;

        if (isCharacterCardAnimationOn)
            return;

        // 'animate' it to come up
        characterCardContainer.style.left = Length.Percent(characterCardLeftPercent);
        isCharacterCardAnimationOn = true;
        characterCardScheduler = characterCardContainer.schedule.Execute(() => AnimateCharacterCardRight()).Every(5); // ms
    }

    public void HideCharacterCard()
    {
        // skip hidding it, if it is already hidden;
        if (characterCardContainer.style.left.value.value == characterCardHideValue)
            return;

        if (isCharacterCardAnimationOn)
            return;

        isCharacterCardAnimationOn = true;
        characterCardScheduler = characterCardContainer.schedule.Execute(() => AnimateCharacterCardLeft()).Every(5); // ms
    }

    void AnimateCharacterCardRight()
    {
        if (characterCardLeftPercent <= characterCardShowValue)
        {
            characterCardContainer.style.left = Length.Percent(characterCardLeftPercent);
            characterCardLeftPercent++;
            return;
        }
        // TODO: idk how to destroy scheduler...
        isCharacterCardAnimationOn = false;

        characterCardScheduler.Pause();
    }

    void AnimateCharacterCardLeft()
    {
        if (characterCardLeftPercent >= characterCardHideValue)
        {
            characterCardContainer.style.left = Length.Percent(characterCardLeftPercent);
            characterCardLeftPercent--;
            return;
        }

        // TODO: idk how to destroy scheduler...
        isCharacterCardAnimationOn = false;

        characterCardScheduler.Pause();
    }


    // TODO: this is code repetition
    void PopulateCharacterCard(CharacterStats chStats)
    {
        characterCardName.text = chStats.character.characterName;
        characterCardPortrait.style.backgroundImage = chStats.character.portrait.texture;

        characterCardHealth.text = chStats.currentHealth + "/" + chStats.maxHealth.GetValue();
        characterCardMana.text = chStats.currentMana + "/" + chStats.maxMana.GetValue();

        // (float) casts are not redundant, without them it does not work
        float missingHealthPerc = ((float)chStats.maxHealth.GetValue() - (float)chStats.currentHealth) / (float)chStats.maxHealth.GetValue();
        missingHealthPerc = Mathf.Clamp(missingHealthPerc, 0, 1);
        float missingManaPerc = ((float)chStats.maxMana.GetValue() - (float)chStats.currentMana) / (float)chStats.maxMana.GetValue();

        characterCardHealthBarMissingHealth.style.width = Length.Percent(missingHealthPerc * 100);
        characterCardManaBarMissingMana.style.width = Length.Percent(missingManaPerc * 100);

        characterCardStrengthAmount.text = "" + chStats.strength.GetValue();
        characterCardIntelligenceAmount.text = "" + chStats.intelligence.GetValue();
        characterCardAgilityAmount.text = "" + chStats.agility.GetValue();
        characterCardStaminaAmount.text = "" + chStats.stamina.GetValue();
        characterCardArmorAmount.text = "" + chStats.armor.GetValue();
        characterCardRangeAmount.text = "" + chStats.movementRange.GetValue();
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
