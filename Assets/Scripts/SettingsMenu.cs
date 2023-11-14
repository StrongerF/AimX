using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    private Resolution[] resolutions;
    private List<string> resolutionStrings;

    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private string resolutionStringFormat = "{0}x{1}@{2}Hz";
    [SerializeField] private TMP_Dropdown fullscreenModeDropdown;
    [SerializeField] private Button saveButton;


    private string currentResolutionString;
    private string selectedResolutionString;

    private FullScreenMode currentFullscreenMode;
    private FullScreenMode selectedFullscreenMode;

    private void Awake()
    {
        SettingsManager.ApplySettingsOnStartup();
        resolutions = Screen.resolutions;
        currentResolutionString = Screen.currentResolution.ToStringExtended(resolutionStringFormat);
        currentFullscreenMode = Screen.fullScreenMode;

        InitializeUI();
        LoadCurrentSettings();
    }

    private void LoadCurrentSettings()
    {
        selectedFullscreenMode = Screen.fullScreenMode;
        selectedResolutionString = Screen.currentResolution.ToStringExtended(resolutionStringFormat);

        SetDropdownValues();
    }

    private void SetDropdownValues()
    {
        resolutionDropdown.value = resolutionStrings.IndexOf(selectedResolutionString);
        fullscreenModeDropdown.value = GameSettings.FullScreenModes.IndexOf(selectedFullscreenMode);
    }

    private void InitializeUI()
    {
        resolutionDropdown.ClearOptions();
        resolutionStrings = new List<string>(resolutions.Length);

        foreach (var resolution in resolutions)
        {
            string resolutionString = resolution.ToStringExtended(resolutionStringFormat);
            resolutionStrings.Add(resolutionString);
        }

        resolutionDropdown.AddOptions(resolutionStrings);
    }

    public void OnFullscreenModeDropdownChanged(int index)
    {
        selectedFullscreenMode = GameSettings.GetFullScreenMode(index);
        CheckChanges();
    }

    public void OnResolutionDropdownChanged(int index)
    {
        selectedResolutionString = resolutionStrings[index];
        CheckChanges();
    }

    private void CheckChanges()
    {
        bool resolutionChanged = selectedResolutionString != currentResolutionString;
        bool fullscreenChanged = selectedFullscreenMode != currentFullscreenMode;

        Debug.Log($"ResolutionChanged\nValue: {resolutionChanged}");
        Debug.Log($"FullscreenChanged\nValue: {fullscreenChanged}");

        saveButton.interactable = resolutionChanged || fullscreenChanged;
    }

    public void SaveChanges()
    {
        int resolutionIndex = resolutionDropdown.value;
        Resolution res = resolutions[resolutionIndex];
        GameSettings settings = new GameSettings(res.width, res.height, selectedFullscreenMode, (int)res.refreshRateRatio.value);


        SettingsManager.ApplySettings(settings);
        SettingsManager.SaveChanges(settings);

        currentResolutionString = selectedResolutionString;
        currentFullscreenMode = selectedFullscreenMode;

        Debug.Log("CHANGES SAVED");

        CheckChanges();
    }
}

public static class ResolutionExtension
{
    public static string ToStringExtended(this Resolution resolution, string format)
    {
        return string.Format(format,
                             resolution.width,
                             resolution.height,
                             ((int)resolution.refreshRateRatio.value).ToString("0"));
    }
}
