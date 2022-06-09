using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "ScriptableObject/Statuses/Electryfication")]
public class ElectryficationStatus : Status
{
    public GameObject Effect;

    public override async void FirstTrigger()
    {
        base.FirstTrigger();
        AddFlag();
        GameObject effectInstance = Instantiate(Effect, _selfGameObject.transform.position, Quaternion.identity);
        ElectricLineController effectController = effectInstance.GetComponent<ElectricLineController>();
        Vector3 startPositionRandomized = new Vector3(_selfGameObject.transform.position.x + Random.Range(0, 0.5f),
                                              _selfGameObject.transform.position.y + Random.Range(0, 0.5f));

        effectController.Electrify(startPositionRandomized);

        // spawn effect on the tile
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector3 pos = new Vector3(_selfGameObject.transform.position.x + x, _selfGameObject.transform.position.y + y);
                Vector3 endPosistionRandomized = new Vector3(pos.x + Random.Range(0, 0.5f),
                                             pos.y + Random.Range(0, 0.5f));
                effectController.AddPosition(endPosistionRandomized);

                SpreadElectrification(pos);
                await Task.Delay(50);
            }
        }
        if (effectInstance != null)
            Destroy(effectInstance, 1f);
    }

    public override void TriggerStatus()
    {
        base.TriggerStatus();

        if (_characterStats == null)
            return;

        int dmg = Value;
        if (_characterStats.IsWet)
            dmg *= 2;
        _characterStats.TakeDamageNoDodgeNoRetaliation(dmg).GetAwaiter();
    }

    void SpreadElectrification(Vector3 pos)
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(pos, 0.2f);
        foreach (Collider2D c in cols)
        {
            if (c.TryGetComponent(out CharacterStats stats)) // electrify characters
                if (!stats.IsElectrified)
                    stats.AddStatus(this, Attacker);
            if (c.TryGetComponent(out WaterOnTile waterOnTile))
            {
                Debug.Log("elkectrifying water");
                waterOnTile.ElectrifyWater(_selfGameObject, this);
            } // electrify water puddles
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
