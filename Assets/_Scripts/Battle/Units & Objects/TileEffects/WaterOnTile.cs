using UnityEngine;
using System.Threading.Tasks;

public class WaterOnTile : TileEffect
{
    ObjectStats _objectStats;

    [SerializeField] Status _wetStatus;

    bool _isElectrified;
    [SerializeField] Sprite _electrifiedWaterPuddleSprite;
    Status _electrificationStatus;
    GameObject _characterThatElectrified;

    public override async Task Initialize(Vector3 pos, Ability ability, string tag = "")
    {
        await base.Initialize(pos, ability, tag);
        _objectStats = GetComponent<ObjectStats>();
        CheckCollision(ability, pos);

        await Task.Yield();
    }

    protected override void CheckCollision(Ability ability, Vector3 pos)
    {
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
        if (other.TryGetComponent(out CharacterStats stats))
        {
            Wet(stats);
            if (_isElectrified)
                ElectrifyCharacter(stats);
        }
    }

    void Wet(CharacterStats stats)
    {
        // if burning, remove bruning
        if (ResolveBurnStatus(stats))
            return;

        // else add wet
        stats.AddStatus(_wetStatus, _ability.CharacterGameObject);
    }

    void ElectrifyCharacter(CharacterStats stats)
    {
        stats.AddStatus(_electrificationStatus, _characterThatElectrified);
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

    public void ElectrifyWater(Status status, GameObject character)
    {
        _isElectrified = true;

        GetComponentInChildren<SpriteRenderer>().sprite = _electrifiedWaterPuddleSprite;

        _createdByTag = character.tag;
        _numberOfTurnsLeft = 3; // TODO: hardcoded
        _electrificationStatus = status;
        _characterThatElectrified = character;

        if (!_objectStats.IsElectrified)
            _objectStats.AddStatus(status, character);

        //refresh card
        InfoCardUI.Instance.ShowTileInfo(DisplayText());
    }

    protected override async Task DecrementTurnsLeft()
    {
        // normal water puddles last forever. 
        if (!_isElectrified)
            return;

        await base.DecrementTurnsLeft();
    }

    public override string DisplayText()
    {
        if (_isElectrified)
            return "Electrifies characters walking through it.";
        return "Makes characters walking through it wet. Can be electrified.";
    }

    public override void DestroySelf()
    {
        // TODO: an effect;
        base.DestroySelf();
    }

}
