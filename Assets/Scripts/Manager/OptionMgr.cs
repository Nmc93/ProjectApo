using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionMgr : MgrBase
{
    public static OptionMgr instance;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    #region Get

    /// <summary> ID�� �����ϴ� Value�� ������ </summary>
    /// <param name="id"></param>
    /// <param name="isSave"> ������ ���� ���� </param>
    /// <param name="value"> ��ȯ </param>
    private static bool OptionValue(string id, bool isSave, out string value)
    {
        //�� �ʱ�ȭ
        value = string.Empty;

        //������ �˻�
        if (isSave)
        {
            if (PlayerPrefs.HasKey(id))
            {
                value = PlayerPrefs.GetString(id);
                return true;
            }
        }

        //���̺� �˻�
        if (TableMgr.Get(id, out OptionTableData tData))
        {
            value = tData.OptionValue;

            //������ ���� Ÿ���� �ɼ��� ��� �������� ����
            if (isSave)
            {
                PlayerPrefs.SetString(id, value);
                PlayerPrefs.Save();
            }
            return true;
        }

        return false;
    }

    /// <summary> ID�� ���� bool Ÿ������ ������ </summary>
    /// <param name="id">OptionTableData�� ID</param>
    /// <param name="isSave"> true�� ��� �������� �����Ѱ� Ȯ��, true�� ����Ʈ</param>
    /// <returns> �޾ƿ� ���̺� �����Ϳ� ������ ���� ��� false ��ȯ </returns>
    public static bool GetBoolOption(string id, bool isSave = true)
    {
        if(!OptionValue(id,isSave,out string value))
        {
            Debug.LogError($"{id}�� ID�� ���� �����͸� ã�� �� �����ϴ�.");
            return false;
        }

        //bool�� ����Ʈ
        if (!bool.TryParse(value, out bool result))
        {
            Debug.LogError($"{id}�� bool Ÿ�� �ɼ��� �ƴϰų� ��[{value}]�� �߸� �ԷµǾ� �ֽ��ϴ�.");
        }

        return result;
    }

    /// <summary> ID�� ���� long Ÿ������ ������ </summary>
    /// <param name="id">OptionTableData�� ID</param>
    /// <param name="isSave"> true�� ��� �������� �����Ѱ� Ȯ��, true�� ����Ʈ</param>
    /// <returns> �޾ƿ� ���̺� �����Ϳ� ������ ���� ��� 0 ��ȯ </returns>
    public static long GetLongOption(string id, bool isSave = true)
    {
        if (!OptionValue(id, isSave, out string value))
        {
            Debug.LogError($"{id}�� ID�� ���� �����͸� ã�� �� �����ϴ�.");
            return 0;
        }

        //bool�� ����Ʈ
        if (!long.TryParse(value, out long result))
        {
            Debug.LogError($"{id}�� bool Ÿ�� �ɼ��� �ƴϰų� ��[{value}]�� �߸� �ԷµǾ� �ֽ��ϴ�.");
        }

        return result;
    }

    /// <summary> ID�� ���� int Ÿ������ ������ </summary>
    /// <param name="id">OptionTableData�� ID</param>
    /// <param name="isSave"> true�� ��� �������� �����Ѱ� Ȯ��, true�� ����Ʈ</param>
    /// <returns> �޾ƿ� ���̺� �����Ϳ� ������ ���� ��� 0 ��ȯ </returns>
    public static int GetIntOption(string id, bool isSave = true)
    {
        if (!OptionValue(id, isSave, out string value))
        {
            Debug.LogError($"{id}�� ID�� ���� �����͸� ã�� �� �����ϴ�.");
            return 0;
        }

        //bool�� ����Ʈ
        if (!int.TryParse(value, out int result))
        {
            Debug.LogError($"{id}�� bool Ÿ�� �ɼ��� �ƴϰų� ��[{value}]�� �߸� �ԷµǾ� �ֽ��ϴ�.");
        }

        return result;
    }

    /// <summary> ID�� ���� float Ÿ������ ������ </summary>
    /// <param name="id">OptionTableData�� ID</param>
    /// <param name="isSave"> true�� ��� �������� �����Ѱ� Ȯ��, true�� ����Ʈ</param>
    /// <returns> �޾ƿ� ���̺� �����Ϳ� ������ ���� ��� 0 ��ȯ </returns>
    public static float GetfloatOption(string id, bool isSave = true)
    {
        if (!OptionValue(id, isSave, out string value))
        {
            Debug.LogError($"{id}�� ID�� ���� �����͸� ã�� �� �����ϴ�.");
            return 0;
        }

        //bool�� ����Ʈ
        if (!float.TryParse(value, out float result))
        {
            Debug.LogError($"{id}�� bool Ÿ�� �ɼ��� �ƴϰų� ��[{value}]�� �߸� �ԷµǾ� �ֽ��ϴ�.");
        }

        return result;
    }

    /// <summary> ID�� ���� string Ÿ������ ������ </summary>
    /// <param name="id">OptionTableData�� ID</param>
    /// <param name="isSave"> true�� ��� �������� �����Ѱ� Ȯ��, true�� ����Ʈ</param>
    /// <returns> �޾ƿ� ���̺� �����Ϳ� ������ ���� ��� string.Empty ��ȯ </returns>
    public static string GetStringOption(string id, bool isSave = true)
    {
        if (!OptionValue(id, isSave, out string value))
        {
            Debug.LogError($"{id}�� ID�� ���� �����͸� ã�� �� �����ϴ�.");
        }

        return value;
    }

    #endregion Get

}