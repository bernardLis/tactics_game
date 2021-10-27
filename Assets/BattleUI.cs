using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleUI : MonoBehaviour
{
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
    Button characterDButton;
    Button characterFButton;

    VisualElement characterQSkillIcon;
    VisualElement characterWSkillIcon;
    VisualElement characterESkillIcon;
    VisualElement characterRSkillIcon;

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
        characterDButton = root.Q<Button>("characterDButton");
        characterFButton = root.Q<Button>("characterFButton");

        characterQSkillIcon = root.Q<VisualElement>("characterQSkillIcon");
        characterWSkillIcon = root.Q<VisualElement>("characterWSkillIcon");
        characterESkillIcon = root.Q<VisualElement>("characterESkillIcon");
        characterRSkillIcon = root.Q<VisualElement>("characterRSkillIcon");
    }

    // TODO: 
    // * hook up buttons and interactions with character battle controller
    // * on hover or on selection show ability tooltip with some info
    // * update character UI on interaction 


    public void ShowCharacterUI(int currentHealth, int currentMana, Character _character)
    {
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

    public void HideCharacterIU()
    {
        characterUIContainer.style.display = DisplayStyle.None;
    }
}
