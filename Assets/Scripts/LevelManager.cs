using UnityEngine;
using System.Collections.Generic;

public class LevelManager : Singleton<LevelManager>
{
    public AudioSource musicAudioSource;
    public AudioSource sfxAudioSource;

    public List<GameObject> planets = new List<GameObject>();

    private void Start()
    {
        musicAudioSource.volume = GameManager.Instance.musicVol;
        sfxAudioSource.volume = GameManager.Instance.skfVol;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.CompareTag("Planet"))
                {
                    // if no units selected, select units at this planet
                    if (FighterManager.Instance.selectedFighters.Count == 0)
                    {
                        foreach (GameObject fighter in hit.transform.GetComponent<Planet>().orbitingFighters)
                        {
                            if (fighter.CompareTag("Player"))
                            {
                                FighterManager.Instance.AddSelectedFighters(fighter);
                            }
                        }
                    }
                    // if units are selected, move them to this planet
                    else
                    {
                        FighterManager.Instance.MoveFighters(hit.transform.gameObject);
                    }
                }
                else
                {
                    // deselect units
                    FighterManager.Instance.DeselectAllFighters();
                }
            }
        }
    }

    // planet ownership: 0 - player, 1 - enemy
    public void GameOver(bool win)
    {
        int planetStillOwned = planets.Count; ;
        if (win)
        {           
            UIManager.Instance.ShowGameOverPanel("A WINNER IS YOU :D");
        }
        else
        {         
            UIManager.Instance.ShowGameOverPanel("OH NO YOU LOST :(");
        }
    }
}
