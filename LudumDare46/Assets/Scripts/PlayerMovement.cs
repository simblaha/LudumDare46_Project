using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public int jumpForce;

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
            rb.AddForce(Vector2.up * jumpForce);
        }
    }
}
