using UnityEngine;

[System.Serializable]
public class EnemyBasicInfo : BasicInfo
{
    public enum MonsterType 
    { 
        None = 0,
        Bat,
        Chest,
        Dye,
        Earth,
        Ghast,
        SkullFlaming,
        SlimeSquare,
        Snake
    }

    public MonsterType Type;
    public float AttackDistance;
    public float ViewDistance;
    public float RunSpeed;

    public EnemyBasicInfo(EnemyBasicInfo info)
    {
        Survival = true;
        HP = info.HP;
        AttackPoint = info.AttackPoint;
        Speed = info.Speed;

        Type = info.Type;
        AttackDistance = info.AttackDistance;
        ViewDistance = info.ViewDistance;
        RunSpeed = info.RunSpeed;
    }

}
