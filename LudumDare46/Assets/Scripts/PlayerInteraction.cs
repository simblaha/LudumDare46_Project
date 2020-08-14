using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Food Throwing")]
    public GameObject foodPrefab;
    public GameObject foodRepPrefab;
    public int throwForce;
    public float trajectoryDuration;
    public int foodCount;
    public Text UI_FoodCount;

    [Header("Hiding")]
    public GameObject hideVFX;
    public AudioClip[] hideSFX;
    public bool isHidden;

    private bool canHide;
    private float trajectoryTime = 0;
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
    }

    private void Update()
    {
        if (foodCount > 0)
        {
            if (Input.GetButton("Throw"))
            {
                if (trajectoryTime > 0)
                    trajectoryTime -= Time.deltaTime;
                else
                {
                    if (foodRep == null)
                        foodRep = Instantiate(foodRepPrefab, (Vector2)transform.position + Vector2.up * 0.25f, Quaternion.identity);
                    else if (!foodRep.activeSelf)
                        foodRep.SetActive(true);
                    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(Camera.main.transform.position.z)));
                    Vector2 direction = mousePosition - transform.position;
                    foodRep.transform.position = (Vector2)transform.position + Vector2.up * 0.25f;
                    foodRep.GetComponent<Rigidbody2D>().gravityScale = 1;
                    foodRep.GetComponent<Rigidbody2D>().AddForce(direction.normalized * throwForce);
                    trajectoryTime = trajectoryDuration;
                }
            }
            if (Input.GetButtonUp("Throw"))
            {
                if (foodRep.activeSelf)
                    foodRep.SetActive(false);
                trajectoryTime = 0;
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(Camera.main.transform.position.z)));
                Vector2 direction = mousePosition - transform.position;
                GameObject food = Instantiate(foodPrefab, (Vector2)transform.position + Vector2.up * 0.25f, Quaternion.identity);
                food.GetComponent<Rigidbody2D>().AddForce(direction.normalized * throwForce);
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
        audioSource.PlayOneShot(hideSFX[Random.Range(0, hideSFX.Length - 1)], 0.25f);
        isHidden = state;
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
