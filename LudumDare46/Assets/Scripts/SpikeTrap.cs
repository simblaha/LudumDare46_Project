using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    public TrapTrigger trigger;
    public int damage;
    public float extendSpeed;
    public float retractSpeed;
    public float extendAmount;
    public AudioClip spikeTrapSFX;

    private AudioSource audioSource;
    private Vector2 retractPosition;
    private Vector2 extendPosition;
    private Collider2D col;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        col = GetComponent<Collider2D>();
        col.enabled = false;
        retractPosition = transform.position;
        extendPosition = (Vector2)transform.position + (Vector2.up * extendAmount);
    }

    private void Update()
    {
        if (trigger.isTriggered)
            Extend();
        else
            Retract();
    }

    private void Extend()
    {
        if (Vector2.Distance(transform.position, extendPosition) > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, extendPosition, Time.deltaTime * extendSpeed);
        }
        else if (!col.enabled)
        {
            col.enabled = true;
            //audioSource.PlayOneShot(spikeTrapSFX, 0.25f);
        }
    }

    private void Retract()
    {
        if (col.enabled)
        {
            col.enabled = false;
        }
        if (Vector2.Distance(transform.position, retractPosition) > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, retractPosition, Time.deltaTime * retractSpeed);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Hit");
        if (collision.tag == "Zombie")
        {
            collision.GetComponent<Zombie>().ChangeHealth(-damage, true);
        }
        else
        {
            Destroy(collision.gameObject);
        }
    }
}
