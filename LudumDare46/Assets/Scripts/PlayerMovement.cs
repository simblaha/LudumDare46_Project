using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public int jumpForce;
    public LayerMask groundLayer;

    private Vector2 move;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        move = new Vector2(Input.GetAxisRaw("Horizontal") * moveSpeed, rb.velocity.y);
        rb.velocity = move;
        if (Input.GetButtonDown("Jump"))
        {
            if (GroundCheck())
            {
                rb.AddForce(Vector2.up * jumpForce);
            }
        }
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
