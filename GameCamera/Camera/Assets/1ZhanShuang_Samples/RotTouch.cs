using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class RotTouch : MonoBehaviour
{
    public CinemachineBrain CinemachineBrain;
       public CinemachineFreeLook CinemachineFreeLook;
    public float Value;

    public float VerticalValue;
    
    float ValueTMP;
    public float ValueMul = 30;
    public Transform FollowTgt;
    public KCamCtr camCtr;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    Vector3 pos = Vector3.zero;
    bool isTouchMoving;
    float m_ClickTime;
    // Update is called once per frame
    void Update()
    {

        Debug.DrawRay(FollowTgt.transform.position, FollowTgt.transform.forward * 20, Color.green);

        if (Input.GetMouseButtonUp(0))
        {   
            if (Time.time - m_ClickTime < 0.3f)
            {
                StartUpdateFwdTurn();
            }
            m_ClickTime = Time.time;
        }   


        UpdateFwdTurn();

        if (m_UpdateFwdTurn) return;
        UpdateNormalRot();

    }

    private void UpdateNormalRot()
    {   
        if (Input.GetMouseButtonDown(0))
        {   
            Value = 0;
            VerticalValue = 0;
            
            pos = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {   
            var delta = Input.mousePosition - pos;
            if (delta.magnitude > 1)
            {   
                if (camCtr && camCtr.isTPS == false)
                {   
                    camCtr.SwitchCamToTPS(true);
                }   
            }
            ValueTMP = delta.x;
            //Debug.LogError( delta );
            if (delta.x > 0)
            {
                Value = -1;
                //right
            }
            else if (delta.x < 0)
            {
                Value = 1;
                //left
            }
            else if (delta.x == 0)
            {
                Value = 0;
            }

            if (delta.y > 0)
            {
                VerticalValue = -1;
                //right
            }
            else if (delta.y < 0)
            {
                VerticalValue = 1;
                //left
            }
            else if (delta.y == 0)
            {
                VerticalValue = 0;
            }
            
            pos = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {   
            Value = 0;
            VerticalValue = 0;
            pos = Input.mousePosition;
        }   

        if (CinemachineBrain.ActiveVirtualCamera.Equals(CinemachineFreeLook) && CinemachineBrain.ActiveBlend == null)
        {   
            CinemachineFreeLook.m_XAxis.TouchInputValue = Value * ValueMul;
            CinemachineFreeLook.m_YAxis.TouchInputValue = VerticalValue * ValueMul;
        }   
    }

    void StartUpdateFwdTurn()
    {   
        EndStartUpdateFwdTurn();
        testFacvl = Vector3.Angle(Camera.main.transform.forward, FollowTgt.transform.forward);

        var left = FollowTgt.transform.position + FollowTgt.transform.right * -5;
        var right = FollowTgt.transform.position + FollowTgt.transform.right * 5;

        var disL = Vector3.Distance(Camera.main.transform.position,left);
        var disR = Vector3.Distance(Camera.main.transform.position, right);

        if (disL < disR)
        {   
            m_FwdRotMul = 1;
        }
        else
        {
            m_FwdRotMul = -1;
        }
        //Debug.LogError(testFacvl + "   " + m_FwdRotMul);

       m_UpdateFwdTurn = true;
    }   

    void EndStartUpdateFwdTurn()
    {   
        m_UpdateFwdTurn = false;
    }

    public int m_FwdRotMul;
    public bool m_UpdateFwdTurn;
    public void UpdateFwdTurn()
    {   
        if (m_UpdateFwdTurn == false)
            return;

        testFacvl = Vector3.SignedAngle(Camera.main.transform.forward, FollowTgt.transform.forward, Vector3.up);
        testFacvl = Mathf.Abs(testFacvl);
        if (testFacvl < 180)
        {   
            CinemachineFreeLook.m_XAxis.TouchInputValue = 30 * m_FwdRotMul;
            if (testFacvl < 60)
            {   
                CinemachineFreeLook.m_XAxis.TouchInputValue = 0.2f * m_FwdRotMul;
            }   
            if (testFacvl < 20)
            {   
                EndStartUpdateFwdTurn();
                CinemachineFreeLook.m_XAxis.TouchInputValue = 0;
            }   
        }
    }

    float testFacvl  =120 ;

    
}
