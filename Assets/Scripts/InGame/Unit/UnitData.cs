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
        int hatType,
        int hairType,
        int backHairType,
        int faceType,
        int faceDecoType,
        int bodyType,
        int maxHp,
        int attack,
        int defence,
        float attackSpeed,
        float moveSpeed)
    {
        switch(unitType)
        {
            case 0: this.unitType = eUnitType.None; break;
            case 1: this.unitType = eUnitType.Human; break;
            case 2: this.unitType = eUnitType.Zombie; break;
        }

        this.hatType = hatType;
        this.hairType = hairType;
        this.hairType = hairType;
        this.backHairType = backHairType;
        this.faceType = faceType;
        this.faceDecoType = faceDecoType;
        this.bodyType = bodyType;
        this.maxHp = maxHp;
        this.attack = attack;
        this.defence = defence;
        this.attackSpeed = attackSpeed;
        this.moveSpeed = moveSpeed;
    }

    /// <summary> 유닛 타입 </summary>
    public eUnitType unitType;

    /// <summary> 무기 타입 </summary>
    public eWeaponType weaponType;
    /// <summary> 무기 ID </summary>
    public int weaponID;
    
    /// <summary> 유닛 이름 </summary>
    public string name;
    
    #region 외형 정보
    /// <summary> 모자 타입 </summary>
    public int hatType;
    /// <summary> 머리카락 타입 </summary>
    public int hairType;
    /// <summary> 뒷머리 타입 </summary>
    public int backHairType;
    /// <summary> 얼굴 타입 </summary>
    public int faceType;
    /// <summary> 얼굴 장식 타입(콧수염 등) </summary>
    public int faceDecoType;
    /// <summary> 몸 타입 </summary>
    public int bodyType;
    #endregion 외형 정보 

    #region 스텟 정보
    /// <summary> 유닛 최대 체력 </summary>
    public int maxHp;
    /// <summary> 유닛 현재 체력 </summary>
    public int curHp;

    /// <summary> 유닛 공격력 </summary>
    public int attack;
    /// <summary> 유닛 방어력 </summary>
    public int defence;

    /// <summary> 유닛의 공격 속도 </summary>
    public float attackSpeed;
    /// <summary> 유닛의 이동 속도 </summary>
    public float moveSpeed;
    #endregion 스텟 정보
}