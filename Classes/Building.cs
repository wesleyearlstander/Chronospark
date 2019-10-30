using System.Collections.Generic;
using UnityEngine;

public class Building
{
    public string _name;
    public TimeManager _age;
    public BuildingType _buildingType;
    public BuildingState _buildingState;
    public int _humanMax = 2;
    public bool _taskDone = false;

    public List<Human> humans;
    public Human[] breeding = new Human[2];

    public enum BuildingType { House, Shrine }
    public enum BuildingState { Learn, Breed, Work, idle }




    public Building (string name, GameObject go, BuildingType t)
    {
        _age = new TimeManager(go);
        humans = new List<Human>();
        _name = name;
        _buildingType = t;
        SetBuildingParameters();
    }

    void SetBuildingParameters ()
    {
        switch (_buildingType)
        {
            case BuildingType.House:
                {
                    _humanMax = 2;
                    _buildingState = BuildingState.idle;
                    break;
                }
            case BuildingType.Shrine:
                {
                    _buildingState = BuildingState.idle;
                    EventHandler.instance.AddAchievement(Achievement.shrine);
                    GameController.instance.GetComponent<AudioSource>().clip = GameController.instance.finalClip;
                    GameController.instance.GetComponent<AudioSource>().Play();
                    Fader.instance.SetEnding(Fader.Endings.builtShrine);
                    break;
                }
        }
    }

    public int HumanIndex (Human human)
    {
        int count = 0;
        foreach (Human h in humans)
        {
            if (h != human)
            {
                count++;
            }
            else
            {
                return count;
            }
        }
        return count; 
    }

    public bool CanAddHuman ()
    {
        if (humans.Count + 1 <= _humanMax && _buildingState != BuildingState.Work && _buildingType != BuildingType.Shrine)
        {
            return true;
        }
        else return false;
    }

    public bool AddHuman (Human h)
    {
        if (CanAddHuman())
        {
            humans.Add(h);
            int count = 0;
            if (humans.Count > 1)
            {
                while (count < humans.Count)
                {
                    Combo temp = CheckForCombo(humans[count], h);
                    if (temp != Combo.None)
                    {
                        if (temp == Combo.Learn)
                        {
                            _buildingState = BuildingState.Learn;
                            _age._go.GetComponent<BuildingController>().buildingAudioSource.PlayOneShot(GameController.instance.learningAssign, 1);
                            return true;
                        }
                        else if (temp == Combo.Breed)
                        {
                            if (GameController.instance.gameManager.humanManager.CanAddHuman())
                            {
                                _buildingState = BuildingState.Breed;
                          //      _age._go.GetComponent<BuildingController>().buildingAudioSource.PlayOneShot(GameController.instance.breedingAssign, 1);
                            }
                            else _buildingState = BuildingState.idle;
                            return true;
                        }
                    }
                    count++;
                }
            }
        }
        return false;
    }

    enum Combo { None, Breed, Learn }

    Combo CheckForCombo (Human h1, Human h2)
    {
        if (h1._ageZone < Human.AgeZone.Adult)
        {
            if (h2._ageZone > Human.AgeZone.Teen)
            {
                _age.AddTaskEvent(h1._age._time / h1._age.ageSpeedModifier, h1._age.timeEvents[(int)Human.AgeZone.Adult].time / h1._age.ageSpeedModifier);
                return Combo.Learn;
            }
        } else if (h2._ageZone < Human.AgeZone.Adult)
        {
            return CheckForCombo(h2, h1);
        } else if (h1._ageZone >= Human.AgeZone.Adult && h2._ageZone >= Human.AgeZone.Adult)
        {
            if (GameController.instance.gameManager.humanManager.CanAddHuman()) {
                if (h1._gender != h2._gender && h1._ageZone == Human.AgeZone.Adult && h2._ageZone == Human.AgeZone.Adult)// && h1._surname != h2._surname) incest plausable currently
                {
                    if ((h1._gender == Human.Gender.female && h1._father == h2) || (h2._gender == Human.Gender.female && h2._father == h1))
                    {
                        EventHandler.instance.AddAchievement(Achievement.wincest);
                    }
                    if ((h1._gender == Human.Gender.male && h1._mother == h2) || (h2._gender == Human.Gender.female && h2._father == h1))
                    {
                        EventHandler.instance.AddAchievement(Achievement.wincest);
                    }
                    h1._isVirgin = false;
                    h2._isVirgin = false;
                    _age._go.GetComponent<AudioSource>().PlayOneShot(GameController.instance.breedingAssign);
                    return Combo.Breed;
                } else 
                {
                    h1._isVirgin = false;
                    h2._isVirgin = false;
                    _age._go.GetComponent<AudioSource>().PlayOneShot(GameController.instance.breedingAssign);
                    if (h1._gender == h2._gender && h1._gender == Human.Gender.male && h1._ageZone == h2._ageZone && h1._ageZone == Human.AgeZone.Elder) EventHandler.instance.AddAchievement(Achievement.lemon);
                }
            } else
            {
                _age._go.GetComponent<BuildingController>().StartCoroutine(UIScript.instance.FlashPopulationPanel(UIScript.Option.population, Color.red));
                _age._go.GetComponent<AudioSource>().PlayOneShot(GameController.instance.breedingAssign);
                h1._isVirgin = false;
                h2._isVirgin = false;
            }
        }
        return Combo.None;
    }

