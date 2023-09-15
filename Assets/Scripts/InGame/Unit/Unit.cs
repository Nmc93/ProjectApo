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
        if (TableMgr.Get(data.headID,out tbl))
        {
            head.sprite = AssetsMgr.GetSprite(eAtlasType.Unit_Human, tbl.Path);
        }

        // ��
        if (TableMgr.Get(data.faceID, out tbl))
        {
            face.sprite = AssetsMgr.GetSprite(eAtlasType.Unit_Human, tbl.Path);
        }

        //�� ���
        if (TableMgr.Get(data.faceDecoID, out tbl))
        {
            faceDeco.sprite = AssetsMgr.GetSprite(eAtlasType.Unit_Human, tbl.Path);
        }
        faceDeco.gameObject.SetActive(tbl != null);

        //�Ӹ�ī�� ����
        if (TableMgr.Get(data.hairID, out tbl))
        {
            hair.sprite = AssetsMgr.GetSprite(eAtlasType.Unit_Human, tbl.Path);
        }
        hair.gameObject.SetActive(tbl != null);

        //�޸Ӹ� ����
        if (TableMgr.Get(data.backHairID, out tbl))
        {
            backHair.sprite = AssetsMgr.GetSprite(eAtlasType.Unit_Human, tbl.Path);
        }
        backHair.gameObject.SetActive(tbl != null);

        //���� ����
        if (TableMgr.Get(data.hatID, out tbl))
        {
            hat.sprite = AssetsMgr.GetSprite(eAtlasType.Unit_Human, tbl.Path);
        }
        hat.gameObject.SetActive(tbl != null);

        //���� ����(�Ǽ��� ��� �������� ����)
        if(data.weaponTbl.Path == "None")
        {
            weapon.sprite = AssetsMgr.GetSprite(eAtlasType.Unit_Human, data.weaponTbl.Path);
            weapon.gameObject.SetActive(true);
        }
        else
        {
            weapon.gameObject.SetActive(false);
        }

        //��, �� ���� (�ִϸ��̼� ��Ʈ�ѷ�)
        animator.runtimeAnimatorController = AssetsMgr.GetUnitRuntimeAnimatorController(data.bodyID);
    }

    public void ChangeSprite(Sprite sprite, UnitAppearanceTableData tbl)
    {
        //���̺��� ���� ��� ������
        if(tbl == null)
        {
            return;
        }

        //�̹��� �� �ִϸ��̼� ����
        sprite = AssetsMgr.GetSprite(eAtlasType.Unit_Human, tbl.Path);
    }

    /// <summary> ���� ���� </summary>
    /// <param name="weaponID"> ������ ID </param>
    public void ChangeWeapon(int weaponID)
    {
        //���� ���� ����
        data.SetWeaponData(weaponID);

        //�̹��� �� �ִϸ��̼� ����
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

    /// <summary> ĳ���� ���� ��� </summary>
    private void RefreshStat()
    {
        data.RefreshStat();
    }
}