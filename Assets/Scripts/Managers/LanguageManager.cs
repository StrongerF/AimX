using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;
using static Settings.ControlSettings;

public class LanguageManager : MonoBehaviour
{
    private static LanguageManager instance;
    public static LanguageManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<LanguageManager>();
                if (instance == null)
                {
                    GameObject gameObject = new GameObject("LanguageManager");
                    instance = gameObject.AddComponent<LanguageManager>();
                }
            }
            return instance;
        }
    }
    
    public static string CurrentLanguage { get; private set; }

    // PlayerPrefs key
    private const string LanguageCodeKey = "SelectedLanguageCode";

    public static readonly Dictionary<string, string> LocalesDict = new Dictionary<string, string>()
    {
        { "en", "English" },
        { "ru", "Русский" }
    };

    public static List<string> LocalesCodes
    {
        get => LocalesDict.Keys.ToList();
    }
    public static List<string> LocalesNames
    {
        get => LocalesDict.Values.ToList();
    }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadSelectedLanguage();
    }

    public void SetLanguage(string languageCode)
    {
        // Set the language in Localization Package
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(languageCode);
        CurrentLanguage = LocalesDict[languageCode];

        PlayerPrefs.SetString(LanguageCodeKey, languageCode);
        PlayerPrefs.Save();
    }

    private void LoadSelectedLanguage()
    {
        string languageCode = PlayerPrefs.GetString(LanguageCodeKey, "en");

        // Set the language in Localization Package
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(languageCode);
        CurrentLanguage = LocalesDict[languageCode];
    }
}
