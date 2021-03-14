using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject mainButtons;
    public GameObject settingsPanel;

    public Text difficultyBtnText;
    public Slider musicVolume;
    public Slider sfxSlider;

    public void StartGame()
    {
        GameManager.Instance.musicVol = musicVolume.value;
        GameManager.Instance.skfVol = sfxSlider.value;
        GameManager.Instance.SetDifficulty(difficultyBtnText.text);
        SceneManager.LoadScene("GAME");
    }

    public void HideOrShowMainButtons(bool toggle)
    {
        mainButtons.SetActive(toggle);
    }

    public void HideOrShowSettingsPanel(bool toggle)
    {
        settingsPanel.SetActive(toggle);
    }

    public void OnDifficultyBtnPress()
    {
        if (difficultyBtnText.text == "NORMAL")
        {
            difficultyBtnText.text = "HARD";
        }
        else if (difficultyBtnText.text == "HARD")
        {
            difficultyBtnText.GetComponent<Text>().text = "EASY";
        }
        else
        {
            difficultyBtnText.GetComponent<Text>().text = "NORMAL";
        }
    }
}
