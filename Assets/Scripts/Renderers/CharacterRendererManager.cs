using System.Collections;
using UnityEngine;
using Pathfinding;

public class CharacterRendererManager : MonoBehaviour
{
    public Vector2 direction;
    Vector2 lastDirection;
    Animator animator;
    CharacterRenderer characterRenderer;
    WeaponHolder weaponHolder;
    SpriteRenderer weaponRenderer;
    AILerp AI;
    bool noIdleAnimation;

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        characterRenderer = GetComponent<CharacterRenderer>();
        AI = GetComponentInParent<AILerp>();
    }

    // Update is called once per frame
    // TODO: something smarter I probably don't need to set direction when character movement is not active
    void Update()
    {
        if (AI.canMove)
            direction = AI.myDirection;

        if (direction.sqrMagnitude > 0)
            lastDirection = direction;

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
    public void AttackAnimation()
    {
        if (weaponHolder.weapon == null)
            return;

        if (weaponHolder.weapon.weaponType == WeaponType.SLASH)
            StartCoroutine(Slash());
        if (weaponHolder.weapon.weaponType == WeaponType.THRUST)
            StartCoroutine(Thrust());
        if (weaponHolder.weapon.weaponType == WeaponType.SHOOT)
            StartCoroutine(Shoot());
    }

    public void Face(Vector2 dir)
    {
        // TODO: this is hacky and incorrect, make something smarter
        direction = dir;
        Invoke("DirectionZero", 0.1f);
    }

    void DirectionZero()
    {
        direction = Vector2.zero;
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

    IEnumerator Spellcast()
    {
        noIdleAnimation = true;

        animator.Play("Spellcast");
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Weapon"))
                continue;

            Animator an = child.GetComponent<Animator>();
            if (an != null && an.runtimeAnimatorController != null && an.gameObject.activeSelf)
            {
                an.Play("Spellcast");
            }
        }

        yield return new WaitForSeconds(0.5f);
        weaponRenderer.sprite = null;
        noIdleAnimation = false;
    }

    IEnumerator Slash()
    {
        noIdleAnimation = true;

        weaponHolder.gameObject.SetActive(true);

        animator.Play("Slash");
        foreach (Transform child in transform)
        {
            Animator an = child.GetComponent<Animator>();
            if (an != null && an.runtimeAnimatorController != null && an.gameObject.activeSelf)
            {
                an.Play("Slash");
            }
        }

        yield return new WaitForSeconds(0.8f);
        weaponHolder.gameObject.SetActive(false);
        weaponRenderer.sprite = null;
        noIdleAnimation = false;
    }

    IEnumerator Thrust()
    {
        noIdleAnimation = true;

        weaponHolder.gameObject.SetActive(true);

        animator.Play("Thrust");
        foreach (Transform child in transform)
        {
            Animator an = child.GetComponent<Animator>();
            if (an != null && an.runtimeAnimatorController != null && an.gameObject.activeSelf)
            {
                an.Play("Thrust");
            }
        }

        yield return new WaitForSeconds(0.8f);
        weaponHolder.gameObject.SetActive(false);
        weaponRenderer.sprite = null;
        noIdleAnimation = false;
    }

    IEnumerator Shoot()
    {
        noIdleAnimation = true;

        weaponHolder.gameObject.SetActive(true);

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
                an.Play("Bow");
            }
        }

        yield return new WaitForSeconds(1.2f);

        weaponHolder.gameObject.SetActive(false);
        arrowAnimator.transform.GetComponent<SpriteRenderer>().sprite = null;
        weaponRenderer.sprite = null;
        noIdleAnimation = false;
    }
}
