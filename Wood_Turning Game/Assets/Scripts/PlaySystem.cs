using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySystem : MonoBehaviour
{
    public LevelStartValueCache startValueCache;

    public ChiselTable chiselTable;

    public GameObject patternProjGo_1;

    public GameObject patternProjGo_2;

    public GameObject targetProjGo;

    public GameObject targetGo;

    public GameObject painterGo;

    private static PlaySystem instance;

    private PlayState currentState;
    public GameObject cutterIconParent;

    public MeshRenderer backgrounImage;
    public Material backGroundEvaluuatingmet;
    public Material backGroundWoodmet;
    public CanvasController canvasController;
    public bool doneSomethingInCutting;
    public bool doneSomethingInPainting;
    public bool doneSomethingInDecalPainting;
    public static PlaySystem Instance
    {
        get
        {
            if (!instance)
            {
                instance = GameObject.FindObjectOfType<PlaySystem>();

                if (!instance)
                {
                    var go = new GameObject("PlaySystem");
                    go.AddComponent<PlaySystem>();
                }
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance.GetInstanceID() != GetInstanceID())
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void OnDestroy()
    {
        instance = null;
    }

    public void Start()
    {
        CreateOrChangeToNextState();
    }

    public void Update()
    {
        if (currentState != null) currentState.Update();
    }

    public void SetState(PlayState state)
    {
        if (currentState != null)
        {
            currentState.Exit();            
        }

        currentState = state;

        if (currentState != null)
        { 
            currentState.Enter();
        }       
    }

    public PlayState GetState()
    {
        return currentState;
    }  

    public void CreateOrChangeToNextState()
    {//
        if (currentState == null)
        {
            LevelManager.Instance.Load(true);

            GUIEventDispatcher.Instance.NotifyEvent(GUIEventID.StartLevel);

            Transform targetTransform = targetGo.transform;
            targetTransform.position = startValueCache.targetSpawnTransform.position;
            targetTransform.rotation = startValueCache.targetSpawnTransform.rotation;
            targetGo.GetComponent<TurningController>().ResetState();
            targetGo.GetComponent<PaintingController>().ResetState();
            targetGo.SetActive(true);

            painterGo.GetComponent<Painter>().brushTexture = startValueCache.painterTexture;

            GUIEventDispatcher.Instance.NotifyEvent(GUIEventID.UpdateMoney, LevelManager.Instance.PlayerData.moneyScore);

            patternProjGo_1.transform.position = targetGo.transform.position;
            patternProjGo_1.transform.rotation = targetGo.transform.rotation;

            TurningState state = new TurningState(this);
            state.backgrounImage = backgrounImage;
            state.backGroundWoodmet = backGroundWoodmet;
            state.targetGo = targetGo;
            state.targetTransform = targetTransform;
            state.chiselSpawnTransform = startValueCache.chiselSpawnTransform;
          state.cutterIconParent = cutterIconParent;
            state.targetProjGo = targetProjGo;
            state.patternProjGo_1 = patternProjGo_1;
            state.patternProjGo_2 = patternProjGo_2;

            SetState(state);
        }
        else if (currentState is TurningState)
        {
            ColorPaintingState state = new ColorPaintingState(this);
            state.target = targetGo.GetComponent<PaintingController>();
            state.painter = painterGo.GetComponent<Painter>();
     //       state.cutterIconParent = cutterIconParent;
            state.targetTransform = state.target.transform;
      

            SetState(state);
        }else if(currentState is SmoothingState)
        {
            SmoothingState state = new SmoothingState(this);
            state.targetGo=null;
            state.brush = null;
            state.brushSpwanTransform = null;
            state.targetTransform = null;
            SetState(state);
        }
        else if (currentState is ColorPaintingState)
        {
            DecalPaintingState state = new DecalPaintingState(this);
            state.target = targetGo.GetComponent<PaintingController>();
            state.painter = painterGo.GetComponent<Painter>();
            state.targetTransform = state.target.transform;

            SetState(state);
        }
        else if (currentState is DecalPaintingState)
        {
            EvaluatingState state = new EvaluatingState(this);
            state.target = targetGo;
            state.patternProjectionGo = patternProjGo_2;
            state.backgrounImage = backgrounImage;
            state.backGroundEvaluuatingmet = backGroundEvaluuatingmet;
            state.canvasController = canvasController;
            SetState(state);
        }
        else if (currentState is EvaluatingState)
        {
            
            SetState(null);
            CreateOrChangeToNextState();
        }
    }

    public void ResetState()
    {
        if (currentState != null)
        {
            if (currentState is EvaluatingState)
            {
                var state = currentState as EvaluatingState;
                if (state.PlayerPassed)
                {
                    var playerData = LevelManager.Instance.PlayerData;
                    playerData.levelIndex -= 1;
                    playerData.Save();     
                }

                SetState(null);
                CreateOrChangeToNextState();
            }
            else
            {
                currentState.Reset();
            }
        }
    }
}
