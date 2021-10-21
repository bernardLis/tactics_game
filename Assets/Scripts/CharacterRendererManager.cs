using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class CharacterRendererManager : MonoBehaviour
{
    Vector2 direction;
    Vector2 lastDirection;
    Animator animator;
    CharacterRenderer characterRenderer;
    WeaponHolder weaponHolder;
    SpriteRenderer weaponRenderer;
    AILerp AI;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        characterRenderer = GetComponent<CharacterRenderer>();
        AI = GetComponentInParent<AILerp>();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: something smarter I probably don't need to set direction when character movement is not active;
        direction = AI.myDirection;

        if (direction.sqrMagnitude > 0)
            lastDirection = direction;
        else
            AnimatorSetLastDir();

        characterRenderer.SetDirection(direction);

        foreach (Transform child in transform)
        {
            CharacterRenderer cr = child.GetComponent<CharacterRenderer>();
            if (cr != null)
                cr.SetDirection(direction);
        }
    }

    public void SetDirection(Vector2 dir)
    {
        if (dir.sqrMagnitude > 0)
            lastDirection = dir;
        else
            AnimatorSetLastDir();

        direction = dir;

    }
    // TODO: is that necessary in tactics game? 
    void AnimatorSetLastDir()
    {
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            animator.SetFloat("Last Horizontal", lastDirection.x);
            animator.SetFloat("Last Vertical", lastDirection.y);
        }

        foreach (Transform child in transform)
        {
            Animator an = child.GetComponent<Animator>();

            if (an == null || an.runtimeAnimatorController == null)
                continue;

            an.SetFloat("Last Horizontal", lastDirection.x);
            an.SetFloat("Last Vertical", lastDirection.y);
        }
    }
}
