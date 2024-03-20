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
        int searchSize,
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
        this.maxHp = maxHp;
        this.attack = attack;
        this.defence = defence;
        this.attackSpeed = attackSpeed;
        this.moveSpeed = moveSpeed;
        this.searchSize = searchSize;

        //무기 세팅
        weaponTbl = TableMgr.Get<UnitWeaponTableData>(weaponID);

        //스탯 갱신
        RefreshStat();
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
    /// <summary> 유닛 체력 </summary>
    public int maxHp;

    /// <summary> 유닛 공격력 </summary>
    public int attack;
    /// <summary> 유닛 방어력 </summary>
    public int defence;

    /// <summary> 유닛의 공격속도 </summary>
    public float attackSpeed;
    /// <summary> 유닛의 이동속도 </summary>
    public float moveSpeed;
    /// <summary> 유닛의 탐색범위 </summary>
    public int searchSize;

    /// <summary> 반응속도 - 초 </summary>
    public float reactionSpeed;

    #endregion 기본 스탯

    #region 적용 스탯

    /// <summary> 유닛 현재 체력 </summary>
    public int curMaxHp;
    /// <summary> 유닛 현재 체력 </summary>
    public int curHp;
    /// <summary> 현재 공격력 </summary>
    public int dmg;
    /// <summary> 현재 방어력 </summary>
    public int def;

    /// <summary> 현재 공격속도 </summary>
    public float aSpeed;
    /// <summary> 현재 이동속도 </summary>
    public float mSpeed;

    /// <summary> 탐색 범위 </summary>
    public int sSize;

    #endregion 적용 스탯

    #endregion 스텟 정보

    /// <summary> 현재 기본 스탯을 기반으로 스탯 계산 </summary>
    public void RefreshStat()
    {
        //체력 세팅
        curHp = curMaxHp = maxHp;

        dmg = attack;// + 총기 데미지
        def = defence;// + 추가적인 무언가

        aSpeed = attackSpeed; // 장비 특성에 따라 비율 조절 추가
        mSpeed = moveSpeed; // 장비 특성에 따라 비율 조절 추가

        sSize = searchSize; // 장비나 특성에 따라 비율 조절 추가
    }
}