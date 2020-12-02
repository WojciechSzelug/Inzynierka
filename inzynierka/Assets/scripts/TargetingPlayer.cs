using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingPlayer : MonoBehaviour
{

    public Transform target;

    public float smoothTime = 0.125f;
    public Vector3 offset;
    private Vector3 velocity = Vector3.zero;

    void FixedUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition,ref velocity,smoothTime);
        transform.position = smoothedPosition;
    }
}
