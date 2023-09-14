using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

public class Unit : MonoBehaviour
{
    #region �ν�����

    [Header("[���� �ִϸ��̼�]"),Tooltip("���� �ִϸ��̼�")]
    [SerializeField] private Animator animator;

    [Header("[���� ��������Ʈ ����]")]
    [Tooltip("�Ӹ�")]
    [SerializeField] private SpriteRenderer head;
    [Tooltip("��")]
    [SerializeField] private SpriteRenderer face;
    [Tooltip("�� ����")]
    [SerializeField] private SpriteRenderer faceDeco;
    [Tooltip("�Ӹ�ī��")]
    [SerializeField] private SpriteRenderer hair;
    [Tooltip("�޸Ӹ�")]
    [SerializeField] private SpriteRenderer backHair;
    [Tooltip("����")]
    [SerializeField] private SpriteRenderer hat;

    [Tooltip("����")]
    [SerializeField] private SpriteRenderer weapon;

    #endregion �ν�����

    /// <summary> �ش� ������ ���� </summary>
    public UnitData data;

    /// <summary> ���� ������ �ൿ </summary>
    public eUintState state;

    /// <summary> ������ �� ���� ���� </summary>
    public void Init(UnitData data)
    {
        //���� ������ ����
        this.data = data;

        //��������Ʈ ���ÿ� ���̺�
        UnitAppearanceTableData tbl;

        // �Ӹ� ����
        if (TableMgr.Get(data.headType,out tbl))
        {
            head.sprite = AssetsMgr.GetSprite(eAtlasType.Unit_Human, tbl.Path);
        }

        // ��
        if (TableMgr.Get(data.faceType, out tbl))
        {
            face.sprite = AssetsMgr.GetSprite(eAtlasType.Unit_Human, tbl.Path);
        }

        //�� ���
        if (TableMgr.Get(data.faceDecoType, out tbl))
        {
            faceDeco.sprite = AssetsMgr.GetSprite(eAtlasType.Unit_Human, tbl.Path);
        }
        faceDeco.gameObject.SetActive(tbl != null);

        //�Ӹ�ī�� ����
        if (TableMgr.Get(data.hairType, out tbl))
        {
            hair.sprite = AssetsMgr.GetSprite(eAtlasType.Unit_Human, tbl.Path);
        }
        hair.gameObject.SetActive(tbl != null);

        //�޸Ӹ� ����
        if (TableMgr.Get(data.backHairType, out tbl))
        {
            backHair.sprite = AssetsMgr.GetSprite(eAtlasType.Unit_Human, tbl.Path);
        }
        backHair.gameObject.SetActive(tbl != null);

        //���� ����
        if (TableMgr.Get(data.hatType, out tbl))
        {
            hat.sprite = AssetsMgr.GetSprite(eAtlasType.Unit_Human, tbl.Path);
        }
        hat.gameObject.SetActive(tbl != null);

        //��, �� ���� (�ִϸ��̼� ��Ʈ�ѷ�)
        animator.runtimeAnimatorController = AssetsMgr.GetUnitRuntimeAnimatorController(data.bodyType);

        //���� ����
        if (TableMgr.Get(data.weaponType, out tbl))
        {
            weapon.sprite = AssetsMgr.GetSprite(eAtlasType.Unit_Human, tbl.Path);
        }
        weapon.gameObject.SetActive(tbl != null);
    }
}
