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
    Character currentCharacter;

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
        Debug.Log("q button clicked");
        if (!characterBattleController.canInteract())
            return;

        currentCharacter.characterAbilities[0].HighlightTargetable();
    }
    void WButtonClicked()
    {
        Debug.Log("w button clicked");
        if (!characterBattleController.canInteract())
            return;

        currentCharacter.characterAbilities[1].HighlightTargetable();
    }
    void EButtonClicked()
    {
        Debug.Log("E button clicked");
        if (!characterBattleController.canInteract())
            return;

        currentCharacter.characterAbilities[2].HighlightTargetable();
    }
    void RButtonClicked()
    {
        Debug.Log("r button clicked");
        if (!characterBattleController.canInteract())
            return;

        currentCharacter.characterAbilities[3].HighlightTargetable();
    }
    void TButtonClicked()
    {
        Debug.Log("T button clicked - basic attack");
        if (!characterBattleController.canInteract())
            return;
    }
    void YButtonClicked()
    {
        Debug.Log("Y button clicked - basic defend");
        if (!characterBattleController.canInteract())
            return;
    }

    public void ShowCharacterUI(int currentHealth, int currentMana, Character _character)
    {
        currentCharacter = _character;

        characterUIContainer.style.display = DisplayStyle.Flex;

        characterName.text = _character.characterName;
        characterPortrait.style.backgroundImage = _character.portrait.texture;
        characterHealth.text = currentHealth + "/" + _character.maxHealth;
        characterMana.text = currentMana + "/" + _character.maxMana;

        characterQSkillIcon.style.backgroundImage = _character.characterAbilities[0].aIcon.texture;
        characterWSkillIcon.style.backgroundImage = _character.characterAbilities[1].aIcon.texture;
        characterESkillIcon.style.backgroundImage = _character.characterAbilities[2].aIcon.texture;
        characterRSkillIcon.style.backgroundImage = _character.characterAbilities[3].aIcon.texture;
    }

    public void HideCharacterUI()
    {
        currentCharacter = null;

        characterUIContainer.style.display = DisplayStyle.None;
    }

    // for keyboard input
    public void SimulateQButtonClicked()
    {
        Debug.Log("simulated q button clicked");
        // https://forum.unity.com/threads/trigger-button-click-from-code.1124356/
        using (var e = new NavigationSubmitEvent() { target = characterQButton })
            characterQButton.SendEvent(e);
    }

    public void SimulateWButtonClicked()
    {
        Debug.Log("simulated W button clicked");
        using (var e = new NavigationSubmitEvent() { target = characterWButton })
            characterWButton.SendEvent(e);
    }

    public void SimulateEButtonClicked()
    {
        Debug.Log("simulated E button clicked");
        using (var e = new NavigationSubmitEvent() { target = characterEButton })
            characterEButton.SendEvent(e);
    }

    public void SimulateRButtonClicked()
    {
        Debug.Log("simulated R button clicked");
        using (var e = new NavigationSubmitEvent() { target = characterRButton })
            characterRButton.SendEvent(e);
    }

    public void SimulateTButtonClicked()
    {
        Debug.Log("simulated T button clicked");
        using (var e = new NavigationSubmitEvent() { target = characterTButton })
            characterTButton.SendEvent(e);
    }

    public void SimulateYButtonClicked()
    {
        Debug.Log("simulated Y button clicked");
        using (var e = new NavigationSubmitEvent() { target = characterYButton })
            characterYButton.SendEvent(e);
    }

}
