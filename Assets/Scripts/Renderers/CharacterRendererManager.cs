using System.Collections;
using UnityEngine;
using Pathfinding;

public class CharacterRendererManager : MonoBehaviour
{
    public Vector2 direction;
    public Vector2 lastDirection;

    Animator animator;
    CharacterRenderer characterRenderer;

    WeaponHolder weaponHolder;
    SpriteRenderer weaponRenderer;

    AILerp AI;

    bool noIdleAnimation; // TODO: wat is dat?

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        characterRenderer = GetComponent<CharacterRenderer>();
        AI = GetComponentInParent<AILerp>();
        weaponHolder = GetComponentInChildren<WeaponHolder>();
        weaponRenderer = weaponHolder.transform.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    // TODO: something smarter I probably don't need to set direction when character movement is not active
    void Update()
    {
        if (AI.canMove)
            direction = AI.myDirection;

        if (direction.sqrMagnitude > 0)
            lastDirection = direction;
        
        // TODO: there is probably way to improve that;
        if (!noIdleAnimation)
        {
            characterRenderer.SetDirection(direction);

            foreach (Transform child in transform)
            {
                CharacterRenderer cr = child.GetComponent<CharacterRenderer>();
                if (cr != null)
                    cr.SetDirection(direction);
            }
        }
    }

    // TODO: noone calls it
    // TODO: no weapon holder
    public void AttackAnimation(Vector2 dir)
    {
        if (weaponHolder.weapon == null)
            return;

        if (weaponHolder.weapon.weaponType == WeaponType.SLASH)
            StartCoroutine(Slash(dir));
        if (weaponHolder.weapon.weaponType == WeaponType.THRUST)
            StartCoroutine(Thrust(dir));
        if (weaponHolder.weapon.weaponType == WeaponType.SHOOT)
            StartCoroutine(Shoot(dir));
    }

    public void SpellcastAnimation(Vector2 dir)
    {
        StartCoroutine(Spellcast(dir));
    }

    public void Face(Vector2 dir)
    {
        direction = dir;

        characterRenderer.SetDirection(direction);

        foreach (Transform child in transform)
        {
            CharacterRenderer cr = child.GetComponent<CharacterRenderer>();
            if (cr != null)
                cr.SetDirection(direction);
        }
    }

    public void PlayDieAnimation()
    {
        StartCoroutine(Die());
    }

    IEnumerator Die()
    {
        noIdleAnimation = true;

        animator.Play("Hurt");

        foreach (Transform child in transform)
        {
            Animator an = child.GetComponent<Animator>();

            if (an != null && an.runtimeAnimatorController != null && an.gameObject.activeSelf)
                an.Play("Hurt");
        }

        yield return new WaitForSeconds(0.5f);
        noIdleAnimation = false;
    }

    // TODO: code repetition between all 3 methods.
    IEnumerator Spellcast(Vector2 dir)
    {        
        noIdleAnimation = true;

        // set direction and play animation
        animator.SetFloat("Last Horizontal", dir.x);
        animator.SetFloat("Last Vertical", dir.y);
        animator.Play("Spellcast");

        foreach (Transform child in transform)
        {
            if (child.CompareTag("Weapon"))
                continue;

            Animator an = child.GetComponent<Animator>();
            if (an != null && an.runtimeAnimatorController != null && an.gameObject.activeSelf)
            {
                // set direction and play animation
                an.SetFloat("Last Horizontal", dir.x);
                an.SetFloat("Last Vertical", dir.y);
                an.Play("Spellcast");
            }
        }

        // TODO: this is not perfect, waiting for animation to finish
        yield return new WaitForSeconds(0.5f);

        Face(dir);

        noIdleAnimation = false;
    }

    IEnumerator Slash(Vector2 dir)
    {
        noIdleAnimation = true;

        weaponHolder.gameObject.SetActive(true);
        animator.SetFloat("Last Horizontal", dir.x);
        animator.SetFloat("Last Vertical", dir.y);
        animator.Play("Slash");

        foreach (Transform child in transform)
        {
            Animator an = child.GetComponent<Animator>();
            if (an != null && an.runtimeAnimatorController != null && an.gameObject.activeSelf)
            {
                an.SetFloat("Last Horizontal", dir.x);
                an.SetFloat("Last Vertical", dir.y);
                an.Play("Slash");

            }
        }
        // TODO: this is not perfect, waiting for animation to finish
        yield return new WaitForSeconds(0.8f);

        weaponHolder.gameObject.SetActive(false);
        weaponRenderer.sprite = null;
        Face(dir);

        noIdleAnimation = false;
    }

    IEnumerator Thrust(Vector2 dir)
    {
        noIdleAnimation = true;

        weaponHolder.gameObject.SetActive(true);

        animator.SetFloat("Last Horizontal", dir.x);
        animator.SetFloat("Last Vertical", dir.y);
        animator.Play("Thrust");

        foreach (Transform child in transform)
        {
            Animator an = child.GetComponent<Animator>();
            if (an != null && an.runtimeAnimatorController != null && an.gameObject.activeSelf)
            {
                an.SetFloat("Last Horizontal", dir.x);
                an.SetFloat("Last Vertical", dir.y);
                an.Play("Thrust");
            }
        }
        // TODO: this is not perfect, waiting for animation to finish
        yield return new WaitForSeconds(0.8f);

        weaponHolder.gameObject.SetActive(false);
        weaponRenderer.sprite = null;
        Face(dir);

        noIdleAnimation = false;
    }

    IEnumerator Shoot(Vector2 dir)
    {
        noIdleAnimation = true;

        weaponHolder.gameObject.SetActive(true);

        animator.SetFloat("Last Horizontal", dir.x);
        animator.SetFloat("Last Vertical", dir.y);
        animator.Play("Bow");

        Animator arrowAnimator = weaponHolder.transform.Find("Arrow").GetComponent<Animator>();
        arrowAnimator.SetFloat("Last Horizontal", lastDirection.x);
        arrowAnimator.SetFloat("Last Vertical", lastDirection.y);
        arrowAnimator.Play("Bow");

        foreach (Transform child in transform)
        {
            Animator an = child.GetComponent<Animator>();
            if (an != null && an.runtimeAnimatorController != null && an.gameObject.activeSelf)
            {
                an.SetFloat("Last Horizontal", dir.x);
                an.SetFloat("Last Vertical", dir.y);
                an.Play("Bow");
            }
        }
        // TODO: this is not perfect, waiting for animation to finish
        yield return new WaitForSeconds(1.2f);

        weaponHolder.gameObject.SetActive(false);
        arrowAnimator.transform.GetComponent<SpriteRenderer>().sprite = null;
        weaponRenderer.sprite = null;
        Face(dir);

        noIdleAnimation = false;
    }
}
