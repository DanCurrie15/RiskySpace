using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    public Text teamAText;
    public Text teamBText;

    public void UpdateTeamAText(int count)
    {
        teamAText.text = "TeamA Units: " + count;
    }

    public void UpdateTeamBText(int count)
    {
        teamBText.text = "TeamB Units: " + count;
    }

    public void ReloadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
