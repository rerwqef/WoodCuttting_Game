using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayState 
{
    protected PlaySystem system;

    public PlayState(PlaySystem system)
    {
        this.system = system;
    }

    public abstract void Enter();

    public abstract void Exit();

    public abstract void Update();

    public abstract void Reset();
}

public class TurningState : PlayState
{
    public GameObject targetGo;

    private GameObject chiselGo;

    public GameObject targetProjGo;

    public GameObject patternProjGo_1;

    public GameObject patternProjGo_2;

    public Transform targetTransform;

    public Transform chiselSpawnTransform;

    public GameObject cutterIconParent;
    public MeshRenderer backgrounImage;
    public Material backGroundWoodmet;
  
    public TurningState(PlaySystem system) : base(system)
    {        
    }

    public void ReplaceChisel(GameObject chiselPrefab)
    {
        if (chiselGo && chiselGo.name == chiselPrefab.name) return;

        Vector3 chiselOldPosition = chiselSpawnTransform.position;
        if (chiselGo)
        {
            chiselOldPosition = chiselGo.transform.position;
            Object.Destroy(chiselGo);
        }

        chiselGo = Object.Instantiate(chiselPrefab, chiselOldPosition, Quaternion.identity);
        chiselGo.GetComponent<TurningChisel>().Subject = targetGo.GetComponent<TurningController>();
    }

    public override void Enter()
    {
        backgrounImage.material = backGroundWoodmet;
      cutterIconParent.SetActive(true);
        targetProjGo.SetActive(true);
        patternProjGo_1.SetActive(true);
        patternProjGo_2.SetActive(true);

        patternProjGo_2.transform.position = targetProjGo.transform.position;
        patternProjGo_2.transform.localScale = targetProjGo.transform.localScale;

        Mesh patternMesh = PatternShape.BuildMeshFromShape(PatternShape.Current);
        patternProjGo_1.GetComponent<MeshFilter>().mesh = patternMesh;
        patternProjGo_2.GetComponent<MeshFilter>().mesh = patternMesh;

        GUIEventDispatcher.Instance.NotifyEvent(GUIEventID.EnterTurning);      
    }

    public override void Exit()
    {
        Object.Destroy(chiselGo);
      cutterIconParent.SetActive(false);
       // Debug.Log("turning state exit");
        targetProjGo.SetActive(false);
        patternProjGo_1.SetActive(false);
        patternProjGo_2.SetActive(false);

        GUIEventDispatcher.Instance.NotifyEvent(GUIEventID.ExitTurning);        
    }

    public override void Update()
    {
        targetTransform.Rotate(new Vector3(1200f * Time.deltaTime, 0f, 0f));
    }

    public override void Reset()
    {
        PlaySystem.Instance.doneSomethingInCutting = false;
        chiselGo.GetComponent<TurningChisel>().ChangePosition(chiselSpawnTransform.position);
        targetGo.GetComponent<TurningController>().ResetState();
    }   
}
public class SmoothingState : PlayState
{
    public GameObject targetGo;
    public Transform targetTransform;
    public GameObject brush;
    public GameObject brushSpwanTransform;
    public SmoothingState(PlaySystem system) : base(system)
    {
    }
    public override void Reset()
    {
      
    }
    public override void Enter()
    {
      
    }
    public override void Exit()
    {
        
    }
    public override void Update()
    {
        targetTransform.Rotate(new Vector3(1200f * Time.deltaTime, 0f, 0f));
    }
}
public class ColorPaintingState : PlayState
{
    public PaintingController target;

    public Painter painter;
    public GameObject Spraypaintingbottile;
    public Transform targetTransform;
    GameObject sprayBottleGo;
  //  bool isPainting;
  //  public GameObject cutterIconParent;
    public ColorPaintingState(PlaySystem system) : base(system)
    {
    }

    public void ChangePainterColor(Color color)
    {
        painter.color = color;
    }
   
