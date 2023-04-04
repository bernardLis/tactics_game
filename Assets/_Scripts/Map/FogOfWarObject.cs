using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FogOfWarObject : MonoBehaviour
{
    FogOfWarSquare _yourSquareInfo;
    public bool IsVisibleInFogOfWar;
    [SerializeField] GameObject _gfx;

    public void SetYourSquareInfo(FogOfWarSquare info)
    {
        _yourSquareInfo = info;
        _yourSquareInfo.OnVisibilityChanged += UpdateVisibility;
    }

    public void SetVisibility(bool isVisible) { _gfx.SetActive(isVisible); }

    public void UpdateVisibility(bool isSquareVisible)
    {
        SetVisibility(false);
        if (_yourSquareInfo.IsVisible)
        {
            SetVisibility(true);
            return;
        }

        if (_yourSquareInfo.IsExplored && IsVisibleInFogOfWar)
        {
            SetVisibility(true);
        }
    }
}
