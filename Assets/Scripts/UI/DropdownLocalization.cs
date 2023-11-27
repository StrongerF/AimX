using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public class DropdownLocalization : MonoBehaviour
{
    [SerializeField] private LocalizedStringTable localizedStringTable;
    [SerializeField] private TMPro.TMP_Dropdown dropdown;
    [SerializeField] private List<TMPro.TMP_Dropdown.OptionData> optionDataKeys;

    private void Start()
    {
        LocalizeDropdownItems();
    }

    public void LocalizeDropdownItems()
    {
        StringTable localizedTable = localizedStringTable.GetTable();

        List<TMPro.TMP_Dropdown.OptionData> localizedOptions = new List<TMPro.TMP_Dropdown.OptionData>();

        foreach (TMPro.TMP_Dropdown.OptionData optionText in optionDataKeys)
        {
            string localizedText = localizedTable.GetEntry(optionText.text).LocalizedValue;
            localizedOptions.Add(new TMPro.TMP_Dropdown.OptionData(localizedText));
        }

        dropdown.options = localizedOptions;
    }
}
