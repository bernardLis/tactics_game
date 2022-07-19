using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenPipeText : MonoBehaviour, IUITextDisplayable
{
    public string DisplayText()
    {
        return "Broken pipe. Water is coming out of it. Someone will need to fix it.";
    }
}