    public void PerformingAction ()
    {
        if (_buildingState == BuildingState.Learn)
        {
            if ((int)humans[0]._ageZone < (int)humans[1]._ageZone)
            {
                humans[0]._humanSkills.LearnSkills(humans[1]._humanSkills, humans[1]._age.deltaTime, (humans[0]._age.timeEvents[(int)Human.AgeZone.Adult].time.year * 12 + humans[0]._age.timeEvents[(int)Human.AgeZone.Adult].time.month) * GameManager.secondsPerMonth);
                if (humans[0]._ageZone == Human.AgeZone.Adult)
                {
                    _buildingState = BuildingState.idle;
                    EventHandler.instance.AddHiddenEvent(EventHandler.EventType.doneTeaching);
                }
            } else
            {
                humans[1]._humanSkills.LearnSkills(humans[0]._humanSkills, humans[0]._age.deltaTime, (humans[1]._age.timeEvents[(int)Human.AgeZone.Adult].time.year * 12 + humans[1]._age.timeEvents[(int)Human.AgeZone.Adult].time.month) * GameManager.secondsPerMonth);
                if (humans[1]._ageZone == Human.AgeZone.Adult)
                {
                    EventHandler.instance.AddHiddenEvent(EventHandler.EventType.doneTeaching);
                    _buildingState = BuildingState.idle;
                }
            }
        } else if (_buildingState == BuildingState.Breed)
        {
            if (humans[0]._gender == Human.Gender.female)
            {
                breeding[0] = humans[1];
                breeding[1] = humans[0];
                Human h = humans[1];
                h._go.GetComponent<HumanController>().StartCoroutine(h._go.GetComponent<HumanController>().WaitAndLeave());
            }
            else
            {
                breeding[0] = humans[0];
                breeding[1] = humans[1];
                Human h = humans[0];
                h._go.GetComponent<HumanController>().StartCoroutine(h._go.GetComponent<HumanController>().WaitAndLeave());
            }
            _buildingState = BuildingState.Work;
            _age._taskTime = null;
            _taskDone = false;
            bool worstMale = true;
            bool worstFemale = true;
            foreach (Human h in GameController.instance.gameManager.humanManager.humans)
            {
                if (h._gender == breeding[0]._gender && h._humanSkills.SkillModified(HumanSkills.SkillType.Farming).currentAmount < breeding[0]._humanSkills.SkillModified(HumanSkills.SkillType.Farming).currentAmount)
                {
                    worstMale = false;
                } else if (h._gender == breeding[1]._gender && h._humanSkills.SkillModified(HumanSkills.SkillType.Farming).currentAmount < breeding[1]._humanSkills.SkillModified(HumanSkills.SkillType.Farming).currentAmount)
                {
                    worstFemale = false;
                }
            }
            if (worstMale && worstFemale)
            {
                EventHandler.instance.AddAchievement(Achievement.incompotent);
            }
            if (breeding[1]._age._go.GetComponent<HumanController>().movieAction == ActionType.Talk && GameController.instance.GetComponent<MovieController>().isActiveAndEnabled)
                _age._go.GetComponent<BuildingController>().WaitForSpace(breeding[0], breeding[1]);
            else
                _age.AddTaskEvent(new TimeManager.YearMonth(0, 9));
        } else if (_buildingState == BuildingState.Work)
        {
            if (_taskDone)
            {
                _taskDone = false;
                _age._taskTime = null;
                _age._go.GetComponent<BuildingController>().BirthChild(breeding[0], breeding[1]);
            }
        }
    }

    public bool RemoveHuman (Human h)
    {
        if (_buildingState == BuildingState.Work && h._gender != Human.Gender.female)
        {
            humans.Remove(h);
            _buildingState = BuildingState.Work;
            return true;
        } else if (_buildingState != BuildingState.Work)
        {
            humans.Remove(h);
            if (_buildingState == BuildingState.Learn)
                _age._taskTime = null;
            _buildingState = BuildingState.idle;
            return true;
        }
        return false;

        
    }

}