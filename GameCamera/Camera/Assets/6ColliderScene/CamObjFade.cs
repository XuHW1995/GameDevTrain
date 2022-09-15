using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.DemiLib;
    
public class CamObjFade : MonoBehaviour
{   
    public float CheckIntnelal = 0.33f;
     public Transform target;
    public LayerMask LayerMask;
    //public Material fadeMat;
       // Start is called before the first frame update
    void Start()
    {   

         
    }   
    RaycastHit[] hits;
    float _timer;
    // Update is called once per frame
    void Update()
    {   
        if (Time.time - _timer > CheckIntnelal)
        {   
            _timer = Time.time;
        }   
        else { return; }

        
        Vector3 dir = (target.position - transform.position ).normalized;
        var castDisntance = (transform.position - target.position).magnitude ;
        hits = Physics.RaycastAll(transform.position, dir ,castDisntance, LayerMask);
        
        _tobeFadeObjs.ForEach(x => DoObjFade(x, false));
        _tobeFadeObjs.Clear();
        
        for (int i = 0; i < hits.Length; i++)
        {   
            RaycastHit hit = hits[i];

            if (_tobeFadeObjs.Contains(hit.transform.gameObject) == false)
            {   
                _tobeFadeObjs.Add( hit.transform.gameObject );
            }   
        }

        _tobeFadeObjs.ForEach(x => DoObjFade(x, true));
    }           

    void DoObjFade( GameObject obj, bool value )
    {   
        float val = value ? 0.33f : 1f;
        var render = obj.GetComponent<Renderer>();
        if(render)
            render.material.DOFade(val, "_Color", 0f);
    }   

    List<GameObject> _tobeFadeObjs = new List<GameObject>(); 
}
