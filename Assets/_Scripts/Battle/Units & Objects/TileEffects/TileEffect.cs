using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class TileEffect : MonoBehaviour, IUITextDisplayable, ICreatable<Vector3, Ability>
{
    protected BoxCollider2D _selfCollider;

    protected Ability _ability;
    protected int _numberOfTurnsLeft;

    public virtual async Task Initialize(Vector3 pos, Ability ability)
    {
        _selfCollider = GetComponent<BoxCollider2D>();
        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;

        await Task.Yield();
    }

    protected virtual void CheckCollision(Ability ability, Vector3 pos)
    {
        // meant to be overwritten
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        // meant to be overwritten

    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        // meant to be overwritten
    }

    void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (_ability.CharacterGameObject.CompareTag(Tags.Player) && state == BattleState.PlayerTurn)
            DecrementTurnsLeft();

        if (_ability.CharacterGameObject.CompareTag(Tags.Enemy) && state == BattleState.EnemyTurn)
            DecrementTurnsLeft();
    }

    protected virtual void DecrementTurnsLeft()
    {
        Debug.Log("in decreement turns");
        _numberOfTurnsLeft -= 1;
        if (_numberOfTurnsLeft <= 0)
            DestroySelf();
    }

    public virtual void DestroySelf()
    {
        TurnManager.OnBattleStateChanged -= TurnManager_OnBattleStateChanged;
        Debug.Log("in destory self");

        if (gameObject != null)
            Destroy(gameObject);
    }

    public virtual string DisplayText()
    {
        // meant to be overwritten
        return null;
    }



}
