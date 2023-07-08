using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float runSpeed = 7f;
    [SerializeField] float jumpSpeed = 20f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2(30f, 30f);
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;
    Vector2 moveInput;
    Rigidbody2D myRigidbody;
    Animator animator;
    CapsuleCollider2D bodyCollider;
    BoxCollider2D feetCollider;
    float startingGravityScale;

    bool isAlive = true;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        bodyCollider = GetComponent<CapsuleCollider2D>();
        feetCollider = GetComponent<BoxCollider2D>();
        startingGravityScale = myRigidbody.gravityScale;
    }

    private void Update()
    {
        if (!isAlive) { return; }
        Run();
        FlipSprite();
        ClimbLadder();
        Die();
    }

    void OnMove(InputValue value)
    {
        if (!isAlive) { return; }
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value) {
        if (!isAlive) { return; }
        if (!feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            return;
        }
        if (value.isPressed) {
            myRigidbody.velocity += new Vector2(0f, jumpSpeed);
            // transform.localScale = new Vector2(1f, 1.5f);
            // transform.localScale += new Vector3(0f, 1.5f, 0f);
        }
    }

    void OnFire(InputValue value) {
        if (!isAlive) { return; }
        Instantiate(bullet, gun.position,transform.rotation);
    }

    void Run() {
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;

        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        animator.SetBool("isRunning", playerHasHorizontalSpeed);
    }

    void FlipSprite() {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed) {
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x), 1f);
        }
    }

    void ClimbLadder() {
        if (!feetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))) {
            myRigidbody.gravityScale = startingGravityScale;
            animator.SetBool("isClimbing", false);
            return;
        }
        Vector2 climbVelocity = new Vector2(myRigidbody.velocity.x, moveInput.y * climbSpeed);
        myRigidbody.velocity = climbVelocity;
        myRigidbody.gravityScale = 0f;

        bool playerHasVerticalSpeed = Mathf.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
        animator.SetBool("isClimbing", playerHasVerticalSpeed);
    }

    void Die() {
        if (bodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards"))) {
            isAlive = false;
            animator.SetTrigger("Dying");
            myRigidbody.velocity = deathKick;
            // transform.localScale += new Vector3(2f, 2f, 0f);
            myRigidbody.velocity = new Vector2(0f, myRigidbody.velocity.y);

            Collider2D[] colliders = GetComponents<Collider2D>();
            foreach (Collider2D collider in colliders) {
                collider.enabled = false;
            }
        }
    }

}
