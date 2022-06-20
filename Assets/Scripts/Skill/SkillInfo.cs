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
    public Vector2 BoxSize;
    public GameObject Throw = null;
    public float Speed;
    public bool Removing;
    public AudioClip Audio;
    public GameObject UseSkill(int beforeMP, out int atferMp, out string triggerName)
    {
        atferMp = beforeMP - ConsumedMP;
        triggerName = this.TriggerName;
        return Throw;
    }

    public GameObject UseSkill(out string triggerName)
    {
        triggerName = this.TriggerName;
        return Throw;
    }
    
   
}
