/******************************************************************************
* Created by: XuHongWei
* Date: 2022-07-01 14:05:58
* Des: 1.绕一个轴移动
 * 2.绕自身轴旋转
 * 3.看向某个朝向
*******************************************************************************/

using System;
using UnityEngine;

public class RotateLearn : MonoBehaviour
{
    public Camera mainCamera;

    [Header("绕轴移动")] 
    public Transform rotateWithObj;
    [Header("绕轴移动")] 
    public float rotateDis;
    [Header("绕轴移动角速度")] 
    public float rotateAngleSpeed = 60;
    [Header("拉伸速度")] 
    public float moveSpeed = 10;
    [Header("距离目标最远距离")] 
    public float maxDisToTarget = 10;
    [Header("距离目标最近距离")] 
    public float minDisToTarget = 3;

    public void Start()
    {
        mainCamera.transform.position = rotateWithObj.transform.position + Vector3.back * 5;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            TransfromRotate();
        }
        
        //RotateMoveWithAxis();

        //UpdateCube();
    }

    public void LateUpdate()
    {
        //中键
        float middleMouseValue = Input.GetAxisRaw("Mouse ScrollWheel");
        float disToTarget = Vector3.Distance(mainCamera.transform.position, rotateWithObj.transform.position);
        Debug.Log($"距离目标{disToTarget}");
        
        //当前帧移动最大距离
        float thisFrameMoveDisMax = 0;
        Vector3 moveDir = mainCamera.transform.forward;
        //Vector3 moveDir = (rotateWithObj.transform.position - mainCamera.transform.position).normalized;
        if (middleMouseValue < 0)
        {
            thisFrameMoveDisMax = maxDisToTarget - disToTarget;
            moveDir *= -1;
        }
        else if(middleMouseValue > 0)
        {
            thisFrameMoveDisMax = disToTarget - minDisToTarget;
            moveDir *= 1;
        }
        
        float thisFrameMoveDisClamp = Mathf.Clamp(moveSpeed * Time.deltaTime, 0, thisFrameMoveDisMax);
        if (!thisFrameMoveDisClamp.Equals(0))
        {
            mainCamera.transform.Translate(moveDir * thisFrameMoveDisClamp);
        }
        
        if (Input.GetMouseButton(0))
        {
            float hAxisValue = Input.GetAxis("Mouse X");
            float vAxisValue = Input.GetAxis("Mouse Y");
            mainCamera.transform.RotateAround(rotateWithObj.transform.position, rotateWithObj.transform.up, hAxisValue * rotateAngleSpeed * Time.deltaTime);
            mainCamera.transform.RotateAround(rotateWithObj.transform.position, rotateWithObj.transform.right, vAxisValue * rotateAngleSpeed * Time.deltaTime);
            //mainCamera.transform.LookAt(rotateWithObj.transform.position);
        }
        mainCamera.transform.LookAt(rotateWithObj.transform.position);
    }

    public void RotateMoveWithAxis()
    {
        
        mainCamera.transform.RotateAround(rotateWithObj.transform.position, rotateWithObj.transform.up, 20 * Time.deltaTime);
        
        // Vector3 moveDir = Vector3.Cross()
        // mainCamera.transform.position = 
    }

    [Header("Transform.rotate 验证")]
    public Vector3 transformRotateVector3;

    public Space transformRotateSpace;
    
    [ContextMenu("Transform.rotate 验证")]
    public void TransfromRotate()
    {
        //按照ZXY的顺序，旋转对应的角度
        //mainCamera.transform.Rotate(transformRotateVector3, transformRotateSpace);
        
        //绕一个轴旋转
        mainCamera.transform.Rotate(transformRotateVector3, 10, transformRotateSpace);
    }

    public void OnDrawGizmos()
    {
        DrawRotateAxis();
    }

    private void DrawRotateAxis()
    {
        Gizmos.color = Color.yellow;

        Vector3 rotateAxis;
        if (transformRotateSpace == Space.Self)
        {
            rotateAxis = mainCamera.transform.TransformVector(transformRotateVector3);
        }
        else
        {
            rotateAxis = transformRotateVector3;
        }
        Gizmos.DrawRay(mainCamera.transform.position, rotateAxis);
    }

    #region rotate Cube
    public float smooth = 5.0f;
    public float tiltAngle = 60.0f;

    void UpdateCube()
    {
        if (Input.GetMouseButton(0))
        {
            float hAxisValue = Input.GetAxis("Mouse X");
            float tiltAroundZ = hAxisValue * tiltAngle;
            
            float VAxisValue = Input.GetAxis("Mouse Y");
            float tiltAroundX = VAxisValue * tiltAngle;
        
            // Rotate the cube by converting the angles into a quaternion.
            Quaternion target = Quaternion.Euler(tiltAroundX, 0, tiltAroundZ);

            // Dampen towards the target rotation
            rotateWithObj.transform.rotation = Quaternion.Slerp(rotateWithObj.transform.rotation, target,  Time.deltaTime * smooth);
        }
    }
    #endregion
}