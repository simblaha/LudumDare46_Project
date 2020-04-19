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

    public int health;
    private Vector2 move;
    private Rigidbody2D rb;
    private Transform player;
    private GameObject healthBar;
    private bool isFeeding;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player").transform;
        healthBar = GameObject.Find("Zombie_HealthBar");
        healthBar.transform.position = Camera.main.WorldToScreenPoint((Vector2)transform.position + healthBarOffset);
        health = healthBase;
        target = player;
    }

    private void Start()
    {
        ChangeHealth(-5);
        StartCoroutine(Behaviour());
    }

    private void Update()
    {
        rb.velocity = move;
        healthBar.transform.position = Camera.main.WorldToScreenPoint((Vector2)transform.position + healthBarOffset);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            //Kill Enemy
            target = null;
            Destroy(collision.gameObject);
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
                Destroy(collision.gameObject);
                //Game Over
            }
        }
    }

    public void ChangeHealth(int amount)
    {
        health = Mathf.Clamp(health + amount, 0, healthBase);
        healthBar.transform.Find("Fill").GetComponent<Image>().fillAmount = (float)health / (float)healthBase;
        if (health == 0)
        {
            //Kill Zombie
            //Game Over
        }
    }

    IEnumerator Behaviour()
    {
        while (true)
        {
            if (!isFeeding)
                LookForTarget();
            if (target == null)
                yield return new WaitForEndOfFrame();
            else
            {
                yield return MoveToTarget();
            }
        }
    }

    IEnumerator Feed()
    {
        isFeeding = true;
        Destroy(Instantiate(feedEffect, transform.position, Quaternion.identity), feedDuration + 1);
        yield return new WaitForSeconds(feedDuration);
        ChangeHealth(+healthGainPerFood);
        isFeeding = false;
    }

    IEnumerator MoveToTarget()
    {
        float distanceToTarget;
        while (target != null && !isFeeding)
        {
            distanceToTarget = Vector2.Distance(new Vector2(target.position.x, transform.position.y), new Vector2(transform.position.x, transform.position.y));
            if (distanceToTarget > 0.01f)
            {
                Vector2 direction = (new Vector2(target.position.x, 0) - new Vector2(transform.position.x, 0)).normalized;
                move = new Vector2(direction.x * moveSpeed, 0);

            }
            else
            {
                move = Vector2.zero;
                target = null;
            }
            LookForTarget();
            yield return new WaitForEndOfFrame();
        }
        move = Vector2.zero;
    }

    void LookForTarget()
    {
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
                    target = player;
        }
    }
}
