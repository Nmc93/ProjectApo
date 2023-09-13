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

    /// <summary> 한번 사용된 랜덤 목록을 캐싱 </summary>
    private static Dictionary<int, UnitRandomData> dicRandomData = new Dictionary<int, UnitRandomData>();

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
            //unitObj.name = 

            unit = unitObj.GetComponent<Unit>();
        }

        //생성된 유닛을 활성화된 유닛 리스트에 세팅
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

        //해당 ID의 랜덤데이터 검색 - 없을 경우 생성 후 캐싱
        if(!dicRandomData.TryGetValue(unitRanID, out UnitRandomData ranData))
        {
            if (TableMgr.Get(unitRanID, out UnitRandomTableData tbl))
            {
                ranData = new UnitRandomData(tbl);
                dicRandomData.Add(unitRanID, ranData);
            }
            else
            {
                Debug.LogError($"[{unitRanID}]의 ID를 가진 랜덤데이터를 테이블에서 찾을 수 없습니다.");
                return null;
            }
        }

        int[] stats = ranData.GetRanStats;
        //데이터를 랜덤으로 삽입
        unitData = new UnitData(
            ranData.unitType,
            ranData.GetRanHat,
            ranData.GetRanHair,
            ranData.GetRanBackHair,
            ranData.GetRanFace,
            ranData.GetRanFaceDeco,
            ranData.GetRanBody,
            stats[0],
            stats[1],
            stats[2],
            (float)stats[3] / 100,
            (float)stats[4] / 100);

        return unitData;
    }

    /// <summary> 무작위 유닛 정보 생성 후 반환 </summary>
    /// <param name="unitType"> 생성할 유닛의 종류 </param>
    /// <returns> 유닛 데이터를 반환 </returns>
    private static UnitData CreateRandomUnitData(eUnitType unitType)
    {
        //UnitData unitData = new UnitData();

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

/// <summary> 유닛 랜덤 데이터 </summary>
public class UnitRandomData
{
    public UnitRandomData(UnitRandomTableData tbl)
    {
        if (tbl == null)
        {
            Debug.LogError("유닛랜덤테이블이 null입니다.");
            return;
        }

        id = tbl.ID;
        unitType = tbl.UnitType;

        hairs = Convert(tbl.Hair);
        backHairs = Convert(tbl.BackHair);
        faces = Convert(tbl.Face);
        faceDecos = Convert(tbl.FaceDeco);
        hats = Convert(tbl.Hat);
        bodys = Convert(tbl.Body);
        stats = Convert(tbl.Stat);
    }

    public int[] Convert(string str)
    {
        string[] strs = str.Split("/");
        int[] result = new int[strs.Length];

        int value;
        for(int i = 0; i < strs.Length; ++i)
        {
            if(!int.TryParse(strs[i],out value))
            {
                Debug.LogError("{strs[i]}를 int로 변환할 수 없어서 테이블의 수정이 필요합니다.");
                value = 0;
            }
            result[i] = value;
        }
        return result;
    }

    public int id;
    public int unitType;
    public int hairCount => hairs.Length;
    public int backHairCount => backHairs.Length;
    public int faceCount => faces.Length;
    public int faceDecoCount => faceDecos.Length;
    public int hatCount => hats.Length;
    public int bodyCount => bodys.Length;
    public int statCount => stats.Length;

    public int GetRanHair => hairs[Random.Range(0, hairs.Length)];
    public int GetRanBackHair => backHairs[Random.Range(0, backHairs.Length)];
    public int GetRanFace => faces[Random.Range(0, faces.Length)];
    public int GetRanFaceDeco => faceDecos[Random.Range(0, faceDecos.Length)];
    public int GetRanHat => hats[Random.Range(0, hats.Length)];
    public int GetRanBody => bodys[Random.Range(0, bodys.Length)];

    /// <summary> [0 : 피]<br/>[1 : 공]<br/>[2 : 방]<br/>[3 : 공속]<br/>[4 : 이속] </summary>
    public int[] GetRanStats
    {
        get
        {
            if (!TableMgr.Get(Random.Range(0, stats.Length), out UnitStatTableData tbl))
            {
                return null;
            }

            int[] statArray = new int[5];

            statArray[0] = Random.Range(tbl.MinHp, tbl.MaxHp);              //체력
            statArray[1] = Random.Range(tbl.MinDmg, tbl.MaxDmg);            //공격력
            statArray[2] = Random.Range(tbl.MinDef, tbl.MaxDef);            //방어력
            statArray[3] = Random.Range(tbl.MinAttSpeed, tbl.MaxAttSpeed);  //공격속도
            statArray[4] = Random.Range(tbl.MinMoveSpeed, tbl.MaxMoveSpeed);//이동속도

            return statArray;
        }
    }

    public int[] hairs;
    public int[] backHairs;
    public int[] faces;
    public int[] faceDecos;
    public int[] hats;
    public int[] bodys;
    public int[] stats;
}