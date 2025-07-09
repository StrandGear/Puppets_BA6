using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class String_CharacterRequirment
{
    [Header("String Character Settings")]
    CharacterType m_characterType = CharacterType.StringPuppet;
    public List<StringPuppetStates> requiredStatesToPass;
    public bool cannotPass;
}