using Lis.Battle.Arena.Building;
using Lis.Core;
using UnityEngine.UIElements;

namespace Lis.Battle.Arena
{
    public class ArchitectScreen : FullScreenElement
    {
        Architect _architect;

        public void InitializeBuilding(Architect architect)
        {
            _architect = architect;

            SetTitle("Architect");

            AddUnlockableBuildings();

            AddContinueButton();
        }


        void AddUnlockableBuildings()
        {
            if (_architect.UnlockableBuildings().Count == 0)
            {
                Label label = new();
                label.text = "All Buildings Are Unlocked!";
                Content.Add(label);
                return;
            }

            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;
            container.style.flexWrap = Wrap.Wrap;
            container.style.justifyContent = Justify.SpaceAround;
            Content.Add(container);

            foreach (Building.Building b in _architect.UnlockableBuildings())
            {
                UnlockableBuildingElement ube = new(b);
                container.Add(ube);
            }
        }
    }
}