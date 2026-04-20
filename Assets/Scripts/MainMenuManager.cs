using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance;


    [SerializeField] private TextMeshProUGUI languageButtonText;
    [SerializeField] private GameObject settingsPanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        UpdateLanguageButtonText();
    }
    public void PlayButton()
    {
        Scene scene = SceneManager.GetSceneByName("LIVE_SCENE");
        if (scene.IsValid())
            SceneManager.LoadScene("LIVE_SCENE");
        else
            SceneManager.LoadScene(1);
    }

    public void SettingsButton()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LanguageButton()
    {
        string current = LocalizationSettings.SelectedLocale.Identifier.Code;
        string next = current == "en" ? "tr" : "en";
        StartCoroutine(SwitchLanguage(next));
    }

    private IEnumerator SwitchLanguage(string localeCode)
    {
        yield return LocalizationSettings.InitializationOperation;

        var locale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);
        if (locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
            PlayerPrefs.SetString("Language", localeCode);
            PlayerPrefs.Save();
        }

        UpdateLanguageButtonText();
    }

    private void UpdateLanguageButtonText()
    {
        if (languageButtonText == null) return;

        string current = LocalizationSettings.SelectedLocale.Identifier.Code;
        // Show the language you'll switch TO, so the player knows what clicking does
        string key = current == "en" ? "mainMenu.language.english" : "mainMenu.language.turkish";
        languageButtonText.text = LocalizationSettings.StringDatabase.GetLocalizedString("UI", key);
    }
}
