using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

public class UnitMgr : MgrBase
{
    public static UnitMgr instance;

    #region 인스펙터

    [Header("[디폴트 ID]")]
    /// <summary> 기본으로 세팅될 인간 ID </summary>
    public static int DefaultHumanID;
    /// <summary> 기본으로 세팅될 좀비 ID </summary>
    public static int DefaultZombieID;

    [Header("[활성화된 유닛 목록]"), Tooltip("활성화된 유닛 목록")]
    /// <summary> 생성된 유닛 목록 </summary>
    public static Dictionary<int, Unit> activeUnits = new Dictionary<int, Unit>();

    [Header("[비활성화된 유닛 목록]"),Tooltip("비활성화된 유닛 목록")]
    public static Queue<Unit> unitPool = new Queue<Unit>();

    #endregion 인스펙터

    #region 변수

    /// <summary> 한번 사용된 랜덤 목록을 캐싱 </summary>
    private static Dictionary<int, UnitRandomData> dicRandomData = new Dictionary<int, UnitRandomData>();

    /// <summary> 다음 만들어질 캐릭터의 UID </summary>
    private static int NextCharUID;

    /// <summary> 사용 대기중인 유닛 이벤트 풀 </summary>
    private static Queue<UnitEventData> unitEventPool = new Queue<UnitEventData>(50);

    #endregion 변수

    #region 오버라이드, 기본 세팅

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    #endregion 오버라이드, 기본 세팅

    #region 유닛 업데이트

    /// <summary> 캐릭터의 업데이트문 이벤트 </summary>
    public static List<System.Action> charUpdateList = new List<System.Action>();
    private void Update()
    {
        //커스텀 업데이트
        foreach(var item in charUpdateList)
        {
            item();
        }
    }

    /// <summary> 유닛의 업데이트 이벤트를 등록 </summary>
    /// <param name="updateAction"> 업데이트 함수 </param>
    public static void AddUpdateEvent(System.Action updateAction)
    {
        if(!charUpdateList.Contains(updateAction))
        {
            charUpdateList.Add(updateAction);
        }
    }

    /// <summary> 유닛의 업데이트 이벤트를 해제 </summary>
    /// <param name="updateAction"> 업데이트 함수 </param>
    public static void RemoveUpdateEvent(System.Action updateAction)
    {
        if(charUpdateList.Contains(updateAction))
        {
            charUpdateList.Remove(updateAction);
        }
    }

    #endregion 유닛 업데이트

    #region 유닛 생성

    /// <summary> 테이블의 ID에 맞는 유닛을 생성 </summary>
    /// <param name="id"> UnitRandomTableData 테이블 ID </param>
    /// <param name="weaponID"> 무기 ID 없을 경우 맨손 </param>
    public static void CreateUnit(Vector3 pos, int id,int weaponID = 0)
    {
        UnitData unitData = CreateUnitData(id, weaponID);
        eUnitType unitType = unitData.unitType;

        //유닛 호출, 생성 - 놀고 있는 유닛을 찾아서 세팅, 없다면 생성
        if (!GetDeActiveUnit(out Unit unit))
        {
            // 유닛 생성
            GameObject unitObj = AssetsMgr.LoadResourcesPrefab("Char/Human");
            unitObj.transform.SetParent(instance.transform);
            unit = unitObj.GetComponent<Unit>();
        }

        //유닛 UID 세팅 - 중복 검사 및 세팅
        while (true)
        {
            //없는 번호일 경우에 사용
            if (activeUnits.ContainsKey(NextCharUID) == false)
            {
                unit.gameObject.name = NextCharUID.ToString();
                unit.UID = NextCharUID++;
                break;
            }
            else
            {
                ++NextCharUID;
            }
        }

        //유닛 정보 세팅
        unit.Init(unitData);

        //생성 지점 세팅
        unit.transform.position = pos;

        //생성된 유닛을 활성화된 유닛 리스트에 세팅
        activeUnits.Add(unit.UID, unit);
    }

    /// <summary> 유닛데이터만 생성해서 반환 </summary>
    public static UnitData CreateUnitDate(int id, int weaponID = 0)
    {
       return CreateUnitData(id, weaponID);
    }

    #endregion 유닛 생성

    #region 풀에 저장된 유닛 반환

    /// <summary> 풀 안에 비활성화 된 유닛이 있다면 저장하고 성공 여부를 반환 </summary>
    /// <param name="unit"> 찾은 유닛을 저장할 유닛 변수 </param>
    /// <returns> 비활성화된 유닛을 저장에 성공한다면 True 반환 </returns>
    private static bool GetDeActiveUnit(out Unit unit)
    {
        unit = null;

        if (unitPool.Count > 0)
        {
            unit = unitPool.Dequeue();
        }

        return unit != null;
    }
    #endregion 풀에 저장된 유닛 반환

    #region 유닛 데이터 생성

