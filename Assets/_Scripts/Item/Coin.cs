using UnityEngine;

public class Coin : Items
{
    private Animator anim;
    private AudioSource audioSource;
    [SerializeField] private AudioClip collectSound;

    private void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public override void Interact()
    {
        GameController.Instance.AddScore(10f);
        audioSource.PlayOneShot(collectSound);
        anim.SetTrigger("Collected");
    }
}