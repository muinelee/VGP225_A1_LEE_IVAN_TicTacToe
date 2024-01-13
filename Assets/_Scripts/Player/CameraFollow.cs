using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offsetPosition;
    public float followSpeed = 25.0f;
    public float rotationSpeed = 20.0f;

    private void LateUpdate()
    {
        Vector3 desiredPosition = target.TransformPoint(offsetPosition);
        // Smoothly interpolate between the camera's current position and the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position);
        // Smoothly interpolate between the camera's current rotation and the desired rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
    }
}
