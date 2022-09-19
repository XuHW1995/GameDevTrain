using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class attackbehavoir : StateMachineBehaviour
{   
    public string SFXNAme = "SFX";
    public GameObject particle;
    public float NormaTime = 0.12f;
    public Vector3 UpOffet = new Vector3( 0,1.5f,0 );
    public float withScael = 1f;
    public float withScaelYY = 1f;
    public float RotY = 0f;
    protected GameObject clone;
    protected List<GameObject> clones;
    public float animHreight = 0f;
    public bool AnimHRest;
    public bool useAnimHRest;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {   
        if (particle == null)
            particle = GameObject.Find(SFXNAme);
        //Debug.LogError("On Attack OnStateEnter ");
        if (clone)
            Destroy(clone);

        var cr = animator.GetComponent<CharCtr>();
        if (cr && cr.atkIndx % 2 == 1)
        {   
            var pos = cr.transform.parent.position;
            cr.transform.parent.position = new Vector3( pos.x, animHreight, pos.z );
        }
        
        if (cr && cr.Targets.Length > 0)
        {   
            foreach (var VARIABLE in cr.Targets)
            {   
                GameObject go = null;
                float dis = Vector3.Distance( VARIABLE.transform.position, animator.transform.position  );
                    
                if (dis < MobBhr.Get.OutofRange)
                    HandleTagets( go,animator, VARIABLE );
            }   
        }
        else
        {   
            HandleTagets( clone,animator );
        }
    }   

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {   
        if (clone)
            Destroy(clone,0.5f);
        
        var cr = animator.GetComponent<CharCtr>();
        if (cr && AnimHRest)
        {   
            var pos = cr.transform.parent.position;
            cr.transform.parent.position = new Vector3(pos.x, 0, pos.z);
        }

        if (cr && useAnimHRest)
        {   
            cr.atkIndx += 1;
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {   
        //Debug.LogError("On Attack Update  normalizedTime" + stateInfo.normalizedTime);
        if (stateInfo.normalizedTime > NormaTime)
        {   

        }   
    }

    void HandleTagets( GameObject clone, Animator animator,CharCtr index = null )
    {   
        if (clone == null)
        {   
            clone = GameObject.Instantiate( particle , animator.transform.position + UpOffet, animator.transform.rotation);
            var a = clone.transform.localScale;
            clone.transform.localScale = new Vector3( a.x * withScaelYY, a.y * this.withScael,a.z);
        }   
        if (clone)
        {   
            var cr = animator.GetComponent<CharCtr>();
            if (index == null)
            {
                if (cr && cr.Target)
                {   
                    clone.transform.LookAt(cr.Target.transform.position + UpOffet);
                    clone.transform.DOMove(cr.Target.transform.position + UpOffet, 1f).SetDelay( this.NormaTime );
                    cr.Target.DoHurtDelay(1f);
                    Destroy( clone,1f );
                }   
            }
            else
            {   
                CharCtr Target = index;
                if (Target)
                {   
                    clone.transform.LookAt(Target.transform.position + UpOffet);
                    clone.transform.DOMove(Target.transform.position + UpOffet, 1f);
                    Target.DoHurtDelay(1f);
                    Destroy( clone,1f );
                }
            }
        }
    }
    

    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.LogError("On Attack Move ");
    }

    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    Debug.LogError("On Attack IK ");
    //}
}
