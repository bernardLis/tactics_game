using Lis.Core;

namespace Lis.HeroCreation
{
    public class BodyTypeSelectionScreen : FullScreenElement
    {

        public BodyTypeSelectionScreen()
        {
            // button to create hero
            MyButton createHeroButton = new MyButton("Create Hero", USSCommonButton, ChooseBodyType);
            // button to use random hero
            // buttons with previously created heroes

        }

        void ChooseBodyType()
        {
        }

    }
}