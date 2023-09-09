using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CursorManager : Singleton<CursorManager>
{
    [SerializeField] MyCursor[] _cursors;
    [SerializeField] MyCursor _defaultCursor;

    bool _isCustomCursorActive;

    void Start()
    {
        SetCursor(_defaultCursor);
    }

    public void SetCursorByName(string name)
    {
        if (_isCustomCursorActive) return;
        _isCustomCursorActive = true;

        MyCursor cursor = _cursors.FirstOrDefault(c => c.name == name);
        if (cursor != null)
            SetCursor(cursor);
    }

    public void ClearCursor()
    {
        _isCustomCursorActive = false;
        SetCursor(_defaultCursor);
    }

    public void SetCursor(MyCursor cursor)
    {
        Cursor.SetCursor(cursor.Texture, Vector2.zero, CursorMode.Auto);
    }

}
