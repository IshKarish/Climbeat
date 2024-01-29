using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnimator : MonoBehaviour
{
    [SerializeField] private InputActionProperty triggerInput; 
    [SerializeField] private InputActionProperty gripInput;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        float triggerValue = triggerInput.action.ReadValue<float>();
        animator.SetFloat("Trigger", triggerValue);

        float gripValue = gripInput.action.ReadValue<float>();
        animator.SetFloat("Grip", gripValue);
    }
}
