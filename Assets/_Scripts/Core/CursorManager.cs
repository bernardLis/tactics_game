using System.Linq;
using Lis.Core.Utilities;
using UnityEngine;

namespace Lis.Core
{
    public class CursorManager : Singleton<CursorManager>
    {
        [SerializeField] private MyCursor[] _cursors;
        [SerializeField] private MyCursor _defaultCursor;

        private bool _isCustomCursorActive;

        private void Start()
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
}