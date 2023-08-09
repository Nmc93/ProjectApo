using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// ������Ʈ Ȱ��ȭ �� �ش� �ؽ�Ʈ

public class UISetString : MonoBehaviour
{
    [Header("[�ؽ�Ʈ ������Ʈ]"), Tooltip("�ؽ�Ʈ ������Ʈ")]
    [SerializeField] TextMeshProUGUI text;

    [Header("[�ش� �ؽ�Ʈ�� ����� Ű��]"), Tooltip("�ش� �ؽ�Ʈ�� ����� Ű��")]
    [SerializeField] string key;

    /// <summary> ����� ���ڿ� </summary>
    private string textString;

    private void OnEnable()
    {
        //�ؽ�Ʈ�� ����� ���ڿ��� �غ�Ǿ����� ���� ���
        if (string.IsNullOrEmpty(textString))
        {
            //���̺��� �޾ƿ�
            textString = TableMgr.Get(key, out StringTableData data) ? data.Text : string.Empty;
        }

        //�ؽ�Ʈ ����
        text.text = textString;
    }
}
