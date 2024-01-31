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
        Color climbPointColor = Color.clear;
        
        bool IsGrabButtonPressed = grabInput.action.ReadValue<float>() > 0.1f;
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, radius, grabLayer, QueryTriggerInteraction.Ignore);
        
        if (IsGrabButtonPressed && !isGrabbing)
        {
            if (nearbyColliders.Length > 0)
            {
                Grab(nearbyColliders[0]);
                climbPointColor = nearbyColliders[0].gameObject.GetComponent<MeshRenderer>().material.color;
            }
        }
        else Release();
        
        if (isGrabbing && climbPointColor != Color.clear)
        {
            if (climbPointColor == Color.red)
                FindObjectOfType<GameManager>().AddPoint(1);
            else if (climbPointColor == Color.yellow)
                FindObjectOfType<GameManager>().AddPoint(0.5f);
        }
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
