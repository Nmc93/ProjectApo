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
    public eUintState state;

    /// <summary> 데이터 및 기초 세팅 </summary>
    public void Init(UnitData data)
    {
        //유닛 데이터 세팅
        this.data = data;

        //스프라이트 세팅용 테이블
        UnitAppearanceTableData tbl;

        // 머리 세팅
        if (TableMgr.Get(data.headID,out tbl))
        {
            head.sprite = AssetsMgr.GetSprite(eAtlasType.Unit_Human, tbl.Path);
        }

        // 얼굴
        if (TableMgr.Get(data.faceID, out tbl))
        {
            face.sprite = AssetsMgr.GetSprite(eAtlasType.Unit_Human, tbl.Path);
        }

        //얼굴 장식
        if (TableMgr.Get(data.faceDecoID, out tbl))
        {
            faceDeco.sprite = AssetsMgr.GetSprite(eAtlasType.Unit_Human, tbl.Path);
        }
        faceDeco.gameObject.SetActive(tbl != null);

        //머리카락 세팅
        if (TableMgr.Get(data.hairID, out tbl))
        {
            hair.sprite = AssetsMgr.GetSprite(eAtlasType.Unit_Human, tbl.Path);
        }
        hair.gameObject.SetActive(tbl != null);

        //뒷머리 세팅
        if (TableMgr.Get(data.backHairID, out tbl))
        {
            backHair.sprite = AssetsMgr.GetSprite(eAtlasType.Unit_Human, tbl.Path);
        }
        backHair.gameObject.SetActive(tbl != null);

        //모자 세팅
        if (TableMgr.Get(data.hatID, out tbl))
        {
            hat.sprite = AssetsMgr.GetSprite(eAtlasType.Unit_Human, tbl.Path);
        }
        hat.gameObject.SetActive(tbl != null);

        //무기 세팅(맨손일 경우 세팅하지 않음)
        if(data.weaponTbl.Path == "None")
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
    }

    public void ChangeSprite(Sprite sprite, UnitAppearanceTableData tbl)
    {
        //테이블이 없을 경우 종료함
        if(tbl == null)
        {
            return;
        }

        //이미지 및 애니메이션 변경
        sprite = AssetsMgr.GetSprite(eAtlasType.Unit_Human, tbl.Path);
    }

    /// <summary> 무기 변경 </summary>
    /// <param name="weaponID"> 무기의 ID </param>
    public void ChangeWeapon(int weaponID)
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

    /// <summary> 캐릭터 스탯 계산 </summary>
    private void RefreshStat()
    {
        data.RefreshStat();
    }
}