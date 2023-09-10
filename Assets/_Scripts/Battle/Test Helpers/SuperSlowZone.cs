using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

public class SuperSlowZone : MonoBehaviour
{

    // Start is called before the first frame update

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out BattleCreature bc))
        {
            bc.Creature.Speed.ApplyBonusValueChange(-30);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.TryGetComponent(out BattleCreature bc))
        {
            bc.Creature.Speed.ApplyBonusValueChange(30);
        }
    }


}
