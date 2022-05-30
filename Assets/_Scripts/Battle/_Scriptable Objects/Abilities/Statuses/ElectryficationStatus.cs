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
        _characterStats.SetIsElectrified(true);
        GameObject effectInstance = Instantiate(Effect, _characterGameObject.transform.position, Quaternion.identity);
        ElectricLineController effectController = effectInstance.GetComponent<ElectricLineController>();
        Vector3 startPositionRandomized = new Vector3(_characterGameObject.transform.position.x + Random.Range(0, 0.5f),
                                              _characterGameObject.transform.position.y + Random.Range(0, 0.5f));

        effectController.Electrify(startPositionRandomized);

        // spawn effect on the tile
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {

                Vector3 pos = new Vector3(_characterGameObject.transform.position.x + x, _characterGameObject.transform.position.y + y);
                Vector3 endPosistionRandomized = new Vector3(pos.x + Random.Range(0, 0.5f),
                                             pos.y + Random.Range(0, 0.5f));

                /*

                effectInstance.GetComponent<ElectricLineController>()
                              .Electrify(startPositionRandomized, endPosistionRandomized);
                */
                effectController.AddPosition(endPosistionRandomized);

                SpreadElectrification(pos);
                await Task.Delay(50);
            }
        }
        if (effectInstance != null)
            Destroy(effectInstance, 1f);
    }

    void SpreadElectrification(Vector3 pos)
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(pos, 0.2f);
        foreach (Collider2D c in cols)
            if (c.TryGetComponent(out CharacterStats stats))
                if (!stats.IsElectrified)
                    stats.AddStatus(this, _attacker);
    }

    public override void ResetFlag()
    {
        _characterStats.SetIsElectrified(false);
    }

    public override string GetDescription()
    {
        return "Electrified for " + NumberOfTurns + " turn/s.";
    }



}
