using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleUI : MonoBehaviour
{

    // global utility
    BattleInputController battleInputController;
    CharacterBattleController characterBattleController;

    // UI Elements
    UIDocument UIDocument;

    VisualElement characterUIContainer;

    VisualElement characterUITooltipContainer;
    Label characterUITooltipText;

    Label characterName;
    VisualElement characterPortrait;

    Label characterHealth;
    Label characterMana;

    Button characterQButton;
    Button characterWButton;
    Button characterEButton;
    Button characterRButton;
    Button characterTButton;
    Button characterYButton;

    VisualElement characterQSkillIcon;
    VisualElement characterWSkillIcon;
    VisualElement characterESkillIcon;
    VisualElement characterRSkillIcon;

    // local
    PlayerStats selectedPlayerStats;

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
        characterUITooltipText = root.Q<Label>("characterUITooltipText");

        characterName = root.Q<Label>("characterName");
        characterPortrait = root.Q<VisualElement>("characterPortrait");
        characterHealth = root.Q<Label>("characterHealth");
        characterMana = root.Q<Label>("characterMana");

        characterQButton = root.Q<Button>("characterQButton");
        characterWButton = root.Q<Button>("characterWButton");
        characterEButton = root.Q<Button>("characterEButton");
        characterRButton = root.Q<Button>("characterRButton");
        characterTButton = root.Q<Button>("characterTButton");
        characterYButton = root.Q<Button>("characterYButton");

        // register interaction callbacks (buttons)
        characterQButton.clickable.clicked += QButtonClicked;
        characterWButton.clickable.clicked += WButtonClicked;
        characterEButton.clickable.clicked += EButtonClicked;
        characterRButton.clickable.clicked += RButtonClicked;
        characterTButton.clickable.clicked += TButtonClicked;
        characterYButton.clickable.clicked += YButtonClicked;

        characterQSkillIcon = root.Q<VisualElement>("characterQSkillIcon");
        characterWSkillIcon = root.Q<VisualElement>("characterWSkillIcon");
        characterESkillIcon = root.Q<VisualElement>("characterESkillIcon");
        characterRSkillIcon = root.Q<VisualElement>("characterRSkillIcon");
    }

    void Start()
    {
        battleInputController = BattleInputController.instance;
        characterBattleController = CharacterBattleController.instance;
    }

    // TODO: 
    // * hook up buttons and interactions with character battle controller
    // * on hover or on selection show ability tooltip with some info
    // * update character UI on interaction


    // allow clicks only when not moving and character is selected & did not finish its turn
    void QButtonClicked()
    {
        if (!characterBattleController.CanInteract())
            return;

        selectedPlayerStats.abilities[0].HighlightTargetable();
        characterBattleController.SetSelectedAbility(selectedPlayerStats.abilities[0]);
    }

    void WButtonClicked()
    {
        if (!characterBattleController.CanInteract())
            return;

        selectedPlayerStats.abilities[1].HighlightTargetable();
        characterBattleController.SetSelectedAbility(selectedPlayerStats.abilities[1]);
    }

    void EButtonClicked()
    {
        if (!characterBattleController.CanInteract())
            return;

        selectedPlayerStats.abilities[2].HighlightTargetable();
        characterBattleController.SetSelectedAbility(selectedPlayerStats.abilities[2]);
    }

    void RButtonClicked()
    {
        if (!characterBattleController.CanInteract())
            return;

        selectedPlayerStats.abilities[3].HighlightTargetable();
        characterBattleController.SetSelectedAbility(selectedPlayerStats.abilities[3]);
    }

    void TButtonClicked()
    {
        Debug.Log("T button clicked - basic attack");
        if (!characterBattleController.CanInteract())
            return;
    }

    void YButtonClicked()
    {
        Debug.Log("Y button clicked - basic defend");
        if (!characterBattleController.CanInteract())
            return;
    }

    public void ShowCharacterUI(PlayerStats playerStats)
    {
        // current character is not in the scene, keep that in mind. It's a static scriptable object.
        selectedPlayerStats = playerStats;

        characterUIContainer.style.display = DisplayStyle.Flex;

        characterName.text = selectedPlayerStats.character.characterName;
        characterPortrait.style.backgroundImage = selectedPlayerStats.character.portrait.texture;
        characterHealth.text = selectedPlayerStats.currentHealth + "/" + selectedPlayerStats.maxHealth.GetValue();
        characterMana.text = selectedPlayerStats.currentMana + "/" + selectedPlayerStats.maxMana.GetValue();

        characterQSkillIcon.style.backgroundImage = selectedPlayerStats.abilities[0].aIcon.texture;
        characterWSkillIcon.style.backgroundImage = selectedPlayerStats.abilities[1].aIcon.texture;
        characterESkillIcon.style.backgroundImage = selectedPlayerStats.abilities[2].aIcon.texture;
        characterRSkillIcon.style.backgroundImage = selectedPlayerStats.abilities[3].aIcon.texture;
    }

    public void HideCharacterUI()
    {
        selectedPlayerStats = null;

        characterUIContainer.style.display = DisplayStyle.None;
    }

    // for keyboard input
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

    public void SimulateTButtonClicked()
    {
        using (var e = new NavigationSubmitEvent() { target = characterTButton })
            characterTButton.SendEvent(e);
    }

    public void SimulateYButtonClicked()
    {
        using (var e = new NavigationSubmitEvent() { target = characterYButton })
            characterYButton.SendEvent(e);
    }

}
