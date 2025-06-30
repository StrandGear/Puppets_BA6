using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Ability : MonoBehaviour
{
    [SerializeField] InputAction m_abilityButton;
    private void Start()
    {
        if (m_abilityButton == null) { Debug.LogError("Skill input action was not assigned, disabling script."); this.enabled = false; return; }

        m_abilityButton.performed += UseAbility;
    }

    public virtual void UseAbility(InputAction.CallbackContext obj)
    {
        throw new NotImplementedException();
    }
}
