using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    public List<GameObject> fighters = new List<GameObject>();
    public List<GameObject> selectedFighters = new List<GameObject>();

    public List<Planet> planets = new List<Planet>();

    private bool _foundMovableFighters = false;

    // Update is called once per frame
    void Update()
    {
        if (selectedFighters.Count < 1)
        {
            if (fighters.Count < 10 && FighterManager.Instance.fighters.Count < 10)
            {
                OpeningState();
            }
            else if (FighterManager.Instance.fighters.Count - fighters.Count > 30f)
            {
                EndGameLosing();
            }
            else if (fighters.Count - FighterManager.Instance.fighters.Count > 30f)
            {
                EndGameWinning();
            }
            else
            {
                MidGameState();
            }
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
                foreach (Planet planet1 in planets)
                {
                    if (planet1._ownership < 1 && planet1.orbitingEnemies < 1)
                    {
                        float tempDistance = Vector3.Distance(planet.transform.position, planet1.transform.position);
                        if (tempDistance < distance)
                        {
                            distance = tempDistance;
                            moveToPlanet = planet1.gameObject;
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
                foreach (Planet planet1 in planets)
                {
                    if (planet1._ownership < 1 && planet1.orbitingEnemies < planet1.orbitingPlayers)
                    {
                        float tempDistance = Vector3.Distance(planet.transform.position, planet1.transform.position);
                        if (tempDistance < distance)
                        {
                            distance = tempDistance;
                            moveToPlanet = planet1.gameObject;
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
                foreach (Planet planet1 in planets)
                {
                    if (planet1._ownership <= 1 && (planet1.orbitingEnemies < 5 || planet1.orbitingPlayers < 5) && (planet != planet1))
                    {
                        float tempDistance = Vector3.Distance(planet.transform.position, planet1.transform.position);
                        if (tempDistance < distance)
                        {
                            distance = tempDistance;
                            moveToPlanet = planet1.gameObject;
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
                float distance = Mathf.Infinity;
                foreach (Planet planet1 in planets)
                {
                    if (planet1._ownership < 1)
                    {
                        float tempDistance = Vector3.Distance(planet.transform.position, planet1.transform.position);
                        if (tempDistance < distance)
                        {
                            distance = tempDistance;
                            moveToPlanet = planet1.gameObject;
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
}
