using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TileEffectSpawner : MonoBehaviour, IUITextDisplayable
{
    TurnManager _turnManager;
    [SerializeField] Creatable _tileEffectToSpawn;

    void Start()
    {
        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;
    }

    void OnDestroy()
    {
        TurnManager.OnBattleStateChanged -= TurnManager_OnBattleStateChanged;
    }

    void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (state == BattleState.PlayerTurn)
            HandlePlayerTurn();
    }

    async void HandlePlayerTurn()
    {
        Vector3 pos = ChooseSpawnPosition();
        if (pos == Vector3.zero)
            return;

        Creatable c = Instantiate(_tileEffectToSpawn, pos, Quaternion.identity);
        await c.Initialize(pos, null, null);
    }

    Vector3 ChooseSpawnPosition()
    {
        Vector3[] positions = new Vector3[8] {
            new Vector3(transform.position.x + 1, transform.position.y),
            new Vector3(transform.position.x - 1, transform.position.y),
            new Vector3(transform.position.x, transform.position.y + 1),
            new Vector3(transform.position.x, transform.position.y - 1),
            new Vector3(transform.position.x + 1, transform.position.y + 1),
            new Vector3(transform.position.x - 1, transform.position.y - 1),
            new Vector3(transform.position.x - 1, transform.position.y + 1),
            new Vector3(transform.position.x + 1, transform.position.y - 1)
        };

        // TODO: should it look for empty position? 
        
        if (positions.Length == 0)
            return Vector3.zero;

        return positions[Random.Range(0, positions.Length)];
    }

    public VisualElement DisplayText()
    {
        return new Label("Spawns a tile effect every turn.");
    }

}
