using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class Ending : MonoBehaviour
{
    [SerializeField] private PlayerController player;

    private bool endingStarted = false;

    public PlayableDirector timeline;
    [SerializeField] private float animationTimer;

    private void Start()
    {
        timeline = GetComponent<PlayableDirector>();
    }

    private void Update()
    {
        if (endingStarted)
        {
            timeline.Play();
            animationTimer = Timer(animationTimer);

            if (animationTimer <= 0)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("ending");
            timeline.Play();
            endingStarted = true;
            player.DisableInput();
        }
    }

    private float Timer(float timer)
    {
        timer -= Time.deltaTime;
        return timer;
    }
}
