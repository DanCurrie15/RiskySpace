using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FighterManager : MonoBehaviour
{
    public List<GameObject> fighters = new List<GameObject>();
    public List<GameObject> selectedFighters = new List<GameObject>();

    public GameObject targetPlanet;

    // Start is called before the first frame update
    void Start()
    {
        
    }
}
