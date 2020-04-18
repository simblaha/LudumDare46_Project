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

    IEnumerator Behavior()
    {
        while (true)
        {
            if (target == null)
                LookForTarget();
            else
            {
                yield return MoveToTarget();
                LookForTarget();
            }
        }
    }

    IEnumerator MoveToTarget()
    {
        while (Vector2.Distance(new Vector2(target.position.x, transform.position.y), new Vector2(transform.position.x, transform.position.y)) > 0.1f)
        {
            Vector2 direction = (new Vector2(target.position.x, transform.position.y) - new Vector2(transform.position.x, transform.position.y)).normalized;
            move = new Vector2(direction.x * moveSpeed, rb.velocity.y);
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
        else
            target = player;
    }
}
