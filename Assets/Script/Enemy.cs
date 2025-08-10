using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("General Stats")]
    public GameObject target_;
    public ElementState elementState = ElementState.Solid;
    public float _moveSpeed;
    private bool canAttack = true;
    public int health = 5;
    public GameObject itemDropPrefab;
    public GameObject DeadVfx;

    public List<Item> items = new List<Item>();
    public List<ElementalStructure> elementals = new List<ElementalStructure>();

    [Header("Combat")]
    public int damage = 1;
    public float attackRange = 1.5f;
    public float attackDelay = 1f;

    [Header("Timers")]
    [SerializeField]
    private float elapsedTime = 0f;
    private float randomMoveTimer = 0f;
    private float randomMoveInterval = 2f;
    [SerializeField]
    private float idleTime = 1f;
    private bool isIdle = false;
    private float idleTimer = 0f;

    [Header("Movement Settings")]
    [SerializeField]
    private float movementArea = 30f;
    [SerializeField]
    private float chaseDistance = 3f;
    private Vector2 randomDirection;
    private Vector2 spawnPosition;

    [Header("Model and Direction")]
    Animator animator;
    public GameObject Model;
    [SerializeField]
    private bool moveRight = true;

    private bool isStunned = false;
    private bool isAggro = false;

    public AudioClip Dead_sfx;
    private void Start()
    {
        this.target_ = GameObject.FindGameObjectWithTag("Player").gameObject;
        animator = Model.GetComponent<Animator>();
        spawnPosition = transform.position;
        ChooseRandomDirection();

    }

    void FixedUpdate()
    {
        if (target_ != null)
        {
            MoveToPosition();
            CheckOverlapAttack();
        }
    }

    void MoveToPosition()
    {
        if (isStunned) return;

        float distanceToTarget = Vector2.Distance(transform.position, target_.transform.position);

        if (distanceToTarget <= chaseDistance || isAggro)
        {
            isAggro = true;
            Vector2 directionToTarget = (target_.transform.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, target_.transform.position, _moveSpeed * Time.deltaTime);

            FlipCharacter(directionToTarget.x);
            AnimationMove(true);
        }
        else if (!isAggro)
        {
            if (isIdle)
            {
                idleTimer += Time.deltaTime;
                if (idleTimer >= idleTime)
                {
                    isIdle = false;
                    idleTimer = 0f;
                    ChooseRandomDirection();
                }
                else
                {
                    AnimationMove(false);
                }
            }
            else
            {
                randomMoveTimer += Time.deltaTime;
                if (randomMoveTimer >= randomMoveInterval)
                {
                    isIdle = true;
                    randomMoveTimer = 0f;
                    randomMoveInterval = UnityEngine.Random.Range(1f, 4f);
                }

                if (!isIdle)
                {
                    Vector2 newPosition = (Vector2)transform.position + randomDirection * _moveSpeed * Time.deltaTime;
                    float clampedX = Mathf.Clamp(newPosition.x, spawnPosition.x - movementArea, spawnPosition.x + movementArea);
                    float clampedY = Mathf.Clamp(newPosition.y, spawnPosition.y - movementArea, spawnPosition.y + movementArea);
                    Vector2 clampedPosition = new Vector2(clampedX, clampedY);

                    if (Vector2.Distance(transform.position, clampedPosition) < 0.01f)
                    {
                        AnimationMove(false);
                    }
                    else
                    {
                        transform.position = clampedPosition;
                        FlipCharacter(randomDirection.x);
                        AnimationMove(true);
                    }
                }
            }
        }
    }

    void CheckOverlapAttack()
    {
        if (elapsedTime < attackDelay)
        {
            elapsedTime += Time.deltaTime;
        }
        else if (elapsedTime > attackDelay)
        {
            canAttack = true;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player") && canAttack)
            {
                isAggro = true;
                hit.GetComponent<Player>().TakeDamage(damage);
                canAttack = false;
                elapsedTime = 0;

                StartCoroutine(StunEnemy(2f));

                break;
            }
        }
    }

    private IEnumerator StunEnemy(float duration)
    {
        isStunned = true;
        AnimationMove(false);
        yield return new WaitForSeconds(duration);
        isStunned = false;
    }

    void ChooseRandomDirection()
    {
        float randomX = moveRight ? UnityEngine.Random.Range(0f, 1f) : UnityEngine.Random.Range(-10f, 10f);
        float randomY = UnityEngine.Random.Range(-10f, 10f);
        randomDirection = new Vector2(randomX, randomY).normalized;
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

    public void TakeDamage(int damage)
    {
        health -= damage;
        isAggro = true;

        if (health <= 0)
        {
            DropItems();
            GameObject obj = Instantiate(DeadVfx, transform.position, Quaternion.identity);
            Destroy(obj, 2f);
            if (Dead_sfx != null)
            {
                SoundManager.Instance.PlaySoundEffect(Dead_sfx);
            }
            Destroy(gameObject);
        }
    }

    public void DropItems()
    {
        foreach (Item item in items)
        {
            Vector2 randomOffset = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
            Vector2 dropPosition = (Vector2)transform.position + randomOffset;
            GameObject itemDrop = Instantiate(itemDropPrefab, dropPosition, Quaternion.identity);
            itemDrop.GetComponent<itemDrop>().item = item;
        }

        foreach (ElementalStructure elemental in elementals)
        {
            Vector2 randomOffset = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
            Vector2 dropPosition = (Vector2)transform.position + randomOffset;
            GameObject itemDrop = Instantiate(itemDropPrefab, dropPosition, Quaternion.identity);
            itemDrop.GetComponent<itemDrop>().elementalStructure = elemental;
        }


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(spawnPosition, movementArea);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public Action OnEnemyDestroyed;
    private void OnDestroy()
    {
        OnEnemyDestroyed?.Invoke();
    }
}
