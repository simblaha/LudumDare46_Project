using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    [Header("Basic Stats")]
    public int healthBase;
    public float moveSpeed;
    [Header("TargetDetection")]
    public int detectionRange;
    public LayerMask detectedLayers;
    public Transform target;

    [HideInInspector]
    public int health;
    private Vector2 move;
    private Rigidbody2D rb;
    private Transform player;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player").transform;
        health = healthBase;
        target = player;
    }

    private void Start()
    {
        StartCoroutine(Behavior());
    }

    private void Update()
    {
        rb.velocity = move;
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
            ChangeHealth(+1);
            target = null;
            Destroy(collision.gameObject);
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
        if (health == 0)
        {
            //Kill Zombie
            //Game Over
        }
    }

    IEnumerator Behavior()
    {
        while (player != null)
        {
            if (target == null)
                LookForTarget();
            else
            {
                yield return MoveToTarget();
            }
        }
        move = Vector2.zero;
    }

    IEnumerator MoveToTarget()
    {
        float distanceToTarget;
        while (target != null)
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
        else if (player != null)
            target = player;
    }
}
