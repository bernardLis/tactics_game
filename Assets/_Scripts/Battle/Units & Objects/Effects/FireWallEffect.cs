using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;

public class FireWallEffect : Effect
{
    Animator _animator;

    string[] _animations = { "FireLionN", "FireLionW", "FireLionS", "FireLionE" };

    public override async Task Play(Ability ability, Vector3 targetPos)
    {
        _animator = GetComponent<Animator>();

        // target pos is the first tile
        Vector3 dir = (targetPos - ability.CharacterGameObject.transform.position).normalized;
        Debug.Log($"dir: {dir}");
        _animator.Play(_animations[DirectionToIndex(dir, 4)]);
        Debug.Log($"target pos {targetPos} ");

        Vector3 endOfTheLine = MovePointController.Instance.gameObject.transform.position;
        // TODO: that's wrong
        // and move to target pos
        transform.DOMove(endOfTheLine, 3f).OnComplete(DestroySelf);
        await Task.Delay(3000);
        Debug.Log("after delay");
    }

    // from character renderer by Unity
    int DirectionToIndex(Vector2 dir, int sliceCount)
    {
        //get the normalized direction
        Vector2 normDir = dir.normalized;
        //calculate how many degrees one slice is
        float step = 360f / sliceCount;
        //calculate how many degress half a slice is.
        //we need this to offset the pie, so that the North (UP) slice is aligned in the center
        float halfstep = step / 2;
        //get the angle from -180 to 180 of the direction vector relative to the Up vector.
        //this will return the angle between dir and North.
        float angle = Vector2.SignedAngle(Vector2.up, normDir);

        //add the halfslice offset
        angle += halfstep;
        //if angle is negative, then let's make it positive by adding 360 to wrap it around.
        if (angle < 0)
        {
            angle += 360;
        }
        //calculate the amount of steps required to reach this angle
        float stepCount = angle / step;
        //round it, and we have the answer!
        return Mathf.FloorToInt(stepCount);
    }


}
