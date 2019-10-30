using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanSkills {
    
    public enum SkillType { Manual, Farming, Teaching }

    public struct Skill
    {
        public SkillType skillType;
        public float baseAmount;
        public float currentAmount;

        public Skill (SkillType t, float b, float c)
        {
            skillType = t;
            baseAmount = b;
            currentAmount = c;
        }

        public static Skill operator * (Skill s, float m)
        {
            return new Skill(s.skillType, s.baseAmount * m, s.currentAmount * m);
        }

        public static Skill operator / (Skill s, float d)
        {
            return new Skill(s.skillType, s.baseAmount / d, s.currentAmount / d);
        }
    }

    public Skill _manualSkill = new Skill(SkillType.Manual, 3, 3);
    public Skill _farmingSkill = new Skill(SkillType.Farming, 3, 3);
    public Skill _teachingSkill = new Skill(SkillType.Teaching, 3, 3);

    public float manualSkillModifier = 1;
    public float farmingSkillModifier = 1;
    public float teachingSkillModifier = 1;
    public float foodUpKeepModifier = 1;

    GameObject _go;

    //Constant Settings
    const float skillVariation = 0.5f;

    public HumanSkills (GameObject g)
    {
        _go = g;

    }

    public HumanSkills (int template, GameObject g)
    {
        _go = g;
        switch (template)
        {
            case 0:
                {
                    _manualSkill = new Skill(SkillType.Manual, 3, 4);
                    _farmingSkill = new Skill(SkillType.Farming, 3, 2);
                    _teachingSkill = new Skill(SkillType.Teaching, 3, 2);
                    break;
                }
            case 1:
                {
                    _teachingSkill = new Skill(SkillType.Teaching, 3, 4);
                    _farmingSkill = new Skill(SkillType.Farming, 3, 2);
                    _manualSkill = new Skill(SkillType.Manual, 3, 2);
                    break;
                }
            case 2:
                {
                    _farmingSkill = new Skill(SkillType.Farming, 3, 4);
                    _manualSkill = new Skill(SkillType.Manual, 3, 2);
                    _teachingSkill = new Skill(SkillType.Teaching, 3, 2);
                    break;
                }
            case 3:
                {
                    _teachingSkill = new Skill(SkillType.Teaching, 3, 4);
                    _farmingSkill = new Skill(SkillType.Farming, 3, 4);
                    _manualSkill = new Skill(SkillType.Manual, 3, 2);
                    break;
                }
            case 4:
                {
                    _teachingSkill = new Skill(SkillType.Teaching, 3, 4);
                    _farmingSkill = new Skill(SkillType.Farming, 3, 2);
                    _manualSkill = new Skill(SkillType.Manual, 3, 4);
                    break;
                }
            case 5:
                {
                    _teachingSkill = new Skill(SkillType.Teaching, 3, 2);
                    _farmingSkill = new Skill(SkillType.Farming, 3, 4);
                    _manualSkill = new Skill(SkillType.Manual, 3, 4);
                    break;
                }
            case 6:
                {
                    _teachingSkill = new Skill(SkillType.Teaching, 3, 4);
                    _farmingSkill = new Skill(SkillType.Farming, 3, 4);
                    _manualSkill = new Skill(SkillType.Manual, 3, 4);
                    break;
                }
            case 7:
                {
                    _teachingSkill = new Skill(SkillType.Teaching, 3, 3);
                    _farmingSkill = new Skill(SkillType.Farming, 3, 3);
                    _manualSkill = new Skill(SkillType.Manual, 3, 3);
                    break;
                }
        }
        
    }

    public HumanSkills (HumanSkills fatherSkills, HumanSkills motherSkills, GameObject g)
    {
        _go = g;
        _manualSkill = CreateSkill(fatherSkills._manualSkill, motherSkills._manualSkill);
        _farmingSkill = CreateSkill(fatherSkills._farmingSkill, motherSkills._farmingSkill);
        _teachingSkill = CreateSkill(fatherSkills._teachingSkill, motherSkills._teachingSkill);
    }

    public Skill CreateSkill (Skill first, Skill second)
    {
        float diff1 = first.currentAmount - first.baseAmount;
        float diff2 = second.currentAmount - second.baseAmount;
        float finalDiff = diff1 + diff2;
        float newBase = (first.baseAmount + second.baseAmount) / 2 + finalDiff/2;
        return new Skill (first.skillType, first.baseAmount, newBase);
    }

    public void LearnSkills ( HumanSkills teacher, float deltaTime, float childTime)
    {
        float teachingAbility = teacher._teachingSkill.currentAmount * skillVariation;
        _manualSkill.currentAmount = Mathf.Clamp(Mathf.Clamp(_manualSkill.currentAmount + (teachingAbility * teacher._manualSkill.currentAmount * deltaTime / childTime) * _manualSkill.baseAmount / 3.5f, _manualSkill.baseAmount, _manualSkill.baseAmount + (teachingAbility * _manualSkill.baseAmount)),0,7);
        _farmingSkill.currentAmount = Mathf.Clamp(Mathf.Clamp(_farmingSkill.currentAmount + (teachingAbility * teacher._farmingSkill.currentAmount * deltaTime / childTime) * _farmingSkill.baseAmount / 3.5f, _farmingSkill.baseAmount, _farmingSkill.baseAmount + (teachingAbility * _farmingSkill.baseAmount)), 0, 7);
        _teachingSkill.currentAmount = Mathf.Clamp(Mathf.Clamp(_teachingSkill.currentAmount + (teachingAbility * teacher._teachingSkill.currentAmount * deltaTime / childTime) * _teachingSkill.baseAmount / 3.5f, _teachingSkill.baseAmount, _teachingSkill.baseAmount + (teachingAbility * _teachingSkill.baseAmount)), 0, 7);
        if (_manualSkill.currentAmount == 7 || _farmingSkill.currentAmount == 7 || _teachingSkill.currentAmount == 7)
        {
            EventHandler.instance.AddAchievement(Achievement.prodigy);
        }
    }

    public Skill SkillModified (SkillType t)
    {
        switch (t)
        {
            case SkillType.Manual:
                {
                    return _manualSkill / 3 * manualSkillModifier;
                }
            case SkillType.Farming:
                {
                    return _farmingSkill / 3 * farmingSkillModifier;
                }
            case SkillType.Teaching:
                {
                    return _teachingSkill / 3 * teachingSkillModifier;
                }
        }
        return _manualSkill / 3 * manualSkillModifier; //default
    }

    public float FoodUpKeep ()
    {
        float temp = _manualSkill.currentAmount * 3;
        temp += _farmingSkill.currentAmount * 3;
        temp += _teachingSkill.currentAmount * 3;
        if (_go.GetComponent<HumanController>().human._ageZone != Human.AgeZone.Adult)
            return Mathf.Round(temp / 2 * foodUpKeepModifier);
        else return Mathf.Round(temp * foodUpKeepModifier);
    }
}
