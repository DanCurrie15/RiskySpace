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
    public GameObject enemyFighter;

    [SerializeField]
    private float _ownership; // 0 - player, 1 - enemy, 0.5 - neutral

    public List<GameObject> orbitingFighters = new List<GameObject>();

    private float _nextSpawn;

    private void Start()
    {
        _nextSpawn = 0;
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
        if ((Time.time > _nextSpawn))
        {
            _nextSpawn = Time.time + spawnRate;

            int numPlayer = 0;
            int numEnemy = 0;

            foreach(GameObject fighter in orbitingFighters)
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

            if (numPlayer > 0 && numEnemy == 0)
            {
                if (_ownership < 0)
                {
                    SpawnShip(playerFighter);
                }
                else
                {
                    _ownership -= 0.1f;
                }
            }
            else if (numEnemy > 0 && numPlayer == 0)
            {
                if (_ownership > 1)
                {
                    SpawnShip(enemyFighter);
                }
                else
                {
                    _ownership += 0.1f;
                }
            }
        }        
    }

    private void SpawnShip(GameObject ship)
    {
        GameObject _ship = Instantiate(ship, spawnPoint.transform.position, Quaternion.identity);
        _ship.GetComponent<Fighter>()._orbitingPlanet = this.gameObject;
    }
}
