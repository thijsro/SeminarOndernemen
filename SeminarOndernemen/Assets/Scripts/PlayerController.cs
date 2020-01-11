using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float runSpeed = 0f;
    [SerializeField] private float jumpSpeed = 0f;
    [SerializeField] private LayerMask floorLayerMask;
    [SerializeField] private CameraShake cameraShake;

    [SerializeField] private float forceIncrease = 5f;
    [SerializeField] private float maxForce = 500f;
    [SerializeField] private float startDashTime;
    [SerializeField] private float throwBackForce = 20f;
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip chargeDashSound;
    [SerializeField] private AudioClip jumpSound;

    private bool isDashing = false;
    private bool canDash = true;
    private bool canDoubleJump = true;
    private bool canDoInput = true;
    private bool firstJump = true;
    private bool playedAudio;
    private int direction;
    private float dashTime;
    private float force = 0;

    private float jumpTime = 0.5f;
    private float currentJumpTime;
    private bool isJumping = false;


    private Rigidbody2D rb;
    private BoxCollider2D boxCollider2d;
    private AudioSource audioSource;
    

    //animation
    private bool idle;
    private bool run;
    private bool jump;
    private bool fall;
    private bool backonitsfeet;
    private bool dash;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider2d = GetComponent<BoxCollider2D>();
        direction = 1;
        dashTime = startDashTime;
        currentJumpTime = jumpTime;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Input.GetAxisRaw("Horizontal");

        if (isGrounded())
        {
            canDoInput = true;
            canDoubleJump = true;
            canDash = true;
            firstJump = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {

            if (canDash)
            {
                /*if (!playedAudio)
                {
                    audioSource.clip = chargeDashSound;
                    audioSource.Play();
                    playedAudio = true;
                }*/
                ChargeDash();
            }
        }

        if (isDashing)
        {
            Dash();
        }
        else if (!isDashing)
        {
            rb.isKinematic = false;
        }

        CheckDirection();

        Debug.Log("firstjump " + firstJump);

        AnimationManager();

        if (isJumping)
        {
            currentJumpTime = Timer(currentJumpTime);
            if(currentJumpTime <= 0)
            {
                isJumping = false;
                currentJumpTime = jumpTime;
            }
        }

        Debug.Log(isJumping);
    }

    private void AnimationManager()
    {
        if (idle)
        {
            //play idle
        }
        else if (run)
        {
            //play run
        }
        else if (jump)
        {
            //play jump
        }
        else if (dash)
        {
            //play dash
        }
        else if (fall)
        {
            //play fall
        }
        else if (backonitsfeet)
        {
            //play backonitsfeet
        }
    }

    // Move the player 
    private void FixedUpdate()
    {
        if (canDoInput)
        {
            Move();
        }
    }
    private void Move()
    {
        float move = Input.GetAxis("Horizontal");
        if(move < 0) { GetComponent<SpriteRenderer>().flipX = true; }
        else if(move>0) { GetComponent<SpriteRenderer>().flipX = false; }
        rb.velocity = new Vector2(runSpeed * move, rb.velocity.y);
    }

    private void Jump()
    {
        if (firstJump && !isJumping && !isDashing) // TODO Make player jump twice 
        {
            rb.velocity = Vector2.up * jumpSpeed; // Jump
            audioSource.clip = jumpSound;
            audioSource.Play();
            isJumping = true;
            firstJump = false;

        }
        else if (!isGrounded() && canDoubleJump) //Check Able to Double Jump
        {
            rb.velocity = Vector2.up * jumpSpeed; //DoubleJump
            canDoubleJump = false;
            firstJump = false;
            audioSource.clip = jumpSound;
            audioSource.Play();
        }
    }

    private void ChargeDash()
    {
        if (!isDashing)
        {
            canDoInput = false;
            rb.isKinematic = true;
            rb.velocity = new Vector2(0, 0); // STILL STAAN
            force += forceIncrease;

            if (force > maxForce)
            {
                isDashing = true;
                force = maxForce;
            }
        }
    }

    private void Dash()
    {
        Debug.Log("isDashing");
        rb.isKinematic = false;
        if (dashTime <= 0)
        {
            ResetDash();
        }
        else
        {
            audioSource.clip = dashSound;
            audioSource.Play();
            playedAudio = false; //resets charge sound

            dashTime -= Time.deltaTime;

            cameraShake.doShake = true;

            if (direction == 1)
            {
                rb.AddForce(new Vector2(force, 0)); //Dash RIGHT
            }
            else if (direction == 2)
            {
                rb.AddForce(new Vector2(-force, 0)); //Dash LEFT
            }
        }
    }

    private void ResetDash() //After performed dash, reset
    {
        dashTime = startDashTime;
        isDashing = false;
        rb.velocity = Vector2.zero;
        canDoInput = true;
        canDash = false;
        force = 0;
    }

    private void CheckDirection()
    {
        if (Input.GetAxisRaw("Horizontal") == 1) //Right
        {
            direction = 1;
        }
        else if (Input.GetAxisRaw("Horizontal") == -1) //Left
        {
            direction = 2;
        }
    }

    //Check is player is on a surface
    private bool isGrounded()
    {
        RaycastHit2D raycastHit2D =  Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size, 0f, Vector2.down, 0.01f, floorLayerMask);
        return raycastHit2D.collider != null;
    }

    public void ThrowBack() 
    {
        Debug.Log("throwback");
        canDash = false;
        canDoInput = false;
        rb.velocity = Vector2.left * throwBackForce;
    }

    public void DisableInput()
    {
        canDash = false;
        canDoInput = false;
        canDoubleJump = false;
    }

    private float Timer(float timer)
    {
        timer -= Time.deltaTime;
        return timer;
    }
}
