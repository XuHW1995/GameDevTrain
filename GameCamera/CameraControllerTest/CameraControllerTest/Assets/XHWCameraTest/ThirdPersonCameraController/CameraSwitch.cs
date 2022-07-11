/******************************************************************************
* Created by: XuHongWei
* Date: 2022-07-11 17:11:55
* Des: 
*******************************************************************************/

using System;
using UnityEngine;

public class CameraSwitch: MonoBehaviour
{
    public GameObject FirstPersonCamera;
    public GameObject UpdownCamera;
    public GameObject ThirdPersonCamera;
    
    public void OnGUI()
    {
        if(GUI.Button(new Rect(100f, 100f,100, 50), "第一人称"))
        {
            FirstPersonCamera.SetActive(true);
            UpdownCamera.SetActive(false);
            ThirdPersonCamera.SetActive(false);
        }
        
        if(GUI.Button(new Rect(100f, 200,100, 50), "第三人称"))
        {
            FirstPersonCamera.SetActive(false);
            UpdownCamera.SetActive(false);
            ThirdPersonCamera.SetActive(true);
        }
        
        if(GUI.Button(new Rect(100f, 300,100, 50), "顶视角"))
        {
            FirstPersonCamera.SetActive(false);
            UpdownCamera.SetActive(true);
            ThirdPersonCamera.SetActive(false);
        }
    }
}