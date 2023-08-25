using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

public class Unit : MonoBehaviour
{
    #region 인스펙터

    [Header("유닛 애니메이션")]
    [Tooltip("유닛 애니메이션")]
    [SerializeField] private Animator animator;

    [Header("유닛 스프라이트 정보")]
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

    [Tooltip("정면 팔")]
    [SerializeField] private SpriteRenderer frontArm;
    [Tooltip("후면 팔")]
    [SerializeField] private SpriteRenderer backArm;

    [Tooltip("무기")]
    [SerializeField] private SpriteRenderer weapon;

    #endregion 인스펙터

    /// <summary> 해당 유닛의 정보 </summary>
    public UnitData data;

    /// <summary> 현재 유닛의 행동 </summary>
    public eUintState state;

    /// <summary> 데이터 및 기초 세팅 </summary>
    /// <param name="data">유닛 데이터</param>
    /// <param name="aniCtlr">애니메이션 컨트롤러</param>
    /// <param name="head">머리 스프라이트</param>
    /// <param name="face">얼굴 스프라이트</param>
    /// <param name="faceDeco">얼굴 데코(콧수염 등)</param>
    /// <param name="hair">머리카락</param>
    /// <param name="backHair">뒷머리</param>
    /// <param name="hat">모자</param>
    /// <param name="frontArm">전면 팔</param>
    /// <param name="backArm">후면 팔</param>
    /// <param name="weapon">무기</param>
    public void Init(
        UnitData data,
        RuntimeAnimatorController aniCtlr,
        Sprite head,
        Sprite face,
        Sprite faceDeco,
        Sprite hair,
        Sprite backHair,
        Sprite hat,
        Sprite frontArm,
        Sprite backArm,
        Sprite weapon)                  
    {
        //유닛 데이터 세팅
        this.data = data;
        //애니메이션 컨트롤러 세팅
        animator.runtimeAnimatorController = aniCtlr;

        //없으면 안되는 스프라이트 세팅
        this.head.sprite = head;            // 머리
        this.face.sprite = face;            // 얼굴
        this.frontArm.sprite = frontArm;    // 전면 팔
        this.backArm.sprite = backArm;      // 후면 팔

        //얼굴 데코 세팅
        this.faceDeco.gameObject.SetActive(faceDeco != null);
        if (this.faceDeco.gameObject.activeSelf)
        {
            this.faceDeco.sprite = faceDeco;
        }

        //머리카락 세팅
        this.hair.gameObject.SetActive(hair != null);
        if (this.hair.gameObject.activeSelf)
        {
            this.hair.sprite = hair;
        }

        //뒷머리 세팅
        this.backHair.gameObject.SetActive(backHair != null);
        if (this.backHair.gameObject.activeSelf)
        {
            this.backHair.sprite = backHair;
        }

        //모자 세팅
        this.hat.gameObject.SetActive(hat != null);
        if (this.hat.gameObject.activeSelf)
        {
            this.hat.sprite = hat;
        }

        //무기 세팅
        this.weapon.gameObject.SetActive(weapon != null);
        if (this.weapon.gameObject.activeSelf)
        {
            this.weapon.sprite = weapon;
        }
    }

}
