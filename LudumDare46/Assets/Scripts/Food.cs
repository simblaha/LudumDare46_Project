using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] throwingSFX;
    public AudioClip landingSFX;

    private void Awake()
    {
        audioSource.PlayOneShot(throwingSFX[Random.Range(0, throwingSFX.Length - 1)], 0.25f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player")
        {
            audioSource.PlayOneShot(landingSFX, 0.25f);
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            GetComponent<Rigidbody2D>().gravityScale = 0;
        }
    }
}
