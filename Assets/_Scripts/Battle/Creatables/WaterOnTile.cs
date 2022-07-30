using UnityEngine;
using System.Threading.Tasks;

public class WaterOnTile : Creatable
{
    ObjectStats _objectStats;

    public bool _isElectrified { get; private set; }
    [SerializeField] GameObject _electrifiedWaterEffect;
    public Status _electrificationStatus { get; private set; }
    public GameObject _characterThatElectrified;

    public override async Task Initialize(Vector3 pos, Ability ability, string tag = "")
    {
        await base.Initialize(pos, ability, tag);
        _objectStats = GetComponent<ObjectStats>();
        CheckCollision(ability, pos);
        await CheckAround(); //if there is electrified water around, electrify yourself
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

            if (c.CompareTag(Tags.WaterOnTile) || c.CompareTag(Tags.PushableObstacle)
             || c.CompareTag(Tags.Obstacle) || c.CompareTag(Tags.BoundCollider))
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

    async Task CheckAround()
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector3 pos = new Vector3(transform.position.x + x, transform.position.y + y);
                if (await ResolveElectrify(pos))
                    return;
            }
        }
    }

    async Task<bool> ResolveElectrify(Vector3 pos)
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(pos, 0.1f);
        foreach (Collider2D c in cols)
        {
            if (c.TryGetComponent(out WaterOnTile water))
            {
                if (water._isElectrified)
                {
                    await ElectrifyWater(water._electrificationStatus, _ability.CharacterGameObject);
                    return true;
                }
            }
        }
        return false;
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

    async void Wet(CharacterStats stats)
    {
        // if burning, remove bruning
        if (ResolveBurnStatus(stats))
            return;

        // else add wet
        if (_ability != null)
            await stats.AddStatus(Status, _ability.CharacterGameObject);
        else
            await stats.AddStatus(Status, null);

    }

    async void ElectrifyCharacter(CharacterStats stats)
    {
        await stats.AddStatus(_electrificationStatus, _characterThatElectrified);
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

    public async Task ElectrifyWater(Status status, GameObject character)
    {
        if (_isElectrified)
            return;

        _isElectrified = true;

        _electrifiedWaterEffect.SetActive(true);

        _createdByTag = character.tag;
        _numberOfTurnsLeft = 3; // TODO: hardcoded
        _electrificationStatus = status;
        _characterThatElectrified = character;

        if (!_objectStats.IsElectrified)
            await _objectStats.AddStatus(status, character);

        // refresh card
        InfoCardUI.Instance.ShowTileInfo(transform.position);
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
            return "Electrified Water. Electrifies characters.";
        return "Water. Makes characters wet. Can be electrified.";
    }

    public override async Task DestroySelf()
    {
        // TODO: an effect;
        await base.DestroySelf();
    }

}
