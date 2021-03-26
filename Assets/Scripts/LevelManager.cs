using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class LevelManager : Singleton<LevelManager>
{
    public AudioSource musicAudioSource;
    public AudioSource sfxAudioSource;
    public NavMeshSurface surface;

    public Camera mainCamera;
    public List<GameObject> levelLayout = new List<GameObject>();
    public GameObject enemyFighterManager2;

    public GameObject selectedPlanet;

    private void Start()
    {
        if (GameManager.Instance.gameType == "1v1v1")
        {
            levelLayout[0].SetActive(false);
            levelLayout[1].SetActive(true);
            enemyFighterManager2.SetActive(true);
            mainCamera.fieldOfView = 70f;
        }
        else
        {
            levelLayout[0].SetActive(true);
            levelLayout[1].SetActive(false);
            enemyFighterManager2.SetActive(false);
            mainCamera.fieldOfView = 60f;
        }
        musicAudioSource.volume = GameManager.Instance.musicVol;
        sfxAudioSource.volume = GameManager.Instance.skfVol;
        surface.BuildNavMesh();
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
                    UIManager.Instance.HideBuildStationBtn();
                }
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.CompareTag("Planet") && (hit.transform.GetComponent<Planet>().planetOwnedBy == "PLAYER")
                    && hit.transform.GetComponent<Planet>().activeStation == null)
                {
                    UIManager.Instance.ShowBuildStationBtn(hit.transform.position);
                    selectedPlanet = hit.transform.gameObject;
                }
            }
        }
    }

    // planet ownership: 0 - player, 1 - enemy
    public void GameOver(bool win)
    {
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
