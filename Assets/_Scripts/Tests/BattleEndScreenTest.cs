using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEndScreenTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        BattleUI.Instance.ShowBattleWonScreen();
    }

    // Update is called once per frame
    void Update()
    {

    }

#if UNITY_EDITOR

    [ContextMenu("Show Battle Won Screen")]
    void ShowBattleWonScreen()
    {
        BattleUI.Instance.ShowBattleWonScreen();
    }

#endif

}
