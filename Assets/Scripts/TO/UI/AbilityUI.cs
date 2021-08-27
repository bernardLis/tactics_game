using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AbilityUI : MonoBehaviour
{
	public UIDocument UIDocument;
	VisualElement abilityInfo;
	Label abilityNameLabel;
	Label abilityDescriptionLabel;
	Label targetNameLabel;
	Label abilityResultLabel;

	#region Singleton
	public static AbilityUI instance;
	void Awake()
	{
		// singleton
		if (instance != null)
		{
			Debug.LogWarning("More than one instance of AbilityUI found");
			return;
		}
		instance = this;

		#endregion

		// getting ui elements
		var root = UIDocument.rootVisualElement;
		abilityInfo = root.Q<VisualElement>("abilityInfo");
		abilityNameLabel = root.Q<Label>("abilityName");
		abilityResultLabel = root.Q<Label>("abilityResult");
	}

	public void UpdateAbilityUI(string abilityName, string abilityResult)
	{
		abilityNameLabel.text = abilityName;
		abilityResultLabel.text = abilityResult;
	}

	public void ShowAbilityUI()
	{
		//show UI;
		abilityInfo.style.display = DisplayStyle.Flex;
	}

	public void HideAbilityUI()
	{
		//show UI;
		abilityInfo.style.display = DisplayStyle.None;
	}


}
