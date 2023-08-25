using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GEnum;

public class Unit : MonoBehaviour
{
    #region �ν�����

    [Header("���� �ִϸ��̼�")]
    [Tooltip("���� �ִϸ��̼�")]
    [SerializeField] private Animator animator;

    [Header("���� ��������Ʈ ����")]
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

    [Tooltip("���� ��")]
    [SerializeField] private SpriteRenderer frontArm;
    [Tooltip("�ĸ� ��")]
    [SerializeField] private SpriteRenderer backArm;

    [Tooltip("����")]
    [SerializeField] private SpriteRenderer weapon;

    #endregion �ν�����

    /// <summary> �ش� ������ ���� </summary>
    public UnitData data;

    /// <summary> ���� ������ �ൿ </summary>
    public eUintState state;

    /// <summary> ������ �� ���� ���� </summary>
    /// <param name="data">���� ������</param>
    /// <param name="aniCtlr">�ִϸ��̼� ��Ʈ�ѷ�</param>
    /// <param name="head">�Ӹ� ��������Ʈ</param>
    /// <param name="face">�� ��������Ʈ</param>
    /// <param name="faceDeco">�� ����(����� ��)</param>
    /// <param name="hair">�Ӹ�ī��</param>
    /// <param name="backHair">�޸Ӹ�</param>
    /// <param name="hat">����</param>
    /// <param name="frontArm">���� ��</param>
    /// <param name="backArm">�ĸ� ��</param>
    /// <param name="weapon">����</param>
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
        //���� ������ ����
        this.data = data;
        //�ִϸ��̼� ��Ʈ�ѷ� ����
        animator.runtimeAnimatorController = aniCtlr;

        //������ �ȵǴ� ��������Ʈ ����
        this.head.sprite = head;            // �Ӹ�
        this.face.sprite = face;            // ��
        this.frontArm.sprite = frontArm;    // ���� ��
        this.backArm.sprite = backArm;      // �ĸ� ��

        //�� ���� ����
        this.faceDeco.gameObject.SetActive(faceDeco != null);
        if (this.faceDeco.gameObject.activeSelf)
        {
            this.faceDeco.sprite = faceDeco;
        }

        //�Ӹ�ī�� ����
        this.hair.gameObject.SetActive(hair != null);
        if (this.hair.gameObject.activeSelf)
        {
            this.hair.sprite = hair;
        }

        //�޸Ӹ� ����
        this.backHair.gameObject.SetActive(backHair != null);
        if (this.backHair.gameObject.activeSelf)
        {
            this.backHair.sprite = backHair;
        }

        //���� ����
        this.hat.gameObject.SetActive(hat != null);
        if (this.hat.gameObject.activeSelf)
        {
            this.hat.sprite = hat;
        }

        //���� ����
        this.weapon.gameObject.SetActive(weapon != null);
        if (this.weapon.gameObject.activeSelf)
        {
            this.weapon.sprite = weapon;
        }
    }

}
