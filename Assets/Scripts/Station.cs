using UnityEngine;
using System.Collections;

public class Station : MonoBehaviour
{
    private int _health = 10;
    public bool _isOrbiting = false;
    private GameObject _orbitingPlanet;
    private float _orbitSpeed = 10f;
    private Vector3 _scaleChange = new Vector3(0.002f, 0.002f, 0.002f);
    private bool _isBuilding = false;

    public GameObject explosionPrefab;

    private void Start()
    {
        _isOrbiting = false;
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    }

    private void Update()
    {
        if (_isOrbiting)
        {
            transform.RotateAround(_orbitingPlanet.transform.position, Vector3.up, _orbitSpeed * Time.deltaTime);
        }
        if (_isBuilding)
        {
            transform.localScale += _scaleChange;
            if (transform.localScale.y > 1.0f)
            {
                _isBuilding = false;
                transform.localScale = new Vector3(1f, 1f, 1f);
                _isOrbiting = true;
                Debug.Log("Station - finished building");
            }
        }
    }

    public void SubtractHealth()
    {
        _health--;
        if (_health == 0)
        {
            _orbitingPlanet.GetComponent<Planet>().activeStation = null;
            Destroy(this.gameObject, 0.05f);
        }
        SoundManager.Instance.PlaySoundEffect(SoundEffect.ShipExplosion);
        Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
    }

    public void BuildStation(GameObject planetToOrbit)
    {
        Debug.Log("Station - Build Station");
        _orbitingPlanet = planetToOrbit;
        _isBuilding = true;
    }
}
