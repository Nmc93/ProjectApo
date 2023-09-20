using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

public class Unit : MonoBehaviour
{
    #region 인스펙터

    [Header("[유닛 애니메이션]"),Tooltip("유닛 애니메이션")]
    [SerializeField] private Animator animator;

    [Header("[유닛 스프라이트 정보]")]
    [Tooltip("머리")]
    [SerializeField] private SpriteRenderer head;
    [Tooltip("얼굴")]
    [SerializeField] private SpriteRenderer face;
    [Tooltip("얼굴 데코")]
    [SerializeField] private SpriteRenderer faceDeco;
    [Tooltip("머리카락")]
    [SerializeField] private SpriteRenderer hair;
    [Tooltip("뒷머리")]
    [SerializeField] private SpriteRenderer backHair;
    [Tooltip("모자")]
    [SerializeField] private SpriteRenderer hat;

    [Tooltip("무기")]
    [SerializeField] private SpriteRenderer weapon;

    #endregion 인스펙터

    /// <summary> 해당 유닛의 정보 </summary>
    public UnitData data;

    /// <summary> 현재 유닛의 행동 </summary>
    public eUintState uState;

    /// <summary> 해당 유닛의 AI </summary>
    public UnitAI ai;

    /// <summary> 데이터 및 기초 세팅 </summary>
    public void Init(UnitData data)
    {
        //유닛 데이터 세팅
        this.data = data;

        // 머리 세팅
        ChangeSprite(head, data.headID);
        // 얼굴
        ChangeSprite(face, data.faceID);
        //얼굴 장식
        ChangeSprite(faceDeco, data.faceDecoID);
        //머리카락 세팅
        ChangeSprite(hair, data.hairID);
        //뒷머리 세팅
        ChangeSprite(backHair, data.backHairID);
        //모자 세팅
        ChangeSprite(hat, data.hatID);

        //무기 세팅(맨손일 경우 세팅하지 않음)
        if (data.weaponTbl.Path == "None")
        {
            weapon.sprite = AssetsMgr.GetSprite(eAtlasType.Unit_Human, data.weaponTbl.Path);
            weapon.gameObject.SetActive(true);
        }
        else
        {
            weapon.gameObject.SetActive(false);
        }

        //몸, 팔 세팅 (애니메이션 컨트롤러)
        animator.runtimeAnimatorController = AssetsMgr.GetUnitRuntimeAnimatorController(data.bodyID);

        //AI 세팅
        SetAI();
    }

    /// <summary> 캐릭터 스탯 계산 </summary>
    private void RefreshStat()
    {
        data.RefreshStat();
    }

    #region AI

    //행동 함수 - 여기선 행동 타입이 정해졌을때 행동할 함수들, 타입은 ai에서 결정함
    //이동 함수
    //경계 함수 경계 범위 같은 함수 필요
    //공격 함수 같은 것도 필요? - 무기 함수 생각해봐야할듯

    /// <summary> 타입에 맞는 AI를 생성 및 세팅 </summary>
    private void SetAI()
    {
        //타입에 맞는 AI 세팅
        ai = null;
        switch (data.unitType)
        {
            case eUnitType.Human:
                ai = new NormalHumanAI();
                break;
            case eUnitType.Zombie:
                //ai = 
                break;
        }

        //ai 세팅
        ai.Setting(data, animator);
    }

    #endregion AI

    #region 이미지 변경
    /// <summary> 스프라이트랜더러의 스프라이트를 변경 </summary>
    /// <param name="renderer"> 변경할 스프라이트 랜더러 </param>
    /// <param name="id"> UnitAppearanceTableData의 ID 참조 </param>
    private void ChangeSprite(SpriteRenderer renderer, int id)
    {
        //테이블이 없거나 None일 경우 비활성화 후 종료
        if(!TableMgr.Get(id, out UnitAppearanceTableData tbl) || tbl.Path == "None")
        {
            renderer.gameObject.SetActive(false);
            return;
        }

        //이미지 및 애니메이션 변경
        renderer.sprite = AssetsMgr.GetSprite(eAtlasType.Unit_Human, tbl.Path);
        renderer.gameObject.SetActive(true);
    }

    /// <summary> 무기 변경 </summary>
    /// <param name="weaponID"> 무기의 ID </param>
    private void ChangeWeapon(int weaponID)
    {
        //무기 정보 변경
        data.SetWeaponData(weaponID);

        //이미지 및 애니메이션 변경
        if (data.weaponTbl.Path == "None")
        {
            weapon.sprite = AssetsMgr.GetSprite(eAtlasType.Unit_Human, data.weaponTbl.Path);
            weapon.gameObject.SetActive(true);
        }
        else
        {
            weapon.gameObject.SetActive(false);
        }
    }
    #endregion 이미지 변경
}