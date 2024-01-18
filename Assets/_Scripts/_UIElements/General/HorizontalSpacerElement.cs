using UnityEngine.UIElements;

namespace Lis
{
    public class HorizontalSpacerElement : VisualElement
    {
        const string _ussCommonSpacer = "common__horizontal-spacer";

        public HorizontalSpacerElement()
        {
            AddToClassList(_ussCommonSpacer);
        }
    }
}
