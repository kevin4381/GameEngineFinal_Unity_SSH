using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private int enemyKilled = 0;
    public int enemiesToKillForBoss = 10;
    public Boss boss;

    public void EnemyKilled()
    {
        enemyKilled++;

        if (enemyKilled >= enemiesToKillForBoss)
        {
            boss.ActivateBoss();
        }
    }
}
