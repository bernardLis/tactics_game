using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleHero : MonoBehaviour
{
    public Hero Hero { get; private set; }
    public void InitializeHero(Hero hero)
    {
        Hero = hero;


    }
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.TryGetComponent(out BattleEntity entity))
        {
            Debug.Log($"entityname {entity.gameObject.name}");
        }

    }
    void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent(out BattleEntity entity))
        {
            Debug.Log($"entityname {entity.gameObject.name}");
        }
    }

}
