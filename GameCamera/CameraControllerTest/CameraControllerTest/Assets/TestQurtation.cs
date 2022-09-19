using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestQurtation : MonoBehaviour
{
    public Vector3 form;
    public Vector3 to;
    public bool useUpdate = false;
    
    void Start()
    {
        // Sets the rotation so that the transform's y-axis goes along the z-axis
        //transform.rotation = Quaternion.FromToRotation(Vector3.up, transform.forward);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (useUpdate)
        {
            //transform.rotation = Quaternion.FromToRotation(form, to);
            
            // Sets the rotation so that the transform's y-axis goes along the z-axis
            transform.rotation =
                Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(30, Vector3.up), Time.deltaTime); 
        }
    }
}
