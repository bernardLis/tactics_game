using UnityEngine;
using Pathfinding;
using System.Threading.Tasks;
using DG.Tweening;

public class CharacterRendererManager : MonoBehaviour
{
    Vector2 _direction;

    Animator _animator;
    CharacterRenderer _characterRenderer;

    WeaponHolder _weaponHolder;
    SpriteRenderer _weaponRenderer;

    AILerp _aiLerp;

    Vector2 _directionFromFace;

    bool _noIdleAnimation;

    // TODO: I am not certain if it is correctly set
    //public Vector2 FaceDir { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        _animator = GetComponent<Animator>();
        _characterRenderer = GetComponent<CharacterRenderer>();
        _aiLerp = GetComponentInParent<AILerp>();
        _weaponHolder = GetComponentInChildren<WeaponHolder>();
        _weaponRenderer = _weaponHolder.transform.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    // TODO: something smarter I probably don't need to set direction when character movement is not active
    void Update()
    {
        if (_aiLerp.canMove)
            _direction = _aiLerp.myDirection;

        if (_direction.sqrMagnitude > 0)
            SetFaceDir(_direction);

        // TODO: there is probably a way to improve that;
        if (!_noIdleAnimation)
        {
            _characterRenderer.SetDirection(_direction);

            foreach (Transform child in transform)
            {
                CharacterRenderer cr = child.GetComponent<CharacterRenderer>();
                if (cr != null)
                    cr.SetDirection(_direction);
            }
        }
    }

    public async Task AttackAnimation()
    {
        // if character does not have a weapon
        if (_weaponHolder.Weapon == null)
            await Thrust(_directionFromFace); // punch animation

        if (_weaponHolder.Weapon.WeaponType == WeaponType.Slash)
            await Slash(_directionFromFace);
        if (_weaponHolder.Weapon.WeaponType == WeaponType.Thrust)
            await Thrust(_directionFromFace);
        if (_weaponHolder.Weapon.WeaponType == WeaponType.Shoot)
            await Shoot(_directionFromFace);
    }

    void SetFaceDir(Vector2 dir)
    {
        // This is because at the beginning of the game I was making enemies face right and than 0, for them to stop walking.
        // It was setting facedir to 0, which made attack dir calculations wrong. Now it works, but is hacky.
        if (dir == Vector2.zero)
            return;

        _directionFromFace = dir;
        // TODO: this is weirdness...
        float x = Mathf.FloorToInt(dir.x);
        float y = Mathf.FloorToInt(dir.y);

        //FaceDir = new Vector2(x, y);
    }

    public Vector2 GetFaceDir()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle N"))
            return Vector2.up;
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle S"))
            return Vector2.down;
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle W"))
            return Vector2.right;
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle E"))
            return Vector2.left;

        return Vector2.zero;
    }


    public async Task SpellcastAnimation()
    {
        // TODO: this is the set-up coz i might want to add some other animations or effects that should be awaited sequentially.
        await Spellcast(_directionFromFace);
    }

    public void Face(Vector2 dir)
    {
        _direction = dir;
        SetFaceDir(dir);

        _characterRenderer.SetDirection(_direction);

        foreach (Transform child in transform)
        {
            CharacterRenderer cr = child.GetComponent<CharacterRenderer>();
            if (cr != null)
                cr.SetDirection(_direction);
        }
    }

    public async Task Die()
    {
        _noIdleAnimation = true;

        _animator.Play("Hurt");

        foreach (Transform child in transform)
        {
            Animator an = child.GetComponent<Animator>();

            if (an != null && an.runtimeAnimatorController != null && an.gameObject.activeSelf)
                an.Play("Hurt");
        }

        await Task.Delay(500);
        _noIdleAnimation = false;
    }

    // TODO: code repetition between all 3 methods.
    async Task Spellcast(Vector2 dir)
    {
        _noIdleAnimation = true;

        // set direction and play animation
        _animator.SetFloat("Last Horizontal", dir.x);
        _animator.SetFloat("Last Vertical", dir.y);
        _animator.Play("Spellcast");

        foreach (Transform child in transform)
        {
            if (child.CompareTag(Tags.Weapon))
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

        _noIdleAnimation = false;
    }

    async Task Slash(Vector2 dir)
    {
        _noIdleAnimation = true;

        _weaponHolder.gameObject.SetActive(true);
        _animator.SetFloat("Last Horizontal", dir.x);
        _animator.SetFloat("Last Vertical", dir.y);
        _animator.Play("Slash");

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

        _weaponHolder.gameObject.SetActive(false);
        _weaponRenderer.sprite = null;
        Face(dir);

        _noIdleAnimation = false;
    }

    async Task Thrust(Vector2 dir)
    {
        _noIdleAnimation = true;

        _weaponHolder.gameObject.SetActive(true);

        _animator.SetFloat("Last Horizontal", dir.x);
        _animator.SetFloat("Last Vertical", dir.y);
        _animator.Play("Thrust");

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

        _weaponHolder.gameObject.SetActive(false);
        _weaponRenderer.sprite = null;
        Face(dir);

        _noIdleAnimation = false;
    }

    async Task Shoot(Vector2 dir)
    {
        _noIdleAnimation = true;

        _weaponHolder.gameObject.SetActive(true);

        _animator.SetFloat("Last Horizontal", dir.x);
        _animator.SetFloat("Last Vertical", dir.y);
        _animator.Play("Bow");

        Animator arrowAnimator = _weaponHolder.transform.Find("Arrow").GetComponent<Animator>();
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

        _weaponHolder.gameObject.SetActive(false);
        arrowAnimator.transform.GetComponent<SpriteRenderer>().sprite = null;
        _weaponRenderer.sprite = null;
        Face(dir);

        _noIdleAnimation = false;
    }

    async Task PunchEffect(Vector2 dir, int delay)
    {
        await Task.Delay(Mathf.FloorToInt(delay * 0.5f));
        transform.DOPunchPosition(dir * 0.2f, 0.4f, 1, 0, false);
        await Task.Delay(Mathf.FloorToInt(delay * 0.5f));
    }
}
