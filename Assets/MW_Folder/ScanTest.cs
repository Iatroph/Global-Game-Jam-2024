using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanTest : MonoBehaviour, IcanPing
{
    [SerializeField]
    float allertRad, attackRad;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PingForEnemy(transform.position, allertRad, attackRad);
        }
    }

    public void PingForEnemy(Vector3 pos, float allertRad, float attackRad)
    {
        EnemyMovement enemy = null;
        Collider[] collisions = Physics.OverlapSphere(pos, allertRad);

        foreach (Collider x in collisions)
        {
            if (x.TryGetComponent<EnemyMovement>(out enemy))
            {
                break;
            }
        }

        if (enemy != null)
        {
            float dist = Vector3.Distance(pos, enemy.transform.position);
            if (dist < allertRad)
            {
                enemy.alertMe(transform.position);
            }
            if (dist < attackRad)
            {
                enemy.attackPlayer();
            }

        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = new Vector4(1f, 0f, 0f, .4f);
        Gizmos.DrawSphere(transform.position, attackRad);

        Gizmos.color = new Vector4(0f, 1f, 0f, .2f);
        Gizmos.DrawSphere(transform.position, allertRad);
    }

}
