using Settings;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace SettingsMenuNamespace
{
    public partial class SettingsMenu : MonoBehaviour
    {
        private List<string> resolutionStrings;

        [Header("Settings references")]
        [SerializeField] private Video Video;
        [SerializeField] private Environment Environment;
        [SerializeField] private Control Control;


        [Header("Buttons")]
        [SerializeField] private Button saveButton;

        private GameSettings currentSettings;
        private GameSettings selectedSettings;

        private string currentResolutionString;
        private string resolutionStringFormat = "{0}x{1}";

        bool isVideoChanged;
        bool isControlChanged;
        bool isEnvironmentChanged;


        #region Environment Variables
        private float wallWidth;
        private float wallHeight;
        #endregion


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && isActiveAndEnabled)
            {
                PauseMenu.HideSettings();
            }
        }

        private void OnEnable()
        {
            LoadSettings();
            UpdateSaveButtonInteractivity(false);
            isVideoChanged = false;
            isEnvironmentChanged = false;
            isControlChanged = false;
        }

        private void LoadSettings()
        {
            GetCurrentValues();
            InitializeUI();
            SetMenuValues();
        }

        private void GetCurrentValues()
        {
            currentSettings = SettingsManager.GetSettings();

            selectedSettings = new GameSettings(currentSettings);

            currentResolutionString = currentSettings.Video.Resolution.ToStringExtended(resolutionStringFormat);

            wallWidth = ObjectSpawner.WallSize.x;
            wallHeight = ObjectSpawner.WallSize.y;
        }

        private void InitializeUI()
        {
            #region Video Settings
            // Add values in resolution dropdown
            Video.resolutionDropdown.ClearOptions();

            resolutionStrings = new List<string>(VideoSettings.Resolutions.Length);
            foreach (var resolution in VideoSettings.Resolutions)
            {
                string resolutionString = resolution.ToStringExtended(resolutionStringFormat);
                resolutionStrings.Add(resolutionString);
            }

            Video.resolutionDropdown.AddOptions(resolutionStrings);

            // Add values in language dropdown
            Video.languageDropdown.ClearOptions();
            Video.languageDropdown.AddOptions(LanguageManager.LocalesNames);
            #endregion

            #region Control Settings
            Control.gameDropdown.ClearOptions();
            Control.gameDropdown.AddOptions(MouseSensitivity.GameTitles);
            #endregion

            #region Environment Settings

            Environment.wallWidthInputField.placeholder.GetComponent<TMP_Text>().text = $"{ObjectSpawner.MinWallSize.x} - {ObjectSpawner.MaxWallSize.x}";
            Environment.wallHeightInputField.placeholder.GetComponent<TMP_Text>().text = $"{ObjectSpawner.MinWallSize.y} - {ObjectSpawner.MaxWallSize.y}";
            Environment.wallDistanceSlider.minValue = ObjectSpawner.MinWallDistance;
            Environment.wallDistanceSlider.maxValue = ObjectSpawner.MaxWallDistance;

            Environment.targetSizeSlider.minValue = ObjectSpawner.MinTargetSize;
            Environment.targetSizeSlider.maxValue = ObjectSpawner.MaxTargetSize;

            Environment.targetsCountSlider.minValue = ObjectSpawner.MinTargetsCount;
            Environment.targetsCountSlider.maxValue = ObjectSpawner.MaxTargetsCount;
            #endregion
        }

        private void SetMenuValues()
        {
            // Video
            Video.languageDropdown.value = LanguageManager.LocalesNames.IndexOf(LanguageManager.CurrentLanguage);
            Video.resolutionDropdown.value = resolutionStrings.IndexOf(currentResolutionString);
            Video.displayModeDropdown.value = VideoSettings.DisplayModes.IndexOf(currentSettings.Video.FullscreenMode);

            // Control
            Control.gameDropdown.value = MouseSensitivity.GameTitles.IndexOf(currentSettings.Control.Sensitivity.SourceGame);
            Control.sensitivityInputField.text = currentSettings.Control.Sensitivity.SourceGameSensitivity.ToString();

            // Environment
            Environment.wallWidthInputField.text = currentSettings.Environment.WallWidth.ToString();
            Environment.wallHeightInputField.text = currentSettings.Environment.WallHeight.ToString();
            UpdateTargetsMinDistanceMaxValue();
            Environment.wallDistanceSlider.value = currentSettings.Environment.WallDistance;

            Environment.targetSizeSlider.value = currentSettings.Environment.TargetSize;
            Environment.targetsCountSlider.value = currentSettings.Environment.TargetsCount;

            Environment.minDistanceBetweenTargetsSlider.value = currentSettings.Environment.TargetsMinDistance;
        }



        #region VideoChanged

        public void OnFullscreenModeDropdownChanged(int index)
        {
            selectedSettings.Video.FullscreenMode = VideoSettings.GetFullScreenMode(index);

            isVideoChanged = true;
            UpdateSaveButtonInteractivity(true);
        }

        public void OnResolutionDropdownChanged(int index)
        {
            selectedSettings.Video.Resolution = VideoSettings.Resolutions[index];

            isVideoChanged = true;
            UpdateSaveButtonInteractivity(true);
        }

        public void OnLanguageDropdownChanged(int index)
        {
            LanguageManager.Instance.SetLanguage(LanguageManager.LocalesCodes[index]);
        }

        #endregion

        #region ControlChanged

        public void OnGameDropdownChanged(int index)
        {
            string sourceGame = selectedSettings.Control.Sensitivity.SourceGame;
            string targetGame = MouseSensitivity.GameTitles[index];

            float sensitivity = MouseSensitivity.ConvertBetweenGames(sourceGame, targetGame, selectedSettings.Control.Sensitivity.SourceGameSensitivity);
            Control.sensitivityInputField.text = Math.Round(sensitivity, 3).ToString();

            selectedSettings.Control.Sensitivity.SourceGame = targetGame;
            selectedSettings.Control.Sensitivity.SourceGameSensitivity = sensitivity;

            isControlChanged = true;
            UpdateSaveButtonInteractivity(true);
        }

        public void OnSensitivityEndEdit(string value)
        {
            float sensitivityValue;

            if (!float.TryParse(value, out sensitivityValue))
            {
                sensitivityValue = 1;
                Control.sensitivityInputField.text = sensitivityValue.ToString();
            }

            if (sensitivityValue < 0)
            {
                sensitivityValue = Mathf.Abs(sensitivityValue);
                Control.sensitivityInputField.text = sensitivityValue.ToString();
            }
            if (sensitivityValue == 0)
            {
                sensitivityValue = 0.01f;
                Control.sensitivityInputField.text = sensitivityValue.ToString();
            }

            selectedSettings.Control.Sensitivity.SourceGameSensitivity = sensitivityValue;

            Debug.Log($"Sensitivity changed to: {sensitivityValue}");
        }





        #endregion

        #region EnvironmentChanged

        public void OnWallWidthEndEdit(string value)
        {
            if (!float.TryParse(Environment.wallWidthInputField.text, out wallWidth))
            {
                Environment.wallWidthInputField.text = wallWidth.ToString();
            }

        
            wallWidth = Mathf.Clamp(wallWidth, ObjectSpawner.MinWallSize.x, ObjectSpawner.MaxWallSize.x);
            Environment.wallWidthInputField.text = wallWidth.ToString();

            selectedSettings.Environment.WallWidth = wallWidth;

            OnSizeEndEdit();
        }

        public void OnWallHeightEndEdit(string value)
        {
            if (!float.TryParse(Environment.wallHeightInputField.text, out wallHeight))
            {
                Environment.wallHeightInputField.text = wallHeight.ToString();
            }

            wallHeight = Mathf.Clamp(wallHeight, ObjectSpawner.MinWallSize.y, ObjectSpawner.MaxWallSize.y);
            Environment.wallHeightInputField.text = wallHeight.ToString();

            selectedSettings.Environment.WallHeight = wallHeight;

            OnSizeEndEdit();
        }

        private void OnSizeEndEdit()
        {
            UpdateTargetsMinDistanceMaxValue();

            isEnvironmentChanged = true;
            UpdateSaveButtonInteractivity(true);
        }

        private void UpdateTargetsMinDistanceMaxValue()
        {
            Environment.minDistanceBetweenTargetsSlider.maxValue = Mathf.Sqrt(wallWidth * wallWidth + wallHeight * wallHeight);

            selectedSettings.Environment.TargetsMinDistance = Environment.minDistanceBetweenTargetsSlider.value;
        }

        public void OnWallDistanceSliderValueChanged(float value)
        {
            Environment.wallDistanceSliderValue.text = Environment.wallDistanceSlider.value.ToString();

            selectedSettings.Environment.WallDistance = (int)value;

            isEnvironmentChanged = true;
            UpdateSaveButtonInteractivity(true);
        }

        public void OnTargetSizeSliderValueChanged(float value)
        {
            value = Convert.ToSingle(Math.Round(Environment.targetSizeSlider.value, 1));
            Environment.targetSizeSliderValue.text = value.ToString();

            selectedSettings.Environment.TargetSize = value;

            isEnvironmentChanged = true;
            UpdateSaveButtonInteractivity(true);
        }

        public void OnTargetsCountSliderValueChanged(float value)
        {
            Environment.targetsCountSliderValue.text = Environment.targetsCountSlider.value.ToString();

            selectedSettings.Environment.TargetsCount = (int)value;

            isEnvironmentChanged = true;
            UpdateSaveButtonInteractivity(true);
        }

        public void OnTargetsDistanceSliderValueChanged(float value)
        {
            value = Convert.ToSingle(Math.Round(Environment.minDistanceBetweenTargetsSlider.value, 1));
            Environment.minDistanceBetweenTargetsSliderValue.text = value.ToString();

            selectedSettings.Environment.TargetsMinDistance = value;

            isEnvironmentChanged = true;
            UpdateSaveButtonInteractivity(true);
        }

        #endregion

        public void UpdateSaveButtonInteractivity(bool interactable)
        {
            saveButton.interactable = interactable;
        }

        public void SaveChanges()
        {
            if (!isVideoChanged) selectedSettings.Video = null;
            if (!isControlChanged) selectedSettings.Control = null;
            if (!isEnvironmentChanged) selectedSettings.Environment = null;

            GameSettings settings = new GameSettings()
            {
                Video = selectedSettings.Video,
                Environment = selectedSettings.Environment,
                Control = selectedSettings.Control
            };

            SettingsManager.ApplySettings(settings);
            SettingsManager.SaveSettings(settings);

            PauseMenu.HideSettings();
        }
    }

    public static class ResolutionExtension
    {
        public static string ToStringExtended(this Resolution resolution, string format)
        {
            return string.Format(format,
                                 resolution.width,
                                 resolution.height);
        }
    }
}
