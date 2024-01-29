using UnityEngine;
using UnityEngine.InputSystem;

public class Grabber : MonoBehaviour
{
    [SerializeField] private InputActionProperty grabInput;
    [SerializeField] private float radius = 0.1f;
    [SerializeField] private LayerMask grabLayer;

    private FixedJoint fixedJoint;
    private bool isGrabbing;
    
    private void FixedUpdate()
    {
        bool IsGrabButtonPressed = grabInput.action.ReadValue<float>() > 0.1f;
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, radius, grabLayer, QueryTriggerInteraction.Ignore);
        
        if (IsGrabButtonPressed && !isGrabbing)
        {
            if (nearbyColliders.Length > 0) Grab(nearbyColliders[0]);
        }
        else Release();
    }

    void Grab(Collider nearbyCollider)
    {
        Rigidbody nearbyRb = nearbyCollider.attachedRigidbody;
            
        fixedJoint = gameObject.AddComponent<FixedJoint>();
        fixedJoint.autoConfigureConnectedAnchor = false;

        if (nearbyRb)
        {
            fixedJoint.connectedBody = nearbyRb;
            fixedJoint.connectedAnchor = nearbyRb.transform.InverseTransformPoint(transform.position);
        }
        else
        {
            fixedJoint.connectedAnchor = transform.position;
        }

        isGrabbing = true;
    }

    void Release()
    {
        isGrabbing = false;
        
        if(fixedJoint) Destroy(fixedJoint);
    }
}
