// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using Boxophobic.StyledGUI;

namespace Boxophobic.Utils
{
    public class SettingsData : StyledScriptableObject
    {
        [StyledBanner(0.65f, 0.65f, 0.65f, "Settings Data")]
        public bool styledBanner;

        [Space]
        public string data = "";
    }
}