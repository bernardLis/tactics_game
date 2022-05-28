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

        // spawn effect on the tile
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                // TODO: it would be cool if I was able to draw and effect from  
                Vector3 pos = new Vector3(_characterGameObject.transform.position.x + x, _characterGameObject.transform.position.y + y);
                GameObject effectInstance = Instantiate(Effect, _characterGameObject.transform.position, Quaternion.identity);
                Destroy(effectInstance, 1f);
                effectInstance.GetComponent<ElectricLineController>().Electrify(_characterGameObject.transform.position, pos);
                SpreadElectrification(pos);
                await Task.Delay(300);
            }
        }
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
