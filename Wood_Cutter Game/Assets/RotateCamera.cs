using System.Collections;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    public Transform target;            // The object the camera will focus on
    public float rotationSpeed = 10f;   // Speed of rotation (degrees per second)
    public float smoothSpeed = 0.125f;  // Smoothness of camera movement
    public float rotationAngle = 75f;   // The total rotation angle (in degrees)
    public Vector3 offset = new Vector3(0, 2, -10); // Initial camera offset
    public float pauseDuration = 1f;    // Duration to pause at the end of each rotation

    private Vector3 initialPosition;    // The initial position of the camera
    private Quaternion initialRotation; // The initial rotation of the camera

    void Start()
    {
        // Save the initial position and rotation of the camera
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // Set the initial position and make the camera look at the target
        transform.position = target.position + offset;
        transform.LookAt(target);

        // Start the rotation coroutine
        StartCoroutine(RotateCameraAroundTarget());
    }

    IEnumerator RotateCameraAroundTarget()
    {
        while (true)
        {
            // Rotate towards the target angle
            yield return StartCoroutine(RotateToAngle(rotationAngle));

            // Pause at the end of the rotation
            yield return new WaitForSeconds(pauseDuration);

            // Rotate back to the starting angle
            yield return StartCoroutine(RotateToAngle(initialRotation.y));

            // Pause before starting the next rotation
            yield return new WaitForSeconds(pauseDuration);
        }
    }

    IEnumerator RotateToAngle(float targetAngle)
    {
        // Determine the current angle and target angle
        float startAngle = transform.eulerAngles.y;
        float endAngle = (startAngle + targetAngle) % 360f;

        // Determine the direction to rotate (clockwise or counter-clockwise)
        float direction = Mathf.Sign(targetAngle);

        while (Mathf.Abs(transform.eulerAngles.y - endAngle) > 0.1f)
        {
            float step = rotationSpeed * Time.deltaTime;
            float currentAngle = transform.eulerAngles.y;

            // Calculate the shortest path for rotation
            float angleToRotate = Mathf.DeltaAngle(currentAngle, endAngle);

            // Rotate around the target
            transform.RotateAround(target.position, Vector3.up, Mathf.Clamp(angleToRotate, -step, step));

            // Keep the camera looking at the target
            transform.LookAt(target);

            yield return null;
        }

        // Ensure the camera ends precisely at the target angle
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, endAngle, transform.eulerAngles.z);
    }
}
