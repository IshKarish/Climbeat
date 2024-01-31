using UnityEngine;

public class PhysicsRig : MonoBehaviour
{
    [Header("XR Rig stuff")]
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;
    
    [Header("Joints")]
    [SerializeField] private ConfigurableJoint leftHandJoint;
    [SerializeField] private ConfigurableJoint rightHandJoint;

    private void FixedUpdate()
    {
        leftHandJoint.targetPosition = leftHand.localPosition;
        leftHandJoint.targetRotation = leftHand.localRotation;

        rightHandJoint.targetPosition = rightHand.localPosition;
        rightHandJoint.targetRotation = rightHand.localRotation;
    }
}
