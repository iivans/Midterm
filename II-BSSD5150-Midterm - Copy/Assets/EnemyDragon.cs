using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDragon : MonoBehaviour
{
    public GameObject fireballPrefab;
    public float minFireballInterval = 3f;
    public float maxFireballInterval = 5f;
    public float idleDuration = 2f;
    public float walkDuration = 5f;
    public float moveSpeed = 2f;

    private bool movingRight = true;
    private Animator animator;
    private Rigidbody2D rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(RandomActions());
    }

    void MoveRight()
    {
        if (ShouldFlip())
        {
            Flip();
        }

        animator.SetBool("Walking", true);
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
    }

    void MoveLeft()
    {
        if (ShouldFlip())
        {
            Flip();
        }

        animator.SetBool("Walking", true);
        rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
    }

    bool ShouldFlip()
    {
        // Check if the dragon is touching the object with the specified tag
        Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.2f);

        return hit != null && hit.CompareTag("EnemyAreaBoundary");
    }

    void Flip()
    {
        movingRight = !movingRight;
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }

    void ShootFireball()
    {
        animator.SetBool("Walking", false);
        animator.SetBool("Idle", false);
        animator.SetBool("Attack", true);

        // Instantiate a fireball prefab at the enemy's position
        GameObject fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);

        // Get the FireballController script and set its direction
        FireballController fireballController = fireball.GetComponent<FireballController>();
        if (fireballController != null)
        {
            // Set the direction based on the enemy's facing direction
            fireballController.SetDirection(movingRight ? Vector2.right : Vector2.left);
        }

        // Destroy the fireball after a certain time
        Destroy(fireball, 3.6f);
        animator.SetBool("Attack", false);
    }

    IEnumerator RandomActions()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minFireballInterval, maxFireballInterval));

            // Decide whether to shoot a fireball or idle
            if (Random.Range(0f, 1f) < 0.5f)
            {
                ShootFireball();
            }
            else
            {
                // Idle for a random duration
                animator.SetBool("Idle", true);
                yield return new WaitForSeconds(Random.Range(idleDuration / 2, idleDuration));
                animator.SetBool("Idle", false);

                // Walk for a random duration
                if (movingRight)
                    MoveRight();
                else
                    MoveLeft();

                yield return new WaitForSeconds(Random.Range(walkDuration / 2, walkDuration));

                // Stop walking
                animator.SetBool("Walking", false);
            }
        }
    }
}

