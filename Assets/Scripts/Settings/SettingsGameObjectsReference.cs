using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SettingsMenuNamespace
{
    [Serializable]
    internal class Video
    {
        public TMP_Dropdown resolutionDropdown;
        public TMP_Dropdown displayModeDropdown; 
        public TMP_Dropdown languageDropdown;
    }

    [Serializable]
    internal class Environment
    {
        [Header("Wall")]
        public TMP_InputField wallWidthInputField;
        public TMP_InputField wallHeightInputField;
        public Slider wallDistanceSlider;
        public TMP_Text wallDistanceSliderValue;

        [Header("Target")]
        public Slider targetSizeSlider;
        public TMP_Text targetSizeSliderValue;
        public Slider targetsCountSlider;
        public TMP_Text targetsCountSliderValue;

        [Header("Advanced")]
        public Slider minDistanceBetweenTargetsSlider;
        public TMP_Text minDistanceBetweenTargetsSliderValue;
    }

    [Serializable]
    internal class Control
    {
        public TMP_Dropdown gameDropdown;
        public TMP_InputField sensitivityInputField;
    }
}
