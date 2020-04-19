using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public float climbSpeed;
    public int jumpForce;
    public LayerMask groundLayer;

    private Vector2 move;
    private Rigidbody2D rb;
    private bool canClimb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (canClimb)
            move = new Vector2(Input.GetAxisRaw("Horizontal") * moveSpeed, Input.GetAxisRaw("Vertical") * climbSpeed);
        else
            move = new Vector2(Input.GetAxisRaw("Horizontal") * moveSpeed, rb.velocity.y);
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
}
