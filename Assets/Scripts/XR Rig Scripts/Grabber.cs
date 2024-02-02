using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grabber : MonoBehaviour
{
    [SerializeField] private InputActionProperty grabInput;
    [SerializeField] private float radius = 0.1f;
    [SerializeField] private LayerMask grabLayer;
    
    private ScoreTracker scoreTracker;

    private FixedJoint fixedJoint;
    private bool isGrabbing;

    private GameObject currentGrabbed;

    private void Awake()
    {
        scoreTracker = GetComponentInParent<ScoreTracker>();
    }

    private void FixedUpdate()
    {
        bool IsGrabButtonPressed = grabInput.action.ReadValue<float>() > 0.1f;
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, radius, grabLayer, QueryTriggerInteraction.Ignore);
        
        if (nearbyColliders.Length > 0)
        {
            if (IsGrabButtonPressed && !isGrabbing)
            {
                Grab(nearbyColliders[0]);
            }
            else if (!IsGrabButtonPressed && isGrabbing)
            {
                Release();
            }
        }
        else
        {
            Release();
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

        currentGrabbed = nearbyCollider.gameObject;
        currentGrabbed.tag = "Grabbed";
        
        CheckColor();
    }

    void Release()
    {
        if (currentGrabbed)
        {
            currentGrabbed.tag = "Untagged";
            currentGrabbed = null;
        }
        
        isGrabbing = false;
        if(fixedJoint) Destroy(fixedJoint);
    }

    void CheckColor()
    {
        Color color = currentGrabbed.GetComponent<MeshRenderer>().material.color;

        if (color == Color.red) scoreTracker.AddPoints(1);
        else if (color == Color.yellow) scoreTracker.AddPoints(0.5f);
    }
}
