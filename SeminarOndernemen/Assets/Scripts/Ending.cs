using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ending : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private float maxTime = 2f;
    private float time = 0f;

    private bool endingStarted = false;

    private void Start()
    {
        time = maxTime;
    }

    private void Update()
    {
        if (endingStarted)
        {
            //play animation

            if (time <= 0)
            {
                SceneManager.LoadScene(0);
            }
            else
            {
                time -= Time.deltaTime;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("ending");
            endingStarted = true;
            player.DisableInput();

        }
    }
}
