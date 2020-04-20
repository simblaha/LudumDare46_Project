using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Basic Stats")]
    public int attackDamage;
    public float attackSpeed;
    public float moveSpeed;
    public bool isMelee;
    public float meleeAttackRange;
    public bool turnLeft;
    public GameObject deathEffect;
    public AudioClip[] shootSFX;
    public float animationDurationMelee;
    public float animationDurationRange;

    [Header("TargetDetection")]
    public float detectionRange;
    public LayerMask detectedLayers;
    public Transform target;

    private Vector2 move;
    private Vector2 facingDirection;
    private bool inRange = false;
    private Rigidbody2D rb;
    private AudioSource audioSource;
    private Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        anim = GetComponentInChildren<Animator>();
        facingDirection = Vector2.right;
        if (turnLeft)
            Turn(-1);
        StartCoroutine(Behaviour());    
    }

    IEnumerator Behaviour()
    {
        while (true)
        {
            LookForTarget();
            if (target == null)
                yield return new WaitForEndOfFrame();
            else
            {
                if (isMelee)
                {
                    if (!inRange)
                        yield return MoveToTarget();
                    else
                    {
                        if (anim)
                            anim.SetTrigger("Attack");
                        yield return new WaitForSeconds(animationDurationMelee);
                        AttackTarget();
                        yield return new WaitForSeconds(attackSpeed - animationDurationMelee);
                    }
                }
                else
                {
                    anim.SetTrigger("Attack");
                    yield return new WaitForSeconds(animationDurationRange);
                    AttackTarget();
                    yield return new WaitForSeconds(attackSpeed - animationDurationRange);
                }
            }
        }
    }

    IEnumerator MoveToTarget()
    {
        float distanceToTarget;
        while (target != null)
        {
            distanceToTarget = Vector2.Distance(new Vector2(target.position.x, transform.position.y), new Vector2(transform.position.x, transform.position.y));
            if (distanceToTarget > meleeAttackRange)
            {
                Vector2 direction = (new Vector2(target.position.x, 0) - new Vector2(transform.position.x, 0)).normalized;
                move = new Vector2(direction.x * moveSpeed, 0);
                rb.velocity = move;
            }
            else
            {
                move = Vector2.zero;
                rb.velocity = move;
                inRange = true;
                break;
            }
            LookForTarget();
            yield return new WaitForEndOfFrame();
        }
        move = Vector2.zero;
        rb.velocity = move;
    }

    void LookForTarget()
    {
        target = null;
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, facingDirection, detectionRange, detectedLayers);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.tag == "Player")
            {
                if (!hit.transform.GetComponent<PlayerInteraction>().isHidden && target == null)
                    target = hit.transform;
            }
            else
            {
                target = hit.transform;
            }
        }
    }

    void AttackTarget()
    {
        if (!isMelee)
            audioSource.PlayOneShot(shootSFX[Random.Range(0, shootSFX.Length - 1)], 0.1f);
        if (target.tag == "Player")
        {
            Destroy(Instantiate(deathEffect, target.transform.position, Quaternion.identity), 5f);
            Destroy(target.gameObject);
        }
        else
        {
            target.GetComponent<Zombie>().ChangeHealth(-attackDamage);
        }
    }

    void Turn(int value)
    {
        transform.localScale = new Vector3(value, 1, 1);
        facingDirection = new Vector2(value, 0);
    }
}
