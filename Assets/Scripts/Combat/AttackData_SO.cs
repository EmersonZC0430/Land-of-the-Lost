using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Attack", menuName = "Attack/Attack Data")]

public class AttackData_SO : ScriptableObject
{
    public float attackRange;//基本的攻击距离
    public float skillRange;//远程的攻击距离
    public float coolDown;//冷却

    public int minDamge;
    public int maxDamge;

    public float criticalMultiplier;//暴击加成百分比
    public float criticalChance;//暴击率


}
