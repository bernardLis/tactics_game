using UnityEngine.UIElements;

namespace Lis.Core
{
    public class NatureComboElement : VisualElement
    {
        public NatureComboElement(NatureAdvanced nature)
        {
            style.flexDirection = FlexDirection.Row;
            style.justifyContent = Justify.Center;

            VisualElement elementContainer1 = new();
            elementContainer1.style.alignItems = Align.Center;
            elementContainer1.Add(new NatureElement(nature.FirstNature, 100));
            elementContainer1.Add(new Label($"{nature.FirstNature.NatureName}"));
            Add(elementContainer1);

            Add(new Label("+"));

            VisualElement elementContainer2 = new();
            elementContainer2.style.alignItems = Align.Center;

            elementContainer2.Add(new NatureElement(nature.SecondNature, 100));
            elementContainer2.Add(new Label($"{nature.SecondNature.NatureName}"));
            Add(elementContainer2);

            Add(new Label("="));

            VisualElement elementContainer3 = new();
            elementContainer3.style.alignItems = Align.Center;

            elementContainer3.Add(new NatureElement(nature, 100));
            elementContainer3.Add(new Label($"{nature.NatureName}"));
            Add(elementContainer3);
        }
    }
}