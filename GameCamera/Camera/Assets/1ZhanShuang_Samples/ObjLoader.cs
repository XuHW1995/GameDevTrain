using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ObjLoader : MonoBehaviour
{   
    public Cinemachine.CinemachineBrain Brain;
    public CinemachineBlenderSettings BlenderSettings;
    public string BlendsName = "Common Blends";
    public CinemachineImpulseSource CinemachineImpulseSource;
    public CinemachineFixedSignal CinemachineFixedSignal;
    
    // Start is called before the first frame update
    void Start()
    {   
        if (Brain)
        {
            if (Brain.m_CustomBlends == null)
            {
                if (BlenderSettings)
                    Brain.m_CustomBlends = BlenderSettings;
                else
                {   
                    BlenderSettings = Resources.Load( BlendsName,typeof(CinemachineBlenderSettings) )as CinemachineBlenderSettings;
                    Brain.m_CustomBlends = BlenderSettings;
                }
            }
        }
        //
        if (CinemachineImpulseSource)
        {
            if( CinemachineFixedSignal )
                CinemachineImpulseSource.m_ImpulseDefinition.m_RawSignal = CinemachineFixedSignal;
            else
            {   
                CinemachineImpulseSource.m_ImpulseDefinition.m_RawSignal = Resources.Load( "SifuSignal",typeof(CinemachineFixedSignal) )as CinemachineFixedSignal;
            }
        }

        
    }   

    // Update is called once per frame
    void Update()
    {
        
    }
}
