using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

public class UnitMgr : MgrBase
{
    public static UnitMgr instance;


    [Header("[����Ʈ ID]")]
    /// <summary> �⺻���� ���õ� �ΰ� ID </summary>
    public static int DefaultHumanID;
    /// <summary> �⺻���� ���õ� ���� ID </summary>
    public static int DefaultZombieID;

    /// <summary> ������ ���� ��� </summary>
    public static List<Unit> unitList = new List<Unit>();

    /// <summary> ��Ȱ�� ���� ��� </summary>
    private static Queue<Unit> unitPool = new Queue<Unit>();
    /// <summary> ��Ȱ�� ���� ��� </summary>
    private static Queue<Unit> zombiePool = new Queue<Unit>();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    #region ���� ����

    ///// <summary> �⺻���� ������ ���� ���� </summary>
    //public static void CreateDefaultUnit(eUnitType unitType)
    //{
    //    //�⺻ �ΰ�, ���� Ÿ�� ID ����
    //    int unitID = unitType == eUnitType.Human ? 0 : 1;
    //
    //    CreateUnit(unitID);
    //}

    /// <summary> ���̺��� ID�� �´� ������ ���� </summary>
    public static void CreateUnit(int id)
    {
        UnitData unitData = CreateUnitData(id);
        eUnitType unitType = unitData.unitType;

        //��� �ִ� ������ ã�Ƽ� ����, ���ٸ� ����
        if (!GetDeActiveUnit(unitType,out Unit unit))
        {
            string path = unitType == eUnitType.Human ? "Char/Human" : "Char/Zombie";
            GameObject unitObj = Instantiate(AssetsMgr.LoadResourcesPrefab(path));
            unitObj.transform.SetParent(instance.transform);
        }

        //unit.Init(unitData,);

        unitList.Add(unit);
    }

    #endregion ���� ����

    #region Ǯ�� ����� ���� ��ȯ

    /// <summary> Ǯ �ȿ� ��Ȱ��ȭ �� ������ �ִٸ� �����ϰ� ���� ���θ� ��ȯ </summary>
    /// <param name="unitType"> ��ȯ�� ������ Ÿ�� </param>
    /// <param name="unit"> ã�� ������ ������ ���� ���� </param>
    /// <returns> ��Ȱ��ȭ�� ������ ���忡 �����Ѵٸ� True ��ȯ </returns>
    private static bool GetDeActiveUnit(eUnitType unitType, out Unit unit)
    {
        unit = null;

        switch (unitType)
        {
            case eUnitType.Human:
                {
                    if (unitPool.Count > 0)
                    {
                        unit = unitPool.Dequeue();
                    }
                }
                break;
            case eUnitType.Zombie:
                {
                    if (zombiePool.Count > 0)
                    {
                        unit = zombiePool.Dequeue();
                    }
                }
                break;
        }

        return unit != null;
    }
    #endregion Ǯ�� ����� ���� ��ȯ

    /// <summary> �������̺��� ID�� ������� ������ ����, ��ȯ </summary>
    /// <param name="unitRanID"> UnitRandomTable�� ID </param>
    /// <returns> �ش� ID�� ������ ���� ��� null ��ȯ </returns>
    private static UnitData CreateUnitData(int unitRanID)
    {
        UnitData unitData = null;

        if(TableMgr.Get(unitRanID, out UnitRandomTableData data))
        {
            //data.
        }



        return unitData;
    }

    /// <summary> ������ ���� ���� ���� �� ��ȯ </summary>
    /// <param name="unitType"> ������ ������ ���� </param>
    /// <returns> ���� �����͸� ��ȯ </returns>
    private static UnitData CreateRandomUnitData(eUnitType unitType)
    {
        UnitData unitData = new UnitData();

        switch(unitType)
        {
            case eUnitType.Human:
                {
                }
                break;
            case eUnitType.Zombie:
                {
                }
                break;
        }

        return null;
    }
}
