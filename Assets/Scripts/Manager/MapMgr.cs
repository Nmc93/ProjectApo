using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMgr : MgrBase
{
    public static MapMgr instance;

    /// <summary> °ÔÀÓ ¸Ê </summary>
    private MainMap map;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    //¸Ê »ý¼º
    public void CreateMap()
    {
        //¸Ê ¼¼ÆÃ
        GameObject mapObj = new GameObject();
        map = mapObj.GetComponent<MainMap>();

        map.FirstRandomSetting();
    }
}
