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
    public eUintState uState;

    /// <summary> ������ �� ���� ���� </summary>
    public void Init(UnitData data)
    {
        //���� ������ ����
        this.data = data;

        // �Ӹ� ����
        ChangeSprite(head, data.headID);
        // ��
        ChangeSprite(face, data.faceID);
        //�� ���
        ChangeSprite(faceDeco, data.faceDecoID);
        //�Ӹ�ī�� ����
        ChangeSprite(hair, data.hairID);
        //�޸Ӹ� ����
        ChangeSprite(backHair, data.backHairID);
        //���� ����
        ChangeSprite(hat, data.hatID);

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

    public void ChangeSprite(SpriteRenderer renderer, int id)
    {
        //���̺��� ���ų� None�� ��� ��Ȱ��ȭ �� ����
        if(!TableMgr.Get(id, out UnitAppearanceTableData tbl) || tbl.Path == "None")
        {
            renderer.gameObject.SetActive(false);
            return;
        }

        //�̹��� �� �ִϸ��̼� ����
        renderer.sprite = AssetsMgr.GetSprite(eAtlasType.Unit_Human, tbl.Path);
        renderer.gameObject.SetActive(true);
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