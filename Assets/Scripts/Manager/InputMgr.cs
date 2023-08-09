using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GEnum;

public class InputMgr : MgrBase
{
    public static InputMgr instance;

    [Serializable]
    public class ClickData
    {
        public ClickData(KeyCode key)
        {
            this.key = key;
        }

        /// <summary> Ű�� ������ �ִ��� ���� </summary>
        public bool isClickKey;
        /// <summary> ��ġŰ </summary>
        public KeyCode key;
        /// <summary> ��ġ �̺�Ʈ ��� </summary>
        public HashSet<Action> Actions = new HashSet<Action>();
    }

    /// <summary> ����ϴ� Ű ��� [��Ȳ�� ���� �����Ͱ� �����ǰ� �߰���]</summary>
    private static List<ClickData> keyList = new List<ClickData>();
    /// <summary> ��ɿ� �Ҵ�� Ű Ȯ�� [�����Ͱ� �����Ǵ����� �������] </summary>
    private static Dictionary<eInputType, KeyCode?> dicInUseData = new Dictionary<eInputType, KeyCode?>();

    #region ���� ����
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);

        //���� Ű ����
        KeySetting();
    }

    /// <summary> ����Ǿ� �ִ� ���·� Ű ���� </summary>
    private void KeySetting()
    {
        //Ű ����� �ʿ��Ѱ�
        // eInputType       - .Tostring()�� ���̺�� �������� Ű���� ����
        // InputKeyTable    - ����Ʈ Ű ���� ����
        // PlayerPrefs      - ������ Ű ������ ����, �����Ͱ� ���ٸ� ���̺����� ����� ���
        // InputKeyTable�� PlayerPrefs�� ����� ���� KeyCode.*.ToString()�� ����
        // �������� �ʾ� ������� �ʴ� ����� ���� Null�� ����

        //Ű ����
        int count = (int)eInputType.Count;
        for (int i = 0; i < count; ++i)
        {
            eInputType type = (eInputType)i;
            string typeName = type.ToString();

            //Ű�� ���� ��� ����Ʈ���� Ű ���� �� ����
            if (!PlayerPrefs.HasKey(typeName))
            {
                PlayerPrefs.SetString(typeName, TableMgr.Get<InputKeyTableData>(typeName).KeyString);
                PlayerPrefs.Save();
            }

            //�ش� Ű ����
            KeyCode? code = ConvertStringToKeyCode(PlayerPrefs.GetString(typeName));
            dicInUseData.Add(type, code);
        }
    }
    #endregion ���� ����

    #region �̺�Ʈ �˻�

    private void Update()
    {
        // �Է��� ���ٸ� ����
        if (!Input.anyKey)
        {
            return;
        }

        //Ű �˻�
        foreach(var data in keyList)
        {
            // 1. ù��° ��ư �ٿ��� ���
            if (!data.isClickKey)
            {
                // 1-1. ����Ű�� ������ Ű�� ���ų� NoneŸ���� ���(None�� AnyKey�� �����)
                if (Input.GetKeyDown(data.key) || data.key == KeyCode.None)
                {
                    //Ŭ�� üũ
                    data.isClickKey = true;

                    //������ �� �ִ� ��ġ �̺�Ʈ�� ���� ��� ����
                    if (data.Actions.Count > 0)
                    {
                        foreach (var action in data.Actions)
                        {
                            action();
                        }
                    }
                }
            }
            // 2. ��ư �ٿ��� �������� ���
            else
            {
                // 2-1. ��ư �� �̺�Ʈ�� ���
                if (Input.GetKeyUp(data.key))
                {
                    //Ŭ�� ����
                    data.isClickKey = true;
                }
            }
        }
    }
    
    #endregion �̺�Ʈ �˻�

    #region Ű �̺�Ʈ ����

    /// <summary> Ű Ŭ�� �̺�Ʈ ��� </summary>
    public static void AddKeyEvent(eInputType type, Action callback)
    {
        //Ÿ�Կ� ������ Ű �˻�
        if(dicInUseData.TryGetValue(type, out KeyCode? code) && code != null)
        {
            //��Ͽ� �����Ͱ� ���� ��� ������ ����
            ClickData data = keyList.Find(item => item.key == code.Value);
            if (data == null)
            {
                data = new ClickData(code.Value);
                keyList.Add(data);
            }

            //�̺�Ʈ ���
            if (!data.Actions.Add(callback))
            {
                Debug.LogError($"{type} �̺�Ʈ�� �ߺ� ����ǰ� �ֽ��ϴ�.");
            }
        }
        else
        {
            Debug.LogError($"{type}Ÿ���� �����͸� ã�� �� �����ϴ�.");
        }
    }

    /// <summary> Ű Ŭ�� �̺�Ʈ ���� </summary>
    public static void RemoveKeyEvent(eInputType type, Action callback)
    { 
        // Ÿ�Կ� ������ Ű �˻�
        if (dicInUseData.TryGetValue(type, out KeyCode? code) && code != null)
        {
            // ������ Ű�� ���� ��� �̺�Ʈ ����
            ClickData data = keyList.Find(item => item.key == code.Value);
            if (data != null)
            {
                // ��ϵ� �̺�Ʈ�� ���
                if (data.Actions.Contains(callback))
                {
                    //�̺�Ʈ ����
                    data.Actions.Remove(callback);

                    //��� �̺�Ʈ�� �������� ��� �ش� Ŭ�� �̺�Ʈ ����
                    if(data.Actions.Count <= 0)
                    {
                        keyList.Remove(data);
                    }
                }
                // ��ϵ��� ���� �̺�Ʈ�� ���
                else
                {
                    Debug.LogError($"������ {type}Ÿ���� �����͸� ã�� �� �����ϴ�.");
                }
            }
            //������ Ű�� ���� ���
            else
            {
                Debug.LogError($"{type}�� ������ Ű�� �����ϴ�.");
            }
        }
        //�ش� Ÿ�Կ� ���õ� �����Ͱ� ���� ���
        else
        {
            Debug.LogError($"{type}Ÿ���� �����͸� ã�� �� �����ϴ�.");
        }
    }

    #endregion Ű �̺�Ʈ ����

    #region ��ȯ
    /// <summary> string�� KeyCode�� ��ȯ </summary>
    /// <param name="key"> KeyCode�� �׸�� �̸��� �����ؾ���</param>
    /// <returns> ã�� �� ���ٸ� null ��ȯ </returns>
    private KeyCode? ConvertStringToKeyCode(string key)
    {
        if(key == "Null")
        {
            return null;
        }

        try
        {
            KeyCode code = (KeyCode)Enum.Parse(typeof(KeyCode), key);
            return code;
        }
        catch (Exception ex)
        {
            Debug.LogError($"KeyCode ��ȯ ���� : [{ex.Message}], Key : [{key}]");
            return null;
        }
    }
    #endregion ��ȯ

}