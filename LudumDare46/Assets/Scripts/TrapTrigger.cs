using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
    public bool isTriggered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isTriggered)
            isTriggered = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isTriggered)
            isTriggered = false;
    }
}
