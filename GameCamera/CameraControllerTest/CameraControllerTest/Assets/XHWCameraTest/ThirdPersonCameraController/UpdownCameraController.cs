/******************************************************************************
* Created by: XuHongWei
* Date: 2022-07-11 16:55:20
* Des: https://www.youtube.com/watch?v=rnqF6S7PfFA
*******************************************************************************/

using System;
using UnityEngine;

public class UpdownCameraController : MonoBehaviour
{
    public Transform followTrans;
    public Camera topDownCamera;
    public Transform cameraTransform;
    
    public float moveSpeed;
    public float smoothTime;
    private Vector3 moveTargetPos;

    public Quaternion rotateTargetQuaternion;
    public float rotationAmount;

    public Vector3 zoomTarget;
    public Vector3 zoomAmount = new Vector3(0,5,-5);
    
    
    public void Start()
    {
        moveTargetPos = transform.position;
        rotateTargetQuaternion = transform.rotation;
        zoomTarget = cameraTransform.localPosition;
    }

    public void LateUpdate()
    {
        if (followTrans != null)
        {
            transform.position = followTrans.position + cameraTransform.localPosition;
        }
        else
        {
            HandlerMouseInput();
            HandlerKeyBoardInput();
        }
    }

    private void HandlerKeyBoardInput()
    {
        //移动
        float hAxis = Input.GetAxis("Horizontal");
        float vAxiss = Input.GetAxis("Vertical");

        Vector3 moveDir = (transform.forward * vAxiss + transform.right * hAxis).normalized;
        moveTargetPos += moveDir * moveSpeed * Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, moveTargetPos, smoothTime * Time.deltaTime);

        //旋转
        if (Input.GetKeyDown(KeyCode.Q))
        {
            rotateTargetQuaternion = Quaternion.Euler(Vector3.up * rotationAmount);
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            rotateTargetQuaternion = Quaternion.Euler(Vector3.up * -rotationAmount);
        }
        
        transform.rotation = Quaternion.Lerp(transform.rotation, rotateTargetQuaternion, Time.deltaTime * smoothTime);
        
        //缩放
        if (Input.GetKeyDown(KeyCode.R))
        {
            zoomTarget += zoomAmount;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            zoomTarget -= zoomAmount;
        }

        cameraTransform.localPosition =
            Vector3.Lerp(cameraTransform.localPosition, zoomTarget, Time.deltaTime * smoothTime);
    }
    
    public Vector3 dragStartPosition;
    public Vector3 drayCurPosition;

    public Vector3 rotateStartPos;
    public Vector3 rotateCurPos;
    
    public void HandlerMouseInput()
    {
        //移动
        if (Input.GetMouseButtonDown(0))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = topDownCamera.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out float entry))
            {
                dragStartPosition = ray.GetPoint(entry);
            }
        }
        
        if (Input.GetMouseButton(0))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = topDownCamera.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out float entry))
            {
                drayCurPosition = ray.GetPoint(entry);

                moveTargetPos = transform.position + dragStartPosition - drayCurPosition;
            }
        }

        //缩放
        if (Input.mouseScrollDelta.y != 0)
        {
            zoomTarget += Input.mouseScrollDelta.y * zoomAmount;
        }
        
        //旋转
        if (Input.GetMouseButtonDown(2))
        {
            rotateStartPos = Input.mousePosition;
        }

        if (Input.GetMouseButton(2))
        {
            rotateCurPos = Input.mousePosition;

            Vector3 diff = rotateStartPos - rotateCurPos;
            rotateStartPos = rotateCurPos;
            rotateTargetQuaternion *= Quaternion.Euler(Vector3.up * (-diff.x / 5f));
        }
    }
}