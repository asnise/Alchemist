using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float dodgeSpeed = 10f; // Speed during dodge
    [SerializeField]
    private float dodgeDuration = 0.3f; // Dodge time

    private bool isDodging = false;
    private Vector2 dodgeDirection;
    private float normalSpeed;

    public Animator animator;
    public GameObject Model;

    private Rigidbody2D rb;
    public virtual bool canMove { get; set; } = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        normalSpeed = moveSpeed; // Store the default speed
    }

    private void Update()
    {
        if (canMove && !isDodging)
        {

            float moveHorizontal = Input.GetAxisRaw("Horizontal");
            float moveVertical = Input.GetAxisRaw("Vertical");

            Vector2 movement = new Vector2(moveHorizontal, moveVertical).normalized;
            rb.velocity = movement * moveSpeed;

            AnimationMove(movement.magnitude > 0);
            FlipCharacter(moveHorizontal);

            if (Input.GetKeyDown(KeyCode.Space) && movement.magnitude > 0)
            {
                StartCoroutine(Dodge(movement));
            }
        }
    }

    private void AnimationMove(bool isMoving)
    {
        animator.SetBool("IsMoving", isMoving);
    }

    private void FlipCharacter(float moveHorizontal)
    {
        Vector3 modelScale = Model.transform.localScale;
        if (moveHorizontal < 0)
        {
            modelScale.x = Mathf.Abs(modelScale.x);
        }
        else if (moveHorizontal > 0)
        {
            modelScale.x = -Mathf.Abs(modelScale.x);
        }
        Model.transform.localScale = modelScale;
    }

    private IEnumerator Dodge(Vector2 direction)
    {
        isDodging = true;
        moveSpeed = dodgeSpeed;
        rb.velocity = direction * dodgeSpeed;
        //animator.SetTrigger("Dodge"); // If you have a dodge animation

        yield return new WaitForSeconds(dodgeDuration);

        moveSpeed = normalSpeed;
        isDodging = false;
    }

    public float GetMoveSpeed()
    {
        return moveSpeed;
    }

    public void SetMoveSpeed(float newMoveSpeed)
    {
        moveSpeed = newMoveSpeed;
    }
}
