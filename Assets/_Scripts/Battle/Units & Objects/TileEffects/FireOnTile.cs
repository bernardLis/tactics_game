using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class FireOnTile : TileEffect, ICreatable<Vector3, Ability>
{

    Vector3 _offsetVector = new Vector3(-0.16f, 0.21f, 0f);
    Collider2D _burnedCharacter;

    public override async Task Initialize(Vector3 pos, Ability ability)
    {
        await base.Initialize(pos, ability);
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
        Vector3 pos = new Vector3(transform.position.x + x, transform.position.y + y);

        Instantiate(this.gameObject, pos, Quaternion.identity);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"on trigger enter: {other.name}");
        if (other.TryGetComponent(out CharacterStats stats))
        {
            _burnedCharacter = other;
            Burn(stats);
        }
    }
    protected override void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"on trigger enter: {other.name}");
        if (other == _burnedCharacter)
        {
            // this won't work coz you can go back with the same character and get burned again...
            _selfCollider.enabled = true;
            _burnedCharacter = null;
        }
    }


    void Burn(CharacterStats stats)
    {
        // TODO: maybe spawn an effect?
        _selfCollider.enabled = false;
        stats.AddStatus(_ability.Status, _ability.CharacterGameObject);
    }


}
