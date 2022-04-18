using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class CheatManager : MonoBehaviour
{
    Button _enemiesKillButton;
    Button _friendliesKillButton;

    PlayerInput _playerInput;

    [SerializeField] GameObject _boulder;

    void Start()
    {
        // getting ui elements
        var root = GetComponent<UIDocument>().rootVisualElement;

        _enemiesKillButton = root.Q<Button>("enemiesKillButton");
        _friendliesKillButton = root.Q<Button>("friendliesKillButton");

        _enemiesKillButton.clickable.clicked += KillAllEnemies;
        _friendliesKillButton.clickable.clicked += KillAFriend;

        _playerInput = BattleInputController.Instance.GetComponent<PlayerInput>();
    }

    void OnEnable()
    {
        // inputs
        _playerInput = BattleInputController.Instance.GetComponent<PlayerInput>();

        SubscribeInputActions();
    }

    void OnDisable()
    {
        if (_playerInput == null)
            return;

        UnsubscribeInputActions();
    }
    void SubscribeInputActions()
    {
        // char placement specific for now
        _playerInput.actions["Cheat1"].performed += SpawnBoulder;
        _playerInput.actions["Cheat2"].performed += DoSomething;
    }

    void UnsubscribeInputActions()
    {
        // char placement specific for now
        _playerInput.actions["Cheat1"].performed -= SpawnBoulder;
        _playerInput.actions["Cheat2"].performed -= DoSomething;
    }

    void SpawnBoulder(InputAction.CallbackContext ctx)
    {
        Instantiate(_boulder, MovePointController.Instance.transform.position, Quaternion.identity);
    }

    void DoSomething(InputAction.CallbackContext ctx)
    {

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
        stats.TakeDamageNoDodgeNoRetaliation(currentHealth).GetAwaiter();

    }

    void KillAllWithTag(string tag)
    {
        GameObject[] toKill = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject target in toKill)
        {
            CharacterStats stats = target.GetComponent<CharacterStats>();
            int currentHealth = stats.CurrentHealth;
            stats.TakeDamageNoDodgeNoRetaliation(currentHealth + 1).GetAwaiter();
        }

    }




}
