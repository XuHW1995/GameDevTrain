using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class StateCamCtr : MonoBehaviour
{   
    public static StateCamCtr Get;
    public CinemachineStateDrivenCamera VirtualCameraam;
    void Awake()
    {   
        Get = this;
    }

    public void TurnOnCam( bool value )
    {
        if (value)
        {   
            VirtualCameraam.Priority = 11;
        }   
        else
        {   
            VirtualCameraam.Priority = 1;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
