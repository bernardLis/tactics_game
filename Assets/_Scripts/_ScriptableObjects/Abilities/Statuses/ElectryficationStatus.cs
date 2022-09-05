using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "ScriptableObject/Statuses/Electryfication")]
public class ElectryficationStatus : Status
{
    public GameObject Effect;
    GameObject _effectInstance;

    public override async Task FirstTrigger()
    {
        await base.FirstTrigger();
        AddFlag();
        _effectInstance = Instantiate(Effect, _characterGameObject.transform.position, Quaternion.identity);
        ElectricLineController effectController = _effectInstance.GetComponent<ElectricLineController>();
        Vector3 startPositionRandomized = new Vector3(_characterGameObject.transform.position.x + Random.Range(-0.5f, 0.5f),
                                              _characterGameObject.transform.position.y + Random.Range(-0.5f, 0.5f));

        effectController.Electrify(startPositionRandomized);

        // spawn effect on the tile
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector3 pos = new Vector3(startPositionRandomized.x + x, startPositionRandomized.y + y);
                Vector3 endPosistionRandomized = new Vector3(pos.x + Random.Range(0, 0.5f),
                                             pos.y + Random.Range(0, 0.5f));
                effectController.AddPosition(endPosistionRandomized);

                await SpreadElectrification(pos);
                await Task.Delay(20);
            }
        }

        if (_effectInstance != null)
            Destroy(_effectInstance, 1f);
    }

    public async override Task TriggerStatus()
    {
        await base.TriggerStatus();

        if (_characterStats == null)
            return;

        int dmg = Value;
        if (_characterStats.IsWet)
            dmg *= 2;
        await _characterStats.TakeDamageFinal(dmg);
    }

    async Task SpreadElectrification(Vector3 pos)
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(pos, 0.2f);
        foreach (Collider2D c in cols)
        {
            if (c == null)
                return;
            if (c.TryGetComponent(out CharacterStats stats)) // electrify characters
                if (!stats.IsElectrified)
                    await stats.AddStatus(this, Attacker);
            if (c != null && c.TryGetComponent(out WaterOnTile waterOnTile))
                await waterOnTile.ElectrifyWater(this, Attacker);
        }
    }

    public override void AddFlag()
    {
        if (_baseStats == null)
            return;

        _baseStats.SetIsElectrified(true);
    }

    public override void ResetFlag()
    {
        if (_baseStats == null)
            return;

        _baseStats.SetIsElectrified(false);
    }

    public override string GetDescription()
    {
        return "Electrified for " + NumberOfTurns + " turn/s.";
    }
}
