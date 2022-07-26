/******************************************************************************
* Created by: XuHongWei
* Date: 2022-07-26 11:59:33
* Des: 
*******************************************************************************/

using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "预加载资源测试", menuName = "Preload/CreatBattlePreloadMap")]
[InlineEditor()]
public class PreloadObj: ScriptableObject
{
    [BoxGroup("BaseInfo")]
    public string resPath;
    [BoxGroup("BaseInfo")]
    public int id;

    [AssetList]
    [PreviewField(60)]
    public GameObject res;
}