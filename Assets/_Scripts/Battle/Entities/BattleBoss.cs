using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BattleBoss : BattleEntity
{

    // on start get path to home tile
    // move from tile to tile on the path until home tile reached
    NavMeshPath _path;
    public override void InitializeBattle(ref List<BattleEntity> opponents)
    {
        base.InitializeBattle(ref opponents);

        Debug.Log($" initialize battle");
        // get path to home tile
        StartCoroutine(GetPathToHomeTile());

    }

    IEnumerator GetPathToHomeTile()
    {

        _path = new();
        _agent.enabled = true;
        _agent.CalculatePath(new Vector3(7, 0, 11), _path);

        while (_agent.pathPending) yield return null;

        Debug.Log($"path calculated");
        foreach (Vector3 corner in _path.corners)
        {
            Debug.Log($"corner: {corner}");
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
            g.transform.position = corner;
        }

    }

    public override void StopRunEntityCoroutine()
    {
    }

    protected override IEnumerator RunEntity()
    {

        yield return new WaitForSeconds(100f);

    }

    public override void GetEngaged(BattleEntity engager)
    {
    }
}
