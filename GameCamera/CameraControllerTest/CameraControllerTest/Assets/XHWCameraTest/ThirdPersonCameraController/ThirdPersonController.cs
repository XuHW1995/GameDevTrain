/******************************************************************************
* Created by: XuHongWei
* Date: 2022-07-11 14:25:00
* Des: 第三人称角色控制
*******************************************************************************/

using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController: MonoBehaviour
{
    private CharacterController cc;
    public float moveSpeed = 3;
    public Vector3 curVelocityXZ;
    public Vector3 curVelocityY;
    public float gravity = -9.81f;

    public Transform GroundCheckObj;
    public bool isGrounded;
    public float gourndDis = 0.4f;
    public LayerMask groundMask;

    public float jumpHeight = 3f;
    
    public void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    public void Update()
    {
        MoveByKeyBoard();
    }
    
    private void MoveByKeyBoard()
    {
        float xAxis = Input.GetAxis("Horizontal");
        float zAxis = Input.GetAxis("Vertical");
        Vector3 zxMoveDir = (transform.forward * zAxis + transform.right * xAxis).normalized;
        //水平移动速度
        curVelocityXZ = zxMoveDir * moveSpeed;
        cc.Move(curVelocityXZ * Time.deltaTime);
        
        //垂直移动速度
        isGrounded = Physics.CheckSphere(GroundCheckObj.position, gourndDis, groundMask);
        if (isGrounded && curVelocityY.y <= 0f)
        {
            if (Input.GetButtonDown("Jump"))
            {
                curVelocityY.y = Mathf.Sqrt(-2f * gravity * jumpHeight);
            }
            else
            {
                curVelocityY.y = 0f;
            }
        }
        else
        {
            curVelocityY.y += gravity * Time.deltaTime;
        }
        
        //竖直方向移动
        cc.Move(curVelocityY * Time.deltaTime);
    }
}