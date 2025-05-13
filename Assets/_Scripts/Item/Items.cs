using UnityEngine;

public abstract class Items : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 3f);
    }

    public abstract void Interact();
}