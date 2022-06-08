using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class WaterOnTile : TileEffect
{
    [SerializeField] Status _status;


    public override async Task Initialize(Vector3 pos, Ability ability, string tag = "")
    {
        await base.Initialize(pos, ability, tag);

        CheckCollision(ability, pos);

        await Task.Yield();
    }

    protected override void CheckCollision(Ability ability, Vector3 pos)
    {
        Debug.Log($"pos: {pos}");
        Collider2D[] cols = Physics2D.OverlapCircleAll(pos, 0.1f);

        foreach (Collider2D c in cols)
        {
            if (c.gameObject == gameObject)
                continue;

            if (c.CompareTag(Tags.FireOnTile))
            {
                Destroy(c.gameObject);
                Destroy(gameObject);
                continue;
            }

            if (c.CompareTag(Tags.WaterOnTile) || c.CompareTag(Tags.Obstacle) || c.CompareTag(Tags.BoundCollider))
            {
                Destroy(gameObject);
                continue;
            }

            if (c.TryGetComponent(out CharacterStats stats))
            {
                Wet(stats);
                continue;
            }
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("on trigger enter");
        if (other.TryGetComponent(out CharacterStats stats))
            Wet(stats);
    }


    void Wet(CharacterStats stats)
    {
        Debug.Log($"wet {stats.gameObject.name}");

        // if burning, remove bruning
        if (ResolveBurnStatus(stats))
            return;

        // else add wet
        stats.AddStatus(_status, _ability.CharacterGameObject);
    }

    bool ResolveBurnStatus(CharacterStats stats)
    {
        string burnStatusId = "BurnStatus";

        foreach (Status s in stats.Statuses)
            if (s.ReferenceID == burnStatusId)
            {
                stats.RemoveStatus(s);
                return true;
            }

        return false;
    }
    protected override async Task DecrementTurnsLeft()
    {
        // I want it to last forever. 
        await Task.Yield();
        return;
    }
    public override string DisplayText()
    {
        return $"Makes characters walking through it wet.";
    }

    public override void DestroySelf()
    {
        // TODO: an effect;
        base.DestroySelf();
    }


}
