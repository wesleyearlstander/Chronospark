using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "trait", menuName = "Cards/Human Trait", order = 0)]
public class TraitObject : ScriptableObject {
    public Sprite sprite;
    public new string name;
    public HumanTraits.TraitPolarity traitPolarity;
    public int tier;
    public List<HumanTraits.TraitModifier> modifiers;
    public string description;
}
