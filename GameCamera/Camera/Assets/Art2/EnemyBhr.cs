using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBhr : MonoBehaviour
{   
    public Renderer[] renders;
    public Material mat;
    public Material[] m_oriMat;
    public Animator anim;
[ContextMenu( "Test")]
    void Test()
    {
        Set( 0.5f);
    }

    public void Set( float value )
     {  
         foreach (var VARIABLE in (renders))
         {  
             VARIABLE.material = mat;
         }
         anim.Play( "EnemyATK" );
         Invoke( "Reset",value );
         Time.timeScale = 0.5f;
     }

     public void Reset()
     {    
         Time.timeScale = 1f;
         for (int i = 0; i < m_oriMat.Length; i++)
         {  
             renders[i].material = m_oriMat[i];
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
