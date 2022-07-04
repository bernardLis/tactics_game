using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class TileEffect : MonoBehaviour, IUITextDisplayable, ICreatable<Vector3, Ability, string>
{
    protected BoxCollider2D _selfCollider;

    protected Ability _ability;
    protected int _numberOfTurnsLeft;
    protected string _createdByTag; // if character dies and his tileEffect is still on it throws an error

    public virtual async Task Initialize(Vector3 pos, Ability ability, string tag = "")
    {
        _selfCollider = GetComponent<BoxCollider2D>();

        _ability = Instantiate(ability); // clone it for safety
        if (_ability.CharacterGameObject != null)
            _createdByTag = _ability.CharacterGameObject.tag;
        else
            _createdByTag = tag;


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

    async void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (_createdByTag == Tags.Player && state == BattleState.PlayerTurn)
            await DecrementTurnsLeft();

        if (_createdByTag == Tags.Enemy && state == BattleState.EnemyTurn)
            await DecrementTurnsLeft();
    }

    protected virtual async Task DecrementTurnsLeft()
    {
        await Task.Delay(10);
        _numberOfTurnsLeft -= 1;
        if (_numberOfTurnsLeft <= 0)
            DestroySelf();
    }

    public virtual void DestroySelf()
    {
        TurnManager.OnBattleStateChanged -= TurnManager_OnBattleStateChanged;
        gameObject.SetActive(false);
        if (gameObject != null)
            Destroy(gameObject, 1f);
    }
    void OnDestroy()
    {
        DestroySelf();
    }


    public virtual string DisplayText()
    {
        // meant to be overwritten
        return null;
    }



}
