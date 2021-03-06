/******************************************************************************
* Created by: XuHongWei
* Date: 2022-07-11 15:40:07
* Des: https://www.bilibili.com/video/BV1nE411b7X8/?spm_id_from=333.788.recommend_more_video.5&vd_source=248865c4a2cf1a36ae85f642db9d8c41
*******************************************************************************/

using System;
using UnityEngine;

public class FirstPersonCameraController: MonoBehaviour
{
    public Transform player;
    //镜头灵敏度
    public float mouseSensitivity = 100f;
    //镜头绕X轴旋转欧拉角度
    private float XRotate = 0f;

    public void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
    }

    public void LateUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        //旋转角色朝向
        player.Rotate(Vector3.up * mouseX);

        //旋转镜头上下角度
        XRotate -= mouseY;
        XRotate = Mathf.Clamp(XRotate, -60, 60f);
        transform.localRotation = Quaternion.Euler(XRotate, 0, 0);
    }
}