    public override void Enter()
    {
      //  cutterIconParent.SetActive(false);
        painter.enabled = false;
   sprayBottleGo=   Object.Instantiate(Spraypaintingbottile,Spraypaintingbottile.transform.position,Quaternion.identity);
        system.StartCoroutine(Schedule());        
    }

    public override void Exit()
    {
        Object.Destroy(sprayBottleGo);
        GUIEventDispatcher.Instance.NotifyEvent(GUIEventID.ExitColorPainting);
    }

    public override void Update()
    {
        // Check if the user is touching the screen or clicking the mouse
        if (TouchUtility.Enabled && TouchUtility.TouchCount > 0)
        {
            Touch touch = TouchUtility.GetTouch(0);
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));

            if (painter.TouchOnObject(touchPosition))
            {
                if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
                {
                  painter. isPainting = true;
                    painter.Paint(touchPosition);
                    //   Spraypaintingbottile.GetComponent<TouchDragSprayPaintingBottle>().setTouchposition(touchPosition);
                    if (!PlaySystem.Instance.doneSomethingInPainting) PlaySystem.Instance.doneSomethingInPainting=true;
                    //add dodomething in painting varible;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    painter.isPainting = false;
                   // Spraypaintingbottile.GetComponent<TouchDragSprayPaintingBottle>().StopMoving();
                }
            }
        }
        else if (Input.GetMouseButton(0)) // For mouse input
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));

            if (painter.TouchOnObject(mousePosition))
            {
                if (!painter.   isPainting)
                {
                    painter.isPainting = true;
                }
                painter.Paint(mousePosition);
                if (!PlaySystem.Instance.doneSomethingInPainting) PlaySystem.Instance.doneSomethingInPainting = true;
                //   Spraypaintingbottile.GetComponent<TouchDragSprayPaintingBottle>().setTouchposition(mousePosition);
                //Spraypaintingbottile.transform.LookAt(mousePosition);
                //  Spraypaintingbottile.GetComponent<TouchDragSprayPaintingBottle>().SetTargetPosition(mousePosition);
            }
            else
            {
                if (painter.isPainting)
                {
                     painter.isPainting = false;
                }
            //    Spraypaintingbottile.GetComponent<TouchDragSprayPaintingBottle>().StopMoving();
            }
        }
        else
        {
            if (painter.isPainting)
            {
                painter.isPainting = false;
            }
        }
        targetTransform.Rotate(new Vector3(80f * Time.deltaTime, 0f, 0f));
    }
    public override void Reset()
    {
        target.ResetState();
        PlaySystem.Instance.doneSomethingInPainting = false;
    }

    public IEnumerator Schedule()
    {
        float animationDuration;
        var animatorProxy = target.GetComponent<AnimationControllerProxy>();

        animatorProxy.SetEnabled(true);
        animatorProxy.Play("TurningToPainting", out animationDuration);

        yield return new WaitForSeconds(animationDuration + 0.25f);

        animatorProxy.SetEnabled(false);

        painter.enabled = true;
        painter.subject = target;
        GUIEventDispatcher.Instance.NotifyEvent(GUIEventID.EnterColorPainting);        
    }
}

public class DecalPaintingState : PlayState
{
    public PaintingController target;

    public Painter painter;

    public Transform targetTransform;

    public RenderTexture colorTexture;

    public DecalPaintingState(PlaySystem system) : base(system)
    {
    }

    public void ChangeDecalTexture(Texture2D texture)
    {
        painter.brushTexture = texture;
    }

    public override void Enter()
    {
        painter.enabled = true;
        painter.color = Color.white;
        GUIEventDispatcher.Instance.NotifyEvent(GUIEventID.EnterDecalPainting);

        colorTexture = target.GetCopyTexture();
    }

    public override void Exit()
    {
        if (colorTexture.IsCreated())
            colorTexture.Release();

        painter.gameObject.SetActive(false);

        GUIEventDispatcher.Instance.NotifyEvent(GUIEventID.ExitDecalPainting);
    }

