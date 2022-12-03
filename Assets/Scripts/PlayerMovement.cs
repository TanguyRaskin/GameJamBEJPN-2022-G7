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

    //EFFECTS
    //dommage
    private bool isDommaged = false;
    private float dommagePower = 10f;
    private float dommageTimer = 1f;
    //healed
    private bool isHealed = false;
    private float healPower = 10f;
    private float healTimer = 1f;
    //slow
    private bool isSlowed = false;
    private float speedDivider = 2f;
    private float slowTimer = 1f;
    //accelerate
    private bool isAccelereted = false;
    private float speedMultiplier = 2f;
    private float accelecatedTimer = 1f;

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

    private void Die()
    {
        Debug.Log("DIED");
        //TODO death animation
    }

    


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //collsion  detected
        //earth
        if (collision.gameObject.CompareTag("Earth"))
        {
            StartCoroutine(Slowed());
        }
        //air
        if (collision.gameObject.CompareTag("Air"))
        {
            StartCoroutine(Accelerated());
        }
        //water
        if (collision.gameObject.CompareTag("Water"))
        {
            StartCoroutine(Healed());
        }
        //fire
        if (collision.gameObject.CompareTag("Fire"))
        {
            StartCoroutine(Dommaged());
        }
        //void
        if (collision.gameObject.CompareTag("Void"))
        {
            StartCoroutine(Dommaged());
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Earth"))
        {
            if (!isSlowed)
            {
                StartCoroutine(Slowed());
            }
        }
        if (collision.gameObject.CompareTag("Air"))
        {
            if (!isAccelereted)
            {
                StartCoroutine(Accelerated());
            }
        }
        if (collision.gameObject.CompareTag("Water"))
        {
            if (!isHealed)
            {
                StartCoroutine(Healed());
            }
        }
        if (collision.gameObject.CompareTag("Fire"))
        {
            if (!isDommaged)
            {
                StartCoroutine(Dommaged());
            }
        }
        if (collision.gameObject.CompareTag("Void"))
        {
            if (!isDommaged)
            {
                StartCoroutine(Dommaged());
            }
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //earth
        if (collision.gameObject.CompareTag("Earth"))
        {
            StopCoroutine(Slowed());
        }
        //air
        if (collision.gameObject.CompareTag("Air"))
        {
            StopCoroutine(Accelerated());
        }
        //water
        if (collision.gameObject.CompareTag("Water"))
        {
            StopCoroutine(Healed());
        }
        //fire
        if (collision.gameObject.CompareTag("Fire"))
        {
            StopCoroutine(Dommaged());
        }
        //Void
        if (collision.gameObject.CompareTag("Void"))
        {
            StopCoroutine(Dommaged());
        }

    }



    //COROUTINES

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

    private IEnumerator Slowed()
    {
        isSlowed = true;
        float originalSpeed = speed;
        speed = speed / speedDivider;
        yield return new WaitForSeconds(slowTimer);
        isSlowed = false;
        speed = originalSpeed;
    }

    private IEnumerator Accelerated()
    {
        isAccelereted = true;
        float originalSpeed = speed;
        speed = speed * speedMultiplier;
        yield return new WaitForSeconds(accelecatedTimer);
        isAccelereted = false;
        speed = originalSpeed;
    }

    private IEnumerator Healed()
    {
        isHealed = true;
        if (health < 100)
        {
            health += healPower;
        }
        yield return new WaitForSeconds(healTimer);
        isHealed = false;
    }

    private IEnumerator Dommaged()
    {
        isDommaged = true;
        if (health > 0)
        {
            health -= dommagePower;
        }
        if(health <= 0)
        {
            Die();
        }
        yield return new WaitForSeconds(dommageTimer);
        isDommaged = false;
    }

}
