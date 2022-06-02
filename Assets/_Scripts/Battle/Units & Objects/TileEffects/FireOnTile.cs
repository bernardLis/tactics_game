using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Pathfinding;
using DG.Tweening;

public class FireOnTile : TileEffect
{
    BattleCharacterController _battleCharacterController;

    [SerializeField] GameObject _spreadEffect;

    Vector3 _offsetVector = new Vector3(-0.16f, 0.21f, 0f);
    CharacterStats _characterBurnedThisTurn = null;
    public override async Task Initialize(Vector3 pos, Ability ability)
    {
        await base.Initialize(pos, ability);

        Debug.Log("initialize");
        transform.position = transform.position + _offsetVector;

        _battleCharacterController = BattleCharacterController.Instance;


        _ability = ability;
        _numberOfTurnsLeft = 3; // TODO: hardcoded
        CheckCollision(ability, pos);

        await Task.Yield();
    }

    void CheckCollision()
    {
        // don't allow multiple fires on one tile
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 0.2f);
        foreach (Collider2D c in cols)
            if (c.CompareTag(Tags.FireOnTile))
            {
                Debug.Log("destory");
                Destroy(gameObject);

            }
    }

    protected override void DecrementTurnsLeft()
    {
        base.DecrementTurnsLeft();
        Spread();
    }

    void Spread()
    {
        // TODO: I want fire to spread every turn to a random tile
        Debug.Log("fire spreads");
        int change = 1;
        if (Random.Range(0, 2) == 0)
            change = -1;
        Vector3 pos = Vector3.zero;
        if (Random.Range(0, 2) == 0)
            pos = new Vector3(transform.position.x + change, transform.position.y);
        else
            pos = new Vector3(transform.position.x, transform.position.y + change);

        GameObject sEffect = Instantiate(_spreadEffect, transform.position, Quaternion.identity);
        sEffect.transform.DOMove(pos, 0.5f);
        Destroy(sEffect, 1f);

        Collider2D[] cols = Physics2D.OverlapCircleAll(pos, 0.2f);

        bool canSpread = true;
        foreach (Collider2D c in cols)
        {
            if (c.TryGetComponent(out CharacterStats stats))
            {
                Burn(stats);
                continue;
            }
            if (c.CompareTag(Tags.FireOnTile))
            {
                canSpread = false;
                continue;
            }
            if (c.CompareTag(Tags.Obstacle))
            {
                canSpread = false;
                continue;
            }
        }

        if (canSpread)
            Instantiate(this.gameObject, pos, Quaternion.identity);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"on trigger enter: {other.name}");
        Debug.Log($"_battleCharacterController: {_battleCharacterController}");

        if (other.TryGetComponent(out CharacterStats stats))
        {
            if (_characterBurnedThisTurn != null && _characterBurnedThisTurn == stats)
                return;
            if (_battleCharacterController != null && _battleCharacterController.IsMovingBack)
                return;

            Burn(stats);
        }
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"on trigger exit: {other.name}");
        // I am worried that if there is fire on tiles next to each other 
        // character will shake from one tile to another on damage dealt. 
        Invoke("ResetCharacterBurnedThisTurn", 0.6f); // character shakes for 0.5s
    }

    void ResetCharacterBurnedThisTurn()
    {
        _characterBurnedThisTurn = null;
    }

    void Burn(CharacterStats stats)
    {
        _characterBurnedThisTurn = stats;
        stats.AddStatus(_ability.Status, _ability.CharacterGameObject);
        stats.WalkedThroughFire(_ability.Status);
    }

    public override string DisplayText()
    {
        return $"Spreads. Burns character that walks through it. Lasts for {_numberOfTurnsLeft} turns.";
    }

}
