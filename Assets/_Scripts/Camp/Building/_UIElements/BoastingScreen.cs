using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Camp.Building
{
    public class BoastingScreen : RewardScreen
    {
        BoastingPlatform _boastingPlatform;


        public void InitializeBoastingPlatform(BoastingPlatform bp)
        {
            SetTitle("Boasting");

            AddContinueButton();
        }
    }
}