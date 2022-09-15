using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyGroupMgr : MonoBehaviour
{
    public static EnemyGroupMgr instance;

    public CharCtr[] enemyList;

    public int index = 0;
    public CharCtr GetOneEnemy()
    {
        index++;
        if (index > enemyList.Length - 1)
        {
            index = 0;
        }
        //index = Random.Range(0, enemyList.Length);
        CharCtr enemy = enemyList[index];
        return enemy;
    }
    
    public void Start()
    {
        EnemyGroupMgr.instance = this;
        enemyList = GetComponentsInChildren<CharCtr>();
    }
}
