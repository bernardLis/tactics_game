using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class BodyEffectController : MonoBehaviour
{
    CharacterStats _stats;
    Animator _animator;
    SpriteRenderer _spriteRenderer;
    float _nextTime = 0f;

    List<string> _animationsToPlay = new();

    // Start is called before the first frame update
    void Start()
    {
        _stats = GetComponentInParent<CharacterStats>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _stats.OnStatusAdded += OnStatusAdded;
        _stats.OnStatusRemoved += OnStatusRemoved;
    }

    void OnDestroy()
    {
        _stats.OnStatusAdded -= OnStatusAdded;
        _stats.OnStatusRemoved -= OnStatusRemoved;
    }

    void OnStatusAdded(Status status)
    {
        if (status.AnimationEffectOnBody != null)
            _animationsToPlay.Add(status.AnimationEffectOnBody);
    }

    void OnStatusRemoved(Status status)
    {
        if (status.AnimationEffectOnBody != null)
            _animationsToPlay.Remove(status.AnimationEffectOnBody);
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: I don't like being in update loop
        if (_animationsToPlay.Count == 0)
        {
            _nextTime = 0;
            return;
        }

        if (_nextTime == 0)
            _nextTime = Time.time + Random.Range(3, 10);

        if (Time.time > _nextTime)
            PlayAnimation();
    }

    void PlayAnimation()
    {
        _animator.enabled = true;
        string animationToPlay = _animationsToPlay[Random.Range(0, _animationsToPlay.Count)];
        _animator.Play(animationToPlay, -1, 0);
        _nextTime = Time.time + Random.Range(3, 10);
    }

    void AnimationFinished()
    {
        _animator.enabled = false;
        _spriteRenderer.sprite = null;
    }
}
