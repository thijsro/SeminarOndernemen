using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBack : MonoBehaviour
{
    [SerializeField] private PlayerController player;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player.ThrowBack();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player.animator.SetBool("isSliding", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player.animator.SetBool("isSliding", false);
        } 
    }
}
