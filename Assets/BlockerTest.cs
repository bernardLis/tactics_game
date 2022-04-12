using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class BlockerTest : MonoBehaviour
{
    SingleNodeBlocker _blocker;
    public void Start()
    {
        _blocker = GetComponent<SingleNodeBlocker>();
        _blocker.BlockAtCurrentPosition();
    }

    void Update()
    {
        _blocker.BlockAtCurrentPosition();
    }
}
