using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Painter : MonoBehaviour
{
    public Material uvSpaceMaterial;

    public float brushSize = 50; // Size of the spray
    public float sprayDensity = 10; // Number of spray points
    public Color color;

    public PaintingController subject;

    public Texture2D brushTexture;

    private Camera mainCamera;
    bool isspraypainting;
    public Sprite Sparypainting;
    public Sprite handpainting;
  public  bool isPainting;
    private void Awake()
    {
        mainCamera = Camera.main;
    }
    public bool TouchOnObject(Vector3 position)
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return hit.collider.gameObject == subject.gameObject;
        }

        return false;
    }

    public void SetColoRingType(Image img)
    {

        /* var state = PlaySystem.Instance.GetState() as ColorPaintingState;
         if (state != null)
         {
             state.ChangePainterColor(color);
         }*/

        isspraypainting = !isspraypainting;
        if (isspraypainting) img.sprite = Sparypainting;
        else img.sprite = handpainting;
    }

    public void Paint(Vector3 position)
    {
        // Convert world position to screen position
        Vector2 mousePos = mainCamera.WorldToScreenPoint(position);

        // Loop to create spray effect
        for (int i = 0; i < sprayDensity; i++)
        {
            // Random offset for spray effect
            Vector2 offset = Random.insideUnitCircle * brushSize / 2f;
            Vector2 sprayPos = mousePos + offset;

            Vector4 uvRect = new Vector4();
            uvRect.x = (sprayPos.x - brushSize / 2f) / Screen.width;
            uvRect.y = (sprayPos.y - brushSize / 2f) / Screen.height;
            uvRect.z = brushSize / Screen.width;
            uvRect.w = brushSize / Screen.height;

            Vector4 uvClamp = new Vector4();
            uvClamp.x = Mathf.Max(0f, uvRect.x);
            uvClamp.y = Mathf.Max(0f, uvRect.y);
            uvClamp.z = Mathf.Min(1f, uvRect.x + uvRect.z);
            uvClamp.w = Mathf.Min(1f, uvRect.y + uvRect.w);

            uvSpaceMaterial.SetTexture("_BrushTex", brushTexture);
            uvSpaceMaterial.SetMatrix("_PMatrix", mainCamera.projectionMatrix);
            uvSpaceMaterial.SetVector("_BrushUVRect", uvRect);
            uvSpaceMaterial.SetVector("_BrushUVClamp", uvClamp);
            uvSpaceMaterial.SetColor("_BrushColor", color);

            uvSpaceMaterial.SetTexture("_MainTex", subject.mainTexture);
            uvSpaceMaterial.SetMatrix("_MVMatrix", mainCamera.worldToCameraMatrix * subject.transform.localToWorldMatrix);
            uvSpaceMaterial.SetPass(0);
            subject.RenderUVSpace(uvSpaceMaterial);
        }
    }

}
    /*  public void Paintdecals()
      {
          // Define the grid size; this determines how many decals will be placed across the subject
          int gridResolutionX = 100; // Number of steps along the X axis
          int gridResolutionY = 100; // Number of steps along the Y axis

          for (int x = 0; x < gridResolutionX; x++)
          {
              for (int y = 0; y < gridResolutionY; y++)
              {
                  // Calculate normalized UV position on the subject
                  Vector2 uvPos = new Vector2((float)x / (gridResolutionX - 1), (float)y / (gridResolutionY - 1));

                  Vector4 uvRect = new Vector4();
                  uvRect.x = uvPos.x - brushSize / 2f / Screen.width;
                  uvRect.y = uvPos.y - brushSize / 2f / Screen.height;
                  uvRect.z = brushSize / Screen.width;
                  uvRect.w = brushSize / Screen.height;

                  Vector4 uvClamp = new Vector4();
                  uvClamp.x = Mathf.Max(0f, uvRect.x);
                  uvClamp.y = Mathf.Max(0f, uvRect.y);
                  uvClamp.z = Mathf.Min(1f, uvRect.x + uvRect.z);
                  uvClamp.w = Mathf.Min(1f, uvRect.y + uvRect.w);

                  uvSpaceMaterial.SetTexture("_BrushTex", brushTexture);
                  uvSpaceMaterial.SetMatrix("_PMatrix", mainCamera.projectionMatrix);
                  uvSpaceMaterial.SetVector("_BrushUVRect", uvRect);
                  uvSpaceMaterial.SetVector("_BrushUVClamp", uvClamp);
                  uvSpaceMaterial.SetColor("_BrushColor", color);

                  uvSpaceMaterial.SetTexture("_MainTex", subject.mainTexture);
                  uvSpaceMaterial.SetMatrix("_MVMatrix", mainCamera.worldToCameraMatrix * subject.transform.localToWorldMatrix);
                  uvSpaceMaterial.SetPass(0);
                  subject.RenderUVSpace(uvSpaceMaterial);
              }
          }
      }*/

//}
