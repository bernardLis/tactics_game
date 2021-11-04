using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushTriggerable : MonoBehaviour
{
    CharacterStats myStats;
    CharacterStats targetStats;
    CharacterRendererManager characterRendererManager;

    Pushable pushable;

    void Awake()
    {
        myStats = GetComponent<CharacterStats>();
        characterRendererManager = GetComponentInChildren<CharacterRendererManager>();
    }

    public bool Push(GameObject target, int manaCost)
    {
        pushable = target.GetComponent<Pushable>();
        if (pushable == null)
            return false;

        if (myStats.currentMana < manaCost)
            return false;

        // face the target character
        Vector2 dir = target.transform.position - transform.position;
        characterRendererManager.Face(dir);

        // player can push characters/stones
        // TODO: pushing characters with lerp breaks the A*
        Vector3 pushDir = (target.transform.position - transform.position).normalized;

        pushable = target.GetComponent<Pushable>();
        pushable.IsPushed(pushDir);
        return true;
    }
}
