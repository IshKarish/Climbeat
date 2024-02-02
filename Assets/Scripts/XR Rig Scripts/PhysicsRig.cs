using UnityEngine;

public class PhysicsRig : MonoBehaviour
{
    [Header("----------Body----------")]
    public float bodyHeightMin = 0.1f;
    public float bodyHeightMax = 2;
    [SerializeField] CapsuleCollider bodyCollider;
    [SerializeField] Transform playerHead;
    
    [Header("XR Rig Hands")]
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;
    
    [Header("Joints")]
    [SerializeField] private ConfigurableJoint leftHandJoint;
    [SerializeField] private ConfigurableJoint rightHandJoint;

    private void FixedUpdate()
    {
        Vector3 localPosition = playerHead.localPosition;
        bodyCollider.height = Mathf.Clamp(localPosition.y, bodyHeightMin, bodyHeightMax);
        bodyCollider.center = new Vector3(localPosition.x, bodyCollider.height / 2, localPosition.z);
        
        leftHandJoint.targetPosition = leftHand.localPosition;
        leftHandJoint.targetRotation = leftHand.localRotation;

        rightHandJoint.targetPosition = rightHand.localPosition;
        rightHandJoint.targetRotation = rightHand.localRotation;
    }
}
