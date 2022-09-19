using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using DG.Tweening;

public class StateBhr : StateMachineBehaviour
{   
    public string SFXNAme = "SFX";
    public GameObject particle;
    public float NormaTime = 0.12f;
    
    protected GameObject clone;
    public float animHreight = 0f;
    public bool ActStateCam;
    
    public bool moveToTarget;
    public string forceTargetAnimName;
    
    public bool useTargetUpOffet = false;
    public Vector3 TargetUpOffet = new Vector3( 0,1.5f,0 );
    
    public bool overWriteFOV;
    public float overWriteFOVValue;
    public bool doShake = false;
    public float shakeDelay = 0f;
    public bool EnemyDemoBhr;
    
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {   

        if (particle == null)
            particle = GameObject.Find(SFXNAme);
        if (clone)
            Destroy(clone);
        var cr = animator.GetComponent<CharCtr>();
        
        if (StateCamCtr.Get &&ActStateCam)
        {   
            
            StateCamCtr.Get.TurnOnCam( true );
            StateCamCtr.Get.transform.position = cr.transform.position;
            StateCamCtr.Get.transform.rotation = cr.transform.rotation;
        }

        if (string.IsNullOrEmpty(forceTargetAnimName) == false)
        {   
            if (cr && cr.Target)
            {   
                cr.Target.anim.Play( forceTargetAnimName);
            }   
        }
        
        if (cr && cr.Target)
        {   
            var  pos = cr.Target.transform.position;
            if (useTargetUpOffet == false)
            {   
                cr.Target.transform.position = new Vector3(pos.x, 0, pos.z);
            }   
            else
            {   
                cr.Target.transform.position = new Vector3( pos.x,this.TargetUpOffet.y,pos.z );
            }   
        }
        
        if (overWriteFOV)
        {   
            KCamCtr.Get.SretFOV( true,this.overWriteFOVValue );
        }   
        else
        {   
            KCamCtr.Get.SretFOV( false,this.overWriteFOVValue );
        }   

        if (doShake)
        {   
            if (cr)
            {   
                cr.ShowoutLine(shakeDelay);
            }   
            KCamCtr.Get.DoShake( this.shakeDelay );
            
        }    
        
        _timer = 0f;
        _startPos = cr.transform.position;
        _hurtDone = false;

        if (EnemyDemoBhr)
        {   
            if (cr && cr.Target)
            {   
                cr.Target.GetComponent<EnemyBhr>().Set( 0.5f );
            }   
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {   
        if (clone)
            Destroy(clone,0.1f);
        
        var cr = animator.GetComponent<CharCtr>();
        
    if (StateCamCtr.Get &&ActStateCam)
       {   
           StateCamCtr.Get.TurnOnCam( false );
       }
    }
    void DOhurt(Animator animator, AnimatorStateInfo stateInfo)
    {
        var cr = animator.GetComponent<CharCtr>();
        if (stateInfo.normalizedTime > NormaTime && NormaTime > 0)
        {
            {
                if (cr && cr.Target)
                {   
                    _hurtDone = true;
                    cr.Target.SetState(CharCtr.EState.Hurt);
                }
            }
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
{

    if (_hurtDone == false)
    {   
        DOhurt( animator, stateInfo);
    }

    var cr = animator.GetComponent<CharCtr>();
    if (moveToTarget && cr && cr.Target)
    {
        _timer += Time.deltaTime;

        var pos =cr.Target.transform.position + cr.Target.transform.forward * 1.5f;
        cr.transform.parent.position = Vector3.Lerp(_startPos, pos,  _timer/stateInfo.length);
    }

    if (TargetPushForce >0 && cr && cr.Target)
    {
        if (stateInfo.normalizedTime > 0.65f)
        {   
            cr.Target.transform.parent.position += -cr.Target.transform.parent.forward * TargetPushForce * Time.deltaTime;
        }   
    }
    
    if (stateInfo.normalizedTime > NormaTime &&particle &&cr && cr.Target)
    {   
        if (clone == null)
        {   
            clone = GameObject.Instantiate(particle, cr.Target.transform.position + this.TargetUpOffet , Quaternion.identity);
            clone.transform.LookAt(  cr.Target.transform.position +  TargetUpOffet + cr.Target.transform.forward * -5);
        }

        if (clone && sfxSpeed > 0)
        {   
            clone.transform.position = clone.transform.position + -clone.transform.forward * sfxSpeed *Time.deltaTime;
        }
    }
}

private Vector3 _startPos;
private float _timer;
private bool _hurtDone = false;

public float TargetPushForce;
public float sfxSpeed = -1f;
}   
