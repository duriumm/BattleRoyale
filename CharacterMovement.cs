using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterMovement : MonoBehaviour
{
    // Movement variables
    public float moveSpeed = 8.0f;

    public float originalMovementSpeed;
    

    // Components
    private Rigidbody2D rb;

    private Transform playersFaceShooterTransform;
    public Animator animator;

    public Vector2 movement;


    private void Start()
    {
        //rb = transform.Find("TestBuggyAnimPlayer").GetComponent<Rigidbody2D>();

        rb = GetComponent<Rigidbody2D>();

        playersFaceShooterTransform = transform.Find("Face");
        animator = transform.Find("TestBuggyAnimPlayer").GetComponent<Animator>();

    }

    public void MoveAndRotatePlayer()
    {
        // Input for movement
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        // The Actual movement of the character
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

        if (movement != Vector2.zero)
        {
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);

        }

    }

    public void SetHalvedMovementSpeed()
    {
        moveSpeed = moveSpeed / 2.0f;
    }

    public void SetZeroMovementSpeed()
    {
        moveSpeed = 0.0f;
    }
    public void ResumeMovementSpeed()
    {
        moveSpeed = originalMovementSpeed;
    }

}
