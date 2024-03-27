using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

/// <summary> 유닛의 데이터 </summary>
[Serializable]
public class UnitData
{
    public UnitData(
        int unitType,
        int headID,
        int hatID,
        int hairID,
        int backHairID,
        int headAnimID,
        int faceDecoID,
        int bodyAnimID,
        int maxHp,
        int attack,
        int defence,
        float attackSpeed,
        float moveSpeed,
        float reActionSpeed,
        int detectionRange,
        int weaponID = 0)
    {
        //유닛의 타입 세팅
        switch(unitType)
        {
            case 0: this.unitType = eUnitType.None; break;
            case 1: this.unitType = eUnitType.Human; break;
            case 2: this.unitType = eUnitType.Zombie; break;
        }

        //외형 세팅
        this.headID = headID;
        this.hatID = hatID;
        this.hairID = hairID;
        this.backHairID = backHairID;
        this.headAnimID = headAnimID;
        this.faceDecoID = faceDecoID;
        this.bodyAnimID = bodyAnimID;
        
        //베이스 스탯 세팅
        tbl_MaxHp = maxHp;
        tbl_Attack = attack;
        tbl_Defence = defence;
        tbl_ASpeed = attackSpeed;
        tbl_MSpeed = moveSpeed;
        tbl_RSpeed = reActionSpeed;
        tbl_DetectionRange = detectionRange;

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
        f_MaxHp = tbl_MaxHp;          // 최대 체력

        f_Damage = tbl_Attack;                  // + 총기 데미지
        f_Defence = tbl_Defence;                // + 추가적인 무언가

        f_ASpeed = tbl_ASpeed;                  // 장비 특성에 따라 비율 조절 추가
        f_MSpeed = tbl_MSpeed;                  // 장비 특성에 따라 비율 조절 추가
        f_RSpeed = tbl_RSpeed;                  // 장비 특성에 따라 비율 조절 추가

        f_DetectionRange = tbl_DetectionRange; // 장비나 특성에 따라 비율 조절 추가

        // 첫 생성의 경우 현재 체력 == 최대 체력
        if(isCreate)
        {
            f_CurHp = f_MaxHp;
        }
    }
}