using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float runSpeed = 0f;
    [SerializeField] private float jumpSpeed = 0f;
    [SerializeField] private LayerMask floorLayerMask;
    [SerializeField] private CameraShake cameraShake;

    [SerializeField] private float startDashTime;
    [SerializeField] private float force = 250;
    [SerializeField] private float maxForceTime = .4f;
    [SerializeField] private float throwBackForce = 20f;
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip chargeDashSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioSource runSoundSource;

    private bool playedAudio;
    private bool canDoInput = true;
    private bool firstJump = true;
    private bool canDoubleJump = true;
    private bool isDashing = false;
    private bool canDash = true;
    private bool isRunning = false;
    private float dashTime;
    private float horizontalMove;
    private int direction;

    private float jumpTime = 0.5f;
    private float currentJumpTime;
    private bool isJumping = false;

    private float currentForceTime;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider2d;
    private AudioSource audioSource;
    public Animator animator;

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
        audioSource = GetComponent<AudioSource>();
        direction = 1;
        dashTime = startDashTime;
        currentJumpTime = jumpTime;
        currentForceTime = maxForceTime;
    }

    // Update is called once per frame
    void Update()
    {
        Input.GetAxisRaw("Horizontal");

        GroundedReset();

        Jump();
        JumpTimer();

        ChargeDash();
        if (isDashing)
        {
            Dash();
        }
        else if (!isDashing)
        {
            rb.isKinematic = false;
        }

        CheckDirection();
        AnimationManager();

        if (isRunning)
        {
            if(runSoundSource.isPlaying == false)
            {
                runSoundSource.Play();
            }
        }
        else
        {
            runSoundSource.Stop();
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
        animator.SetFloat("Speed", Mathf.Abs(move));
        if (move < 0) { GetComponent<SpriteRenderer>().flipX = true; isRunning = true; }
        else if (move > 0) { GetComponent<SpriteRenderer>().flipX = false; isRunning = false; }
        else { isRunning = false; }
        horizontalMove = Mathf.Abs(move);
        rb.velocity = new Vector2(runSpeed * move, rb.velocity.y);
    }

    private void GroundedReset()
    {
        if (isGrounded())
        {
            canDoInput = true;
            canDoubleJump = true;
            canDash = true;
            firstJump = true;
            animator.SetBool("isJumping", false);
            animator.SetBool("isCharging", false);
            animator.SetBool("isSliding", false);
        }
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (firstJump && !isJumping && !isDashing) // TODO Make player jump twice 
            {
                rb.velocity = Vector2.up * jumpSpeed; // Jump
                audioSource.clip = jumpSound;
                audioSource.Play();
                isJumping = true;
                firstJump = false;
                animator.SetBool("isJumping", true);

            }
            else if (!isGrounded() && canDoubleJump) //Check Able to Double Jump
            {
                rb.velocity = Vector2.up * jumpSpeed; //DoubleJump
                canDoubleJump = false;
                firstJump = false;
                audioSource.clip = jumpSound;
                audioSource.Play();
                animator.SetBool("isJumping", true);
            }
        }
    }

    private void JumpTimer()
    {
        if (isJumping)
        {
            currentJumpTime = Timer(currentJumpTime);
            if (currentJumpTime <= 0)
            {
                isJumping = false;
                currentJumpTime = jumpTime;
            }
        }
    }

    private void ChargeDash()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (canDash)
            {
                if (!isDashing)
                {
                    canDoInput = false;
                    rb.isKinematic = true;
                    rb.velocity = new Vector2(0, 0); // STILL STAAN
                    animator.SetBool("isCharging", true);

                    currentForceTime = Timer(currentForceTime);

                    if (currentForceTime <= 0)
                    {
                        isDashing = true;
                        currentForceTime = maxForceTime;
                        animator.SetBool("isCharging", false);
                    }
                }
            }
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            if (!isDashing)
            {
                ResetDash();
            }
        }
    }

    private void Dash()
    {
        rb.isKinematic = false;
        if (dashTime > 0)
        {
            audioSource.clip = dashSound;
            audioSource.Play();
            playedAudio = false; //resets charge sound

            dashTime = Timer(dashTime);

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
        else
        {
            ResetDash();
        }
    }

    private void ResetDash() //After performed dash, reset
    {
        dashTime = startDashTime;
        isDashing = false;
        rb.velocity = Vector2.zero;
        canDoInput = true;
        canDash = false;
        animator.SetBool("isCharging", false);
        currentForceTime = maxForceTime;
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

    private void AnimationManager()
    {
        if (idle)
        {
            //play idle
        }
        else if (run)
        {
            animator.SetFloat("Speed", horizontalMove);
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

    public void ThrowBack() 
    {
        Debug.Log("throwback");
        canDash = false;
        canDoInput = false;
        firstJump = false;
        canDoubleJump = false;
        rb.velocity = Vector2.left * throwBackForce;
        animator.SetBool("isSliding", true);    
        animator.SetBool("isJumping", false);    
        animator.SetBool("isCharging", false);    
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
