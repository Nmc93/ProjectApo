using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

public class UnitMgr : MgrBase
{
    public static UnitMgr instance;

    /// <summary> ������ ���� ��� </summary>
    public static List<Unit> unitList;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    /// <summary> ���� ���� </summary>
    public static void CreateUnit(eUnitType unitType)
    {

    }
}
