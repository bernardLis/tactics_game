using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class BattleAbilityManager : MonoBehaviour
{
    GameManager _gameManager;
    VisualElement _root;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameManager.Instance;
        _root = GetComponent<UIDocument>().rootVisualElement;

        AddAbilityButtons();
    }

    void AddAbilityButtons()
    {
        VisualElement abilityContainer = _root.Q<VisualElement>("AbilityContainer");
        List<Ability> abilities = _gameManager.HeroDatabase.GetAllAbilities();

        foreach (Ability ability in abilities)
        {
            AbilityButton button = new(ability);
            abilityContainer.Add(button);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
