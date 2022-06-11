using UnityEngine;
using UnityEngine.InputSystem;

public class CheatManager : MonoBehaviour
{
    PlayerInput _playerInput;

    [SerializeField] GameObject _boulder;


    void Start()
    {
        _playerInput = GameManager.Instance.GetComponent<PlayerInput>();
    }

    void OnEnable()
    {
        // inputs
        _playerInput = GameManager.Instance.GetComponent<PlayerInput>();

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
        _playerInput.actions["Cheat1"].performed += SpawnBoulder;
        _playerInput.actions["Cheat2"].performed += DoSomething;
    }

    void UnsubscribeInputActions()
    {
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

    [ContextMenu("Kill All Enemies")]
    void KillAllEnemies()
    {

        KillAllWithTag("Enemy");
    }

    [ContextMenu("Kill A Friend")]
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
        stats.TakeDamageFinal(currentHealth).GetAwaiter();

    }

    void KillAllWithTag(string tag)
    {
        GameObject[] toKill = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject target in toKill)
        {
            CharacterStats stats = target.GetComponent<CharacterStats>();
            int currentHealth = stats.CurrentHealth;
            stats.TakeDamageFinal(currentHealth + 1).GetAwaiter();
        }

    }




}
