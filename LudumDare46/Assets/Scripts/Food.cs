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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.transform.name);
        audioSource.PlayOneShot(landingSFX, 0.25f);
    }
}
