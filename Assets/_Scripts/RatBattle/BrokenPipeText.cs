using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BrokenPipeText : MonoBehaviour, IUITextDisplayable
{
    public VisualElement DisplayText()
    {
        return new Label("Broken pipe. Water is coming out of it. Someone will need to fix it.");
    }
}
