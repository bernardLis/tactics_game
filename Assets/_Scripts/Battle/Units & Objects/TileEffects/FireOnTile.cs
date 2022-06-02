using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Pathfinding;

public class FireOnTile : TileEffect//, ICreatable<Vector3, Ability>
{
    BattleCharacterController _battleCharacterController;

    Vector3 _offsetVector = new Vector3(-0.16f, 0.21f, 0f);

    CharacterStats _characterBurnedThisTurn = null;
    public override async Task Initialize(Vector3 pos, Ability ability)
    {
        await base.Initialize(pos, ability);
        _battleCharacterController = BattleCharacterController.Instance;

        transform.position = transform.position + _offsetVector;

        _ability = ability;
        _numberOfTurnsLeft = 3; // TODO: hardcoded

        await Task.Yield();
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
        int x = Random.Range(-1, 2);
        int y = Random.Range(-1, 2);
        // TODO: need to check whether it is a valid tile before I instantiate it
        if(Random.Range(0,2))
        Vector3 pos = new Vector3(transform.position.x + x, transform.position.y + y);
        // check if there is fire already on the tile
        // check if there is an obstacle
        // don't allow diagonal spreading
        // if spreading on the character, burn them
        // TODO: I do need some spawn effect


        Instantiate(this.gameObject, pos, Quaternion.identity);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"on trigger enter: {other.name}");
        if (other.TryGetComponent(out CharacterStats stats))
        {
            if (_characterBurnedThisTurn != null && _characterBurnedThisTurn == stats)
                return;
            if (_battleCharacterController.IsMovingBack)
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
