﻿using UnityEngine;

public class BasicCameraFollow : MonoBehaviour
{

    public Transform followTarget;
    Vector3 targetPos;
    public float moveSpeed;

    public static BasicCameraFollow instance;
    void Awake()
    {
        #region Singleton
        // singleton
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Camera Follow found");
            return;
        }
        instance = this;
        #endregion
    }


    void Update()
    {
        if (followTarget == null)
            return;

        // follow the target
        targetPos = new Vector3(followTarget.position.x, followTarget.position.y, transform.position.z);
        Vector3 velocity = (targetPos - transform.position) * moveSpeed;
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, 1.0f, Time.deltaTime);

    }
}