    public override void Update()
    {
     // if (GameSettings.Instance.canplay)
       // {
            if (TouchUtility.Enabled && TouchUtility.TouchCount > 0)
            {
                Touch touch = TouchUtility.GetTouch(0);
                if (touch.phase == TouchPhase.Began && !TouchUtility.TouchedUI(touch.fingerId))
                {
                //  painter.Paint();
                  painter. Paintdecals();
                if (PlaySystem.Instance.doneSomethingInDecalPainting != true) PlaySystem.Instance.doneSomethingInDecalPainting = true;
             
            }
            }
        //}


        targetTransform.Rotate(new Vector3(80f * Time.deltaTime, 0f, 0f));
    }

    public override void Reset()
    {
        target.SetTexture(colorTexture);
        colorTexture = target.GetCopyTexture();
     PlaySystem.Instance.doneSomethingInDecalPainting = false;

    }
}

public class EvaluatingState : PlayState
{
    public GameObject target;

    public GameObject patternProjectionGo;
    public MeshRenderer backgrounImage;
    public Material backGroundEvaluuatingmet;
    public CanvasController  canvasController;
    public bool PlayerPassed { get; set; }

    bool rotateTarget = false;
    bool m=false;
    public EvaluatingState(PlaySystem system) : base(system)
    {

    }
    public override void Enter()
    {
        backgrounImage.material = backGroundEvaluuatingmet;
        canvasController.EnableStateButtons(false);
        system.StartCoroutine(Schedule());
    }

    public override void Exit()
    {
        
    }

    public override void Reset()
    {
        
    }

    public override void Update()
    {

        if (TouchUtility.Enabled && TouchUtility.TouchCount > 0)
        {
            Touch touch = TouchUtility.GetTouch(0);
            if (touch.phase == TouchPhase.Began && !TouchUtility.TouchedUI(touch.fingerId))
            {
                if( m)PlaySystem.Instance.CreateOrChangeToNextState();
            }
        }
                    //  canvasController.EnableStateButtons(false);
        if (rotateTarget)
        {
            target.transform.Rotate(new Vector3(40f * Time.deltaTime, 0f, 0f));
        }
    }

    public IEnumerator Schedule()
    {
        float animationDuration;
        var animatorProxy = target.GetComponent<AnimationControllerProxy>();

       animatorProxy.SetEnabled(true);
        animatorProxy.Play("PaintingToEvaluating", out animationDuration);

        yield return new WaitForSeconds(animationDuration);

        animatorProxy.SetEnabled(false);

        float accuracy = AccuracyEvaluator.CalculateAccuracy();
        GUIEventDispatcher.Instance.NotifyEvent(GUIEventID.StartEvaluating, accuracy);

        patternProjectionGo.SetActive(true);

        Vector3 position = target.transform.localPosition;
        Quaternion rotation = target.transform.localRotation;
        Vector3 scale = target.transform.localScale;

        var xf = patternProjectionGo.transform;
        xf.localPosition = new Vector3(-position.x, position.y, position.z);
        xf.localRotation = rotation;
        xf.localScale = scale;

        yield return new WaitForSeconds(1.5f);

        rotateTarget = true;

        if (accuracy > 0.3f)
        {
            m = true;
            PlayerPassed = true;
            var playerData = LevelManager.Instance.PlayerData;
            playerData.levelIndex += 1;
            playerData.moneyScore += Mathf.RoundToInt(accuracy * 500f);
            playerData.Save();
         
            GUIEventDispatcher.Instance.NotifyEvent(GUIEventID.UpdateMoney, playerData.moneyScore);
            
        //    GUIEventDispatcher.Instance.NotifyEvent(GUIEventID.CanRestartOrGoNext);
        }            
        else
        {
            PlayerPassed = false;
            GUIEventDispatcher.Instance.NotifyEvent(GUIEventID.LetTryAgain);
        }
            
    }
}
