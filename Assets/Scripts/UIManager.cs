using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    public Text teamAText;
    public Text teamBText;

    public GameObject gameOverPanel;
    public Text gameOverPanelText;

    public void UpdateTeamAText(int count)
    {
        teamAText.text = "TEAM A UNITS: " + count;
    }

    public void UpdateTeamBText(int count)
    {
        teamBText.text = "TEAM B UNITS: " + count;
    }

    public void ReloadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void ShowGameOverPanel(string panelText)
    {
        gameOverPanel.SetActive(true);
        gameOverPanelText.text = panelText;
    }
}
