using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource {
    public TimeManager _age;
    public ResourceManager.ResourceType resourceT;
    public int _ratePerMonth;
    public int _humanMax = 1;
    public int resourceMax;
    public float currentResources;
    public bool dead = false;
    
    public List<Human> humans;

    public int _manualSkill;
    public int _farmingSkill;
    public int _artisanSkill;

    public Resource (GameObject go, int max, ResourceManager.ResourceType res, int rate, int rm)
    {
        _age = new TimeManager(go);
        _humanMax = max;
        resourceT = res;
        _ratePerMonth = rate;
        humans = new List<Human> ();
        resourceMax = rm;
        currentResources = resourceMax;
        GameController.instance.gameManager.resourceManager.AddResource(this);
    }

    public void SetSkills (int m, int t, int a)
    {
        _manualSkill = m;
        _farmingSkill = t;
        _artisanSkill = a;
    }

    public bool CanAddHuman (Human h)
    {
        if (humans.Count + 1 <= _humanMax && !dead)
        {
            if (h._ageZone == Human.AgeZone.Adult || h._ageZone == Human.AgeZone.Teen)
                return true;
        }
        return false;
    }

    public bool AddHuman (Human h)
    {
        if (CanAddHuman(h))
        {
            humans.Add(h);
            h._performingTask = true;
            h._age.AddTaskEvent(TaskTime(h));
            GameController.instance.gameManager.resourceManager.peopleOnCrops++;
            return true;
        } else
        {
            return false;
        }
    }

    public void RemoveHuman (Human h)
    {
        h._performingTask = false;
        GameController.instance.gameManager.resourceManager.peopleOnCrops--;
        _age._go.GetComponent<ResourceController>().resourceAudioSource.PlayOneShot(GameController.instance.removeFromWork);
        humans.Remove(h);
    }

    public bool AddTime (float time) //true if returns resource
    {
        _age.AddMonth(time);
        if (humans.Count == 0) return false;
        else
        {
            foreach (Human h in humans)
            {
                if (h._taskDone)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public ResourceManager.Resources GetResources ()
    {
        ResourceManager.Resources temp;
        temp.resourceType = resourceT;
        temp.amount = 0;
        foreach (Human h in humans)
        {
            if (h._taskDone)
            {
                temp.amount += _ratePerMonth * h.multiplier; //multiplier required for big time
                h._taskDone = false;
                h._age.AddTaskEvent(TaskTime(h));
            }
        }
        if (temp.amount <= currentResources)
        {
            currentResources -= temp.amount;
            return temp;
        } else
        {
            //resource dies;
            dead = true;
            if (humans[0]._age._go.GetComponent<HumanController>().selected)
            {
                humans[0]._age._go.GetComponent<HumanController>().Select(false);
            }
            PlayerController.instance.previousSelection.selected = null;
            humans[0]._age._go.GetComponent<HumanController>().LeaveResource();
            temp.amount = currentResources;
            if (_age._go.GetComponent<TreeController>() != null)
            {
                _age._go.GetComponent<TreeController>().Die();
            } else
            {
                _age._go.GetComponent<Gravity>().Gravitate = true;
                _age._go.GetComponent<ResourceController>().DestroyAfterSeconds(4);
                GameController.instance.cropDeadCounter++;
                if (GameController.instance.cropDeadCounter == 3)
                {
                    EventHandler.instance.AddAchievement(Achievement.farmer);
                }
            }
            return temp;
        }
        
    }

    public TimeManager.YearMonth TaskTime (Human h)
    {
        float multiplier = 1;
        if (resourceT == ResourceManager.ResourceType.Wood)
        {
            multiplier /= h._humanSkills.SkillModified(HumanSkills.SkillType.Manual).currentAmount * _manualSkill / 10;
        } else if (resourceT == ResourceManager.ResourceType.Food)
        {
            multiplier /= h._humanSkills.SkillModified(HumanSkills.SkillType.Farming).currentAmount * _farmingSkill / 10;
        }
        TimeManager.YearMonth temp;
        temp.month = multiplier;
        temp.year = 0;
        return temp;
    }

}
