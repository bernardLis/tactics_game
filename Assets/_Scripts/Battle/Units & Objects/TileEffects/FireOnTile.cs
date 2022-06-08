using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;

public class FireOnTile : TileEffect
{
    BattleCharacterController _battleCharacterController;

    [SerializeField] Status _status;
    [SerializeField] GameObject _spreadEffect;

    CharacterStats _characterBurnedThisTurn = null;
    bool _isInitialized;

    public override async Task Initialize(Vector3 pos, Ability ability, string tag = "")
    {
        await base.Initialize(pos, ability, tag);
        _battleCharacterController = BattleCharacterController.Instance;

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

            if (c.CompareTag(Tags.WaterOnTile))
            {
                Destroy(c.gameObject);
                Destroy(gameObject);
                continue;
            }

            if (c.CompareTag(Tags.FireOnTile) || c.CompareTag(Tags.Obstacle) || c.CompareTag(Tags.BoundCollider))
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

    protected override async Task DecrementTurnsLeft()
    {
        if (!gameObject.activeSelf)
            return;
        await Spread();
        await base.DecrementTurnsLeft();

    }

    async Task Spread()
    {
        if (gameObject == null)
            return;

        Vector3 pos = ChooseSpreadPosition();

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
            await newFire.GetComponent<FireOnTile>().Initialize(pos, _ability, _createdByTag);
        }

        await Task.Yield();

    }

    Vector3 ChooseSpreadPosition()
    {
        Vector3 pos = Vector3.zero;

        int change = 1;
        if (Random.Range(0, 2) == 0)
            change = -1;

        pos = new Vector3(transform.position.x, transform.position.y + change);
        if (Random.Range(0, 2) == 0)
            pos = new Vector3(transform.position.x + change, transform.position.y);

        return pos;
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

    async void Burn(CharacterStats stats)
    {
        _characterBurnedThisTurn = stats;

        if (stats.IsWet)
        {
            RemoveWetStatus(stats);
            await stats.ShakeOnDamageTaken();
            return;
        }

        stats.AddStatus(_status, _ability.CharacterGameObject);
        if (_battleCharacterController.HasCharacterStartedMoving)
            stats.WalkedThroughFire(_status);
    }


    void RemoveWetStatus(CharacterStats stats)
    {
        string wetReferenceId = "WetStatus";

        foreach (Status s in stats.Statuses)
            if (s.ReferenceID == wetReferenceId)
            {
                stats.RemoveStatus(s);
                return;
            }
    }

    public override string DisplayText()
    {
        return $"Spreads. Burns anyone who walks through it. Lasts for {_numberOfTurnsLeft} turn/s.";
    }

    public override void DestroySelf()
    {
        // TODO: different effect;
        Destroy(Instantiate(_spreadEffect, transform.position, Quaternion.identity), 1f);
        base.DestroySelf();
    }

}
