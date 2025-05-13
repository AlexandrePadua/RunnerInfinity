using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform groundCheckTransform;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private AudioClip deathSound;
 
    private Rigidbody2D rig;
    private AudioSource audioSource;

    [Header("Gameplay Settings")]
    [SerializeField] private float swipeSensivity = 1f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;

    private Vector2 fingerDown;
    private Vector2 fingerUp;

    private bool isGrounded = true;
    public bool IsGrounded { get => isGrounded; set => isGrounded = value; }

    public static event Action onPlayerDeath;

    private bool isDead = false;
    public bool IsDead { get => isDead; set => isDead = value; }

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (IsDead) return;

        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                fingerDown = touch.position;
                fingerUp = touch.position;
            }

            if (touch.phase == TouchPhase.Ended)
            {
                fingerDown = touch.position;
                CheckSwipe();
            }
        }

        if(rig.linearVelocity.y < 0.1f)
        {
            rig.gravityScale = 2.2f;
        }
        else
        {
            rig.gravityScale = 1.5f;
        }

        IsGrounded = Physics2D.OverlapCircle(groundCheckTransform.position, groundCheckRadius, groundLayerMask);
    }

    void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (IsDead) return;

        Vector2 velocity = new Vector2(moveSpeed * GameController.Instance.difficultyMultiplier, rig.linearVelocity.y);
        rig.linearVelocity = velocity;
    }

    void CheckSwipe()
    {
        if (VerticalMove() > swipeSensivity && VerticalMove() > HorizontalMove())
        {
            if (fingerDown.y - fingerUp.y > 0)
            {
                Debug.Log("Up");

                OnSwipeUp();
            }
            else if (fingerDown.y - fingerUp.y < 0)
            {
                Debug.Log("Down");
            }
        }
        else if (HorizontalMove() > swipeSensivity && HorizontalMove() > VerticalMove())
        {
            if (fingerDown.x - fingerUp.x > 0)
            {
                Debug.Log("Boost");
            }
            else if (fingerDown.x - fingerUp.x < 0)
            {
                Debug.Log("Nothing");
            }
        }
    }

    private void OnSwipeUp()
    {
        if (IsGrounded)
        {
            Jump();
        }
    }

    private float VerticalMove()
    {
        return Mathf.Abs(fingerDown.y - fingerUp.y);
    }

    private float HorizontalMove()
    {
        return Mathf.Abs(fingerDown.x - fingerUp.x);
    }

    void Jump()
    {
        rig.linearVelocity = new Vector2(rig.linearVelocity.x, 0);
        rig.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            collision.gameObject.GetComponent<Items>().Interact();
        }

        if (collision.CompareTag("Enemy"))
        {
            audioSource.PlayOneShot(deathSound);
            rig.linearVelocity = Vector2.zero;
            onPlayerDeath?.Invoke();
            IsDead = true;            
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheckTransform.position, groundCheckRadius);
    }
}