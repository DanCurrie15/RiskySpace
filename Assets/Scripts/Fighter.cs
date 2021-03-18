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

    void Start()
    {
        _isOrbiting = true;
        _orbitingPlanet.GetComponent<Planet>().RegisterFighter(this.gameObject);
    }

    private void OnEnable()
    {
        if (this.gameObject.CompareTag("Player"))
        {
            FighterManager.Instance.AddFighter(this.gameObject);
            outline.enabled = false;
        }
        else if (this.gameObject.CompareTag("Enemy"))
        {
            EnemyManager.Instance.AddFighter(this.gameObject);
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
            if (EnemyManager.Instance != null)
            {
                EnemyManager.Instance.RemoveFighter(this.gameObject);
                EnemyManager.Instance.RemoveSelectedFighter(this.gameObject);
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
        else if (this.gameObject.CompareTag("Enemy"))
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
