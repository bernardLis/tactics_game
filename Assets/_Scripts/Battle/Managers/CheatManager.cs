using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class CheatManager : MonoBehaviour
{
    Button _enemiesKillButton;
    Button _friendliesKillButton;

    void Start()
    {
        // getting ui elements
        var root = GetComponent<UIDocument>().rootVisualElement;

        _enemiesKillButton = root.Q<Button>("enemiesKillButton");
        _friendliesKillButton = root.Q<Button>("friendliesKillButton");

        _enemiesKillButton.clickable.clicked += KillAllEnemies;
        _friendliesKillButton.clickable.clicked += KillAFriend;
    }

    void KillAllEnemies()
    {
        KillAllWithTag("Enemy");
    }

    void KillAFriend()
    {
        KillRandomWithTag("Player");
    }

    void KillRandomWithTag(string _tag)
    {
        GameObject[] toKill = GameObject.FindGameObjectsWithTag(_tag);
        GameObject target = toKill[Random.Range(0, toKill.Length)];
        CharacterStats stats = target.GetComponent<CharacterStats>();
        int currentHealth = stats.CurrentHealth;
        stats.TakeDamageNoDodgeNoRetaliation(currentHealth);

    }

    void KillAllWithTag(string tag)
    {
        GameObject[] toKill = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject target in toKill)
        {
            CharacterStats stats = target.GetComponent<CharacterStats>();
            int currentHealth = stats.CurrentHealth;
            stats.TakeDamageNoDodgeNoRetaliation(currentHealth + 1);
        }

    }




}
