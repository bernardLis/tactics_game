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

    public async Task WalkCharacterTo(GameObject character, Vector3 endPos, float speed = 2)
    {
        Vector3 finalPos = CheckPosition(endPos); // checks if position is taken
        _cameraManager.SetTarget(character.transform);
        // TODO: I should have a component that I can call to move someone from place to place (awaitable ideally)
        AILerp aiLerp = character.GetComponent<AILerp>();
        aiLerp.speed = speed;
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

    Vector3 CheckPosition(Vector3 pos)
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(pos, 0.2f);
        if (cols.Length == 0)
            return pos;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector3 newPos = pos + new Vector3(x, y);
                Collider2D[] newCols = Physics2D.OverlapCircleAll(newPos, 0.2f);
                if (newCols.Length == 0)
                    return newPos;
            }
        }

        return Vector3.zero;
    }


}
