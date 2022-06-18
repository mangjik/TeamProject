using UnityEngine;

[System.Serializable]
public class SkillInfo
{
    public string name;
    public int ConsumedMP;
    public int AttackPoint;
    public float Delay;
    //public float CoolTime;

    public Sprite Icon;
    public string TriggerName;
    public GameObject Throw = null;
    public float Speed;
    public bool Removing;

    public GameObject UseSkill(int beforeMP, out int atferMp, out string triggerName)
    {
        atferMp = beforeMP - ConsumedMP;
        triggerName = this.TriggerName;
        return Throw;
    }
}