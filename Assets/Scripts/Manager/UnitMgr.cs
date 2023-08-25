using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

public class UnitMgr : MgrBase
{
    public static UnitMgr instance;

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

    /// <summary> 유닛 생성 </summary>
    public static void CreateUnit(eUnitType unitType, UnitData unitData = null)
    {
        //놀고 있는 유닛을 찾아서 세팅, 없다면 생성
        if(!GetDeActiveUnit(unitType,out Unit unit))
        {
            string path = unitType == eUnitType.Human ? "Char/Human" : "Char/Zombie";
            GameObject unitObj = Instantiate(AssetsMgr.LoadResourcesPrefab(path));
            unitObj.transform.SetParent(instance.transform);
        }

        //유닛의 데이터 세팅
        if (unitData == null)
        {
            unitData = CreateRandomUnitData(unitType);
        }

        //unit.Init(unitData,);

        unitList.Add(unit);
    }

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
