using System;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Ability : MonoBehaviour
{
    [HideInInspector] public InputAction m_abilityButton;

    public CharacterType characterType { get; protected set; }

    protected virtual void Start()
    {
       /* m_abilityButton = InputSystem.actions.FindAction("Interact");
        
        //if (m_abilityButton == null) { Debug.LogError("Skill input action was not assigned, disabling script."); this.enabled = false; return; }
        
        m_abilityButton.Enable();
        Debug.Log("Found action: " + m_abilityButton.name);

        m_abilityButton.performed += UseAbility;
        //m_abilityButton.canceled += UseAbility;*/
    }

    public virtual void UseAbility(InputAction.CallbackContext obj)
    {
        //Debug.Log("Ability used.");
    }
}
