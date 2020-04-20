using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Zombie : MonoBehaviour
{
    [Header("Basic Stats")]
    public int healthBase;
    public Vector2 healthBarOffset;
    public float moveSpeed;

    [Header("TargetDetection")]
    public float detectionRange;
    public LayerMask detectedLayers;
    public Transform target;

    [Header("Misc")]
    public GameObject feedEffect;
    public int healthGainPerFood;
    public float feedDuration;
    public GameObject hitEffect;
    public GameObject deathEffect;
    public AudioClip[] hitMeleeSFX;
    public AudioClip[] idleSFX;

    public int health;
    private Vector2 move;
    private Rigidbody2D rb;
    private Transform player;
    private GameObject healthBar;
    private Vector2 lastPlayerPosition;
    private bool isFeeding;
    private AudioSource audioSource;
    private Animator animator;
    private Vector2 facingDirection;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        facingDirection = Vector2.right;
        player = GameObject.Find("Player").transform;
        healthBar = GameObject.Find("Zombie_HealthBar");
        healthBar.transform.position = Camera.main.WorldToScreenPoint((Vector2)transform.position + healthBarOffset);
        health = healthBase;
        target = player;
    }

    private void Start()
    {
        StartCoroutine(Behaviour());
    }

    private void Update()
    {
        if (move.x != 0)
            animator.SetBool("isWalking", true);
        else
            animator.SetBool("isWalking", false);
        if (move.x > 0)
            Turn(1);
        else if (move.x < 0)
            Turn(-1);
        rb.velocity = move;
        healthBar.transform.position = Camera.main.WorldToScreenPoint((Vector2)transform.position + healthBarOffset);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            target = null;
            Destroy(Instantiate(deathEffect, collision.transform.position, Quaternion.identity), 5f);
            Destroy(collision.gameObject);
            StartCoroutine(Feed());
        }
        else if (collision.tag == "Food")
        {
            if (!isFeeding)
            {
                target = null;
                Destroy(collision.gameObject);
                StartCoroutine(Feed());
            }
        }
        else if (collision.tag == "Player")
        {
            if (!collision.GetComponent<PlayerInteraction>().isHidden)
            {
                player = null;
                target = null;
                Destroy(Instantiate(deathEffect, collision.transform.position, Quaternion.identity), 5f);
                Destroy(collision.gameObject);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Food")
        {
            if (!isFeeding)
            {
                target = null;
                Destroy(collision.gameObject);
                StartCoroutine(Feed());
            }
        }
    }

    public void ChangeHealth(int amount)
    {
        health = Mathf.Clamp(health + amount, 0, healthBase);
        healthBar.transform.Find("Fill").GetComponent<Image>().fillAmount = (float)health / (float)healthBase;
        if (amount < 0)
        {
            Destroy(Instantiate(hitEffect, transform.position, Quaternion.identity), 3f);
            audioSource.PlayOneShot(hitMeleeSFX[Random.Range(0, hitMeleeSFX.Length - 1)], 0.1f);
        }
        if (health == 0)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(healthBar);
            Destroy(gameObject);
        }
    }

    IEnumerator Behaviour()
    {
        while (true)
        {
            if (!isFeeding)
                UpdateTarget();
            if (target == null)
                yield return MoveToLastPlayerPosition();
            else
            {
                yield return MoveToTarget();
            }
        }
    }

    IEnumerator Feed()
    {
        isFeeding = true;
        animator.SetBool("isFeeding", true);
        Destroy(Instantiate(feedEffect, transform.position, Quaternion.identity), feedDuration + 1);
        yield return new WaitForSeconds(feedDuration);
        ChangeHealth(+healthGainPerFood);
        isFeeding = false;
        animator.SetBool("isFeeding", false);
    }

    IEnumerator MoveToTarget()
    {
        float distanceToTarget;
        while (target != null && !isFeeding)
        {
            distanceToTarget = Vector2.Distance(new Vector2(target.position.x, transform.position.y), new Vector2(transform.position.x, transform.position.y));
            if (distanceToTarget > 0.1f)
            {
                Vector2 direction = (new Vector2(target.position.x, 0) - new Vector2(transform.position.x, 0)).normalized;
                move = new Vector2(direction.x * moveSpeed, 0);
                
            }
            else
            {
                move = Vector2.zero;
            }
            yield return new WaitForEndOfFrame();
            UpdateTarget();
        }
        move = Vector2.zero;
    }

    IEnumerator MoveToLastPlayerPosition()
    {
        float distanceToTarget;
        while (target == null && !isFeeding)
        {
            distanceToTarget = Vector2.Distance(new Vector2(lastPlayerPosition.x, transform.position.y), new Vector2(transform.position.x, transform.position.y));
            if (distanceToTarget > 0.01f)
            {
                Vector2 direction = (new Vector2(lastPlayerPosition.x, 0) - new Vector2(transform.position.x, 0)).normalized;
                move = new Vector2(direction.x * moveSpeed, 0);
            }
            else
            {
                move = Vector2.zero;
            }
            yield return new WaitForEndOfFrame();
            UpdateTarget();
        }
        move = Vector2.zero;
    }

    void UpdateTarget()
    {
        target = null;
        Transform newTarget = null;
        RaycastHit2D[] hitsLeft = Physics2D.RaycastAll(transform.position, Vector2.left, detectionRange, detectedLayers);
        RaycastHit2D[] hitsRight = Physics2D.RaycastAll(transform.position, Vector2.right, detectionRange, detectedLayers);
        foreach (RaycastHit2D hit in hitsLeft)
        {
            if (newTarget == null)
                newTarget = hit.transform;
            else
            {
                if (newTarget.tag == "Food")
                {
                    if (hit.transform.tag == "Food")
                    {
                        if (Vector2.Distance(newTarget.position, transform.position) > Vector2.Distance(hit.transform.position, transform.position))
                        {
                            newTarget = hit.transform;
                        }
                    }
                    else if (hit.transform.tag == "Enemy")
                        newTarget = hit.transform;
                }
                else if (newTarget.tag == "Enemy")
                {
                    if (hit.transform.tag == "Enemy")
                    {
                        if (Vector2.Distance(newTarget.position, transform.position) > Vector2.Distance(hit.transform.position, transform.position))
                        {
                            newTarget = hit.transform;
                        }
                    }    
                }
            }
        }
        foreach (RaycastHit2D hit in hitsRight)
        {
            if (newTarget == null)
                newTarget = hit.transform;
            else
            {
                if (newTarget.tag == "Food")
                {
                    if (hit.transform.tag == "Food")
                    {
                        if (Vector2.Distance(newTarget.position, transform.position) > Vector2.Distance(hit.transform.position, transform.position))
                        {
                            newTarget = hit.transform;
                        }
                    }
                    else if (hit.transform.tag == "Enemy")
                        newTarget = hit.transform;
                }
                else if (newTarget.tag == "Enemy")
                {
                    if (hit.transform.tag == "Enemy")
                    {
                        if (Vector2.Distance(newTarget.position, transform.position) > Vector2.Distance(hit.transform.position, transform.position))
                        {
                            newTarget = hit.transform;
                        }
                    }
                }
            }
        }
        if (newTarget != null)
            target = newTarget;
        else
        {
            if (player != null)
                if (!player.GetComponent<PlayerInteraction>().isHidden)
                {
                    lastPlayerPosition = player.transform.position;
                    target = player;
                }
        }
    }

    void Turn(int value)
    {
        transform.localScale = new Vector3(value, 1, 1);
        facingDirection = new Vector2(value, 0);
    }
}
