using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    [Header("Lists of Objects")]
    [SerializeField] private List<MyTabButton> tabButtons;
    [SerializeField] private List<GameObject> pages;

    [Header("Colors")]
    [SerializeField] private Color tabIdle;
    [SerializeField] private Color tabHover;
    [SerializeField] private Color tabActive;



    private MyTabButton selectedTab;

    private void OnEnable()
    {
        if (tabButtons.Count > 0)
        {
            OnTabSelected(tabButtons[0]);
        }
    }

    public void Subscribe(MyTabButton button)
    {
        tabButtons.Add(button);

        if (tabButtons.Count == 1)
        {
            OnTabSelected(button);
        }
    }

    public void OnTabEnter(MyTabButton button)
    {
        ResetTabs();
        if (selectedTab == null || button != selectedTab)
        {
            button.background.color = tabHover;
        }
    }

    public void OnTabExit(MyTabButton button)
    {
        ResetTabs();
    }

    public void OnTabSelected(MyTabButton button)
    {
        selectedTab = button;
        ResetTabs();
        button.background.color = tabActive;

        int index = button.transform.GetSiblingIndex();
        for (int i = 0; i < pages.Count; i++)
        {
            pages[i].SetActive(i == index);
        }
    }

    public void ResetTabs()
    {
        foreach (MyTabButton button in tabButtons)
        {
            if (selectedTab != null && selectedTab == button) continue;
            button.background.color = tabIdle;
        }
    }
}
