using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Food Throwing")]
    public GameObject foodPrefab;
    public GameObject foodRepPrefab;
    public Vector3 foodSpawnOffset;
    public int throwForce;
    public LineRenderer trajectoryLine;
    public int trajectoryResolution;
    public float trajectoryLimitY;
    public int foodCount;
    public Text UI_FoodCount;

    [Header("Hiding")]
    public GameObject hideVFX;
    public AudioClip[] hideSFX;
    public bool isHidden;

    private bool canHide;
    private float gravity;
    public Vector2 throwVector;
    private GameObject foodRep;
    private AudioSource audioSource;
    private SpriteRenderer sr;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        sr = GetComponentInChildren<SpriteRenderer>();
        UI_FoodCount.text = foodCount + "";
        gravity = Mathf.Abs(Physics2D.gravity.y);
    }

    private void Update()
    {
        if (foodCount > 0)
        {
            if (Input.GetButton("Throw"))
            {
                //if (foodRep == null)
                //    foodRep = Instantiate(foodRepPrefab, transform.position + foodSpawnOffset, Quaternion.identity);
                //else if (!foodRep.activeSelf)
               //     foodRep.SetActive(true);
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(Camera.main.transform.position.z)));
                Vector2 direction = mousePosition - (transform.position + foodSpawnOffset);
                throwVector = direction.normalized * throwForce;
                StartCoroutine(DrawTrajectory());
                //foodRep.transform.position = (Vector2)transform.position + Vector2.up * 0.25f;
            }
            if (Input.GetButtonUp("Throw"))
            {
               // if (foodRep.activeSelf)
                //    foodRep.SetActive(false);
                trajectoryLine.positionCount = 0;
                GameObject food = Instantiate(foodPrefab, transform.position + foodSpawnOffset, Quaternion.identity);
                food.GetComponent<Rigidbody2D>().velocity = throwVector;
                foodCount--;
                UI_FoodCount.text = foodCount + "";
                animator.SetTrigger("Throw");
                if (isHidden)
                    Hide(false);
            }
        }
        if (canHide)
        {
            if (Input.GetButtonDown("Interact"))
            {
                Hide(!isHidden);
            }
        }
    }

    void Hide(bool state)
    {
        Color newColor = sr.color;
        newColor.a = state ? 0.5f : 1f;
        sr.color = newColor;
        Destroy(Instantiate(hideVFX, transform.position, Quaternion.identity), 1f);
        audioSource.PlayOneShot(hideSFX[UnityEngine.Random.Range(0, hideSFX.Length - 1)], 0.25f);
        isHidden = state;
    }

    IEnumerator DrawTrajectory()
    {
        trajectoryLine.positionCount = trajectoryResolution + 1;
        trajectoryLine.SetPositions(CalculateLineArray());
        yield return null;
    }

    private Vector3[] CalculateLineArray()
    {
        Vector3[] lineArray = new Vector3[trajectoryResolution + 1];
        float lowestTimeValue = MaxTimeY() / trajectoryResolution;
        for (int i = 0; i < lineArray.Length; i++)
        {
            float t = lowestTimeValue * i;
            lineArray[i] = CalculateLinePoint(t);
        }
        return lineArray;
    }

    private Vector3 CalculateLinePoint(float t)
    {
        float x = throwVector.x * t;
        float y = (throwVector.y * t) - (gravity * Mathf.Pow(t, 2) / 2);
        return new Vector3((transform.position.x + foodSpawnOffset.x) + x, (transform.position.y + foodSpawnOffset.y) + y);
    }

    private float MaxTimeY()
    {
        float vel = throwVector.y;
        float t = (vel + Mathf.Sqrt(vel * vel + 2 * gravity * ((transform.position.y + foodSpawnOffset.y) - trajectoryLimitY))) / gravity;
        return t;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "HidingSpot")
            canHide = true;
        if (collision.tag == "Food")
        {
            if (collision.GetComponent<Rigidbody2D>().gravityScale == 0)
            {
                Destroy(collision.gameObject);
                foodCount++;
                UI_FoodCount.text = foodCount + "";
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "HidingSpot")
        {
            if (isHidden)
            {
                Hide(false);
            }
            canHide = false;
        }
    }
}
