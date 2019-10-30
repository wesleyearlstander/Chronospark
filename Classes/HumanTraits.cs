using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanTraits
{
    public enum TraitPolarity { Time, Genetic, Prodigy, Social }

    public enum Modifier { manualSkill, farmingSkill, teachingSkill, ageSpeed, foodKeepup }

    [System.Serializable]
    public struct TraitModifier
    {
        public Modifier modifier;
        public float value;

        public TraitModifier (Modifier m, float v)
        {
            modifier = m;
            value = v;
        }
    }

    public struct Trait
    {
        public string name;
        public TraitPolarity? traitPolarity;
        public int tier;
        public List<TraitModifier> modifiers;
        public string description;
        public Sprite sprite;

        public Trait (string n, TraitPolarity? p, int t, List<TraitModifier> m, string d, Sprite s)
        {
            name = n;
            traitPolarity = p;
            tier = t;
            modifiers = new List<TraitModifier>();
            foreach (TraitModifier tm in m)
                modifiers.Add(tm);
            description = d;
            sprite = s;
        }

        public Trait (TraitObject t)
        {
            name = t.name;
            traitPolarity = t.traitPolarity;
            tier = t.tier;
            modifiers = new List<TraitModifier>();
            foreach (TraitModifier m in t.modifiers)
                modifiers.Add(m);
            description = t.description;
            sprite = t.sprite;
        }

        public override string ToString()
        {
            return name;
        }
    }

    public Trait[] traits = new Trait[1];
    public GameObject _go;
    public Human _human;

    public Trait this[int key]
    {
        get
        {
            return traits[key];
        }
        set
        {
            traits[key] = value;
        }
    }

    public HumanTraits (GameObject g, Human h) 
    {
        _go = g;
        _human = h;
        List<Trait> temp = new List<Trait>();
        foreach (TraitObject t in GameController.instance.humanTraits)
        {
            temp.Add(new Trait(t));
        }
        int count = 0;
        while (count < 1) //1 trait
        {
            Trait t = temp[Random.Range(0, temp.Count)];
            int count2 = 0;
            while (count2 < count)
            {
                if (t.name == traits[count2].name)
                {
                    count2 = 0;
                    t = temp[Random.Range(0, temp.Count)]; 
                } else
                {
                    count2++;
                }
            }
            traits[count] = t;
            count++;
        }
        ApplyTraits();
    }

    public HumanTraits (GameObject g, Human h, HumanTraits fatherTraits, HumanTraits motherTraits)
    {
        _go = g;
        int count = 0;
        _human = h;
        List<Trait> upgradedTraitsM = new List<Trait>();
        List<Trait> upgradedTraitsF = new List<Trait>();
        foreach (Trait f in fatherTraits.traits) //upgrade runthrough
        {
            foreach (Trait m in motherTraits.traits)
            {
                if (f.traitPolarity == m.traitPolarity && f.tier == m.tier)
                {
                    bool used = false;
                    foreach(Trait t in upgradedTraitsM)
                    {
                        if (t.name == m.name && t.tier == m.tier)
                        { 
                            used = true;
                        }
                    }
                    foreach(Trait t in upgradedTraitsF)
                    {
                        if (t.name == f.name && t.tier == f.tier)
                        {
                            used = true;
                        }
                    }
                    if (!used && count < 3)
                    {
                        List<Trait> temp = new List<Trait>();
                        foreach (TraitObject t in GameController.instance.humanTraits)
                        {
                            if (t.tier == f.tier + 1 && t.traitPolarity != f.traitPolarity)
                            {
                                temp.Add(new Trait(t));
                            }
                        }
                        traits[count] = temp[Random.Range(0, temp.Count)];
                        upgradedTraitsF.Add(f);
                        upgradedTraitsM.Add(m);
                        count++;
                    }
                }
            }
        }
        if (count < 3)
        {
            foreach (Trait f in fatherTraits.traits) //keep higher tier runthrough
            {
                foreach (Trait m in motherTraits.traits)
                {
                    if (f.tier != m.tier)
                    {
                        bool used = false;
                        foreach (Trait t in upgradedTraitsM)
                        {
                            if (t.name == m.name && t.tier == m.tier)
                            {
                                used = true;
                            }
                        }
                        foreach (Trait t in upgradedTraitsF)
                        {
                            if (t.name == f.name && t.tier == f.tier)
                            {
                                used = true;
                            }
                        }
                        if (!used && count < traits.Length)
                        {
                            if (f.tier > m.tier)
                            {
                                traits[count] = f;
                                upgradedTraitsF.Add(f);
                            } else
                            {
                                traits[count] = m;
                                upgradedTraitsM.Add(m);
                            }
                            count++;
                        }
                    }
                }
            }
        }
        if (count < 3)
        {
            foreach (Trait f in fatherTraits.traits) //randomize lower tier if space left
            {
                foreach (Trait m in motherTraits.traits)
                {
                    if (f.tier == m.tier && f.traitPolarity != m.traitPolarity)
                    {
                        bool used = false;
                        foreach (Trait t in upgradedTraitsM)
                        {
                            if (t.name == m.name && t.tier == m.tier)
                            {
                                used = true;
                            }
                        }
                        foreach (Trait t in upgradedTraitsF)
                        {
                            if (t.name == f.name && t.tier == f.tier)
                            {
                                used = true;
                            }
                        }
                        if (!used && count < traits.Length)
                        {
                            int temp = Random.Range(0, 100);
                            if (temp < 25)
                            {
                                traits[count] = f;
                            } else if (temp < 50)
                            {
                                traits[count] = m;
                            } else
                            {
                                List<Trait> tempTraits = new List<Trait>();
                                foreach (TraitObject t in GameController.instance.humanTraits)
                                {
                                    if (t.tier == f.tier)
                                    {
                                        tempTraits.Add(new Trait(t));
                                    }
                                }
                                traits[count] = tempTraits[Random.Range(0, tempTraits.Count)];
                            }
                            count++;
                        }
                    }
                }
            }
        }
        ApplyTraits();
    }

    void ApplyTraits ()
    {
        foreach (Trait t in traits)
        {
            foreach (TraitModifier m in t.modifiers)
            {
                switch (m.modifier)
                {
                    case Modifier.manualSkill:
                        {
                            _human._humanSkills.manualSkillModifier *= m.value;
                            break;
                        }
                    case Modifier.farmingSkill:
                        {
                            _human._humanSkills.farmingSkillModifier *= m.value;
                            break;
                        }
                    case Modifier.teachingSkill:
                        {
                            _human._humanSkills.teachingSkillModifier *= m.value;
                            break;
                        }
                    case Modifier.ageSpeed:
                        {
                            _human._age.ageSpeedModifier *= m.value;
                            break;
                        }
                    case Modifier.foodKeepup:
                        {
                            _human._humanSkills.foodUpKeepModifier *= m.value;
                            break;
                        }
                }
            }
        }
    }

}
