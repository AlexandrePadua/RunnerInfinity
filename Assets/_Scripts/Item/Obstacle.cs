using UnityEngine;

public class Obstacle : Items
{
    void Start()
    {
        
    }

    public override void Interact()
    {
        Debug.Log("Atingiu um obstáculo");
    }
}
