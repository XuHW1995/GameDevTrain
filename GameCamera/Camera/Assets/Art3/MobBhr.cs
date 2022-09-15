using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

public class MobBhr : MonoBehaviour
{   
    public Transform player;
    public Transform[] Targets;
    public Transform TargetOutOfRange;
    public Transform[] TargetOutOfRangePath;
    public float Radius = 10;
    public float OutofRange = 15;

    public CinemachineTargetGroup group;

    public static MobBhr Get;
    // Start is called before the first frame update
    void Start()
    {   
        Get = this;
        InvokeRepeating( "RandomMove",1,4);
        InvokeRepeating( "SeqMove",1,2);
    }   

    // Update is called once per frame
    void Update()
    {   
        
    }
    
    void RandomMove()
    {   
        foreach (var VARIABLE in Targets )
        {   
            TargetMoveRandom(VARIABLE );
        }   
    }
    
    void TargetMoveRandom(Transform t)
    {
        CheckTarget( t );
        var rnd = Random.insideUnitSphere;
        if (rnd.y < 0)
        {   
            rnd.y = -rnd.y;
        }   
        var rndPos = transform.position + rnd * Radius;
        t.transform.DOMove(   rndPos,2f);
    }

    private int _idx = 0;
    void SeqMove()
    {          
        CheckTarget( TargetOutOfRange );
        if (_idx > TargetOutOfRangePath.Length - 1)
            _idx = 0;
        var trans = TargetOutOfRangePath[  _idx];
        TargetOutOfRange.transform.DOMove(trans.position, 1f);
        _idx += 1;
    }   

    public void CheckTarget(Transform trans)
    {   
        int idx = 0;
        CinemachineTargetGroup.Target tt = default( CinemachineTargetGroup.Target );
        //
        foreach (var groupMTarget in group.m_Targets)
        {
            if (groupMTarget.target == trans)
            {   
                tt = groupMTarget;
                break;
            }   
            ++idx;
        }
        
        if (tt.Equals( default(CinemachineTargetGroup.Target)) )
            return;
        
        float dis = Vector3.Distance( player.position, trans.position  );
     
        if (dis > this.OutofRange)
        {   
            if( tt.target.transform.GetComponentInChildren<EPOOutline.Outlinable>())
                tt.target.transform.GetComponentInChildren<EPOOutline.Outlinable>().enabled = false;
            tt.weight = 0;
        }   
        else
        {               
            if( tt.target.transform.GetComponentInChildren<EPOOutline.Outlinable>())
                tt.target.transform.GetComponentInChildren<EPOOutline.Outlinable>().enabled = true;
            tt.weight = dis;
        }
        group.m_Targets[idx] = tt;
    }   
}
