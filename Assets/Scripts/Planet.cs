using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Planet : MonoBehaviour
{
    public float orbitDistance;
    public float spawnRate;
    public GameObject spawnPoint;
    public GameObject stationSpawnPoint;
    public Renderer planetRenderer;
    public Outline outline;

    public GameObject playerFighter;
    public GameObject playerStation;
    public GameObject activeStation;
    public int orbitingPlayers;

    public GameObject enemyFighter;
    public GameObject enemyStation;
    public int orbitingEnemies;

    public GameObject enemy2Fighter;
    public GameObject enemy2Station;
    public int orbitingEnemies2;

    //[Range(0f, 1f)]
    //public float ownership; // 0 - player, 1 - enemy, 0.5 - neutral

    public List<GameObject> orbitingFighters = new List<GameObject>();
    public GameObject orbitingStation;

    private float _nextSpawn;

    private float _nextFight;
    private float _fightRate;

    [Header("Planet Ownership")]
    public string planetOwnedBy;
    [Range(0f, 1f)]
    private float _playerOwnership;
    [Range(0f, 1f)]
    private float _enemy1Ownership;
    [Range(0f, 1f)]
    private float _enemy2Ownership;

    private const float OWNERSHIP_CHANGE_RATE = 0.0007f;

    [Header("Planet Colour")]
    // blank colour is 1f, 1f, 1f, 1f
    // green colour is 0f, 1f, 0.5f, 1f
    private Color greenPlanet = new Color(0f, 1f, 0.5f, 1f);
    // purple colour is 0.5f, 0f, 1f, 1f
    private Color purplePlanet = new Color(0.5f, 0f, 1f, 1f);
    // orange colour is 1f, 0.5f, 0f, 1f
    private Color orangePlanet = new Color(1f, 0.5f, 0f, 1f);
    // neutral / grey colour
    private Color neutralPlanet = new Color(1f, 1f, 1f, 1f);

    [Range(0f, 1f)]
    public float r;
    [Range(0f, 1f)]
    public float g;
    [Range(0f, 1f)]
    public float b;

    private void Start()
    {
        outline.enabled = false;
        orbitingPlayers = 0;
        orbitingEnemies = 0;
        _nextSpawn = 0;
        _fightRate = 1f;
        activeStation = null;
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
        int numEnemy2 = 0;

        foreach (GameObject fighter in orbitingFighters)
        {
            if (fighter != null)
            {
                if (fighter.CompareTag("Player"))
                {
                    numPlayer++;
                }
                else if (fighter.CompareTag("Enemy2"))
                {
                    numEnemy2++;
                }
                else
                {
                    numEnemy++;
                }
            }
        }
        orbitingPlayers = numPlayer;
        orbitingEnemies = numEnemy;
        orbitingEnemies2 = numEnemy2;

        if (numPlayer > 0 && numEnemy == 0 && numEnemy2 == 0 && planetOwnedBy != "PLAYER" && activeStation == null)
        {
            Ownership("PLAYER");
            outline.enabled = false;

        }
        else if (numEnemy > 0 && numPlayer == 0 && numEnemy2 == 0 && planetOwnedBy != "ENEMY" && activeStation == null)
        {
            Ownership("ENEMY");
            outline.enabled = false;
        }
        else if (numEnemy2 > 0 && numPlayer == 0 && numEnemy == 0 && planetOwnedBy != "ENEMY2" && activeStation == null)
        {
            Ownership("ENEMY2");
            outline.enabled = false;
        }
        else if ((Time.time > _nextFight) && ((numEnemy > 0 && numPlayer > 0) || (numEnemy > 0 && numEnemy2 > 0)
                || (numEnemy2 > 0 && numPlayer > 0) || (numPlayer > 0 && numEnemy > 0 && numEnemy2 > 0)))
        {
            outline.enabled = false;
            _nextFight = Time.time + _fightRate;
            int rand = Random.Range(0, 3);
            if (numPlayer == 0 && rand == 0)
            {
                rand = 1;
            }
            else if (numEnemy == 0 && rand == 1)
            {
                rand = 2;
            }
            else if (numEnemy2 == 0 && rand == 2)
            {
                rand = 1;
            }
            foreach(GameObject fighter in orbitingFighters)
            {
                if (rand == 0)
                {
                    if (activeStation != null && activeStation.CompareTag("PlayerStation"))
                    {
                        SoundManager.Instance.PlaySoundEffect(SoundEffect.EnemyLaser);
                        activeStation.GetComponent<Station>().SubtractHealth();
                        break;
                    }
                    else if (fighter.CompareTag("Player"))
                    {
                        SoundManager.Instance.PlaySoundEffect(SoundEffect.EnemyLaser);
                        fighter.GetComponent<Fighter>().Explosion();
                        Destroy(fighter, 0.15f);
                        break;
                    }
                }
                else if (rand == 1)
                {
                    if (activeStation != null && activeStation.CompareTag("EnemyStation"))
                    {
                        SoundManager.Instance.PlaySoundEffect(SoundEffect.PlayerLaser);
                        activeStation.GetComponent<Station>().SubtractHealth();
                        break;
                    }
                    else if (fighter.CompareTag("Enemy"))
                    {
                        SoundManager.Instance.PlaySoundEffect(SoundEffect.PlayerLaser);
                        fighter.GetComponent<Fighter>().Explosion();
                        Destroy(fighter, 0.15f);
                        break;
                    }
                }
                else if (rand == 2)
                {
                    if (activeStation != null && activeStation.CompareTag("Enemy2Station"))
                    {
                        SoundManager.Instance.PlaySoundEffect(SoundEffect.PlayerLaser);
                        activeStation.GetComponent<Station>().SubtractHealth();
                        break;
                    }
                    else if (fighter.CompareTag("Enemy2"))
                    {
                        SoundManager.Instance.PlaySoundEffect(SoundEffect.PlayerLaser);
                        fighter.GetComponent<Fighter>().Explosion();
                        Destroy(fighter, 0.15f);
                        break;
                    }
                }
            }
        }
        else if ((Time.time > _nextSpawn))
        {
            _nextSpawn = Time.time + spawnRate;

            if (activeStation != null && !activeStation.GetComponent<Station>()._isOrbiting)
            {
                return;
            }
            else if (planetOwnedBy == "PLAYER" && numEnemy == 0 && numEnemy2 == 0)
            {                
                planetRenderer.material.color = purplePlanet;
                SpawnShip(playerFighter);
                outline.enabled = true;
                outline.OutlineColor = purplePlanet;
            }
            else if (planetOwnedBy == "ENEMY" && numPlayer == 0 && numEnemy2 == 0)
            {
                planetRenderer.material.color = greenPlanet;
                SpawnShip(enemyFighter);
                outline.enabled = true;
                outline.OutlineColor = greenPlanet;
            }
            else if (planetOwnedBy == "ENEMY2" && numPlayer == 0 && numEnemy == 0)
            {
                planetRenderer.material.color = orangePlanet;
                SpawnShip(enemy2Fighter);
                outline.enabled = true;
                outline.OutlineColor = orangePlanet;
            }
        }
    }

    private void SpawnShip(GameObject ship)
    {
        GameObject _ship = Instantiate(ship, spawnPoint.transform.position, Quaternion.identity);
        _ship.GetComponent<Fighter>()._orbitingPlanet = this.gameObject;
    }

    public void BuildStation(string team)
    {
        GameObject _station = null;
        if (team == "PLAYER")
        {
            _station = Instantiate(playerStation, stationSpawnPoint.transform.position, Quaternion.identity);
            _station.GetComponent<Station>().BuildStation(this.gameObject);            
        }
        else if (team == "ENEMY")
        {
            _station = Instantiate(enemyStation, stationSpawnPoint.transform.position, Quaternion.identity);
            _station.GetComponent<Station>().BuildStation(this.gameObject);
        }
        else if (team == "ENEMY2")
        {
            _station = Instantiate(enemy2Station, stationSpawnPoint.transform.position, Quaternion.identity);
            _station.GetComponent<Station>().BuildStation(this.gameObject);
        }
        activeStation = _station;
    }

    public void Ownership(string team)
    {
        if (team == "PLAYER")
        {
            if (_playerOwnership >= 1f)
            {
                planetOwnedBy = "PLAYER";
            }
            else if (_enemy1Ownership > 0 || _enemy2Ownership > 0)
            {
                _enemy1Ownership -= OWNERSHIP_CHANGE_RATE;
                _enemy2Ownership -= OWNERSHIP_CHANGE_RATE;
                planetRenderer.material.color = Color.Lerp(planetRenderer.material.color, neutralPlanet, Mathf.Clamp01(_enemy1Ownership + _enemy2Ownership) * Time.deltaTime * 0.3f);
            }
            else
            {
                _playerOwnership += OWNERSHIP_CHANGE_RATE;
                planetRenderer.material.color = Color.Lerp(planetRenderer.material.color, purplePlanet, _playerOwnership * Time.deltaTime * 0.3f);
            }
        }
        else if (team == "ENEMY")
        {          
            if (_enemy1Ownership >= 1f)
            {
                planetOwnedBy = "ENEMY";
            }
            else if (_playerOwnership > 0 || _enemy2Ownership > 0)
            {
                _playerOwnership -= OWNERSHIP_CHANGE_RATE;
                _enemy2Ownership -= OWNERSHIP_CHANGE_RATE;
                planetRenderer.material.color = Color.Lerp(planetRenderer.material.color, neutralPlanet, Mathf.Clamp01(_playerOwnership + _enemy2Ownership) * Time.deltaTime * 0.3f);
            }
            else
            {
                _enemy1Ownership += OWNERSHIP_CHANGE_RATE;
                planetRenderer.material.color = Color.Lerp(planetRenderer.material.color, greenPlanet, _enemy1Ownership * Time.deltaTime * 0.3f);
            }
        }
        else if (team == "ENEMY2")
        {             
            if (_enemy2Ownership >= 1f)
            {
                planetOwnedBy = "ENEMY2";
            }
            else if (_playerOwnership > 0 || _enemy1Ownership > 0)
            {
                _playerOwnership -= OWNERSHIP_CHANGE_RATE;
                _enemy1Ownership -= OWNERSHIP_CHANGE_RATE;
                planetRenderer.material.color = Color.Lerp(planetRenderer.material.color, neutralPlanet, Mathf.Clamp01(_enemy1Ownership + _playerOwnership) * Time.deltaTime * 0.3f);
            }
            else
            {
                _enemy2Ownership += OWNERSHIP_CHANGE_RATE;
                planetRenderer.material.color = Color.Lerp(planetRenderer.material.color, orangePlanet, _enemy2Ownership * Time.deltaTime * 0.3f);
            }
        }

        if (_playerOwnership < 1f && _enemy1Ownership < 1f && _enemy2Ownership < 1f)
        {
            planetOwnedBy = "NEUTRAL";
        }
    }

    public int NumOrbitingOpponents(string team)
    {
        if (team == "PLAYER")
        {
            return orbitingEnemies + orbitingEnemies2;
        }
        else if (team == "ENEMY")
        {
            return orbitingEnemies2 + orbitingPlayers;
        }
        else if (team == "ENEMY2")
        {
            return orbitingPlayers + orbitingEnemies;
        }
        return 0;
    }

    public int NumOrbitingFighters(string team)
    {
        if (team == "PLAYER")
        {
            return orbitingPlayers;
        }
        else if (team == "ENEMY")
        {
            return orbitingEnemies;
        }
        else if (team == "ENEMY2")
        {
            return orbitingEnemies2;
        }
        return 0;
    }
}
