using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

public class Unit : MonoBehaviour
{
    #region �ν�����

    [Header("[���� �ִϸ��̼�]"),Tooltip("���� �ִϸ��̼�")]
    [SerializeField] private Animator animator;

    [Header("Ž�� ����")]
    [SerializeField] private BoxCollider2D searchArea;

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

    #region ������

    /// <summary> ������ TID </summary>
    public int TID;

    /// <summary> �ش� ������ ���� </summary>
    public UnitData data;
    /// <summary> �ش� ������ AI </summary>
    public UnitAI ai;

    /// <summary> ���� ������ �ൿ </summary>
    public eUintState uState;

    /// <summary> ��ġ �����ȿ� �ִ� �� ID </summary>
    private List<int> searchEnemyID = new List<int>();

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
        if (data.weaponTbl.Path == "None")
        {
            weapon.sprite = AssetsMgr.GetSprite(eAtlasType.Unit_Human, data.weaponTbl.Path);
            weapon.gameObject.SetActive(true);
        }
        else
        {
            weapon.gameObject.SetActive(false);
        }

        //���� ��� �� ����
        RefreshStat();

        //��, �� ���� (�ִϸ��̼� ��Ʈ�ѷ�)
        animator.runtimeAnimatorController = AssetsMgr.GetUnitRuntimeAnimatorController(data.bodyID);

        //AI ����
        SetAI();
    }

    /// <summary> ĳ���� ���� ��� �� ���� </summary>
    private void RefreshStat()
    {
        //���� ���
        data.RefreshStat();

        // Ž�� ���� ����
        searchArea.size = new Vector2(data.sSize, 1);
        searchArea.offset = new Vector2(-(data.sSize / 2), 0);
    }

    #endregion ������

    #region ����Ƽ �������̵�

    private void Update()
    {
        //������ �༮���� �˻��ؼ� �˻�� �༮�� ����
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!int.TryParse(collision.name, out int tid))
        {
            Debug.LogError($"������ �̸� {collision.name}�� TID�� �̸��� �ִ� �Ծ࿡ ��߳��ϴ�.");
            return;
        }

        //�߰ߵ� ����� ��Ͽ� �߰�
        if(!searchEnemyID.Contains(tid))
        {
            searchEnemyID.Add(tid);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!int.TryParse(collision.name, out int tid))
        {
            Debug.LogError($"������ �̸� {collision.name}�� TID�� �̸��� �ִ� �Ծ࿡ ��߳��ϴ�.");
            return;
        }

        //��� ����� ��Ͽ��� ����
        if (searchEnemyID.Contains(tid))
        {
            searchEnemyID.Remove(tid);
            //tid�� �̿��ؼ� ���� �� Ž���� ���� ������
        }
    }

    #endregion ����Ƽ �������̵�

    #region AI

    //�ൿ �Լ� - ���⼱ �ൿ Ÿ���� ���������� �ൿ�� �Լ���, Ÿ���� ai���� ������
    /// <summary> �̵� </summary>
    private void Move()
    {
        // 
    }

    /// <summary> Ž�� </summary>
    private void Search()
    {

    }

    /// <summary> ���� </summary>
    private void Attack()
    {

    }

    /// <summary> Ÿ�Կ� �´� AI�� ���� �� ���� </summary>
    private void SetAI()
    {
        //Ÿ�Կ� �´� AI ����
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

        //ai ����
        ai.Setting(data, animator);
    }

    #endregion AI

    #region �̹��� ����
    /// <summary> ��������Ʈ�������� ��������Ʈ�� ���� </summary>
    /// <param name="renderer"> ������ ��������Ʈ ������ </param>
    /// <param name="id"> UnitAppearanceTableData�� ID ���� </param>
    private void ChangeSprite(SpriteRenderer renderer, int id)
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
    private void ChangeWeapon(int weaponID)
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
    #endregion �̹��� ����
}