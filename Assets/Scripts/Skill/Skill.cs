using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "Skill/Skill")]
public class Skill : ScriptableObject
{
    public List<SkillInfo> skills;

    public bool GetSkill(out SkillInfo skillInfo ,string skillName)
    {
        foreach(var skill in skills)
        {
            if (skill.name == skillName)
            {
                skillInfo = skill;
                return true;
            }
        }
        skillInfo = null;
        return false;
    }

    public bool GetSkill(out SkillInfo skillInfo, int count)
    {
        skillInfo = null;
        if (count > skills.Count+1) return false;

        skillInfo = skills[count];

        return true;
    }
}
