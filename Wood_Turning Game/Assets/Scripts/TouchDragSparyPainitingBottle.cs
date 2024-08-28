using UnityEngine;

public class TouchDragSprayPaintingBottle : MonoBehaviour
{
    public Vector3 startPosition; // The starting position of the object
    public float moveSpeed = 5f;  // Speed at which the object moves
    public float returnSpeed = 7f; // Speed at which the object returns to the start position
     Painter painter;
    private void Start()
    {
        startPosition = new Vector3(4, -1.15f, -8);
        // Initialize the object's position to startPosition
        transform.position = startPosition;
        painter=FindAnyObjectByType<Painter>();
    }

    private void Update()
    {
        if (painter.isPainting)
        {
            // Get the position from touch or mouse input
            Vector3 targetPosition = GetTouchOrMousePosition();

            // Retain the object's current Z position, allowing movement in X and Y axes
            targetPosition.z = transform.position.z;

            // Move the object towards the target position
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(targetPosition);
        }
        else
        {
            // Return the object to the starting position when no input is detected
            transform.position = Vector3.MoveTowards(transform.position, startPosition, returnSpeed * Time.deltaTime);
        }
    }

    private Vector3 GetTouchOrMousePosition()
    {
        Vector3 inputPosition;

        // Handle touch input
        if (Input.touchCount > 0)
        {
            inputPosition = Input.GetTouch(0).position;
        }
        // Handle mouse input
        else
        {
            inputPosition = Input.mousePosition;
        }

        // Convert screen position to world position, keeping the Z position of the object constant
        inputPosition.z = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
        return Camera.main.ScreenToWorldPoint(inputPosition);
    }
}
