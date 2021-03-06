using UnityEngine;

// it's a script from unity's tilemap project
public class CharacterRenderer : MonoBehaviour
{
    public static readonly string[] StaticDirections = { "Idle N", "Idle W", "Idle S", "Idle E" };
    public static readonly string[] RunDirections = { "Walk N", "Walk W", "Walk S", "Walk E" };

    Animator _animator;
    int _lastDirection;

    void Awake()
    {
        //cache the animator component
        _animator = GetComponent<Animator>();
    }

    public void SetDirection(Vector2 direction)
    {
        //use the Run states by default
        string[] directionArray;

        //measure the magnitude of the input.
        if (direction.magnitude < 0.01f)
        {
            //if we are basically standing still, we'll use the Static states
            //we won't be able to calculate a direction if the user isn't pressing one, anyway!
            directionArray = StaticDirections;
        }
        else
        {
            //we can calculate which direction we are going in
            //use DirectionToIndex to get the index of the slice from the direction vector
            //save the answer to lastDirection
            directionArray = RunDirections;
            _lastDirection = DirectionToIndex(direction, 4);
        }

        //tell the animator to play the requested state
        if (_animator.runtimeAnimatorController != null && _animator.gameObject.activeSelf)
        {
            if (_lastDirection >= directionArray.Length)
            {
                Debug.LogWarning($"omg what is ahppening, lastDirection: {_lastDirection}, directionArray.Length: {directionArray.Length}");
                _lastDirection = 3;
            }
            _animator.Play(directionArray[_lastDirection]);

        }
    }

    //helper functions

    //this function converts a Vector2 direction to an index to a slice around a circle
    //this goes in a counter-clockwise direction.
    public static int DirectionToIndex(Vector2 dir, int sliceCount)
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

    //this function converts a string array to a int (animator hash) array.
    public static int[] AnimatorStringArrayToHashArray(string[] animationArray)
    {
        //allocate the same array length for our hash array
        int[] hashArray = new int[animationArray.Length];
        //loop through the string array
        for (int i = 0; i < animationArray.Length; i++)
        {
            //do the hash and save it to our hash array
            hashArray[i] = Animator.StringToHash(animationArray[i]);
        }
        //we're done!
        return hashArray;
    }
}
