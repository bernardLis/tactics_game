using System.Collections;
using UnityEngine;
using Pathfinding;
using System.Threading.Tasks;
using DG.Tweening;

public class CharacterRendererManager : MonoBehaviour
{
    [HideInInspector] public Vector2 direction;
    [HideInInspector] public Vector2 lastDirection;

    public GameObject flyingArrowPrefab; // TODO: is this the right place for managing the flying arrow?

    Animator animator;
    CharacterRenderer characterRenderer;

    WeaponHolder weaponHolder;
    SpriteRenderer weaponRenderer;

    AILerp AI;

    Vector2 directionFromFace;

    bool noIdleAnimation;

    // TODO: I am not certain if it is correctly set
    public Vector2 faceDir { get; private set; }

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
        {
            lastDirection = direction;
            SetFaceDir(direction);
        }

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

    public async Task AttackAnimation()
    {
        if (weaponHolder.weapon == null)
            return;

        if (weaponHolder.weapon.weaponType == WeaponType.SLASH)
            await Slash(directionFromFace);
        if (weaponHolder.weapon.weaponType == WeaponType.THRUST)
            await Thrust(directionFromFace);
        if (weaponHolder.weapon.weaponType == WeaponType.SHOOT)
            await Shoot(directionFromFace);
    }

    void SetFaceDir(Vector2 _dir)
    {
        directionFromFace = _dir;
        // TODO: this is weirdness...
        float x = Mathf.FloorToInt(_dir.x);
        float y = Mathf.FloorToInt(_dir.y);

        faceDir = new Vector2(x, y);
    }

    public async Task SpellcastAnimation()
    {
        // TODO: this is the set-up coz i might want to add some other animations or effects that should be awaited sequentially.
        await Spellcast(directionFromFace);
    }

    public void Face(Vector2 dir)
    {
        direction = dir;
        SetFaceDir(dir);

        characterRenderer.SetDirection(direction);

        foreach (Transform child in transform)
        {
            CharacterRenderer cr = child.GetComponent<CharacterRenderer>();
            if (cr != null)
                cr.SetDirection(direction);
        }
    }

    public async Task Die()
    {
        noIdleAnimation = true;

        animator.Play("Hurt");

        foreach (Transform child in transform)
        {
            Animator an = child.GetComponent<Animator>();

            if (an != null && an.runtimeAnimatorController != null && an.gameObject.activeSelf)
                an.Play("Hurt");
        }

        await Task.Delay(500);
        noIdleAnimation = false;
    }

    // TODO: code repetition between all 3 methods.
    async Task Spellcast(Vector2 dir)
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
        await Task.Delay(500);

        Face(dir);

        noIdleAnimation = false;
    }

    async Task Slash(Vector2 dir)
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
        // this looks good, I am punching in the middle of animation
        await PunchEffect(dir, 800);

        weaponHolder.gameObject.SetActive(false);
        weaponRenderer.sprite = null;
        Face(dir);

        noIdleAnimation = false;
    }

    async Task Thrust(Vector2 dir)
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
        // this looks good, I am punching in the middle of animation
        await PunchEffect(dir, 800);

        weaponHolder.gameObject.SetActive(false);
        weaponRenderer.sprite = null;
        Face(dir);

        noIdleAnimation = false;
    }

    async Task Shoot(Vector2 dir)
    {
        noIdleAnimation = true;

        weaponHolder.gameObject.SetActive(true);

        animator.SetFloat("Last Horizontal", dir.x);
        animator.SetFloat("Last Vertical", dir.y);
        animator.Play("Bow");

        Animator arrowAnimator = weaponHolder.transform.Find("Arrow").GetComponent<Animator>();
        arrowAnimator.gameObject.SetActive(true);
        arrowAnimator.SetFloat("Last Horizontal", dir.x);
        arrowAnimator.SetFloat("Last Vertical", dir.y);
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
        await Task.Delay(900);

        weaponHolder.gameObject.SetActive(false);
        arrowAnimator.transform.GetComponent<SpriteRenderer>().sprite = null;
        weaponRenderer.sprite = null;
        Face(dir);

        noIdleAnimation = false;
    }

    async Task PunchEffect(Vector2 dir, int delay)
    {
        await Task.Delay(Mathf.FloorToInt(delay * 0.5f));
        transform.DOPunchPosition(dir * 0.2f, 0.4f, 1, 0, false);
        await Task.Delay(Mathf.FloorToInt(delay * 0.5f));
    }

}
