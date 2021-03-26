using UnityEngine;
using System.Collections;

public class Fighter : MonoBehaviour
{
    public GameObject _orbitingPlanet;
    public Outline outline;
    public GameObject explosionPrefab;

    private GameObject _targetPlanet;
    private bool _isOrbiting;
    private float _orbitSpeed = 20f;
    private float _travelSpeed = 20f;
    private GameObject _enemyFighterManager1;
    private GameObject _enemyFighterManager2;

    void Start()
    {
        _isOrbiting = true;
        _orbitingPlanet.GetComponent<Planet>().RegisterFighter(this.gameObject);
    }

    private void OnEnable()
    {        
        if (this.gameObject.CompareTag("Player"))
        {
            outline.enabled = false;
            FighterManager.Instance.AddFighter(this.gameObject);            
        }
        else if (this.gameObject.CompareTag("Enemy"))
        {
            _enemyFighterManager1 = GameObject.Find("EnemyFighterManager1");
            _enemyFighterManager1.GetComponent<EnemyManager>().AddFighter(this.gameObject);
        }
        else if (this.gameObject.CompareTag("Enemy2"))
        {
            _enemyFighterManager2 = GameObject.Find("EnemyFighterManager2");
            _enemyFighterManager2.GetComponent<EnemyManager>().AddFighter(this.gameObject);
        }
    }

    private void OnDisable()
    {     
        if (_orbitingPlanet != null)
        {
            _orbitingPlanet.GetComponent<Planet>().UnregisterFighter(this.gameObject);
            _orbitingPlanet = null;
        }

        if (this.gameObject.CompareTag("Player"))
        {
            if (FighterManager.Instance != null)
            {
                FighterManager.Instance.RemoveFighter(this.gameObject);
                FighterManager.Instance.RemoveSelectedFighter(this.gameObject);
            }            
            
        }
        else if (this.gameObject.CompareTag("Enemy"))
        {
            if (_enemyFighterManager1 != null)
            {
                _enemyFighterManager1.GetComponent<EnemyManager>().RemoveFighter(this.gameObject);
                _enemyFighterManager1.GetComponent<EnemyManager>().RemoveSelectedFighter(this.gameObject);
            }
        }
        else if (this.gameObject.CompareTag("Enemy2"))
        {
            if (_enemyFighterManager2 != null)
            {
                _enemyFighterManager2.GetComponent<EnemyManager>().RemoveFighter(this.gameObject);
                _enemyFighterManager2.GetComponent<EnemyManager>().RemoveSelectedFighter(this.gameObject);
            }
        }
    }

    void Update()
    {
        if (_isOrbiting)
        {
            transform.RotateAround(_orbitingPlanet.transform.position, Vector3.up, _orbitSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPlanet.transform.position, _travelSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, _targetPlanet.transform.position) < _targetPlanet.GetComponent<Planet>().orbitDistance)
            {
                _isOrbiting = true;
                _orbitingPlanet = _targetPlanet;
                _orbitingPlanet.GetComponent<Planet>().RegisterFighter(this.gameObject);
                _targetPlanet = null;
            }
        }
    }

    public void MoveToPlanet(GameObject targetPlanet)
    {
        _isOrbiting = false;
        _orbitingPlanet.GetComponent<Planet>().UnregisterFighter(this.gameObject);
        _orbitingPlanet = null;
        _targetPlanet = targetPlanet;
        if (this.gameObject.CompareTag("Player"))
        {
            SoundManager.Instance.PlaySoundEffect(SoundEffect.PlayerShipMoves);
        }
        else
        {
            SoundManager.Instance.PlaySoundEffect(SoundEffect.EnemyShipMoves);
        }
    }

    public void Explosion()
    {
        StartCoroutine(ExplosionWithWait());
    }

    IEnumerator ExplosionWithWait()
    {
        yield return new WaitForSeconds(0.1f);
        SoundManager.Instance.PlaySoundEffect(SoundEffect.ShipExplosion);
        Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
    }
}
