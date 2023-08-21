using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

public class UnitMgr : MgrBase
{
    public static UnitMgr instance;

    /// <summary> 积己等 蜡粗 格废 </summary>
    public static List<Unit> unitList;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    /// <summary> 蜡粗 积己 </summary>
    public static void CreateUnit(eUnitType unitType)
    {

    }
}
