using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using  DG.Tweening;

public class UIMgr : MonoBehaviour
{   
    public static UIMgr Get;
    public RectTransform upT;
    public RectTransform downT;
    // Start is called before the first frame update
    void Start()
    {   
        Get = this;
        GoToFocusUIMode(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            GoToFocusUIMode( true );
        }
        
        if (Input.GetKeyDown(KeyCode.F2))
        {
            GoToFocusUIMode( false );
        }
    }

    public void GoToFocusUIMode( bool value )
    {   
        if (value)
        {          
            upT.DOAnchorPosY(200, 0);
            downT.DOAnchorPosY(-200, 0);
            upT.DOAnchorPosY(0, 0.5f);
            downT.DOAnchorPosY(0, 0.5f);
        }   
        else
        {   
            upT.DOAnchorPosY(200, 0);
            downT.DOAnchorPosY(-200, 0);
        }
    }   
    
}
