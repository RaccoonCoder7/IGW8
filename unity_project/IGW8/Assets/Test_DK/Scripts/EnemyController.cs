using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed;
    public float recognitionRange;
    public float attackRange;
    public float attackDelay;
    public float recoveryTime;
    public float enemyBulletSpeed;
    public GameObject enemyBullet;
    public EnemyState state = EnemyState.Idle;

    private int playerLayer;
    private float currentAttackDelay;
    private float currentGroggyDelay;
    private bool isDead = false;
    private RaycastHit2D recognitionHit;

    public enum EnemyState
    {
        Idle = 0,
        Trace = 1,
        Battle = 2,
        Groggy = 3,
        Dead = 4
    }

    private void Start()
    {
        playerLayer = 1 << LayerMask.NameToLayer("Player");
        StartCoroutine(EnemyFSM());
    }

    private IEnumerator EnemyFSM()
    {
        while (!isDead)
        {
            recognitionHit = Physics2D.CircleCast(
                transform.position, recognitionRange, Vector2.up,
                Mathf.Infinity, playerLayer);
            switch (state)
            {
                case EnemyState.Idle:
                    OnIdle();
                    break;
                case EnemyState.Trace:
                    OnTrace();
                    break;
                case EnemyState.Battle:
                    OnBattle();
                    break;
                case EnemyState.Groggy:
                    OnGroggy();
                    break;
                case EnemyState.Dead:
                    OnDead();
                    break;
            }
            yield return null;
        }
    }

    private void OnIdle()
    {
        if (recognitionHit.collider)
        {
            state = EnemyState.Trace;
        }
    }

    private void OnTrace()
    {
        var attackHit = Physics2D.CircleCast(
            transform.position, attackRange, Vector2.up,
            Mathf.Infinity, playerLayer);

        if (attackHit.collider)
        {
            currentAttackDelay = attackDelay / 2;
            state = EnemyState.Battle;
            return;
        }

        if (!recognitionHit.collider)
        {
            state = EnemyState.Idle;
            return;
        }

        Vector2 direction = (recognitionHit.collider.transform.position - transform.position).normalized;
        transform.position += new Vector3(direction.x * moveSpeed * Time.deltaTime, 0, 0);
    }

    private void OnBattle()
    {
        var attackHit = Physics2D.CircleCast(
            transform.position, attackRange, Vector2.up,
            Mathf.Infinity, playerLayer);

        if (!attackHit.collider)
        {
            state = EnemyState.Trace;
            return;
        }

        if (currentAttackDelay < attackDelay)
        {
            currentAttackDelay += Time.deltaTime;
            return;
        }

        Vector2 direction = (recognitionHit.collider.transform.position - transform.position).normalized;
        var bullet = Instantiate(enemyBullet, transform.position, Quaternion.identity);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.Rotate(new Vector3(0, 0, angle));
        bullet.GetComponent<Rigidbody2D>().velocity = direction * enemyBulletSpeed;
        Destroy(bullet, 3f);
        currentAttackDelay -= attackDelay;
    }

    private void OnGroggy()
    {
        if (currentGroggyDelay < recoveryTime)
        {
            currentGroggyDelay += Time.deltaTime;
            return;
        }

        state = EnemyState.Idle;
        currentGroggyDelay = 0;
    }

    private void OnDead()
    {
        isDead = true;
        // TODO: 사망시간이 존재할 수 있음
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Bullet"))
        {
            Destroy(other.gameObject);

            if (state == EnemyState.Groggy)
            {
                state = EnemyState.Dead;
                return;
            }

            state = EnemyState.Groggy;
        }
    }
}
