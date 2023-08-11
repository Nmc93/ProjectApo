using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMgr : MgrBase
{
    public static MapMgr instance;

    /// <summary> ���� �� </summary>
    private MainMap map;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    //�� ����
    public void CreateMap()
    {
        //�� ����
        GameObject mapObj = new GameObject();
        map = mapObj.GetComponent<MainMap>();

        map.FirstRandomSetting();
    }
}
