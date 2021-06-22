using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveTypeEnum
{
    Lerp,
    sLerp,
    smoothDamp,
    normal,
    translate,
}

public class Vector3Test : MonoBehaviour
{
    public MoveTypeEnum moveType;
    public float moveTime;
    public float smoothTime = 0.3f;
    
    private Vector3 startPos;
    private Vector3 targetPos;

    private float maxSpeed = 3f;
    private float timer = 0f;
    
    private void Start()
    {
        startPos = transform.position;
        targetPos = startPos + new Vector3(7, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
    }

    void UpdatePosition()
    {
        switch (moveType)
        {
            case MoveTypeEnum.Lerp:
                LerpMove();
                break;
            case MoveTypeEnum.sLerp:
                SlerpMove();
                break;
            case MoveTypeEnum.smoothDamp:
                SmoothDampMove();
                break;
            case MoveTypeEnum.normal:
                NormalMove();
                break;
            case MoveTypeEnum.translate:
                TranslateMove();
                break;
            default:
                NormalMove();
                break;
        }
    }

    
    void LerpMove()
    {
        timer += Time.deltaTime;
        transform.position = Vector3.Lerp(startPos, targetPos, timer/moveTime);
    }

    void SlerpMove()
    {
        timer += Time.deltaTime;
        transform.position = Vector3.Slerp(startPos, targetPos, timer/moveTime);
    }

    private Vector3 tempSpeed = Vector3.zero;
    void SmoothDampMove()
    {
        //transform.position = Vector3.SmoothDamp(transform.position, targetPos,  ref tempSpeed, smoothTime);
        transform.position = Vector3.SmoothDamp(transform.position, targetPos,  ref tempSpeed, smoothTime, 10);
        //transform.position = Vector3.SmoothDamp(transform.position, targetPos,  ref tempSpeed, smoothTime, 2f, 0.3f);
    }

    void NormalMove()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, maxSpeed * Time.deltaTime);
    }

    void TranslateMove()
    {
        transform.Translate((targetPos - transform.position).normalized * (maxSpeed * Time.deltaTime));
    }
}
