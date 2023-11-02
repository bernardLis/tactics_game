using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public void DisplayTooltip();
    public void HideTooltip();
    public bool Interact(BattleInteractor interactor);
}