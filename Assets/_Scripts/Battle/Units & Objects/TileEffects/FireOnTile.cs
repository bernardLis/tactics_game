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

    CharacterStats _characterBurnedThisTurn = null;
    bool _isInitialized;
    bool _wasDestroyed;

    public override async Task Initialize(Vector3 pos, Ability ability)
    {
        await base.Initialize(pos, ability);

        transform.position = transform.position;

        _battleCharacterController = BattleCharacterController.Instance;

        _ability = ability;
        _numberOfTurnsLeft = 1; // TODO: hardcoded
        CheckCollision(ability, pos);
        _isInitialized = true;

        await Task.Yield();
    }

    protected override void CheckCollision(Ability ability, Vector3 pos)
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(pos, 0.2f);

        foreach (Collider2D c in cols)
        {
            if (c.gameObject == gameObject)
                continue;

            if (c.CompareTag(Tags.FireOnTile))
            {
                Destroy(gameObject);
                continue;
            }
            if (c.TryGetComponent(out CharacterStats stats))
            {
                Burn(stats);
                continue;
            }
        }
    }

    protected override async void DecrementTurnsLeft()
    {
        await Spread();
        //await Task.Delay(10); // without that it throws an error sometimes...
        base.DecrementTurnsLeft();
    }

    async Task Spread()
    {

        Debug.Log($"transform.position {transform.position}");
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
            if (c.CompareTag(Tags.Obstacle) || c.CompareTag(Tags.FireOnTile) || c.CompareTag(Tags.BoundCollider))
            {
                canSpread = false;
                continue;
            }
        }

        if (canSpread)
        {
            GameObject newFire = Instantiate(this.gameObject, pos, Quaternion.identity);
            await newFire.GetComponent<FireOnTile>().Initialize(pos, _ability);
        }

        await Task.Yield();
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isInitialized)
            return;

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
        Invoke("ResetCharacterBurnedThisTurn", 0.6f);
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
        return $"Spreads. Burns anyone who walks through it. Lasts for {_numberOfTurnsLeft} turns.";
    }

    public override void DestroySelf()
    {
        // TODO: different effect;
        Destroy(Instantiate(_spreadEffect, transform.position, Quaternion.identity), 1f);
        base.DestroySelf();
    }

}
