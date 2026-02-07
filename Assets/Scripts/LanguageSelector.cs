using Lean.Localization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
public class LanguageSelector : MonoBehaviour
{
    string _currentLanguage;
    public string currentLanguage
    {
        get => _currentLanguage;
        set
        {
            _currentLanguage = value;
            if (languageLabel != null)
            {
                languageLabel.text = value;
            }
        }
    }

    List<string> languages;
    public int currentLanguageIndex = 0;
    public LeanLocalization leanLocalization;
    public TextMeshProUGUI languageLabel;

    private void Start()
    {
        currentLanguage = LeanLocalization.GetFirstCurrentLanguage();
        languages = LeanLocalization.CurrentLanguages.Select(x=>x.Key).ToList();
        currentLanguageIndex = languages.Count > 0 ? languages.IndexOf(currentLanguage) : 0;
        DontDestroyOnLoad(this);
    }

    public void NextLanguage()
    {
        if (languages.Count == 0) return;
        currentLanguageIndex = (currentLanguageIndex + 1) % languages.Count;
        currentLanguage = languages[currentLanguageIndex];
        leanLocalization.SetCurrentLanguage(currentLanguage);
    }

    public void PreviousLanguage()
    {
        if (languages.Count == 0) return;
        currentLanguageIndex = (currentLanguageIndex - 1 + languages.Count) % languages.Count;
        currentLanguage = languages[currentLanguageIndex];
        leanLocalization.SetCurrentLanguage(currentLanguage);
    }
}
