using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public float climbSpeed;
    public int jumpForce;
    public LayerMask groundLayer;
    public AudioClip[] footStepSFX;
    public float footstepDelay;

    private Vector2 move;
    private Rigidbody2D rb;
    private bool canClimb;
    private bool isWalking;
    private Animator animator;
    private Vector2 facingDirection;
    private AudioSource audioSource;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        facingDirection = Vector2.right;
    }

    private void Update()
    {
        if (canClimb)
            move = new Vector2(Input.GetAxisRaw("Horizontal") * moveSpeed, Input.GetAxisRaw("Vertical") * climbSpeed);
        else
            move = new Vector2(Input.GetAxisRaw("Horizontal") * moveSpeed, rb.velocity.y);
        if (move.x != 0 && move.y == 0)
        {
            if (!isWalking)
            {
                isWalking = true;
                StartCoroutine(FootSteps());
                animator.SetBool("isWalking", true);
            }
        }
        else if (isWalking)
        {
            isWalking = false;
            animator.SetBool("isWalking", false);
        }
        if (move.x > 0)
            Turn(1);
        else if (move.x < 0)
            Turn(-1);
        rb.velocity = move;
        if (!GroundCheck() && canClimb)
        {
            rb.gravityScale = 0;
        }
        else
            rb.gravityScale = 1;
        /*
        if (Input.GetButtonDown("Jump"))
        {
            if (GroundCheck())
            {
                rb.AddForce(Vector2.up * jumpForce);
            }
        }
        */
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ladder")
            canClimb = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ladder")
            canClimb = false;
    }

    bool GroundCheck()
    {
        if (Physics2D.Raycast(transform.position, Vector2.down, GetComponent<Collider2D>().bounds.extents.y + 0.01f, groundLayer))
        {
            return true;
        }
        return false;
    }

    void Turn(int value)
    {
        transform.localScale = new Vector3(value, 1, 1);
        facingDirection = new Vector2(value, 0);
    }

    IEnumerator FootSteps()
    {
        while(isWalking)
        {
            audioSource.PlayOneShot(footStepSFX[Random.Range(0, footStepSFX.Length - 1)], 0.1f);
            yield return new WaitForSeconds(footstepDelay);
        }
    }
}
