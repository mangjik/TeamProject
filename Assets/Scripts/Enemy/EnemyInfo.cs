using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Enemy", menuName = "Enemy/Enemy")]
public class EnemyInfo : ScriptableObject
{
    public EnemyBasicInfo enemyBasicInfo;
}
