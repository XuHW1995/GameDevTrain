using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEditor.MemoryProfiler;
using UnityEngine.UI;

public class KCamCtr : MonoBehaviour
{   
    public RectTransform ReticleImage;
    public GameObject CamTPS;
    public GameObject CamLock;
    public Transform TargetRotRef;
    public CinemachineTargetGroup AimGroup;
    public CinemachineTargetGroup followGroup;

    public Transform Target;
    public Transform[] Targets;
    public bool isTPS = true;
    public float farDis = 10;
    public float nearDis = 3;

    public GameObject TPSlockAssiant;
    public CharCtr CharCtr;
    public CharCtr Enemy;
    
    public static KCamCtr Get;
    public CinemachineImpulseSource ImpulseSource;
    public CinemachineBrain CinemachineBrain;
    private void Awake()
    {   
        Get = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Target = null;

        SwitchCamToTPS(true);

    }

    private void FixedUpdate()
    {   

    }

    void LateUpdate()
    {   

    }

    public void DoShake( float value)
    {   
       Invoke( "doShakeIntel",value );
    }

    void doShakeIntel()
    {   
        ImpulseSource.GenerateImpulse();
    }

    public void SretFOV( bool ret,float value )
    {   
        var aa = CamLock.GetComponent<CinemachineVirtualCamera>();
        if (aa)
        {
            if (ret == false)
            {
                aa.m_Lens.FieldOfView = 40;
            }
            else
            {   
                aa.m_Lens.FieldOfView = value;
            }
        }
    }   

    void LogCamDistance()
    {
        return;
        var brain = Camera.main.GetComponent<CinemachineBrain>();
        var curCam = brain.ActiveVirtualCamera;
        CinemachineVirtualCamera cVcam = curCam as CinemachineVirtualCamera;
        var a = Vector3.Distance(cVcam.transform.position, transform.position);
        Debug.LogError("LogCamDistanceLogCamDistance " + a + "  cVcam.position Y " + cVcam.transform.position.y );
    }   
    
    bool _isMoving = false;
    public float m_moveCheckDuration = 1.5f;
    float m_moveCheckTime;
    void CheckMovingState()
    {   
        if (_isMoving)
            return;
        
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
        {   
            m_moveCheckTime = 0f;
        }   
        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {   
            m_moveCheckTime += Time.deltaTime;
        }
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D))
        {   
            m_moveCheckTime = 0f;
        }   
        
        if (m_moveCheckTime > this.m_moveCheckDuration)
        {   
            SwitchCamToTPS(true);
            _isMoving = true;
            //Debug.LogError("m_moveCheckTimem_moveCheckTimem_moveCheckTime");
        }   
    }

    // Update is called once per frame
    void Update()
    {         
        CheckMovingState();
        
        if (isTPS == false)
        {   
            if (CamTPS.GetComponent<CinemachineFreeLook>())
            {   
                CamTPS.transform.position = CamLock.transform.position;
                CamTPS.transform.rotation = CamLock.transform.rotation;
            }
            
            if (Target != null)
            {   
                TargetRotRef.transform.LookAt(Target);
                PlaceReticle( Target );

                var dis = Vector3.Distance(transform.position, Target.position);
                if (dis > farDis || dis < nearDis)
                {
                    isTPS = true;
                    Target = null;

                    SwitchCamToTPS(true);
                }

                if (dis < nearDis)
                {   
                    //SetNUll();
                }   
            }   
        }
        else if (isTPS )
        {   

        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Enemy = EnemyGroupMgr.instance.GetOneEnemy();
            
            CharCtr.SetState(CharCtr.EState.Atk);
            Enemy.SetState( CharCtr.EState.Hurt);
            Enemy.transform.parent.LookAt(CharCtr.transform);
            CharCtr.Target = Enemy;
            
            m_moveCheckTime = 0f;
            _isMoving = false;

            /*
                        var t = GetT();
            if (t == null)
                return;
            */
            
            this.Target = Enemy.middle;
            AimGroup.m_Targets[0].target = Enemy.middle;

            followGroup.m_Targets[0].target = Enemy.middle;
            // TPS to Lock 

            SwitchCamToTPS( false );

            this.transform.LookAt(Target);
            isTPS = false;
        }

        if (Target && TPSlockAssiant)
        {
            TPSlockAssiant.transform.position = Target.transform.position;
        }
    }

    public void OnToTPSCam(bool isTPS )
    {   

    }
    
    private bool _tester = false;
    [ContextMenu( " SwitchCamToTPSSwitchCamToTPS" )]
    void Test()
    {   
        SwitchCamToTPS(_tester);
        _tester = !_tester;
    }

    //public float switchInterval = 1f;
    //float lastTime;
     public void SwitchCamToTPS( bool isTPS )
    {      
        this.isTPS = isTPS;
        
        if( UIMgr.Get != null )
            UIMgr.Get.GoToFocusUIMode(  !isTPS );

        if (isTPS)
        {     
            this.LogCamDistance();
            var a = CamTPS.GetComponent<CinemachineVirtualCamera>();

            if (CamTPS.GetComponent<CinemachineFreeLook>())
            {   
                CamTPS.GetComponent<CinemachineFreeLook>().Priority = 10;
            }

            if (CamTPS.GetComponent<CinemachineVirtualCamera>())
                CamTPS.GetComponent<CinemachineVirtualCamera>().Priority = 10;
            CamLock.GetComponent<CinemachineVirtualCamera>().Priority = 8;
        }   
        else
        {
            if (CamTPS.GetComponent<CinemachineFreeLook>())
                CamTPS.GetComponent<CinemachineFreeLook>().Priority = 8;
            if (CamTPS.GetComponent<CinemachineVirtualCamera>())
                CamTPS.GetComponent<CinemachineVirtualCamera>().Priority = 8;
            CamLock.GetComponent<CinemachineVirtualCamera>().Priority = 10;
        }
    }

    void SetNUll()
    {   
        var a = CamTPS.GetComponent<CinemachineVirtualCamera>();
        if (a)
        {   
            var ct = a.GetCinemachineComponent<CinemachineTransposer>();
            ct.LookRotFwd = null;
        }
    }

    Transform GetT()
    {   
        Transform _tgt = null;
        float value = farDis;
        foreach (var t in Targets)
        {   
            var dis = Vector3.Distance(transform.position, t.position);
            if (dis < value)
            {   
                value = dis;
                _tgt = t;
            }   
        }
        return _tgt;
    }

    void PlaceReticle(Transform  transform)
    {   
        var cam = Camera.main;
        var r = cam.WorldToScreenPoint(transform.position);
        var r2 = new Vector2(r.x - cam.pixelWidth * 0.5f, r.y - cam.pixelHeight * 0.5f);
        ReticleImage.anchoredPosition = r2;
    }   
}
