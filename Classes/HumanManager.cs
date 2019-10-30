using System.Collections.Generic;
using UnityEngine;
using System;

public class HumanManager
{

    public int _currentPopulation = 0;
    public int _maxPopulation = 5;
    public int _lifeExpectancy = 80;

    //=========Constant=random=======//
    const int lifeExpectancyVariation = 10;

    public List<Human> humans;

    public enum SortCondition { idle, manualSkill, farmingSkill, teachingSkill, foodUpKeep } //keep foodupkeep at end for code
    public SortCondition sortCondition = SortCondition.idle;
    public bool sortAscending = false;
    public Human[] sortedArray;

    public HumanManager()
    {
        humans = new List<Human>();
        RepopulateArray();
    }

    public void SortList ()
    {
        switch (sortCondition)
        {
            case SortCondition.idle:
                {
                    List<Human> temp = new List<Human>();
                    HumanController hc;
                    if (sortAscending) //ascending - idle, in building, pregnant, teaching, teaching, lumberjacking, farming 
                    {
                        for (int i = 0; i < sortedArray.Length; i++)
                        {
                            if (sortedArray[i] != null)
                            {
                                hc = sortedArray[i]._go.GetComponent<HumanController>();
                                if (hc.buildingInside == null && hc.resourceBeingGathered == null)
                                {
                                    hc.human.sortState = Human.SortState.idle;
                                    temp.Add(hc.human);
                                    sortedArray[i] = null;
                                }
                            }
                        }
                        for (int i = 0; i < sortedArray.Length; i++)
                        {
                            if (sortedArray[i] != null)
                            {
                                hc = sortedArray[i]._go.GetComponent<HumanController>();
                                if (hc.buildingInside != null)
                                {
                                    if (hc.buildingInside._buildingState == Building.BuildingState.idle)
                                    {
                                        hc.human.sortState = Human.SortState.inBuilding;
                                        temp.Add(hc.human);
                                        sortedArray[i] = null;
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < sortedArray.Length; i++)
                        {
                            if (sortedArray[i] != null)
                            {
                                hc = sortedArray[i]._go.GetComponent<HumanController>();
                                if (hc.buildingInside != null)
                                {
                                    if (hc.buildingInside._buildingState == Building.BuildingState.Work)
                                    {
                                        hc.human.sortState = Human.SortState.pregnant;
                                        temp.Add(hc.human);
                                        sortedArray[i] = null;
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < sortedArray.Length; i++)
                        {
                            if (sortedArray[i] != null)
                            {
                                hc = sortedArray[i]._go.GetComponent<HumanController>();
                                if (hc.buildingInside != null)
                                {
                                    if (hc.buildingInside._buildingState == Building.BuildingState.Learn)
                                    {
                                        if (hc.human._ageZone >= Human.AgeZone.Adult)
                                        {
                                            hc.human.sortState = Human.SortState.teaching;
                                            temp.Add(hc.human);
                                            sortedArray[i] = null;
                                        }
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < sortedArray.Length; i++)
                        {
                            if (sortedArray[i] != null)
                            {
                                hc = sortedArray[i]._go.GetComponent<HumanController>();
                                if (hc.buildingInside != null)
                                {
                                    if (hc.buildingInside._buildingState == Building.BuildingState.Learn)
                                    {
                                        if (hc.human._ageZone < Human.AgeZone.Adult)
                                        {
                                            hc.human.sortState = Human.SortState.teaching;
                                            temp.Add(hc.human);
                                            sortedArray[i] = null;
                                        }
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < sortedArray.Length; i++)
                        {
                            if (sortedArray[i] != null)
                            {
                                hc = sortedArray[i]._go.GetComponent<HumanController>();
                                if (hc.resourceBeingGathered != null)
                                {
                                    if (hc.resourceBeingGathered.resourceT == ResourceManager.ResourceType.Wood)
                                    {
                                        hc.human.sortState = Human.SortState.lumberjacking;
                                        temp.Add(hc.human);
                                        sortedArray[i] = null;
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < sortedArray.Length; i++)
                        {
                            if (sortedArray[i] != null)
                            {
                                hc = sortedArray[i]._go.GetComponent<HumanController>();
                                if (hc.resourceBeingGathered != null)
                                {
                                    if (hc.resourceBeingGathered.resourceT == ResourceManager.ResourceType.Food)
                                    {
                                        hc.human.sortState = Human.SortState.farming;
                                        temp.Add(hc.human);
                                        sortedArray[i] = null;
                                    }
                                }
                            }
                        }
                    } else
                    {
                        for (int i = 0; i < sortedArray.Length; i++)
                        {
                            if (sortedArray[i] != null)
                            {
                                hc = sortedArray[i]._go.GetComponent<HumanController>();
                                if (hc.resourceBeingGathered != null)
                                {
                                    if (hc.resourceBeingGathered.resourceT == ResourceManager.ResourceType.Food)
                                    {
                                        hc.human.sortState = Human.SortState.farming;
                                        temp.Add(hc.human);
                                        sortedArray[i] = null;
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < sortedArray.Length; i++)
                        {
                            if (sortedArray[i] != null)
                            {
                                hc = sortedArray[i]._go.GetComponent<HumanController>();
                                if (hc.resourceBeingGathered != null)
                                {
                                    if (hc.resourceBeingGathered.resourceT == ResourceManager.ResourceType.Wood)
                                    {
                                        hc.human.sortState = Human.SortState.lumberjacking;
                                        temp.Add(hc.human);
                                        sortedArray[i] = null;
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < sortedArray.Length; i++)
                        {
                            if (sortedArray[i] != null)
                            {
                                hc = sortedArray[i]._go.GetComponent<HumanController>();
                                if (hc.buildingInside != null)
                                {
                                    if (hc.buildingInside._buildingState == Building.BuildingState.Learn)
                                    {
                                        if (hc.human._ageZone < Human.AgeZone.Adult)
                                        {
                                            hc.human.sortState = Human.SortState.teaching;
                                            temp.Add(hc.human);
                                            sortedArray[i] = null;
                                        }
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < sortedArray.Length; i++)
                        {
                            if (sortedArray[i] != null)
                            {
                                hc = sortedArray[i]._go.GetComponent<HumanController>();
                                if (hc.buildingInside != null)
                                {
                                    if (hc.buildingInside._buildingState == Building.BuildingState.Learn)
                                    {
                                        if (hc.human._ageZone >= Human.AgeZone.Adult)
                                        {
                                            hc.human.sortState = Human.SortState.teaching;
                                            temp.Add(hc.human);
                                            sortedArray[i] = null;
                                        }
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < sortedArray.Length; i++)
                        {
                            if (sortedArray[i] != null)
                            {
                                hc = sortedArray[i]._go.GetComponent<HumanController>();
                                if (hc.buildingInside != null)
                                {
                                    if (hc.buildingInside._buildingState == Building.BuildingState.Work)
                                    {
                                        hc.human.sortState = Human.SortState.pregnant;
                                        temp.Add(hc.human);
                                        sortedArray[i] = null;
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < sortedArray.Length; i++)
                        {
                            if (sortedArray[i] != null)
                            {
                                hc = sortedArray[i]._go.GetComponent<HumanController>();
                                if (hc.buildingInside != null)
                                {
                                    if (hc.buildingInside._buildingState == Building.BuildingState.idle)
                                    {
                                        hc.human.sortState = Human.SortState.inBuilding;
                                        temp.Add(hc.human);
                                        sortedArray[i] = null;
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < sortedArray.Length; i++)
                        {
                            if (sortedArray[i] != null)
                            {
                                hc = sortedArray[i]._go.GetComponent<HumanController>();
                                if (hc.buildingInside == null && hc.resourceBeingGathered == null)
                                {
                                    hc.human.sortState = Human.SortState.idle;
                                    temp.Add(hc.human);
                                    sortedArray[i] = null;
                                }
                            }
                        }
                    }
                    int a = 0;
                    foreach (Human h in temp)
                    {
                        sortedArray[a] = h;
                        a++;
                    }
                    break;
                }
            case SortCondition.foodUpKeep:
                {
                    if (sortAscending)
                    {
                        Array.Sort(sortedArray, (x, y) => x._humanSkills.FoodUpKeep().CompareTo(y._humanSkills.FoodUpKeep())); //ascending 
                    } else
                    {
                        Array.Sort(sortedArray, (x, y) => y._humanSkills.FoodUpKeep().CompareTo(x._humanSkills.FoodUpKeep())); //descending 
                    }
                    break;
                }
            case SortCondition.manualSkill:
                {
                    if (sortAscending)
                    {
                        Array.Sort(sortedArray, (x, y) => x._humanSkills.SkillModified(HumanSkills.SkillType.Manual).currentAmount.CompareTo(y._humanSkills.SkillModified(HumanSkills.SkillType.Manual).currentAmount));
                    } else
                    {
                        Array.Sort(sortedArray, (x, y) => y._humanSkills.SkillModified(HumanSkills.SkillType.Manual).currentAmount.CompareTo(x._humanSkills.SkillModified(HumanSkills.SkillType.Manual).currentAmount));
                    }
                    break;
                }
            case SortCondition.farmingSkill:
                {
                    if (sortAscending)
                    {
                        Array.Sort(sortedArray, (x, y) => x._humanSkills.SkillModified(HumanSkills.SkillType.Farming).currentAmount.CompareTo(y._humanSkills.SkillModified(HumanSkills.SkillType.Farming).currentAmount));
                    }
                    else
                    {
                        Array.Sort(sortedArray, (x, y) => y._humanSkills.SkillModified(HumanSkills.SkillType.Farming).currentAmount.CompareTo(x._humanSkills.SkillModified(HumanSkills.SkillType.Farming).currentAmount));
                    }
                    break;
                }
            case SortCondition.teachingSkill:
                {
                    if (sortAscending)
                    {
                        Array.Sort(sortedArray, (x, y) => x._humanSkills.SkillModified(HumanSkills.SkillType.Teaching).currentAmount.CompareTo(y._humanSkills.SkillModified(HumanSkills.SkillType.Teaching).currentAmount));
                    }
                    else
                    {
                        Array.Sort(sortedArray, (x, y) => y._humanSkills.SkillModified(HumanSkills.SkillType.Teaching).currentAmount.CompareTo(x._humanSkills.SkillModified(HumanSkills.SkillType.Teaching).currentAmount));
                    }
                    break;
                }
        }
    }



    public void SetMaxPopulation (int pop)
    {
        _maxPopulation = pop;
    }

    bool first = true;

    public void AddMaxPopulation ()
    {
        if (!first)
            _maxPopulation += 5;
        else first = false;
    }

    public void RepopulateArray ()
    {
        sortedArray = new Human[humans.Count];
        int i = 0;
        foreach (Human h in humans)
        {
            sortedArray[i] = h;
            i++;
        }
        SortList();
    }

    public Human AddHuman(GameObject go) 
    {
        if (CanAddHuman())
        {
            humans.Add(new Human(go));
            _currentPopulation += 1;
            SetDeathEvent();
            GameController.instance.gameManager.resourceManager.SetMaxFood(_currentPopulation);
            return humans[humans.Count-1];
        }
        else
        {
            return null;
        }
    }

    public bool CanAddHuman ()
    {
        if (humans.Count < _maxPopulation)
        {
            return true;
        }
        else return false;
    }

    public Human AddHuman(GameObject go, int template)
    {
        if (CanAddHuman())
        {
            humans.Add(new Human(go, template));
            _currentPopulation += 1;
            SetDeathEvent();
            GameController.instance.gameManager.resourceManager.SetMaxFood(_currentPopulation);
            return humans[humans.Count - 1];
        }
        else
        {
            return null;
        }
    }

    public Human AddHuman(GameObject go, Human father, Human mother)
    {
        if (CanAddHuman())
        {
            humans.Add(new Human(go, father, mother));
            _currentPopulation += 1;
            SetDeathEvent();
            GameController.instance.gameManager.resourceManager.SetMaxFood(_currentPopulation);
            return humans[humans.Count - 1];
        }
        else
        {
            return null;
        }
    }

    float Deathtimer = 0;

    public void RemoveHuman(Human human)
    {
        Deathtimer = 0;
        if (human._name == "Nonna" && GameController.instance.GetComponent<MovieController>().isActiveAndEnabled && !Fader.instance.finished)
        {
            Fader.instance.SetEnding(Fader.Endings.Nonna);
            Fader.instance.finished = true;
        }
        humans.Remove(human);
        RepopulateArray();
        _currentPopulation--;
        if (_currentPopulation == 1)
        {
            EventHandler.instance.AddAchievement(Achievement.lonely);
        } else if (_currentPopulation == 0 && !Fader.instance.finished)
        {
            if (Fader.instance.ending == "")
                Fader.instance.SetEnding(Fader.Endings.Dead);
            Fader.instance.finished = true;
        }
        GameController.instance.gameManager.resourceManager.SetMaxFood(_currentPopulation);
    }

    public Human FindHumanByName (string name)
    {
        foreach (Human h in humans)
        {
            if (h._name == name)
            {
                return h;
            }
        }
        return null;
    }

    private void SetDeathEvent()
    {
        int deathYear = _lifeExpectancy + UnityEngine.Random.Range(0, lifeExpectancyVariation) - lifeExpectancyVariation / 2;
        int deathMonth = UnityEngine.Random.Range(0, 12);
        float value = deathYear + deathMonth / 12;
        humans[_currentPopulation - 1]._age.AddTimeEvent(TimeManager.EventType.End, deathYear, deathMonth, true); //set end time event
        humans[_currentPopulation - 1]._age.AddTimeEvent(TimeManager.EventType.AgeZoneUp,Human.childYear,0, true); //set child time event
        humans[_currentPopulation - 1]._age.AddTimeEvent(TimeManager.EventType.AgeZoneUp, Human.teenYear, 0, true);
        humans[_currentPopulation - 1].childDuration = new TimeManager.YearMonth(Human.adultYear- Human.childYear, 0);
        humans[_currentPopulation - 1]._age.AddTimeEvent(TimeManager.EventType.AgeZoneUp, Human.adultYear, 0, true); //set adult time event
        humans[_currentPopulation - 1]._age.AddTimeEvent(TimeManager.EventType.AgeZoneUp, Human.elderYear, 0, true); //set elder time event
    }

    private float timer = 0;

    struct Eat
    {
        public ResourceManager.Resources resource;
        public Human human;
    }
    public bool starving = false;


    public void AddAge(float time) //ages all the humans at each time interval
    {
        timer += time;
        Deathtimer += time * GameManager.secondsPerMonth;
        if (Deathtimer < 5)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                EventHandler.instance.AddAchievement(Achievement.respect);
            }
        }
        SortList();
        if (timer > 1 && GameController.instance.canFoodUpKeep)
        {
            Eat[] resources = new Eat[humans.Count];
            int count = 0;
            foreach (Human h in humans) //foodupkeep
            {
                resources[count].resource = new ResourceManager.Resources(h._humanSkills.FoodUpKeep(), ResourceManager.ResourceType.Food);
                resources[count].human = h;
                count++;
            }
            Array.Sort(resources, (x, y) => y.resource.amount.CompareTo(x.resource.amount)); //descending 
            starving = false;
            foreach (Eat e in resources)
            {
                if (GameController.instance.gameManager.resourceManager.EnoughResources(e.resource))
                {
                    GameController.instance.gameManager.resourceManager.DeductResources(e.resource);
                    e.human.health = Mathf.Clamp(e.human.health + (int)e.resource.amount, -1, 101);
                } else
                {
                    starving = true;
                    e.human.health = Mathf.Clamp(e.human.health - (int)e.resource.amount, -1, 101);
                    if (e.human.health < 0)
                    {
                        EventHandler.instance.AddAchievement(Achievement.diet);
                        e.human._age._go.GetComponent<HumanController>().Die(true);
                    }
                }
            }
            if (starving)
            {
                GameController.instance.StartCoroutine(UIScript.instance.FlashPopulationPanel(UIScript.Option.food, Color.red));
            }
            timer = 0;
        }
        foreach (Human h in humans)
        {
            h._age.AddMonth(time);
        }
    }
}
public static class Extenders
{
    public static string ToStringSpaced(this HumanManager.SortCondition s)
    {
        switch (s)
        {
            case HumanManager.SortCondition.idle:
                return "State";
            case HumanManager.SortCondition.foodUpKeep:
                return "Food Up Keep";
            case HumanManager.SortCondition.teachingSkill:
                return "Teaching Skill";
            case HumanManager.SortCondition.manualSkill:
                return "Manual Skill";
            case HumanManager.SortCondition.farmingSkill:
                return "Farming Skill";
        }
        // other ones, just use the base method
        return s.ToString();
    }

    public static string ToStringSpaced(this Human.SortState s)
    {
        switch (s)
        {
            case Human.SortState.idle: return "Idle";
            case Human.SortState.inBuilding: return "In Building";
            case Human.SortState.teaching: return "Teaching";
            case Human.SortState.learning: return "learning";
            case Human.SortState.pregnant: return "Pregnant";
            case Human.SortState.lumberjacking: return "Lumberjacking";
            case Human.SortState.farming: return "Farming";
        }
        return s.ToString();
    }
}
