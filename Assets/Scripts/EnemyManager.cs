using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    public List<GameObject> fighters = new List<GameObject>();
    public List<GameObject> selectedFighters = new List<GameObject>();

    public List<Planet> planets = new List<Planet>();

    private bool _foundMovableFighters = false;

    private float _actionRate = 2.5f;
    private float _nextAction = 0f;

    // Update is called once per frame
    void Update()
    {
        if (selectedFighters.Count < 1 && Time.time > _nextAction)
        {
            if (fighters.Count < 10 && FighterManager.Instance.fighters.Count < 10)
            {
                //Debug.Log("Enemy State: Opening");
                OpeningState();
            }
            else if (FighterManager.Instance.fighters.Count - fighters.Count > 30f)
            {
                //Debug.Log("Enemy State: End Game Losing");
                EndGameLosing();
            }
            else if (fighters.Count - FighterManager.Instance.fighters.Count > 30f)
            {
                //Debug.Log("Enemy State: End Game Winning");
                EndGameWinning();
            }
            else
            {
                //Debug.Log("Enemy State: Mid Game");
                MidGameState();
            }
            _nextAction = Time.time + _actionRate;
        }
    }

    public void MoveFighters(GameObject targetPlanet)
    {
        foreach (GameObject fighter in selectedFighters)
        {
            fighter.GetComponent<Fighter>().MoveToPlanet(targetPlanet);
        }
        DeselectAllFighters();
    }

    public void AddFighter(GameObject fighter)
    {
        fighters.Add(fighter);
        UIManager.Instance.UpdateTeamBText(fighters.Count);
    }
    public void RemoveFighter(GameObject fighter)
    {
        fighters.Remove(fighter);
        if (fighters.Count < 1)
        {
            GameManager.Instance.GameOver(true);
        }
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateTeamBText(fighters.Count);
        }
    }

    public void AddSelectedFighters(GameObject fighter)
    {
        selectedFighters.Add(fighter);
    }

    public void RemoveSelectedFighter(GameObject fighter)
    {
        selectedFighters.Remove(fighter);
    }

    public void DeselectAllFighters()
    {
        selectedFighters.Clear();
    }

    private void OpeningState()
    {
        foreach (Planet planet in planets)
        {
            GameObject moveToPlanet = null;
            if (planet._ownership >= 1 && planet.orbitingEnemies > 1 && planet.orbitingPlayers < 1 && !_foundMovableFighters)
            {
                selectedFighters.Add(planet.orbitingFighters[0]);
                _foundMovableFighters = true;
            }
            if (_foundMovableFighters)
            {
                float distance = Mathf.Infinity;
                foreach (Planet planetToLocate in planets)
                {
                    if (planetToLocate._ownership < 1 && planetToLocate.orbitingEnemies < 1)
                    {
                        float tempDistance = Vector3.Distance(planet.transform.position, planetToLocate.transform.position);
                        if (tempDistance < distance)
                        {
                            distance = tempDistance;
                            moveToPlanet = planetToLocate.gameObject;
                        }
                    }
                }
                if (moveToPlanet != null)
                {
                    MoveFighters(moveToPlanet);
                    _foundMovableFighters = false;
                }
            }
        }
    }

    private void MidGameState()
    {
        foreach (Planet planet in planets)
        {
            GameObject moveToPlanet = null;
            if (planet._ownership >= 1 && planet.orbitingEnemies > 4 && planet.orbitingPlayers < 1 && !_foundMovableFighters)
            {
                for (int i = 0; i < 3; i++)
                {
                    selectedFighters.Add(planet.orbitingFighters[i]);
                    _foundMovableFighters = true;
                }
            }
            if (_foundMovableFighters)
            {
                float distance = Mathf.Infinity;
                foreach (Planet planetToLocate in planets)
                {
                    if (planetToLocate._ownership <= 1 && planetToLocate.orbitingEnemies < planetToLocate.orbitingPlayers)
                    {
                        float tempDistance = Vector3.Distance(planet.transform.position, planetToLocate.transform.position);
                        if (tempDistance < distance)
                        {
                            distance = tempDistance;
                            moveToPlanet = planetToLocate.gameObject;
                        }
                    }
                }
                if (moveToPlanet != null)
                {
                    MoveFighters(moveToPlanet);
                    _foundMovableFighters = false;
                }
            }
        }
    }

    private void EndGameLosing()
    {
        foreach (Planet planet in planets)
        {
            GameObject moveToPlanet = null;
            if (planet._ownership >= 1 && planet.orbitingEnemies > 8 && planet.orbitingPlayers < 1 && !_foundMovableFighters)
            {
                for (int i = 0; i < 5; i++)
                {
                    selectedFighters.Add(planet.orbitingFighters[i]);
                    _foundMovableFighters = true;
                }
            }
            if (_foundMovableFighters)
            {
                float distance = Mathf.Infinity;
                foreach (Planet planetToLocate in planets)
                {
                    if (planetToLocate._ownership <= 1 && (planetToLocate.orbitingEnemies < 5 || planetToLocate.orbitingPlayers < 5) && (planet != planetToLocate))
                    {
                        float tempDistance = Vector3.Distance(planet.transform.position, planetToLocate.transform.position);
                        if (tempDistance < distance)
                        {
                            distance = tempDistance;
                            moveToPlanet = planetToLocate.gameObject;
                        }
                    }
                }
                if (moveToPlanet != null)
                {
                    MoveFighters(moveToPlanet);
                    _foundMovableFighters = false;
                }
            }
        }
    }

    private void EndGameWinning()
    {
        foreach (Planet planet in planets)
        {
            GameObject moveToPlanet = null;
            if (planet._ownership >= 1 && planet.orbitingEnemies > 4 && planet.orbitingPlayers < 1 && !_foundMovableFighters)
            {
                for (int i = 0; i < (planet.orbitingEnemies - 2); i++)
                {
                    selectedFighters.Add(planet.orbitingFighters[i]);
                    _foundMovableFighters = true;
                }
            }
            if (_foundMovableFighters)
            {
                foreach (Planet plantToLocate in planets)
                {
                    if (plantToLocate._ownership < 1 && plantToLocate.orbitingPlayers > plantToLocate.orbitingEnemies)
                    {
                        moveToPlanet = plantToLocate.gameObject;
                    }
                }
                if (moveToPlanet != null)
                {
                    MoveFighters(moveToPlanet);
                    _foundMovableFighters = false;
                }
                else
                {
                    selectedFighters.Clear();
                }
            }
        }
    }
}
