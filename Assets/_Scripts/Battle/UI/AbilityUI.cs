using UnityEngine;
using UnityEngine.UIElements;

public class AbilityUI : MonoBehaviour
{
    VisualElement _abilityInfo;
    Label _abilityNameLabel;
    Label _abilityDescriptionLabel;
    Label _targetNameLabel;
    Label _abilityResultLabel;

    void Awake()
    {
        // getting ui elements
        var root = GetComponent<UIDocument>().rootVisualElement;

        _abilityInfo = root.Q<VisualElement>("abilityInfo");
        _abilityNameLabel = root.Q<Label>("abilityName");
        _abilityResultLabel = root.Q<Label>("abilityResult");
    }

    public void UpdateAbilityUI(string abilityName, string abilityResult)
    {
        _abilityNameLabel.text = abilityName;
        _abilityResultLabel.text = abilityResult;
    }

    public void ShowAbilityUI()
    {
        //show UI;
        _abilityInfo.style.display = DisplayStyle.Flex;
    }

    public void HideAbilityUI()
    {
        //show UI;
        _abilityInfo.style.display = DisplayStyle.None;
    }


}
