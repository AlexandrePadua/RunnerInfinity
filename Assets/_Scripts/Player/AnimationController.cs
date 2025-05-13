using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationController : MonoBehaviour
{
    private Animator anim;
    private PlayerController player;

    void Awake()
    {
        anim = GetComponent<Animator>();
        player = GetComponentInParent<PlayerController>();
    }

    private void OnEnable()
    {
        PlayerController.onPlayerDeath += GameOver;
    }

    private void OnDisable()
    {
        PlayerController.onPlayerDeath -= GameOver;
    }

    void Update()
    {
        anim.SetBool("Jump", !player.IsGrounded);
    }

    void GameOver()
    {
        anim.SetBool("Dead", true);
    }
}