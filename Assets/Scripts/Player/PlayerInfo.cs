using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Player", menuName = "Player/Career")]
public class PlayerInfo : ScriptableObject
{
    public enum Career { Wizard, Warrior };

    public Career career;

    public PlayerBasicInfo playerBasicInfo;

    public Skill skills;

    public bool UseSkill(int count ,int MP ,out int afterMp, out string triggerName, out GameObject Throw)
    {
        Throw = null;
        SkillInfo skillInfo = null;

        if (skills.GetSkill(out skillInfo, count) == false || MP - skillInfo.ConsumedMP < 0)
        {
            afterMp = MP;
            //coolTime = 0;
            triggerName = null;
            return false;
        }
        else
        {
            Throw = skillInfo.UseSkill(MP, out afterMp, out triggerName);
            return true;
        }
    }






}
