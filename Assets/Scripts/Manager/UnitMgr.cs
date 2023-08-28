using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

public class UnitMgr : MgrBase
{
    public static UnitMgr instance;


    [Header("[디폴트 ID]")]
    /// <summary> 기본으로 세팅될 인간 ID </summary>
    public static int DefaultHumanID;
    /// <summary> 기본으로 세팅될 좀비 ID </summary>
    public static int DefaultZombieID;

    /// <summary> 생성된 유닛 목록 </summary>
    public static List<Unit> unitList = new List<Unit>();

    /// <summary> 비활성 유닛 목록 </summary>
    private static Queue<Unit> unitPool = new Queue<Unit>();
    /// <summary> 비활성 좀비 목록 </summary>
    private static Queue<Unit> zombiePool = new Queue<Unit>();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    #region 유닛 생성

    ///// <summary> 기본으로 지정된 유닛 생성 </summary>
    //public static void CreateDefaultUnit(eUnitType unitType)
    //{
    //    //기본 인간, 좀비 타입 ID 세팅
    //    int unitID = unitType == eUnitType.Human ? 0 : 1;
    //
    //    CreateUnit(unitID);
    //}

    /// <summary> 테이블의 ID에 맞는 유닛을 생성 </summary>
    public static void CreateUnit(int id)
    {
        UnitData unitData = CreateUnitData(id);
        eUnitType unitType = unitData.unitType;

        //놀고 있는 유닛을 찾아서 세팅, 없다면 생성
        if (!GetDeActiveUnit(unitType,out Unit unit))
        {
            string path = unitType == eUnitType.Human ? "Char/Human" : "Char/Zombie";
            GameObject unitObj = Instantiate(AssetsMgr.LoadResourcesPrefab(path));
            unitObj.transform.SetParent(instance.transform);
        }

        //unit.Init(unitData,);

        unitList.Add(unit);
    }

    #endregion 유닛 생성

    #region 풀에 저장된 유닛 반환

    /// <summary> 풀 안에 비활성화 된 유닛이 있다면 저장하고 성공 여부를 반환 </summary>
    /// <param name="unitType"> 반환할 유닛의 타입 </param>
    /// <param name="unit"> 찾은 유닛을 저장할 유닛 변수 </param>
    /// <returns> 비활성화된 유닛을 저장에 성공한다면 True 반환 </returns>
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
    #endregion 풀에 저장된 유닛 반환

    /// <summary> 랜덤테이블의 ID를 기반으로 유닛을 생성, 반환 </summary>
    /// <param name="unitRanID"> UnitRandomTable의 ID </param>
    /// <returns> 해당 ID의 유닛이 없을 경우 null 반환 </returns>
    private static UnitData CreateUnitData(int unitRanID)
    {
        UnitData unitData = null;

        if(TableMgr.Get(unitRanID, out UnitRandomTableData data))
        {
            //data.
        }



        return unitData;
    }

    /// <summary> 무작위 유닛 정보 생성 후 반환 </summary>
    /// <param name="unitType"> 생성할 유닛의 종류 </param>
    /// <returns> 유닛 데이터를 반환 </returns>
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
