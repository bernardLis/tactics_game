using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarObject : MonoBehaviour
{
    FogOfWarSquareInfo _yourSquareInfo;
    public bool IsVisibleInFogOfWar;
    [SerializeField] GameObject _gfx;

    public void SetYourSquareInfo(FogOfWarSquareInfo info) { _yourSquareInfo = info; }

    public void SetVisibility(bool isVisible) { _gfx.SetActive(isVisible); }

    void Update()
    {
        UpdateVisibility();
    }

    public void UpdateVisibility()
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
