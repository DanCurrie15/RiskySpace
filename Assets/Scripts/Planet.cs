using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Planet : MonoBehaviour
{
    public float orbitDistance;
    public float spawnRate;
    public GameObject spawnPoint;
    public Renderer planetRenderer;

    public GameObject playerFighter;
    public int orbitingPlayers;
    public GameObject enemyFighter;
    public int orbitingEnemies;

    [Range(0f, 1f)]
    public float _ownership; // 0 - player, 1 - enemy, 0.5 - neutral

    public List<GameObject> orbitingFighters = new List<GameObject>();

    private float _nextSpawn;

    private float _nextFight;
    private float _fightRate;

    [Header("Planet Colour")]
    // blank colour is 1f, 1f, 1f, 1f
    // green colour is 0f, 1f, 0.5f, 1f
    private Color greenPlanet = new Color(0f, 1f, 0.5f, 1f);
    // purple colour is 0.5f, 0f, 1f, 1f
    private Color purplePlanet = new Color(0.5f, 0f, 1f, 1f);
    [Range(0f, 1f)]
    public float r;
    [Range(0f, 1f)]
    public float g;
    [Range(0f, 1f)]
    public float b;

    private void Start()
    {
        orbitingPlayers = 0;
        orbitingEnemies = 0;
        _nextSpawn = 0;
        _fightRate = 1;
        planetRenderer.material.color = new Color(r, g, b, 1f);
    }

    public void RegisterFighter(GameObject fighter)
    {
        orbitingFighters.Add(fighter);
    }

    public void UnregisterFighter(GameObject fighter)
    {
        orbitingFighters.Remove(fighter);
    }

    private void Update()
    {
        int numPlayer = 0;
        int numEnemy = 0;

        foreach (GameObject fighter in orbitingFighters)
        {
            if (fighter != null)
            {
                if (fighter.CompareTag("Player"))
                {
                    numPlayer++;
                }
                else
                {
                    numEnemy++;
                }
            }
        }
        orbitingPlayers = numPlayer;
        orbitingEnemies = numEnemy;

        if (numPlayer > 0 && numEnemy == 0 && _ownership > 0)
        {
            planetRenderer.material.color = Color.Lerp(planetRenderer.material.color, purplePlanet, (1f - _ownership) * Time.deltaTime * 0.3f);
            _ownership -= 0.0005f;
        }
        else if (numEnemy > 0 && numPlayer == 0 && _ownership < 1)
        {
            planetRenderer.material.color = Color.Lerp(planetRenderer.material.color, greenPlanet, _ownership * Time.deltaTime * 0.3f);
            _ownership += 0.0005f;
        }
        else if (numEnemy > 0 && numPlayer > 0 && (Time.time >_nextFight))
        {
            _nextFight = Time.time + _fightRate;
            int rand = Random.Range(0, 2);
            foreach(GameObject fighter in orbitingFighters)
            {
                if (rand == 0)
                {
                    if (fighter.CompareTag("Player"))
                    {
                        Destroy(fighter, 1f);
                        break;
                    }
                }
                else
                {
                    if (fighter.CompareTag("Enemy"))
                    {
                        Destroy(fighter, 1f);
                        break;
                    }
                }                
            }
        }
        else if ((Time.time > _nextSpawn))
        {
            _nextSpawn = Time.time + spawnRate;         

            if (_ownership <= 0 && numEnemy == 0)
            {
                planetRenderer.material.color = purplePlanet;
                SpawnShip(playerFighter);
                _ownership = 0;
            }
            else if (_ownership >= 1 && numPlayer == 0)
            {
                planetRenderer.material.color = greenPlanet;
                SpawnShip(enemyFighter);
                _ownership = 1;
            }          
        }
    }

    private void SpawnShip(GameObject ship)
    {
        GameObject _ship = Instantiate(ship, spawnPoint.transform.position, Quaternion.identity);
        _ship.GetComponent<Fighter>()._orbitingPlanet = this.gameObject;
    }
}
