using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;
using DG.Tweening;

public class CharacterUI : MonoBehaviour
{

    // global utility
    BattleInputController battleInputController;
    BattleCharacterController battleCharacterController;

    // UI Elements
    UIDocument UIDocument;

    VisualElement characterUI;

    VisualElement characterUITooltipContainer;
    Label characterUITooltipAbilityName;
    Label characterUITooltipAbilityDescription;
    Label characterUITooltipAbilityManaCost;

    Label characterName;
    VisualElement characterPortrait;
    VisualElement characterPortraitSkull;

    Label characterHealth;
    VisualElement characterHealthBarMissingHealth;
    VisualElement characterHealthBarRetaliationResult;

    Label characterMana;
    VisualElement characterManaBarMissingMana;
    VisualElement characterManaBarInteractionResult;

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

    // local
    CharacterStats selectedPlayerStats;

    // animate ui up/down on show/hide
    float characterUIShowValue = 0f;
    float characterUIHideValue = 20f;

    // buttons management
    Queue<IEnumerator> buttonClickQueue = new();
    bool wasClickEnqueued;

    // showing mana use
    string manaUseTweenID = "manaUseTweenID";


    // showing retaliation result
    public Color damageBarColor;
    public Color healBarColor;
    string missingBarTweenID = "characterHealthBarTweenID";


    public static CharacterUI instance;
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

        characterUI = root.Q<VisualElement>("characterUI");
        // otherwise it is null
        characterUI.style.top = Length.Percent(characterUIHideValue);

        characterUITooltipContainer = root.Q<VisualElement>("characterUITooltipContainer");
        characterUITooltipAbilityName = root.Q<Label>("characterUITooltipAbilityName");
        characterUITooltipAbilityDescription = root.Q<Label>("characterUITooltipAbilityDescription");
        characterUITooltipAbilityManaCost = root.Q<Label>("characterUITooltipAbilityManaCost");

        characterName = root.Q<Label>("characterName");
        characterPortrait = root.Q<VisualElement>("characterPortrait");
        characterPortraitSkull = root.Q<VisualElement>("characterPortraitSkull");

        characterHealth = root.Q<Label>("characterHealth");
        characterHealthBarMissingHealth = root.Q<VisualElement>("characterHealthBarMissingHealth");
        characterHealthBarRetaliationResult = root.Q<VisualElement>("characterHealthBarRetaliationResult");

        characterMana = root.Q<Label>("characterMana");
        characterManaBarMissingMana = root.Q<VisualElement>("characterManaBarMissingMana");
        characterManaBarInteractionResult = root.Q<VisualElement>("characterManaBarInteractionResult");

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
        if (!battleCharacterController.CanSelectAbility())
            return;

