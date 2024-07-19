using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

/// <summary> 유닛의 데이터 </summary>
[Serializable]
public class UnitData
{
    public UnitData(UnitRandomData ranData, int weaponID = 0)
    {
        //유닛의 타입 세팅
        switch (ranData.unitType)
        {
            case 0: unitType = eUnitType.None; break;
            case 1: unitType = eUnitType.Human; break;
            case 2: unitType = eUnitType.Zombie; break;
        }

        //외형 세팅
        headID = ranData.GetRanHeads;
        hatID = ranData.GetRanHat;
        hairID = ranData.GetRanHair;
        backHairID = ranData.GetRanBackHair;
        headAnimID = ranData.GetRanHeadAnim;
        faceDecoID = ranData.GetRanFaceDeco;
        bodyAnimID = ranData.GetRanBodyAnim;

        //베이스 스탯 세팅
        int[] stats = ranData.GetRanStats;
        tbl_MaxHp = stats[0];               //피
        tbl_Attack = stats[1];              //공
        tbl_Defence = stats[2];             //방
        tbl_ASpeed = (float)stats[3] / 100; //공속
        tbl_MSpeed = (float)stats[4] / 100; //이속
        tbl_RSpeed = (float)stats[5] / 100; //반응속도
        tbl_DetectionRange = stats[6];      //탐지범위

        //무기 세팅
        weaponTbl = TableMgr.Get<UnitWeaponTableData>(weaponID);

        //스탯 갱신
        RefreshStat(true);
    }

    /// <summary> 무기 데이터 세팅 </summary>
    public void SetWeaponData(int weaponID)
    {
        //무기 슬롯이 비어있거나 교체 무기가 현재와 다를 경우 변경
        if (weaponTbl != null || weaponTbl.ID != weaponID)
        {
            //정보 변경 및 무기 변경으로 인한 스탯 갱신
            weaponTbl = TableMgr.Get<UnitWeaponTableData>(weaponID);
            RefreshStat();
        }
    }

    /// <summary> 유닛 타입 </summary>
    public eUnitType unitType;

    /// <summary> 무기의 테이블 정보 </summary>
    public UnitWeaponTableData weaponTbl;
    
    /// <summary> 유닛 이름 </summary>
    public string name;

    #region 외형 정보
    /// <summary> 머리 타입 </summary>
    public int headID;
    /// <summary> 모자 타입 </summary>
    public int hatID;
    /// <summary> 머리카락 타입 </summary>
    public int hairID;
    /// <summary> 뒷머리 타입 </summary>
    public int backHairID;
    /// <summary> 얼굴 타입 </summary>
    public int headAnimID;
    /// <summary> 얼굴 장식 타입(콧수염 등) </summary>
    public int faceDecoID;
    /// <summary> 몸 타입 </summary>
    public int bodyAnimID;
    #endregion 외형 정보 

    #region 스텟 정보

    #region 기본 스탯
    /// <summary> 기본 - 최대 체력 </summary>
    public int tbl_MaxHp;

    /// <summary> 기본 - 공격력 </summary>
    public int tbl_Attack;
    /// <summary> 기본 - 방어력 </summary>
    public int tbl_Defence;

    /// <summary> 기본 - 공격속도 </summary>
    public float tbl_ASpeed;
    /// <summary> 기본 - 이동속도 </summary>
    public float tbl_MSpeed;
    /// <summary> 기본 - 반응속도(초) </summary>
    public float tbl_RSpeed;

    /// <summary> 유닛의 탐색범위 </summary>
    public int tbl_DetectionRange;

    #endregion 기본 스탯

    #region 최종 스탯

    /// <summary> 최종 최대 체력 </summary>
    public int f_MaxHp;
    /// <summary> 최종 체력 </summary>
    public int f_CurHp;
    /// <summary> 최종 공격력 </summary>
    public int f_Damage;
    /// <summary> 최종 방어력 </summary>
    public int f_Defence;

    /// <summary> 현재 공격속도 </summary>
    public float f_ASpeed;
    /// <summary> 현재 이동속도 </summary>
    public float f_MSpeed;
    /// <summary> 최종 반응 속도 </summary>
    public float f_RSpeed;

    /// <summary> 탐색 범위 </summary>
    public int f_DetectionRange;

    #endregion 최종 스탯

    #endregion 스텟 정보

    /// <summary> 현재 기본 스탯을 기반으로 스탯 계산 </summary>
    public void RefreshStat(bool isCreate = false)
    {
        // 최대 체력
        f_MaxHp = tbl_MaxHp;

        // 공격력
        f_Damage = GetF_Damage();
        // 방어력
        f_Defence = tbl_Defence;

        // 반응속도
        f_RSpeed = tbl_RSpeed;
        // 공격속도
        f_ASpeed = GetF_ASpeed();
        // 이동속도
        f_MSpeed = tbl_MSpeed;

        // 장비나 특성에 따라 비율 조절 추가
        f_DetectionRange = tbl_DetectionRange;

        // 첫 생성이거나 현재 체력이 최대 체력보다 많은 경우
        if(isCreate || f_CurHp > f_MaxHp)
        {
            //현재 체력 == 최대 체력
            f_CurHp = f_MaxHp;
        }
    }

    #region 공식

    /// <summary> 현재 공격력을 계산 후 반환 </summary>
    private int GetF_Damage()
    {
        int fDamage = tbl_Attack;

        if (weaponTbl.WeaponType != 0)
        {
            int limit = weaponTbl.Damage / 20;
            int plusDamage = tbl_Attack > limit ? limit : tbl_Attack;
            f_Damage = weaponTbl.Damage + plusDamage;
        }

        return fDamage < 0 ? 0 : fDamage;
    }

    /// <summary> 현재 공격속도를 계산 후 반환 </summary>
    private float GetF_ASpeed()
    {
        float speed = 0;

        //무기를 들었을 경우
        if (weaponTbl.WeaponType != 0)
        {
            speed = f_RSpeed + (weaponTbl.Speed / 100);
            speed -= (speed * tbl_ASpeed);
        }
        //들지 않았을 경우
        else
        {
            speed = f_RSpeed - (f_RSpeed * tbl_ASpeed);
        }

        return speed < 0 ? 0 : speed;
    }

    #endregion 공식
}

/// <summary> 명령,외.내부에서 들어온 이벤트 데이터 </summary>
public class UnitEventData
{
    /// <summary> 우선순위 </summary>
    public eUnitEventPriority priority;
    /// <summary> 현재 유닛에게 들어온 상황 타입 </summary>
    public eUnitSituation eventType;
    /// <summary> 대기 시간 </summary>
    public float waitTime;

    /// <summary> 이벤트 데이터 세팅 </summary>
    /// <param name="priority"> 우선순위 </param>
    /// <param name="eventType"> 이벤트 타입 </param>
    /// <param name="waitTime"> 시작 전 대기 시간 </param>
    public void SetData(eUnitEventPriority priority, eUnitSituation eventType, float waitTime)
    {
        this.priority = priority;
        this.eventType = eventType;
        this.waitTime = waitTime;
    }

    /// <summary> 데이터 리셋 </summary>
    public void DataReset()
    {
        priority = eUnitEventPriority.WaitState;
        eventType = eUnitSituation.None;
        waitTime = 0;
    }
}