using UnityEngine;
using UnityEngine.Events;

public class PassRequirements : MonoBehaviour
{
    [SerializeField] String_CharacterRequirment stringRequirements;

    [Space(15)]
    [Header("Callbacks eg. sounds, effects.")]
    [Space(5)]
    public UnityEvent OnCannotPass;
    public UnityEvent OnCanPass;

    // This returns if the character with given ability & state can pass
    public bool CanCharacterPass(Ability ability)
    {
        bool result = true;

        switch (ability.characterType)
        {
            case CharacterType.StringPuppet:
                var stringAbility = ability as AdjustTension;
                if (stringAbility != null)
                    result = CheckString(stringAbility.currentAbilityState);
                break;

                // add more types
        }

        if (result)
            OnCanPass?.Invoke();
        else
            OnCannotPass?.Invoke();
        Debug.Log($"Checking if character can pass: {result} for ability: {ability}");

        return result; // fallback: allow
    }

    private bool CheckString(StringPuppetStates state)
    {
        if (stringRequirements.cannotPass) return false;
        if (stringRequirements.requiredStatesToPass.Count == 0) return true; //when nothing is chosen any state can be passed
        return stringRequirements.requiredStatesToPass.Contains(state);
    }
}
