using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Food Throwing")]
    public GameObject foodPrefab;
    public int throwForce;
    public int foodCount;
    public Text UI_FoodCount;

    [Header("Hiding")]
    public bool isHidden;

    private bool canHide;

    private void Awake()
    {
        UI_FoodCount.text = foodCount + "";
    }

    private void Update()
    {
        if (foodCount > 0)
        {
            if (Input.GetButtonDown("Throw"))
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
                Vector2 direction = (mousePosition - transform.position).normalized;
                GameObject food = Instantiate(foodPrefab, transform.position, Quaternion.identity);
                food.GetComponent<Rigidbody2D>().AddForce(direction * throwForce);
                foodCount--;
                UI_FoodCount.text = foodCount + "";
            }
        }
        if (canHide)
        {
            if (Input.GetAxisRaw("Vertical") > 0.1f)
            {
                //Activate Hide Effect
                isHidden = true;
            }
            if (Input.GetAxisRaw("Vertical") < -0.1f)
            {
                //Deactivate Hide Effect
                isHidden = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "HidingSpot")
            canHide = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "HidingSpot")
        {
            if (isHidden)
                isHidden = false;
            canHide = false;
        }
    }
}
