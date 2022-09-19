using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector3AngleTest : MonoBehaviour
{
    public Transform self;
    public Transform target;
    public Transform camera;

    public Transform fixCamera;
    
    public Vector3 cameraFixPos;
    public Vector3 cameraFixDir;

    public Quaternion cameraFixRotate;
    
    public float cameraDis = 5f;
    public float rotateAngle = 45;

    [Range(0, 1)]
    public float precent = 0.5f;
    public float cameraFixYOffset;
    
    [ContextMenu("测试SignedAngle")]
    public void TestAngle()
    {
        //根据当前主角和目标向量，以及当前镜头朝向，决定修正朝向
        Vector3 selfToTargetDir = target.position - self.position;
        selfToTargetDir.y = 0;

        Vector3 selfToCameraDir = camera.position - self.position;
        selfToCameraDir.y = 0;

        float angle = Vector3.SignedAngle(selfToTargetDir, selfToCameraDir, Vector3.up);
        
        Debug.Log("角度 = " + angle);
    }
    
    public static Vector3 RotateRound(Vector3 position, Vector3 center, Vector3 axis, float angle)
    {
        //TODO angleaxis 中的angle 有符号，顺时针为正，满足左手定理
        Vector3 point = Quaternion.AngleAxis(angle, axis) * (position - center);
        Vector3 resultVec3 = center + point;
        return resultVec3;
    }
    
    [ContextMenu("计算修正数据")]
    public void CalculateFixData()
    {
        //根据当前主角和目标向量，以及当前镜头朝向，决定修正朝向
        Vector3 selfToTargetDir = target.position - self.position;
        selfToTargetDir.y = 0;

        Vector3 selfToCameraDir = camera.position - self.position;
        selfToCameraDir.y = 0;

        //TODO 已解决还需要整理的问题1： 需要进行调整的时候，调整位置如何计算，需要转到的点在哪？
        //TODO 待解决问题： 何时需要进行战斗镜头调整，调整执行哪些逻辑 1. 位置和朝向都改 2. 只改朝向，不改位置
        
        //主角和目标的向量，与主角和镜头的向量，夹角正负，决定最终的镜头朝向向量
        float angle = Vector3.SignedAngle(selfToTargetDir, selfToCameraDir, Vector3.up);

        Vector3 battleFixCameraForward = selfToTargetDir;
        
        //angle > 0 selfToTargetDir逆时针转
        if (angle > 0)
        {
            cameraFixDir = Quaternion.AngleAxis(-rotateAngle, Vector3.up) * selfToTargetDir;
            //battleFixCameraForward = Vector3.RotateTowards();
        }
        //selfToTargetDir顺时针转
        else
        {
            cameraFixDir = Quaternion.AngleAxis(rotateAngle, Vector3.up) * selfToTargetDir;
        }
        
        cameraFixRotate = Quaternion.LookRotation(cameraFixDir);
        
        //TODO 待解决问题1： 如何观察主角与目标中心点，如何计算//TODO cinemachine targetgrouop
        //选取世界坐标中心点，几何意义不清楚，需要了解
        cameraFixPos = cameraFixRotate * new Vector3(0, 0, -cameraDis) + GetWorldCenterPos(self.position, target.position, precent, cameraFixYOffset);
        fixCamera.position = cameraFixPos;
        fixCamera.rotation = cameraFixRotate;
    }
    
    //TODO 待解决问题2如何回正，从战斗修正状态，转变回 锁定主角状态
    //1.位置不变，朝向变？？（YES）
    [ContextMenu("旋转回正")]
    public void RotateBack()
    {
        fixCamera.rotation = Quaternion.LookRotation(self.position - Camera.main.transform.position);
    }
    
    //2.朝向不变，位置变？？（NO）
    [ContextMenu("平移回正")]
    public void MoveBackNormal()
    {
        fixCamera.position = self.position + cameraFixRotate.normalized * new Vector3(0, 0, -cameraDis);
    }
    
    private Vector3 GetWorldCenterPos(Vector3 selfPos, Vector3 focusTargetPos,float centerOffsetPrecent, float heightOffset)
    {
        Vector3 forward = (focusTargetPos - selfPos).normalized;
        Vector3 viewCenterPos = selfPos + Vector3.Distance(selfPos, focusTargetPos) * centerOffsetPrecent * forward;
        return viewCenterPos + new Vector3(0, heightOffset, 0);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //主角到目标向量
        Gizmos.DrawLine(self.position, target.position);
        //镜头环绕圈
        Gizmos.DrawWireSphere(self.position, cameraDis);

        Gizmos.color = Color.green;
        //主角到镜头向量
        Gizmos.DrawLine(self.position, camera.position);
        
        Gizmos.color = Color.blue;
        //镜头新向量
        Gizmos.DrawRay(cameraFixPos, cameraFixDir);
    }
}
