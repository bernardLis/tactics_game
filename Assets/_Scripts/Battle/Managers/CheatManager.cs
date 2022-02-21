using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class CheatManager : MonoBehaviour
{
    UIDocument UIDocument;

    Button enemiesKillButton;

    Button friendliesKillButton;

    void Start()
    {
        // getting ui elements
        UIDocument = GetComponent<UIDocument>();
        var root = UIDocument.rootVisualElement;

        enemiesKillButton = root.Q<Button>("enemiesKillButton");
        friendliesKillButton = root.Q<Button>("friendliesKillButton");

        enemiesKillButton.clickable.clicked += KillAllEnemies;
        friendliesKillButton.clickable.clicked += KillAFriend;
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
        int currentHealth = stats.currentHealth;
        stats.TakeDamageNoDodgeNoRetaliation(currentHealth);

    }

    void KillAllWithTag(string _tag)
    {
        GameObject[] toKill = GameObject.FindGameObjectsWithTag(_tag);
        foreach (GameObject target in toKill)
        {
            CharacterStats stats = target.GetComponent<CharacterStats>();
            int currentHealth = stats.currentHealth;
            stats.TakeDamageNoDodgeNoRetaliation(currentHealth + 1);
        }

    }




}