    /// <summary> 랜덤테이블의 ID를 기반으로 유닛을 생성, 반환 </summary>
    /// <param name="unitRanID"> UnitRandomTable의 ID </param>
    /// <returns> 해당 ID의 유닛이 없을 경우 null 반환 </returns>
    private static UnitData CreateUnitData(int unitRanID, int weaponID = 0)
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
            ranData.GetRanHeads,
            ranData.GetRanHat,
            ranData.GetRanHair,
            ranData.GetRanBackHair,
            ranData.GetRanHeadAnim,
            ranData.GetRanFaceDeco,
            ranData.GetRanBodyAnim,
            stats[0],               //피
            stats[1],               //공
            stats[2],               //방
            (float)stats[3] / 100,  //공속
            (float)stats[4] / 100,  //이속
            (float)stats[5] / 100,  //반응속도
            stats[6],               //탐지범위
            weaponID);              //무기

        return unitData;
    }

    #endregion 유닛 데이터 생성

    #region 유닛 이벤트 풀

    /// <summary> 사용 완료한 이벤트 데이터를 반환 </summary>
    /// <param name="eventData"> 사용 완료한 이벤트 데이터 </param>
    public static void UnitEventReturn(UnitEventData eventData)
    {
        eventData.DataReset();
        unitEventPool.Enqueue(eventData);
    }

    /// <summary> 빈 유닛 이벤트를 획득 </summary>
    /// <returns> 풀에 없을 경우 새로 만들어서 반환 </returns>
    public static UnitEventData GetUnitEvent()
    {
        if(unitEventPool.Count <= 0)
        {
            return new UnitEventData();
        }

        return unitEventPool.Dequeue();
    }

    #endregion 유닛 이벤트 풀

    #region Get

    /// <summary> 해당 TID를 가진 유닛의 타입을 반환 </summary>
    /// <param name="uid"> 유닛의 TID </param>
    /// <returns> TID가 없을 경우 eUnitType.None 반환 </returns>
    public static eUnitType GetUnitType(int uid)
    {
        if(activeUnits.TryGetValue(uid, out var unit))
        {
            return unit.data.unitType;
        }

        return eUnitType.None;
    }

    #endregion Get

    #region 유닛 상호작용

    /// <summary> A 유닛이 B유닛을 공격 </summary>
    public void AttackUnit(int targetUID, int damage)
    {
        //TODO: 유닛 공격 처리 작업 필요

        //유닛이 실존한다면 데미지 넣음
        if (activeUnits.TryGetValue(targetUID, out var unit))
        {
            unit.CurHP -= damage;
        }
    }

    #endregion 유닛 상호작용
}

#region 데이터 클래스

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

        heads = Convert(tbl.Head);
        hairs = Convert(tbl.Hair);
        backHairs = Convert(tbl.BackHair);
        faceDecos = Convert(tbl.FaceDeco);
        hats = Convert(tbl.Hat);
        headAnims = Convert(tbl.HeadAnim);
        bodyAnims = Convert(tbl.BodyAnim);
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
                Debug.LogError($"{strs[i]}를 int로 변환할 수 없어서 테이블의 수정이 필요합니다.");
                value = 0;
            }
            result[i] = value;
        }
        return result;
    }

    public int id;
    public int unitType;

    public int[] heads;
    public int[] hairs;
    public int[] backHairs;
    public int[] faceDecos;
    public int[] hats;
    public int[] headAnims;
    public int[] bodyAnims;
    public int[] stats;

    public int GetRanHeads => heads[Random.Range(0, heads.Length)];
    public int GetRanHair => hairs[Random.Range(0, hairs.Length)];
    public int GetRanBackHair => backHairs[Random.Range(0, backHairs.Length)];
    public int GetRanFaceDeco => faceDecos[Random.Range(0, faceDecos.Length)];
    public int GetRanHat => hats[Random.Range(0, hats.Length)];
    public int GetRanHeadAnim => headAnims[Random.Range(0, headAnims.Length)];
    public int GetRanBodyAnim => bodyAnims[Random.Range(0, bodyAnims.Length)];

    /// <summary> [0 : 체력]<br/>[1 : 공격력]<br/>[2 : 방어력]
    /// <br/>[3 : 공격속도]<br/>[4 : 이동속도]<br/>[5 : 반응속도]
    /// <br/>[6 : 탐색범위] </summary>
    public int[] GetRanStats
    {
        get
        {
            if (!TableMgr.Get(Random.Range(0, stats.Length), out UnitStatTableData tbl))
            {
                return null;
            }

            int[] statArray = new int[7];

            statArray[0] = Random.Range(tbl.MinHp, tbl.MaxHp);                          // 체력
            statArray[1] = Random.Range(tbl.MinDmg, tbl.MaxDmg);                        // 공격력
            statArray[2] = Random.Range(tbl.MinDef, tbl.MaxDef);                        // 방어력
            statArray[3] = Random.Range(tbl.MinAttSpeed, tbl.MaxAttSpeed);              // 공격속도
            statArray[4] = Random.Range(tbl.MinMoveSpeed, tbl.MaxMoveSpeed);            // 이동속도
            statArray[5] = Random.Range(tbl.MinxReactionSpeed, tbl.MaxReactionSpeed);   // 반응속도
            statArray[6] = Random.Range(tbl.MinDetectionRange, tbl.MaxDetectionRange);  // 탐지범위

            return statArray;
        }
    }
}

#endregion 데이터 클래스