using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    public Text teamAText;
    public Text teamBText;
    public Text teamCText;
    public GameObject teamTextPanel;

    public GameObject gameOverPanel;
    public Text gameOverPanelText;
    public GameObject buildStationBtn;

    private void Start()
    {
        if (GameManager.Instance.gameType == "1v1v1")
        {
            teamCText.gameObject.SetActive(true);
            teamTextPanel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 120f);
        }
        else
        {
            teamCText.gameObject.SetActive(false);
            teamTextPanel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 90f);
        }
    }

    public void UpdateTeamAText(int count)
    {
        if (teamAText != null)
        {
            teamAText.text = "TEAM A UNITS: " + count;
        }        
    }

    public void UpdateTeamBText(int count)
    {
        if (teamBText != null)
        {
            teamBText.text = "TEAM B UNITS: " + count;
        }        
    }

    public void UpdateTeamCText(int count)
    {
        if (teamCText != null)
        {
            teamCText.text = "TEAM C UNITS: " + count;
        }        
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

    public void ShowBuildStationBtn(Vector3 postion)
    {
        buildStationBtn.transform.position = Camera.main.WorldToScreenPoint(postion);
        buildStationBtn.SetActive(true);
    }

    public void HideBuildStationBtn()
    {
        buildStationBtn.SetActive(false);
    }

    public void OnBuildStationBtnPress()
    {
        LevelManager.Instance.selectedPlanet.GetComponent<Planet>().BuildStation("PLAYER");
        HideBuildStationBtn();
    }
}
