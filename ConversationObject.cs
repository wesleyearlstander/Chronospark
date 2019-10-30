using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Conversation", menuName = "Cards/Conversation", order = 1)]
public class ConversationObject : ScriptableObject
{
    public new string name;
    public ConversationType conversationType;
    public enum ConversationType { family, friend, tutorial }
    public string CharacterLeftName;
    public string[] Conversation_1_Left;
    public string CharacterRightName;
    public string[] Conversation_2_Right;
    public string description;
}
