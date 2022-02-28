using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;

public abstract class BaseTriggerable : MonoBehaviour
{
    protected CharacterStats _myStats;
    protected CharacterRendererManager _characterRendererManager;
    protected FaceDirectionUI _faceDirectionUI;

    void Start()
    {
        _myStats = GetComponent<CharacterStats>();
        _characterRendererManager = GetComponentInChildren<CharacterRendererManager>();
        _faceDirectionUI = GetComponent<FaceDirectionUI>();
    }

    protected async Task<bool> PlayerFaceDirSelection()
    {
        Vector2 dir = Vector2.zero;
        if (_faceDirectionUI != null)
            dir = await _faceDirectionUI.PickDirection();

        // TODO: is that correct, facedir returns vector2.zero when it's broken out of
        if (dir == Vector2.zero)
            return false;

        _characterRendererManager.Face(dir.normalized);

        return true;
    }

}
