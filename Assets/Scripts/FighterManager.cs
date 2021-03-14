using System.Collections.Generic;
using UnityEngine;

public class FighterManager : Singleton<FighterManager>
{
    public List<GameObject> fighters = new List<GameObject>();
    public List<GameObject> selectedFighters = new List<GameObject>();

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
        UIManager.Instance.UpdateTeamAText(fighters.Count);
    }
    public void RemoveFighter(GameObject fighter)
    {
        fighters.Remove(fighter);
        if (fighters.Count < 1)
        {
            LevelManager.Instance.GameOver(false);
        }
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateTeamAText(fighters.Count);
        }        
    }

    public void AddSelectedFighters(GameObject fighter)
    {
        selectedFighters.Add(fighter);
        fighter.GetComponent<Fighter>().outline.enabled = true;
    }

    public void RemoveSelectedFighter(GameObject fighter)
    {
        selectedFighters.Remove(fighter);
        fighter.GetComponent<Fighter>().outline.enabled = false;
    }

    public void DeselectAllFighters()
    {
        foreach (GameObject fighter in selectedFighters)
        {
            fighter.GetComponent<Fighter>().outline.enabled = false;
        }
        selectedFighters.Clear();
    }
}
