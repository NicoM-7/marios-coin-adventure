using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {
    
    public float moveSpeed = 7.5f;
    public float jumpForce = 10;
    public float holdJumpForce = 50f;              
    public float maxHoldJumpTime = 0.25f;          
    public float groundCheckDistance = 0.1f;
    public float wallCheckDistance = 0.1f;
    public float wallJumpForce = 10f;              
    public float wallJumpVerticalForce = 10f;      
    public LayerMask groundLayer;
    public AudioClip death;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isJumping;
    private bool isTouchingWall;
    private bool isWallJumping;
    private bool oneTime;
    private float jumpTimeCounter;
    private int coins;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        animator = GetComponent<Animator>();
        coins = 0;
        oneTime = true;
    }

    private void Update() {
        float moveInput = Input.GetAxis("Horizontal");
        bool isGrounded = CheckGrounded();
        isTouchingWall = CheckWall(moveInput, isGrounded);

        if (isWallJumping) {
            return;
        }

        if(!oneTime) {
            return;
        }

        if (isTouchingWall && !isGrounded) {
            rb.velocity = new Vector2(0, rb.velocity.y);
        } else {
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        }

        if (moveInput != 0) {
            transform.localScale = new Vector3(Mathf.Sign(moveInput), 1, 1);
        }

        if (Input.GetButtonDown("Jump") && isGrounded && !GameObject.FindWithTag("GameManager").GetComponent<GameManager>().GetPaused()) {
            GetComponent<AudioSource>().Play();
            isJumping = true;
            jumpTimeCounter = maxHoldJumpTime;
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }

        if (isTouchingWall && !isGrounded && !isJumping && rb.velocity.y < 0) {
            rb.gravityScale = 0.5f;
            animator.SetBool("grab", true);
            transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
            if ((isTouchingWall && moveInput < 0 && rb.velocity.x >= 0) || (isTouchingWall && moveInput > 0 && rb.velocity.x <= 0)) {
                if (Input.GetButtonDown("Jump")) {
                    animator.SetBool("wallJump", true);
                    rb.gravityScale = 4f;
                    GetComponent<AudioSource>().Play();
                    isWallJumping = true;
                    float wallJumpDirection = moveInput > 0 ? -1 : 1;
                    transform.localScale = new Vector3(wallJumpDirection, 1, 1);
                    rb.velocity = new Vector2(wallJumpDirection * wallJumpForce, wallJumpVerticalForce);
                    StartCoroutine(ResetWallJump());
                }
            }
        } else {
            rb.gravityScale = 4f;
            animator.SetBool("grab", false);
            animator.SetBool("wallJump", false);
        }

        if (Input.GetButton("Jump") && isJumping && jumpTimeCounter > 0) {
            rb.AddForce(new Vector2(0f, holdJumpForce * Time.deltaTime), ForceMode2D.Impulse);
            jumpTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonUp("Jump") || jumpTimeCounter <= 0) {
            isJumping = false;
        }
        
        if (Input.GetKeyDown(KeyCode.Escape)) {
            bool paused = GameObject.FindWithTag("GameManager").GetComponent<GameManager>().GetPaused();
            if (paused) {
                GameObject.FindWithTag("GameManager").GetComponent<GameManager>().Resume();
            } else {
                GameObject.FindWithTag("GameManager").GetComponent<GameManager>().Pause();
            }
        }

        if (isJumping || rb.velocity.y > 0) {
            RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, new Vector2(GetComponent<BoxCollider2D>().size.x, 0.25f), 0f, Vector2.up, 1.1f, groundLayer);
            foreach (RaycastHit2D hit in hits) {
                if (hit.collider) {
                    isJumping = false;
                    if (hit.collider.CompareTag("LuckyBlock")) {
                        hit.collider.gameObject.GetComponent<LuckyBlock>().Hit();
                    } else if (hit.collider.CompareTag("Brick")) {
                        hit.collider.gameObject.GetComponent<Brick>().Hit();
                    }
                }
            }
        }

        if(transform.position.y < -11 && oneTime) {
            oneTime = false;
            rb.velocity = Vector2.zero;
            GameObject.FindWithTag("AudioManager").GetComponent<AudioSource>().Stop();
            StartCoroutine(Restart());
        }

        animator.SetBool("onGround", isGrounded);
        animator.SetFloat("horizontal", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("vertical", rb.velocity.y);
    }

    private bool CheckGrounded() {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, GetComponent<BoxCollider2D>().size, 0, Vector2.down, groundCheckDistance, groundLayer);
        return hit.collider != null;
    }

    private bool CheckWall(float moveInput, bool grounded) {
        Vector2 direction = moveInput > 0 ? Vector2.right : Vector2.left;
        Vector2 position = (Vector2)transform.position + (grounded ? Vector2.up * 0.25f : Vector2.down * 1.0f);
        Vector2 boxSize = new Vector2(GetComponent<BoxCollider2D>().size.x, 0.1f);

        RaycastHit2D hit = Physics2D.BoxCast(position, boxSize, 0, direction, wallCheckDistance, groundLayer);
        return hit.collider != null && Input.GetAxis("Horizontal") != 0;
    }

    public int GetCoins() {
        return coins;
    }

    public void SetCoins(int newCoins) {
        coins = newCoins;
    }

    private IEnumerator ResetWallJump() {
        yield return new WaitForSeconds(0.5f);
        isWallJumping = false;
    }

    private IEnumerator Restart() {
        yield return new WaitForSeconds(0.5f);
        GetComponent<AudioSource>().clip = death;
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("Level1");
    }
}