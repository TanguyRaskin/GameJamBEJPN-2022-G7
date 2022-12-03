using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float health = 100f;

    //basic movements
    private float horizontal;
    private float speed = 8f;
    private float jumpingPower = 15f;
    private bool isFacingRight = true;

    //dash movement
    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 24f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform roofCheck;
    [SerializeField] private LayerMask groundLayer;

    // Update is called once per frame
    void Update()
    {
        if (isDashing)
        {
            return;
        }

        horizontal = Input.GetAxisRaw("Horizontal");

        Jump();
        Move();
        Dash();
        Flip();
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }





    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }

        if (IsRoofed())
        {
            //If hit the roof => STOPPED
            var initialSpeed = rb.velocity.y;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0f);
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }

    private void Move()
    {
        
    }

    private void Dash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dashing());
        }
    }



    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private bool IsRoofed()
    {
        return Physics2D.OverlapCircle(roofCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private IEnumerator Dashing()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        yield return new WaitForSeconds(dashingTime);
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //collsion  detected
        Debug.Log("START Touching Element");
        //earth
        if (collision.gameObject.CompareTag("Earth"))
        {
            Debug.Log("In the mud");
            speed = 4f;
        }
        //air
        if (collision.gameObject.CompareTag("Air"))
        {
            Debug.Log("In the wind");
            speed = 16f;
        }
        //water
        if (collision.gameObject.CompareTag("Water"))
        {
            Debug.Log("In the water");
            //increase health over time
        }
        //fire
        if (collision.gameObject.CompareTag("Fire"))
        {
            Debug.Log("In the Fire");
            //decrease health over time
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //water
        if (collision.gameObject.CompareTag("Water"))
        {
            Debug.Log("In the water");
            if(health < 100)
            {
                health++;
            }
        }
        //fire
        if (collision.gameObject.CompareTag("Fire"))
        {
            Debug.Log("In the Fire");
            
            if (health > 0)
            {
                health--;
            }
            else { Debug.Log("DEAD hp : " + health); }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("STOP Touching Element");
        //earth
        if (collision.gameObject.CompareTag("Earth"))
        {
            Debug.Log("Out the mud");
            speed = 8f;
        }
        //air
        if (collision.gameObject.CompareTag("Air"))
        {
            Debug.Log("Out the wind");
            speed = 8f;
        }
        //water
        if (collision.gameObject.CompareTag("Water"))
        {
            Debug.Log("Out the water");
        }
        //fire
        if (collision.gameObject.CompareTag("Fire"))
        {
            Debug.Log("Out the Fire");
        }

    }

}
