using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "achievement", menuName = "Cards/Achievement", order = 5)]
public class AchievementCard : ScriptableObject
{
    public string Name;
    public Achievement achievement;
    public string description;
}
public enum Achievement { tutorialDone, bestWoodsman, thirsty, farmer, biggestTree, kidsThree, incompotent, hospice, graveyard, babyCano, diet, prodigy, lemon, wincest, toto, childrenCorn, lonely, respect, cliche, slowReader, cheap, poets, smallTown, reap, sponsored, shrine, enviro}