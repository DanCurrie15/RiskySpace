using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterManager : Singleton<FighterManager>
{
    public List<GameObject> fighters = new List<GameObject>();
    public List<GameObject> selectedFighters = new List<GameObject>();
    public GameObject targetPlanet;

    public void MoveFighters(GameObject targetPlanet)
    {
        foreach(GameObject fighter in selectedFighters)
        {
            fighter.GetComponent<Fighter>().MoveToPlanet(targetPlanet);
        }
        DeselectAllFighters();
    }

    public void AddFighter(GameObject fighter)
    {
        fighters.Add(fighter);
    }
    public void RemoveFighter(GameObject fighter)
    {
        fighters.Remove(fighter);
    }

    public void AddSelectedFighters(GameObject fighter)
    {
        selectedFighters.Add(fighter);
    }

    public void DeselectAllFighters()
    {
        selectedFighters.Clear();
    }
}
