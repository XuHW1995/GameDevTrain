/******************************************************************************
* Created by: XuHongWei
* Date: 2022-07-26 14:58:53
* Des: 
*******************************************************************************/

using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class TestMono1: SerializedMonoBehaviour
{
    public int x;
    //
    // [NonSerialized, OdinSerialize]
    [ShowInInspector]
    public TestMono2 Mono2;
}