using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using CandyCoded;
using CandyCoded.HapticFeedback;

public class MoveCutter : MonoBehaviour
{
    public static MoveCutter moveCutter;

    public GameObject PS_shavings;  // particles when cutting
    public GameObject targetObject; // Target object for the Gizmo line

    Cutter cutter = null;
    Wood wood = null;

  public  float kMove = 10;
    float MaxY = 0;

    private GraphicRaycaster m_Raycaster;
    private PointerEventData m_PointerEventData;
    private EventSystem m_EventSystem;
  //  public GameManager gameManager;

    [SerializeField]
    private Vector3 positionOffset; // Serialized position offset for adjusting the cutter's position

    [SerializeField]
    private Vector3 rotationOffset; // Serialized rotation offset for adjusting the cutter's rotation

    float _startX;
    float _startY;
    float _currentX;
    float _currentY;
    float deltaX;
    float deltaY;
    Camera mainCam;
    public float _maxX, _maxY, _minX, _minY;
    public float normalSpeed, cuttingSpeed;

    public bool iscutting;

    private bool isDragging = false;

    [SerializeField]
    private float yDistance; // Variable to store and display the Y-axis distance

    Vector3 startingPos;
    private void Awake()
    {
        moveCutter = this;

        wood = FindObjectOfType<Wood>();

        m_Raycaster = FindObjectOfType<GraphicRaycaster>();
        m_EventSystem = FindObjectOfType<EventSystem>();
        mainCam = Camera.main;
    }
    private void Start()
    {
        startingPos = transform.position;
    }
    public void restart()
    {
        transform.position = startingPos;
    }
    private void FixedUpdate()
    {
        Cut();
    }

    private void Update()
    {
        GetInput();
        BoundChisel();

        // Calculate the Y-axis distance between the two objects
        if (targetObject != null)
        {
            yDistance = Mathf.Abs(transform.position.y - targetObject.transform.position.y);
            if (iscutting)
            {
                if (cuttingSpeed <= 2.5)
                {
                    kMove = 2.5F;
                }
               else if (yDistance <= 3)
                {
                    kMove = cuttingSpeed / 4;
                }
                else if (yDistance <= 3.5)
                {
                    kMove = cuttingSpeed / 2;
                }
                else if (yDistance <= 4.46)
                {
                    kMove = cuttingSpeed;
                }
            }
            else
            {
                kMove = normalSpeed;
            }
            // Set speed based on distance
         
           
        }

        if (isDragging)
        {
            // Your dragging logic
        }
    }

    void GetInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TouchPressed();
        }
        if (Input.GetMouseButton(0))
        {
            TouchHold();
        }
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    void TouchHold()
    {
        Vector3 currentTouch = mainCam.ScreenToViewportPoint(Input.mousePosition);
        _currentX = currentTouch.x;
        _currentY = currentTouch.y;
        deltaX = _currentX - _startX;
        deltaY = _currentY - _startY;

        isDragging = true;
        transform.position += new Vector3(deltaX, deltaY, 0) * kMove * 3 * Time.deltaTime; // Increased sensitivity
        _startX = _currentX;
        _startY = _currentY;
    }

    void BoundChisel()
    {
        Vector3 temp = transform.position;
        if (temp.x >= _maxX)
        {
            temp.x = _maxX;
        }
        if (temp.x <= _minX)
        {
            temp.x = _minX;
        }
        if (temp.y >= _maxY)
        {
            temp.y = _maxY;
        }
        if (temp.y <= _minY)
        {
            temp.y = _minY;
        }
        transform.position = temp;
    }

    void TouchPressed()
    {
        Vector3 startTouch = mainCam.ScreenToViewportPoint(Input.mousePosition);
        _startX = startTouch.x;
        _startY = startTouch.y;
    }

    bool HasUI(Vector2 pos)
    {
        bool b = false;

        if (m_Raycaster != null && m_EventSystem != null)
        {
            m_PointerEventData = new PointerEventData(m_EventSystem);
            m_PointerEventData.position = pos;

            List<RaycastResult> results = new List<RaycastResult>();

            m_Raycaster.Raycast(m_PointerEventData, results);

            if (results.Count > 0)
                b = true;
        }

        return b;
    }

    public void UpdateCutter(GameObject prefab)
    {
        if (cutter != null)
        {
            Destroy(cutter.gameObject);
        }
        cutter = Instantiate(prefab, transform).GetComponentInChildren<Cutter>();
        MaxY = cutter.GetMaxY();

        cutter.transform.localRotation = Quaternion.Euler(rotationOffset);
    }

    public void Cut()
    {
        iscutting = false;
        if (wood.CutContour(cutter.GetAllContour()))
        {
            iscutting = true;
            if (PS_shavings != null)
                Destroy(Instantiate(PS_shavings, transform.position + new Vector3(0, MaxY), Quaternion.identity), 5);

            if (AudioManager.Instance.needVibration)
            {
                HapticFeedback.MediumFeedback();
                Debug.Log("Vibrating");
            }
        }
       
    }

    // Gizmo drawing method
    private void OnDrawGizmos()
    {
        if (targetObject != null)
        {
            Gizmos.color = Color.red; // Set the color of the Gizmo line
            Vector3 startPos = transform.position;
            Vector3 targetPos = targetObject.transform.position;

            // Draw a line only in the Y-axis direction between the two objects
            Gizmos.DrawLine(new Vector3(startPos.x, startPos.y, startPos.z),
                            new Vector3(startPos.x, targetPos.y, startPos.z));
        }
    }
}
