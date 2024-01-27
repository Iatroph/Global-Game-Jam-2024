using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IcanPing
{
    [SerializeField]
    //float allertRad, attackRad;
    
    public void PingForEnemy(Vector3 pos, float allertRad, float attackRad)
    {
        //
        EnemyMovement enemy = null;
        Collider[] collisions = Physics.OverlapSphere(pos, allertRad);
        foreach(Collider x in collisions)
        {
            if (x.TryGetComponent<EnemyMovement>(out enemy))
            {
                break;
            }
        }
        if (enemy != null) {
            float dist = Vector3.Distance(pos, enemy.transform.position);
            if(dist < allertRad)
            {
                enemy.allertMe();
            }
            if (dist < allertRad)
            {
                enemy.attackPlayer();
            }

        }
    }
    private void OnDrawGizmosSelected(Vector3 pos, float allertRad, float attackRad)
    {
        Gizmos.color = new Vector4(1, 0, 0, .5f);

        Gizmos.DrawSphere(pos, allertRad);
        Gizmos.DrawSphere(pos, attackRad);
    }

}
