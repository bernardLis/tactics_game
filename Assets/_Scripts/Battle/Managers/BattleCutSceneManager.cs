using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using Pathfinding;
public class BattleCutSceneManager : Singleton<BattleCutSceneManager>
{

    CameraManager _cameraManager;

    protected override void Awake()
    {
        base.Awake();
        _cameraManager = Camera.main.GetComponent<CameraManager>();
    }

    public async Task WalkCharacterTo(GameObject character, Vector3 endPos)
    {
        _cameraManager.SetTarget(character.transform);
        // TODO: I should have a component that I can call to move someone from place to place (awaitable ideally)
        AILerp aiLerp = character.GetComponent<AILerp>();
        aiLerp.speed = 2;
        // Create a new Path object
        ABPath path = ABPath.Construct(character.transform.position, endPos, null);
        // Calculate the path
        AstarPath.StartPath(path);
        AstarPath.BlockUntilCalculated(path);

        character.SetActive(true);
        aiLerp.SetPath(path);
        aiLerp.destination = endPos;

        while (!aiLerp.reachedEndOfPath)
            await Task.Yield();

        //        await Task.Delay(1000); // TODO: wrong, need to wait for character to get there...
    }

}
