using System.Collections.Generic;
using UnityEngine;

public class Human //Human class that can be further developed
{
    public string _name;
    public string _surname;
    public GameObject _go;
    public bool _alive = true;
    public bool _isVirgin = true;
    public bool _performingTask = false;
    public bool _taskDone = false;
    public int multiplier = 1;
    public bool _speechDone = false;
    public int health = 100;
    public SortState sortState;
    public TimeManager.YearMonth childDuration;

    public enum SortState { idle, inBuilding, pregnant, teaching, learning, lumberjacking, farming }

    public override string ToString()
    {
        return _name + " " + _surname;
    }

    public Human _father;
    public Human _mother;

    public Gender _gender;
    public TimeManager _age; //takes care of age
    public AgeZone _ageZone;
    public HumanSkills _humanSkills;
    public HumanTraits _humanTraits;
    public HumanGenetics _humanGenetics;

    public enum Gender { male = 0, female = 1 };
    public enum AgeZone { Baby, Child, Teen, Adult, Elder };

    //const-variables
    public const int childYear = 4;
    public const int teenYear = 12;
    public const int adultYear = 20;
    public const int elderYear = 60; 


    public Human(GameObject go)
    {
        _go = go;
        _age = new TimeManager(_go, new TimeManager.YearMonth(Random.Range(0,30), 0));
        _father = null;
        _mother = null;
        _humanSkills = new HumanSkills(Random.Range(0, 8), go);
        _humanTraits = new HumanTraits(_go, this);
        _humanGenetics = new HumanGenetics(_go, this);
        RandomizeNameAndGender();
    }

    public Human(GameObject go, int template)
    {
        _go = go;
        _father = null;
        _mother = null;
        switch (template) {
            case 0:
                {
                    _age = new TimeManager(_go,new TimeManager.YearMonth(6,0));
                    //_ageZone = AgeZone.Child;
                    _gender = Gender.female;
                    _name = "Ava";
                    _surname = "Shimmer";
                    
                    break;
                }
            case 1:
                {
                    _age = new TimeManager(_go, new TimeManager.YearMonth(60, 0));
                    //_ageZone = AgeZone.Elder;
                    _gender = Gender.female;
                    _name = "Nonna";
                    _surname = "Shimmer";
                    _isVirgin = false;
                    break;
                }
            case 2:
                {
                    _age = new TimeManager(_go, new TimeManager.YearMonth(28, 0));
                    //_ageZone = AgeZone.Adult;
                    _gender = Gender.male;
                    _humanSkills = new HumanSkills(0, go);
                    _name = "Frase";
                    _surname = "Shimmer";
                    _isVirgin = false;
                    break;
                }
            case 3:
                {
                    _age = new TimeManager(_go, new TimeManager.YearMonth(21, 0));
                    //_ageZone = AgeZone.Adult;
                    _gender = Gender.female;
                    _humanSkills = new HumanSkills(2, go);
                    _name = "Dena";
                    _surname = "Flint";
                    _isVirgin = false;
                    break;
                }
            case 4:
                {
                    _age = new TimeManager(_go, new TimeManager.YearMonth(21, 0));
                    //_ageZone = AgeZone.Adult;
                    _gender = Gender.male;
                    _name = "Heno";
                    _surname = "Pine";
                    _humanSkills = new HumanSkills(Random.Range(0, 8), go);
                    _isVirgin = false;
                    break;
                }
        }
        if (_humanSkills == null)
            _humanSkills = new HumanSkills(template, go);
        if (_humanGenetics == null)
            _humanGenetics = new HumanGenetics(_go, this, template);
        _humanTraits = new HumanTraits(_go, this);
    }

    public Human(GameObject go, Human father, Human mother)
    {
        _go = go;
        _age = new TimeManager(_go);
        _father = father;
        _mother = mother;
        RandomizeNameAndGender(father, mother);
        _humanSkills = new HumanSkills(Random.Range(0, 8), go);
        _humanTraits = new HumanTraits(_go, this);
        _humanGenetics = new HumanGenetics(_go, this, _father, _mother);
    }

    public readonly string[] maleNames = new string[16]
    {
        "Wesley", "Liam", "Faheem", "Keith", "Dean", "Adam", "Lyle", "Gabriel", "Dan", "Andrew", "Ezekiel", "Areli",
        "Eder", "James","Cato", "Claude"
    };

    public readonly string[] femaleNames = new string[18]
    {
        "Dyna", "Delilah", "Eve", "Susanna", "Stacy", "Julia", "Tabith", "Tamara", "Hannah", "Ruth", "Leah",
        "Lois", "Rose", "Alice", "Marie", "Beatrice", "Naomi", "Narah"
    };

    public readonly string[] surnames = new string[10]
    {
        "Havna","Els","Shirley","Glassen","Versksill","Daisy","Arc","Samson","Guiten","Clipper"
    };

    void RandomizeNameAndGender()
    {
        if (Random.Range(0, 2) < 1)
        {
            _gender = Gender.male;
            _name = maleNames[Random.Range(0, maleNames.Length)];
        }
        else
        {
            _gender = Gender.female;
            _name = femaleNames[Random.Range(0, femaleNames.Length)];
        }
        _surname = surnames[Random.Range(0, surnames.Length)];
    }

    void RandomizeNameAndGender(Human father, Human mother) //getting surname from father
    {
        RandomizeNameAndGender();
        _surname = father._surname;
        if (father._name == "Heno" && mother._name == "Dena" && GameController.instance.gameObject.GetComponent<MovieController>().isActiveAndEnabled)
        {
            _name = "Lyle";
            _gender = Gender.male;
            EventHandler.instance.AddHiddenEvent(EventHandler.EventType.childBorn);
        }
    }
}
