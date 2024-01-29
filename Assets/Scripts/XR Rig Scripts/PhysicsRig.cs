using UnityEngine;

public class PhysicsRig : MonoBehaviour
{
    [Header("XR Rig stuff")]
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;

    [Header("Joints")]
    [SerializeField] private ConfigurableJoint leftHandPhysics;
    [SerializeField] private ConfigurableJoint rightHandPhysics;

    private void FixedUpdate()
    {
        leftHandPhysics.targetPosition = leftHand.localPosition;
        leftHandPhysics.targetRotation = leftHand.localRotation;

        rightHandPhysics.targetPosition = rightHand.localPosition;
        rightHandPhysics.targetRotation = rightHand.localRotation;
    }
}
