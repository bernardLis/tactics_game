using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;

public class FireWallEffect : AbilityEffect
{
    Animator _animator;
    [SerializeField] GameObject _fireOnTile;

    string[] _animations = { "FireLionN", "FireLionW", "FireLionS", "FireLionE" };

    bool _animationFinished;

    public override async Task Play(Ability ability, Vector3 targetPos)
    {
        HighlightManager highlightManager = HighlightManager.Instance;
        List<WorldTile> highlightedTiles = new(highlightManager.HighlightedTiles);

        _animator = GetComponent<Animator>();

        // target pos is the first tile
        Vector3 dir = (targetPos - ability.CharacterGameObject.transform.position).normalized;
        _animator.Play(_animations[DirectionToIndex(dir, 4)]);
        foreach (WorldTile tile in highlightedTiles)
        {
            Vector3 endPos = tile.GetMiddleOfTile();
            transform.position = endPos;


            await WaitForAnimation();
            _animationFinished = false;

            GameObject fot = Instantiate(_fireOnTile, endPos, Quaternion.identity);
            await fot.GetComponent<FireOnTile>().Initialize(endPos, ability);
        }
        _animator.enabled = false;
        GetComponent<SpriteRenderer>().sprite = null;

        await Task.Delay(100);
        DestroySelf();
    }

    async Task WaitForAnimation()
    {
        while (!_animationFinished)
            await Task.Yield();
    }
    
    // animation event
    void AnimationFinished()
    {
        _animationFinished = true;
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
