using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRateMaintainer : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        Application.targetFrameRate = 60;
    }

   
}
