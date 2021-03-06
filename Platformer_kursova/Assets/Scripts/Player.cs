﻿using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{

    private Rigidbody2D myRigidbody;

    private Animator myAnimator;

    [SerializeField]
    private float movementSpeed; 

    

    private bool attack;

    private bool facingRight;

    [SerializeField] 
    private Transform[] groundPoints;
    [SerializeField]
    private float groundRadius;
    [SerializeField]
    private LayerMask whatIsGround;
    private bool isGrounded;
    private bool jump;
    [SerializeField]
    private float jumpForce;


    private bool jumpAttack;

	// Use this for initialization
	void Start ()
    {
        facingRight = true;
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
	}
	
    void Update()
    {
        HandleInput();

    }

	// Update is called once per frame
	void FixedUpdate ()
    {
        float horizontal = Input.GetAxis("Horizontal");

        isGrounded = IsGrounded();  

        HandleMovement(horizontal);
        Flip(horizontal);
        HandleAttacks();
        ResetValues();
        HandleLayers();
    }

    private void HandleMovement(float horizontal)
    {
        if(myRigidbody.velocity.y < 0)
        {
            myAnimator.SetBool("land", true); 
        }
        myRigidbody.velocity = new Vector2(horizontal * movementSpeed,myRigidbody.velocity.y);//x = -1; y = 0;
        if(isGrounded && jump)
        {
            isGrounded = false;
            myRigidbody.AddForce(new Vector2(0, jumpForce));
            myAnimator.SetTrigger("jump");
        }
        myAnimator.SetFloat("speed",Mathf.Abs(horizontal));
    }

    private void HandleAttacks()
    {
        if(attack)
        {
            myAnimator.SetTrigger("attack");
        }
        if (jumpAttack && !isGrounded && !this.myAnimator.GetAnimatorTransitionInfo(1).IsName("jumpAttack"))
        {
            myAnimator.SetBool("jumpAttack", true);
        }
        if(!jumpAttack && !this.myAnimator.GetAnimatorTransitionInfo(1).IsName("jumpAttack"))
        {
            myAnimator.SetBool("jumpAttack", false);
        }
    }

    private void HandleInput()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            attack = true;
            jumpAttack = true;
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        }
    }

    private void Flip(float horizontal)
    {
        if(horizontal > 0 && !facingRight || horizontal < 0 && facingRight)
        {
            facingRight = !facingRight;
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }

    private void ResetValues()
    {
        attack = false;
        jump = false;
        jumpAttack = false;
    }

    private bool IsGrounded()
    {
        if(myRigidbody.velocity.y <= 0)
        {
            foreach(Transform point in groundPoints)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(point.position, groundRadius, whatIsGround);

                for(int i = 0; i< colliders.Length; i++)
                {
                    if(colliders[i].gameObject != gameObject)
                    {
                        myAnimator.ResetTrigger("jump");
                        myAnimator.SetBool("land", false);
                        return true;
                    }
                }

            }
        }
        return false;
    }
    private void HandleLayers()
    {
        if (!isGrounded)
        {
            myAnimator.SetLayerWeight(1, 1);
        }
        else
        {
            myAnimator.SetLayerWeight(1, 0);
        }
    }
}
