using System.Linq;
using Lis.Core.Utilities;
using UnityEngine.UIElements;

namespace Lis
{
    public class CampConsoleManager : Singleton<CampConsoleManager>
    {
        ScrollView _campConsoleView;

        void Start()
        {
            _campConsoleView = new();
            GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("campConsoleContainer").Add(_campConsoleView);
        }

        public void ShowMessage(string message)
        {
            if (_campConsoleView == null) return;
            Label label = new();
            label.text = message;
            _campConsoleView.Add(label);
            _campConsoleView.schedule.Execute(() => _campConsoleView.ScrollTo(label)).StartingIn(10);
        }
    }
}