        // TODO: hardcoded ability indexes
        buttonClickQueue.Enqueue(HandleButtonClick(selectedPlayerStats.basicAbilities[0]));
    }

    void SButtonClicked()
    {
        if (!battleCharacterController.CanSelectAbility())
            return;

        // TODO: hardcoded ability indexes
        buttonClickQueue.Enqueue(HandleButtonClick(selectedPlayerStats.basicAbilities[1]));
    }

    void QButtonClicked()
    {
        if (!battleCharacterController.CanSelectAbility())
            return;

        // TODO: hardcoded ability indexes
        if (selectedPlayerStats.abilities[0] == null)
            return;

        buttonClickQueue.Enqueue(HandleButtonClick(selectedPlayerStats.abilities[0]));
    }

    void WButtonClicked()
    {
        if (!battleCharacterController.CanSelectAbility())
            return;

        // TODO: hardcoded ability indexes
        if (selectedPlayerStats.abilities[1] == null)
            return;

        buttonClickQueue.Enqueue(HandleButtonClick(selectedPlayerStats.abilities[1]));
    }

    void EButtonClicked()
    {
        if (!battleCharacterController.CanSelectAbility())
            return;

        // TODO: hardcoded ability indexes
        if (selectedPlayerStats.abilities[2] == null)
            return;

        buttonClickQueue.Enqueue(HandleButtonClick(selectedPlayerStats.abilities[2]));
    }

    void RButtonClicked()
    {
        if (!battleCharacterController.CanSelectAbility())
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
        battleCharacterController.selectedCharacter.GetComponent<FaceDirectionUI>().HideUI();

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

    public void ShowCharacterUI(CharacterStats _playerStats)
    {
        // current character is not in the scene, keep that in mind. It's a static scriptable object.
        selectedPlayerStats = _playerStats;

        characterName.text = selectedPlayerStats.character.characterName;
        characterPortrait.style.backgroundImage = selectedPlayerStats.character.portrait.texture;

        SetCharacterHealthMana(_playerStats);
        SetCharacteristics(_playerStats);
        HandleAbilityButtons();
        DisableSkillButtons();
        EnableSkillButtons();

        DOTween.To(() => characterUI.style.top.value.value, x => characterUI.style.top = Length.Percent(x), characterUIShowValue, 0.5f)
            .SetEase(Ease.InOutSine);
    }

    public void HideCharacterUI()
    {
        HideAbilityTooltip();
        selectedPlayerStats = null;
        DOTween.To(() => characterUI.style.top.value.value, x => characterUI.style.top = Length.Percent(x), characterUIHideValue, 0.5f)
            .SetEase(Ease.InOutSine); ;
    }


    void SetCharacterHealthMana(CharacterStats _playerStats)
    {
        characterHealth.text = _playerStats.currentHealth + "/" + _playerStats.maxHealth.GetValue();
        characterMana.text = _playerStats.currentMana + "/" + _playerStats.maxMana.GetValue();

        // (float) casts are not redundant, without them it does not work
        float missingHealthPerc = ((float)_playerStats.maxHealth.GetValue() - (float)_playerStats.currentHealth) / (float)_playerStats.maxHealth.GetValue();
        float missingManaPerc = ((float)_playerStats.maxMana.GetValue() - (float)_playerStats.currentMana) / (float)_playerStats.maxMana.GetValue();

        characterHealthBarMissingHealth.style.width = Length.Percent(missingHealthPerc * 100);
        characterManaBarMissingMana.style.width = Length.Percent(missingManaPerc * 100);
    }

    void SetCharacteristics(CharacterStats playerStats)
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

        for (int i = 0; i < abilityButtons.Count; i++)
        {
            if (selectedPlayerStats.abilities.Count <= i)
            {
                abilityButtons[i].style.display = DisplayStyle.None;
                continue;
            }

            // show buttons for each ability
            abilityButtons[i].style.display = DisplayStyle.Flex;
            abilityIcons[i].style.backgroundImage = selectedPlayerStats.abilities[i].aIcon.texture;
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
        if (selectedPlayerStats == null)
            return;

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

    // called by infocardUI
    public void ShowManaUse(int _val)
    {
        if (selectedPlayerStats == null)
            return;

        float currentMana = (float)selectedPlayerStats.currentMana;
        float manaAfterInteraction = (float)selectedPlayerStats.currentMana - _val;
        manaAfterInteraction = Mathf.Clamp(manaAfterInteraction, 0, selectedPlayerStats.currentMana);

        // text
        characterMana.text = manaAfterInteraction + "/" + selectedPlayerStats.maxMana.GetValue();

        // bar
        float result = _val / (float)selectedPlayerStats.maxMana.GetValue();

        if (manaAfterInteraction == 0)
            result = currentMana / (float)selectedPlayerStats.maxMana.GetValue();

        characterManaBarInteractionResult.style.display = DisplayStyle.Flex;
        // reset right
        characterManaBarInteractionResult.style.right = Length.Percent(0);
        characterManaBarInteractionResult.style.width = Length.Percent(result * 100);

        // "animate it"
        AnimateManaUse();
    }

    public void HideManaUse()
    {
        DOTween.Pause(manaUseTweenID);

        characterManaBarInteractionResult.style.width = Length.Percent(0);
        if (selectedPlayerStats != null)
            SetCharacterHealthMana(selectedPlayerStats);

    }

    void AnimateManaUse()
    {
        characterManaBarInteractionResult.style.backgroundColor = damageBarColor;

        DOTween.ToAlpha(() => characterManaBarInteractionResult.style.backgroundColor.value,
                         x => characterManaBarInteractionResult.style.backgroundColor = x,
                         0f, 0.8f).SetLoops(-1, LoopType.Yoyo)
                         .SetId(manaUseTweenID);
    }


    // called by infocardUI
    public void ShowRetaliationResult(int _val)
    {
        if (selectedPlayerStats == null)
            return;

        characterPortraitSkull.style.display = DisplayStyle.None;

        float currentHealth = (float)selectedPlayerStats.currentHealth;
        float healthAfterInteraction = (float)selectedPlayerStats.currentHealth - _val;
        healthAfterInteraction = Mathf.Clamp(healthAfterInteraction, 0, selectedPlayerStats.currentHealth);

        // text
        characterHealth.text = healthAfterInteraction + "/" + selectedPlayerStats.maxHealth.GetValue();

        // bar
        float result = _val / (float)selectedPlayerStats.maxHealth.GetValue();

        if (healthAfterInteraction == 0)
            result = currentHealth / (float)selectedPlayerStats.maxHealth.GetValue();

        characterHealthBarRetaliationResult.style.display = DisplayStyle.Flex;
        // reset right
        characterHealthBarRetaliationResult.style.right = Length.Percent(0);
        characterHealthBarRetaliationResult.style.width = Length.Percent(result * 100);

        // death - TODO: display the skull
        if (healthAfterInteraction <= 0)
            characterPortraitSkull.style.display = DisplayStyle.Flex;

        // "animate it"
        AnimateRetaliationnResult();
    }

    public void HideRetaliationResult()
    {
        DOTween.Pause(missingBarTweenID);

        characterHealthBarRetaliationResult.style.width = Length.Percent(0);
        if (selectedPlayerStats != null)
            SetCharacterHealthMana(selectedPlayerStats);

    }

    void AnimateRetaliationnResult()
    {
        characterHealthBarRetaliationResult.style.backgroundColor = damageBarColor;

        DOTween.ToAlpha(() => characterHealthBarRetaliationResult.style.backgroundColor.value,
                         x => characterHealthBarRetaliationResult.style.backgroundColor = x,
                         0f, 0.8f).SetLoops(-1, LoopType.Yoyo)
                         .SetId(missingBarTweenID);
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
