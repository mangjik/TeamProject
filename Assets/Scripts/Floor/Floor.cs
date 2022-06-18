using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Foot" && this.gameObject.tag == "Floor")
        {
            collision.GetComponentInParent<Player>().TouchFloor();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Foot" && this.gameObject.tag == "Floor")
        {
            collision.GetComponentInParent<Player>().ExitFloor();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Skill")
        {
            Destroy(collision.gameObject);
        }
    }
}
