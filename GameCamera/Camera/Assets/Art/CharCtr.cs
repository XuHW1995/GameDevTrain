using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using DG.Tweening;
public class CharCtr : MonoBehaviour
{   
    public enum EState
    {   
        Idle,
        Move,
        Atk,
        Hurt
    }   
    
    public EState State = CharCtr.EState.Idle;
    public Animator anim;
    public PlayerMove PlayerMove;
    
    public CharCtr Target;
    public CharCtr[] Targets;
    public Light lgiht;
    public int atkIndx;

    public Transform middle;
    
    // Start is called before the first frame update
    void Start()
    {   
        
    }
    
    // Update is called once per frame
    void Update()
    {   
        if (PlayerMove == null)
            return;

        var a = anim.GetCurrentAnimatorStateInfo(0);
        switch (State)
        {   
            case EState.Idle:
                anim.Play("idle");
                break;
            case EState.Move:
                anim.Play("walk");
                break;
            case EState.Atk:
                if (a.IsTag("atk") == false)
                {   
                    anim.Play("atk");
                }   
                break;
            case EState.Hurt:

                if (a.IsTag("hurt") )
                {   
                    if (a.normalizedTime > 0.95f)
                    {   
                        anim.Play("idle");
                    }   
                }       
                break;
        }

        if (State != EState.Atk)
        {   
            if (PlayerMove && PlayerMove.m_currentVleocity.magnitude < 1)
            {   
                State = EState.Idle;
            }   
            else if (PlayerMove && PlayerMove.m_currentVleocity.magnitude > 1)
            {
                SetState(EState.Move);
            }   
        }
        else if (State == EState.Atk)
        {   
            if (PlayerMove && PlayerMove.m_currentVleocity.magnitude > 1)
            {
                SetState(EState.Move);
            }
        }

        if (lgiht)
        {
            if (State == EState.Atk)
            {
                lgiht.intensity = 0.33f;
            }
            else
            {
                lgiht.intensity = 1.8f;
            }
        }
    }

    public EPOOutline.Outlinable outLIne;
    public void ShowoutLine(float value)
    {   
        Invoke(  "EnableOutLine" ,value );

        if (this.Target != null)
        {   
            Target.ShowoutLine( value );
        }
    }   

    void EnableOutLine()
    {   
        if (Target)
        {   
            Target.transform.localScale = Vector3.one;
            Target.transform.DOShakePosition(0.5f,0.5f );
            Target.transform.DOShakeScale(0.5f ,Random.onUnitSphere * 0.7f );
        }   
   
        Time.timeScale = 0.5f;
        if (outLIne)
        {   
            outLIne.enabled = true;
        }   
        Invoke(  "DisOutLine" ,0.4f );
    }

    void DisOutLine()
    {           
        Time.timeScale = 1f;
        if (outLIne)
        {
            outLIne.enabled = false;
        }
    }

    public void DoHurtDelay(float value)
    {   
        Invoke( "DoHurt",value );
    }
    
    [ContextMenu("DoHurt")]
    public void DoHurt()
    {   
        State = EState.Hurt;
        if(randomhurtAnim ==false)
            anim.Play("hurt");
        else
        {   
            var a = anim.GetCurrentAnimatorStateInfo(0);
            //if (a.IsTag("hurt") == false  &&  a.IsTag("Force") == false )
            if ( a.IsTag("Force") == false )
            {   
                int num = Random.Range(1, 4);
                anim.Play("hurt" + num);
            }
        }   
    }
    
    [FormerlySerializedAs("randomhurtAnim")] public bool randomhurtAnim = false;
    public void SetState(EState sta  )
    {   
        State = sta;
        if (sta == EState.Hurt)
        {   
            DoHurt(  );
        }
        if (sta == EState.Atk)
        {   
            PlayerMove.Speed = 0;
            PlayerMove.m_currentVleocity = Vector3.zero;
            var a = anim.GetCurrentAnimatorStateInfo(0);
            if (a.IsTag("atk") == false)
            {
                anim.Play("atk");
            }   
            Invoke("Reset",1f);
        }

        if (sta == EState.Move)
        {       
                var pos = transform.parent.position;
                transform.parent.position = new Vector3(pos.x, 0, pos.z);
            
        }
    }

    private void Reset()
    {   
        PlayerMove.Speed = 10;
    }
}
