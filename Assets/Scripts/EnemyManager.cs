using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<GameObject> fighters = new List<GameObject>();
    public List<GameObject> selectedFighters = new List<GameObject>();

    private List<Planet> planets = new List<Planet>();
    public GameObject planets1v1;
    public GameObject planets1v1v1;

    private bool _foundMovableFighters = false;

    private float _actionRate = 1.25f;
    private float _nextAction = 0f;

    private float _buildStationRate = 15.0f;
    private float _buildNextStation = 0f;

    public string enemyName;

    private void Start()
    {
        _actionRate = GameManager.Instance.diffValue;
        if (GameManager.Instance.gameType == "1v1v1")
        {
            foreach(Transform child in planets1v1v1.transform)
            {
                planets.Add(child.GetComponent<Planet>());
            }
        }
        else
        {
            foreach (Transform child in planets1v1.transform)
            {
                planets.Add(child.GetComponent<Planet>());
            }
        }
    }

    void Update()
    {
        if (selectedFighters.Count < 1 && Time.time > _nextAction)
        {
            for (int i = 0; i < planets.Count; i++)
            {
                Planet temp = planets[i];
                int randomIndex = Random.Range(i, planets.Count);
                planets[i] = planets[randomIndex];
                planets[randomIndex] = temp;
            }

            if (fighters.Count < 10 && FighterManager.Instance.fighters.Count < 10)
            {
                OpeningState();
            }
            else if (FighterManager.Instance.fighters.Count - fighters.Count > 35f)
            {
                EndGameLosing();
            }
            else if (fighters.Count - FighterManager.Instance.fighters.Count > 35f)
            {
                EndGameWinning();
            }
            else
            {
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
        if (this.gameObject.name == "EnemyFighterManager1")
        {
            UIManager.Instance.UpdateTeamBText(fighters.Count);
        }
        else
        {
            UIManager.Instance.UpdateTeamCText(fighters.Count);
        }
        
    }
    public void RemoveFighter(GameObject fighter)
    {
        fighters.Remove(fighter);
        if (fighters.Count < 1)
        {
            LevelManager.Instance.GameOver(true);
        }
        if (UIManager.Instance != null)
        {
            if (this.gameObject.name == "EnemyFighterManager1")
            {
                UIManager.Instance.UpdateTeamBText(fighters.Count);
            }
            else
            {
                UIManager.Instance.UpdateTeamCText(fighters.Count);
            }
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
            if (planet.planetOwnedBy == enemyName && planet.NumOrbitingFighters(enemyName) > 1 && planet.NumOrbitingOpponents(enemyName) < 1 && !_foundMovableFighters)
            {
                selectedFighters.Add(planet.orbitingFighters[0]);
                _foundMovableFighters = true;
            }
            if (_foundMovableFighters)
            {
                float distance = Mathf.Infinity;
                foreach (Planet planetToLocate in planets)
                {
                    if (planetToLocate.planetOwnedBy != enemyName && planetToLocate.NumOrbitingFighters(enemyName) < 1 && planetToLocate.NumOrbitingOpponents(enemyName) < 2)
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
                    break;
                }
                else
                {
                    selectedFighters.Clear();
                    _foundMovableFighters = false;
                    break;
                }
            }
        }
    }

    private void MidGameState()
    {
        foreach (Planet planet in planets)
        {
            GameObject moveToPlanet = null;
            if (planet.planetOwnedBy == enemyName && planet.NumOrbitingOpponents(enemyName) < 1 && (FighterManager.Instance.fighters.Count <= fighters.Count) && planet.activeStation == null && (Time.time > _buildNextStation))
            {
                bool firstCloset = false, secondClosest = false;
                float distanceFirstCloset = 20f, distanceSecondClosest = 20f;
                foreach(Planet nearPlanet in planets)
                {
                    float tempDistance = Vector3.Distance(planet.transform.position, nearPlanet.transform.position);
                    if (tempDistance < distanceFirstCloset && (nearPlanet.planetOwnedBy == enemyName) && nearPlanet.NumOrbitingOpponents(enemyName) < 1)
                    {
                        distanceFirstCloset = tempDistance;
                        firstCloset = true;
                    }
                    else if (tempDistance >= distanceFirstCloset && tempDistance < distanceSecondClosest
                        && (nearPlanet.planetOwnedBy == enemyName) && (nearPlanet.NumOrbitingOpponents(enemyName) < 1))
                    {
                        distanceSecondClosest = tempDistance;
                        secondClosest = true;
                    }
                }
                if (firstCloset && secondClosest)
                {
                    if (this.gameObject.name == "EnemyFighterManager1")
                    {
                        planet.BuildStation("ENEMY");
                    }
                    else
                    {
                        planet.BuildStation("ENEMY2");

                    }
                    _buildNextStation = Time.time + _buildStationRate;
                    break;
                }
            }
            if (planet.planetOwnedBy == enemyName && planet.NumOrbitingFighters(enemyName) > 4 && planet.NumOrbitingOpponents(enemyName) < 1 && !_foundMovableFighters)
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
                int orbitingOpponents = 1000;
                foreach (Planet planetToLocate in planets)
                {
                    if (planetToLocate.planetOwnedBy != enemyName && (planetToLocate.NumOrbitingFighters(enemyName) <= planetToLocate.NumOrbitingOpponents(enemyName) || planetToLocate.NumOrbitingOpponents(enemyName) < 1))
                    {
                        float tempDistance = Vector3.Distance(planet.transform.position, planetToLocate.transform.position);
                        int tempOrbitingOpponents = planetToLocate.NumOrbitingOpponents(enemyName);
                        if (tempDistance < distance && tempOrbitingOpponents < orbitingOpponents)
                        {
                            distance = tempDistance;
                            orbitingOpponents = tempOrbitingOpponents;
                            moveToPlanet = planetToLocate.gameObject;
                        }
                    }
                }
                if (moveToPlanet != null)
                {
                    MoveFighters(moveToPlanet);
                    _foundMovableFighters = false;
                    break;
                }
                else
                {
                    selectedFighters.Clear();
                    _foundMovableFighters = false;
                    break;
                }
            }
        }
    }

    private void EndGameLosing()
    {
        foreach (Planet planet in planets)
        {
            GameObject moveToPlanet = null;
            if (planet.planetOwnedBy == enemyName && planet.NumOrbitingFighters(enemyName) > 5 && planet.NumOrbitingOpponents(enemyName) < 1 && !_foundMovableFighters)
            {
                for (int i = 0; i < 4; i++)
                {
                    selectedFighters.Add(planet.orbitingFighters[i]);
                    _foundMovableFighters = true;
                }
            }
            if (_foundMovableFighters)
            {
                float distance = Mathf.Infinity;
                int orbitingOpponents = 1000;
                foreach (Planet planetToLocate in planets)
                {
                    if (planetToLocate.planetOwnedBy != enemyName && (planetToLocate.NumOrbitingFighters(enemyName) < 5) && (planet != planetToLocate))
                    {
                        float tempDistance = Vector3.Distance(planet.transform.position, planetToLocate.transform.position);
                        int tempOrbitingOpponents = planetToLocate.NumOrbitingOpponents(enemyName);
                        if (tempDistance < distance && tempOrbitingOpponents < orbitingOpponents)
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
                    break;
                }
                else
                {
                    selectedFighters.Clear();
                    _foundMovableFighters = false;
                    break;
                }
            }
        }
    }

    private void EndGameWinning()
    {
        foreach (Planet planet in planets)
        {
            GameObject moveToPlanet = null;
            if (planet.planetOwnedBy == enemyName && planet.NumOrbitingFighters(enemyName) > 4 && planet.NumOrbitingOpponents(enemyName) < 1 && !_foundMovableFighters)
            {
                for (int i = 0; i < (planet.NumOrbitingFighters(enemyName) - 2); i++)
                {
                    selectedFighters.Add(planet.orbitingFighters[i]);
                    _foundMovableFighters = true;
                }
            }
            if (_foundMovableFighters)
            {
                foreach (Planet plantToLocate in planets)
                {
                    if (plantToLocate.planetOwnedBy != enemyName && (plantToLocate.NumOrbitingOpponents(enemyName) > plantToLocate.NumOrbitingFighters(enemyName)))
                    {
                        moveToPlanet = plantToLocate.gameObject;
                    }
                }
                if (moveToPlanet != null)
                {
                    MoveFighters(moveToPlanet);
                    _foundMovableFighters = false;
                    break;
                }
                else
                {
                    selectedFighters.Clear();
                    _foundMovableFighters = false;
                    break;
                }
            }
        }
    }
}
