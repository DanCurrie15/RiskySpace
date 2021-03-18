using UnityEngine;

public class Station : MonoBehaviour
{
    private int health = 10;

    public void SubtractHealth()
    {
        health--;
        if (health == 0)
        {
            Destroy(this.gameObject, 0.15f);
        }
    }
}
