using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class TestShakeCamera : MonoBehaviour
{
    public CinemachineImpulseSource impulseSource;
    
    // Start is called before the first frame update
    void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("测试脉冲")]
    public void testShake()
    {
        impulseSource.GenerateImpulse();
    }
}
