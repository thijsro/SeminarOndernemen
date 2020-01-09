using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float runSpeed = 0f;
    [SerializeField] private float jumpSpeed = 0f;
    [SerializeField] private LayerMask floorLayerMask;

    [SerializeField] private float forceIncrease = 5f;
    [SerializeField] private float maxForce = 500f;
    [SerializeField] private float startDashTime;
    [SerializeField] private float throwBackForce = 20f;
    private float force = 0;

    private bool isDashing;
    private bool canDash = true;
    private bool canDoubleJump = true;
    private bool canDoInput = true;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider2d;

    private int direction;
    private float dashTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider2d = GetComponent<BoxCollider2D>();
        direction = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGrounded())
        {
            canDoInput = true;
            canDoubleJump = true;
            canDash = true;
        }

        Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        CheckDirection();

        if (isDashing)
        {
            Dash();
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (canDash)
            {
                ChargeDash();
            }
        }
        /*else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            if (canDash)
            {
                Dash();
            }
        }*/            
        else if(!isDashing)
        {
            rb.isKinematic = false;
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
            dashTime -= Time.deltaTime;

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
        force = 0;
        canDoInput = true;
        canDash = false;
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

    private void Jump()
    {
        if (isGrounded())
        {
            rb.velocity = Vector2.up * jumpSpeed; // Jump
        }
        else if (!isGrounded() && canDoubleJump) //Check Able to Double Jump
        {
            rb.velocity = Vector2.up * jumpSpeed; //DoubleJump
            canDoubleJump = false;
        }
    }

    //Check is player is on a surface
    private bool isGrounded()
    {
        RaycastHit2D raycastHit2D =  Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size, 0f, Vector2.down, 0.1f, floorLayerMask);
        return raycastHit2D.collider != null;
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
        rb.velocity = new Vector2(runSpeed * move, rb.velocity.y);
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

    public void ThrowBack() 
    {
        Debug.Log("throwback");
        canDash = false;
        canDoInput = false;
        rb.velocity = Vector2.left * throwBackForce;
    }
}
