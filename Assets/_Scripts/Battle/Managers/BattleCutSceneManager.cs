using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using Pathfinding;
public class BattleCutSceneManager : Singleton<BattleCutSceneManager>
{

    BattleCameraManager _cameraManager;

    protected override void Awake()
    {
        base.Awake();
        _cameraManager = Camera.main.GetComponent<BattleCameraManager>();
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

        if (character.TryGetComponent(out PlayerCharSelection selection))
            selection.SetPositionTurnStart(endPos); // without this if you move character after they are walked, and click back they go back to the old position

        aiLerp.SetPath(path);
        aiLerp.destination = endPos;

        while (!aiLerp.reachedEndOfPath)
            await Task.Yield();
    }

}
