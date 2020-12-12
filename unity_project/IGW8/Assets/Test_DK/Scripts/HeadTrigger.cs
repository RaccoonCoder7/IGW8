using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadTrigger : MonoBehaviour
{
    public EnemyController enemyController;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Bullet"))
        {
            Destroy(other.gameObject);
            enemyController.state = EnemyController.EnemyState.Dead;
        }
    }
}
