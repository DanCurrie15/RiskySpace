using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public float diffValue = 2.0f;
    public float musicVol = 1.0f;
    public float skfVol = 0.5f;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
    }

    public void SetDifficulty(string diff)
    {
        switch (diff)
        {
            case "EASY":
                diffValue = 2.75f;
                break;
            case "NORMAL":
            default:
                diffValue = 1.5f;
                    break;
            case "HARD":
                diffValue = 0.25f;
                break;
        }
    }
}
