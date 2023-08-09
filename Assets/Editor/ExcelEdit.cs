using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using ExcelDataReader;
using System.Data;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;

namespace ExcelEdit
{
    public enum eDataType
    {
        None = 0,
        String,
        Int,
        Long,
        Bool,
    }

    public class ExcelEdit : EditorWindow
    {
        #region ������ ���
        /// <summary> ���� ���̺� ���� ��� </summary>
        private string tablePath = "Table";
        /// <summary> ���̺��� ���̳ʸ� ���� ��� </summary>
        private string tablebytesPath = "Assets\\Resources\\TableBytes";
        /// <summary> ���̺��� CSV ���� ��� </summary>
        private string tableCSVPath = "TableCSV";
        /// <summary> ���̺��� CS ���� ��� </summary>
        private string tableCSPath = "Assets\\Scripts\\TableData";
        #endregion ������ ��� 

        #region �˻� ����
        /// <summary> �˻� ���̺�� �ؽ�Ʈ </summary>
        private string tableNameText = string.Empty;
        /// <summary> �˻��� ���̺��� ������ ����� </summary>
        private string[] tableArray;
        /// <summary> �˻��� ���̺��� ��ũ�� ������ </summary>
        private Vector2 searchScrollPosition;
        #endregion �˻� ����

        #region ���ý� ����� ����
        /// <summary> ���õ� ���̺� �̸� </summary>
        private string selectTableName = string.Empty;
        /// <summary> ���õ� ���� ���̺� ��� </summary>
        private string selectTablePath = string.Empty;
        /// <summary> ���õ� ���̺� ���̳ʸ� ��� </summary>
        private string selectTableBytesPath = string.Empty;
        /// <summary> ���õ� ���̺� CSV ��� </summary>
        private string selectTableCSVPath = string.Empty;
        /// <summary> ���õ� ���̺� CS ��� </summary>
        private string selectTableCSPath = string.Empty;

        /// <summary> ���õ� ���̺��� �� ���� �̸��� </summary>
        private List<string> selectColumnNameList = new List<string>();
        /// <summary> ���õ� ���̺��� �� ���� Ÿ�Ե� </summary>
        private List<eDataType> selectColumnTypeList = new List<eDataType>();
        /// <summary> �˻��� ���̺��� ��ũ�� ������ </summary>
        private Vector2 selectScrollPosition;
        #endregion ���ý� ����� ����

        /// <summary> CS������ </summary>
        private ScriptGenerator scGenerator = new ScriptGenerator();

        [MenuItem("GameTool/ExcelEditor")]
        public static void ExcelConverter()
        {
            //������ ��� Ŭ���� ȹ��
            EditorWindow wnd = GetWindow<ExcelEdit>();
            //������ �̸� ����
            wnd.titleContent = new GUIContent("ExcelEdit");
        }

        private void OnGUI()
        {
            #region ���̺� �˻�â
            GUILayout.BeginArea(new Rect(0, 0, position.width, 90), GUI.skin.window);
            GUILayout.BeginHorizontal();

            //�ؽ�Ʈ �Է�â(�Է�â �̸�, ���� �ؽ�Ʈ)
            tableNameText = EditorGUILayout.TextField("���̺� �̸� : ", tableNameText);

            //��ư(��ư �̸�)
            if (GUILayout.Button("���̺� �˻�", GUILayout.Width(100f)))
            {
                string[] tableNames;
                List<string> tableList = new List<string>();

                //���̺� �̸��� �Է����� �ʾ��� ���
                if (string.IsNullOrEmpty(tableNameText))
                {
                    tableNames = Directory.GetFiles(tablePath, "*.xlsx");
                }
                //���̺� �̸��� �Է����� ���
                else
                {
                    tableNames = Directory.GetFiles(tablePath, $"*{tableNameText}*");
                }

                for(int i = 0; i < tableNames.Length; ++i)
                {
                    string tableName = tableNames[i].Substring(6);
                    tableName = tableName.Substring(0, tableName.Length - 5);
                    if (tableName.Contains(tableNameText, StringComparison.OrdinalIgnoreCase))
                    {
                        tableList.Add(tableNames[i]);
                    }
                }

                //�˻��� ���̺� ����
                tableArray = tableList.ToArray();
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();

            //���� ��ũ�� Ÿ��Ʋ
            GUILayout.Label($"������ ���� : {selectTableName}", EditorStyles.label);

            GUILayout.EndHorizontal();
            GUILayout.EndArea();
            #endregion ���̺� �˻�â

            #region �˻��� ���� ��ũ��(���� �ڽ�)
            //��ũ�� ũ�� ����
            Rect LeftscrollRect = new Rect(0, 90, position.width / 2, 200);
            GUILayout.BeginArea(LeftscrollRect, GUI.skin.window);

            //�˻��� ���̺��� ���� ��쿡��
            if (tableArray != null && tableArray.Length > 0)
            {
                //��ũ��
                searchScrollPosition = EditorGUILayout.BeginScrollView(searchScrollPosition);

                for (int i = 0; i < tableArray.Length; ++i)
                {
                    if (GUILayout.Button(tableArray[i]))
                    {
                        //���̺� ������ ����
                        SetSelectTableDatas(tableArray[i]);

                        //����
                        Repaint();
                    }
                }

                GUILayout.EndScrollView();
            }

            GUILayout.EndArea();
            #endregion �˻��� ���� ��ũ��(���� �ڽ�)

            #region �˻��� ���̺� ����Ʈ(������ �ڽ�)
            GUILayout.BeginArea(new Rect(position.width / 2, 90, position.width / 2, 200), GUI.skin.window);

            #region ���̺� csv ���� �� ����
            if (GUILayout.Button($"{selectTableName}.bytes(.csv) ����/����"))
            {
                ConvertExcelToBytes();
            }
            #endregion ���̺� csv ���� �� ����

            #region ���̺� cs ���� �� ����
            if (GUILayout.Button($"{selectTableName}.cs ����/����"))
            {
                ConvertExcelToCS();
            }
            #endregion ���̺� cs ���� �� ����

            #region ���� ���̺� ���� ����
            if (GUILayout.Button("���� ���̺� ���� ����"))
            {
                // �ּҰ� �´��� Ȯ��
                if (Directory.Exists(tablePath))
                {
                    try
                    {
                        //���� ����
                        Process.Start(tablePath);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError($"���� ���� ���� : {e.Message}");
                    }
                }
            }
            #endregion ���� ���̺� ���� ����

            #region ���̳ʸ� ���� ����
            if (GUILayout.Button("���̳ʸ� ���� ����"))
            {
                // �ּҰ� �´��� Ȯ��
                if (Directory.Exists(tablebytesPath))
                {
                    try
                    {
                        //���� ����
                        Process.Start(tablebytesPath);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError($"���� ���� ���� : {e.Message}");
                    }
                }
            }
            #endregion ���̳ʸ� ���� ����

            #region CSV ���� ����
            if (GUILayout.Button("CSV ���� ����"))
            {
                // �ּҰ� �´��� Ȯ��
                if (Directory.Exists(tableCSVPath))
                {
                    try
                    {
                        //���� ����
                        Process.Start(tableCSVPath);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError($"���� ���� ���� : {e.Message}");
                    }
                }
            }
            #endregion CSV ���� ����

            #region CS ���� ����
            if (GUILayout.Button("CS ���� ����"))
            {
                // �ּҰ� �´��� Ȯ��
                if (Directory.Exists(tableCSPath))
                {
                    try
                    {
                        //���� ����
                        Process.Start(tableCSPath);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError($"���� ���� ���� : {e.Message}");
                    }
                }
            }
            #endregion CS ���� ����

            GUILayout.EndArea();
            #endregion �˻��� ���̺� ����Ʈ(������ �ڽ�)

            #region ���̺� ����
            //��ũ�� ũ�� ����
            Rect infocrollRect = new Rect(0, 290, position.width, position.height - 290);
            GUILayout.BeginArea(infocrollRect, GUI.skin.window);

            //�˻��� ���̺��� ���� ��쿡��
            if (!string.IsNullOrEmpty(selectTablePath))
            {
                if (selectColumnNameList != null && selectColumnNameList.Count > 0)
                {
                    selectScrollPosition = EditorGUILayout.BeginScrollView(selectScrollPosition);

                    for (int i = 0; i < selectColumnNameList.Count; ++i)
                    {
                        if (selectColumnTypeList[i] != eDataType.None)
                        {
                            int size = 90 - selectColumnNameList[i].Length;
                            GUILayout.BeginHorizontal();
                            GUILayout.Label($"�̸� : {selectColumnNameList[i].PadRight(size > 0 ? size : 0)} Ÿ�� : {selectColumnTypeList[i]}");
                            GUILayout.EndHorizontal();
                            GUILayout.Space(2);
                        }
                    }
                    GUILayout.EndScrollView();
                }
            }

            GUILayout.EndArea();
            #endregion ���̺� ����
        }

        #region ������ ���̺��� ������ ȹ��
        /// <summary> ������ ���̺��� ������ ȹ�� </summary>
        public void SetSelectTableDatas(string excelPath)
        {
            //���̺��� �̸� ����
            selectTableName = GetTableName(excelPath);

            //���� ���̺��� ��� ����
            selectTablePath = excelPath;
            //���̳ʸ� ���  ����
            selectTableBytesPath = string.Format("{0}\\{1}Data.bytes", tablebytesPath, selectTableName);
            //CSV ���̺��� ��� ����
            selectTableCSVPath = string.Format("{0}\\{1}.csv", tableCSVPath, selectTableName);
            //CS ������ Ŭ������ ��� ����
            selectTableCSPath = string.Format("{0}\\{1}Data.cs", tableCSPath, selectTableName);

            //���̺��� �̸��� Ÿ�� ����
            SetExcelData(selectTablePath);
        }
        #endregion ������ ���̺��� ������ ȹ�� 

        #region ������ ��ο� �ִ� �������� �̸��� ��ȯ

        /// <summary> ������ ������ �̸��� ��ȯ </summary>
        private string GetTableName(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                string[] firstText = path.Split(".");
                if (firstText.Length > 0)
                {
                    string[] lastText = firstText[0].Split("\\");
                    if (lastText.Length > 0)
                    {
                        return lastText[lastText.Length - 1];
                    }
                }

            }
            return string.Empty;
        }

        #endregion ������ ��ο� �ִ� �������� �̸��� ��ȯ

        #region ���õ� ������ ������ �̸��� Ÿ���� ��ȯ
        /// <summary> ���õ� ������ ������ �̸��� Ÿ���� ��ȯ </summary>
        public void SetExcelData(string path)
        {
            selectColumnNameList.Clear();
            selectColumnTypeList.Clear();

            //��ο� �ִ� ������ �б� ���� ����
            using (FileStream file = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                //�ش� ������ ���� �����ͷ� ��ȯ�� �� �ִ� ������ ����
                using (var reader = ExcelReaderFactory.CreateReader(file))
                {
                    //�����ͼ����� ��ȯ
                    DataSet data = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = item => new ExcelDataTableConfiguration()
                        {
                            //ù��° ���� �� �̸����� ������� ����
                            UseHeaderRow = false
                        }
                    });

                    if (data.Tables.Count > 0)
                    {
                        //���� ���̺��� ù��° �������� Ȯ��
                        DataTable TableData = data.Tables[0];
                        if (TableData.Rows.Count > 0)
                        {
                            //���̺��� ù��° �࿡ ����
                            DataRow row = TableData.Rows[0];
                            //ù��° ���� ��� ���� ���� ������ ȹ��
                            object[] firstData = row.ItemArray;

                            //ù��° ���� ���� ���� String���� ��ȯ�ؼ� List�� ����
                            List<string> list = new List<string>();
                            foreach (var item in firstData)
                            {
                                string[] values = item.ToString().Split("-");

                                //���̺� �� ��
                                if (values.Length == 2)
                                {
                                    selectColumnNameList.Add(values[0].ToString());
                                    selectColumnTypeList.Add(ExcelUtility.ConvertStringToeDataType(values[1].ToString()));
                                }
                                //����� ��
                                else if (values.Length == 1)
                                {
                                    selectColumnNameList.Add(values[0].ToString());
                                    selectColumnTypeList.Add(eDataType.None);
                                }
                                else
                                {
                                    UnityEngine.Debug.LogError($" �߸��� ���̺� �� �����Դϴ�. : {values.Length}");
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion ���õ� ������ ������ �̸��� Ÿ���� ��ȯ

        #region ���������� CSV,���̳ʸ��� ��ȯ
        /// <summary> ���������� CSV�� ��ȯ </summary>
        public void ConvertExcelToBytes()
        {
            if (string.IsNullOrEmpty(selectTablePath) || 
                string.IsNullOrEmpty(selectTableCSVPath) || 
                string.IsNullOrEmpty(selectTableBytesPath))
            {
                EditorUtility.DisplayDialog("CSV ����/����", "���� : ������ �ּҰ� �����ϴ�.", "Ȯ��");
                return;
            }

            TableData tData = null;
            
            //�� �����
            List<string> csvList = new List<string>();
            string typeName = $"{selectTableName}Data";
            //csv ������ ���� ����Ʈ

            //Ÿ�� ����
            Assembly ab = Assembly.LoadFrom("Library/ScriptAssemblies/Assembly-CSharp.dll");
            Type type = ab.GetType(typeName);
            //Ÿ���� ã�� ������ ���
            if(type == null)
            {
                foreach(var item in AppDomain.CurrentDomain.GetAssemblies())
                {
                    type = item.GetType(typeName);
                    if(type != null)
                        break;
                }
            }

            //��ο� �ִ� ���� ������ �б� ���� ����
            Dictionary<object, TableBase> dicTable = new Dictionary<object, TableBase>();
            using (FileStream file = File.Open(selectTablePath, FileMode.Open, FileAccess.Read))
            {
                //�ش� ������ ���� �����ͷ� ��ȯ�� �� �ִ� ������ ����
                using (var reader = ExcelReaderFactory.CreateReader(file))
                {
                    #region Ÿ��Ʋ�� Ÿ���� ����� ù��° �� ����(Ÿ�Ը� ������)
                    //ù��° �� - Ÿ�� �� ����
                    reader.Read();

                    //���̺��� ���� Ÿ�Ե�
                    string typeStr = string.Empty;

                    //Ÿ�� ����
                    for (int i = 0; i < reader.FieldCount; ++i)
                    {
                        var item = reader[i];
                        if (item != null)
                        {
                            //�ش� ���� ����Ÿ���� ��� ���� ĵ��
                            if (selectColumnTypeList[i] == eDataType.None)
                                continue;

                            //�ؽ�Ʈ ����
                            typeStr = i == 0 ?
                                ExcelUtility.ConverteDataTypeToString(selectColumnTypeList[i]) : 
                                $"{typeStr},{ExcelUtility.ConverteDataTypeToString(selectColumnTypeList[i])}";
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("���̳ʸ� ����/����", $"���� : {i}��° ���� ������ �����ϴ�.", "Ȯ��");
                            return;
                        }
                    }

                    //���� �� CSV �� �ٲ��� ���� "" ����
                    csvList.Add(typeStr);
                    #endregion Ÿ��Ʋ�� Ÿ���� ����� ù��° �� ����(Ÿ�Ը� ������)

                    #region ������ ����ִ� �ι�° ����� ����
                    
                    //�� ������ �����Ͱ� ���� �� ���� ����
                    while (reader.Read())
                    {
                        //���̺� ������ �� ���
                        List<object> valueList = new List<object>();
                        //���̺� ���� ����
                        string rowText = string.Empty;

                        for (int i = 0; i < reader.FieldCount; ++i)
                        {
                            //�ش� �������� Ÿ���� None(�޸��)�� ��� ���� ĵ��
                            if (selectColumnTypeList[i] == eDataType.None)
                                continue;

                            //�����Ϳ� ������ �ִ� ���
                            var item = reader[i];
                            if (item != null)
                            {
                                //Ÿ�Կ� �°� ���� ����ȯ�ؼ� ����
                                valueList.Add(ExcelUtility.GetValue(selectColumnTypeList[i], item.ToString()));
                            }
                            //�����Ϳ� ������ ���� ���
                            else
                            {
                                //Ÿ�Կ� ���� ����Ʈ ���� ����
                                switch(selectColumnTypeList[i])
                                {
                                    case eDataType.Bool:
                                        valueList.Add(false);
                                        break;
                                    case eDataType.Int:
                                    case eDataType.Long:
                                        valueList.Add(0);
                                        break;
                                    case eDataType.String:
                                        valueList.Add(string.Empty);
                                        break;
                                }
                            }

                            //CSV �ؽ�Ʈ ����
                            rowText = rowText == string.Empty ? item.ToString() : $"{rowText},{item}";
                        }

                        //���� �� CSV �� �ٲ��� ���� "" ����
                        csvList.Add(rowText);

                        TableBase tableObj = (TableBase)Activator.CreateInstance(type, valueList.ToArray());
                        dicTable.Add(tableObj.GetKey, tableObj);
                    }
                    #endregion ������ ����ִ� �ι�° ����� ����

                    tData = new TableData(dicTable);
                }
            }

            #region ������ ����
            if (tData != null)
            {
                //���̳ʸ� ����
                using (FileStream binaryFile = new FileStream(selectTableBytesPath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(binaryFile, tData);
                }

                //CSV ����
                using (StreamWriter csvWriter = new StreamWriter(selectTableCSVPath, false, System.Text.Encoding.UTF8))
                {
                    csvWriter.Flush();
                    for (int i = 0; i < csvList.Count; ++i)
                    {
                        csvWriter.WriteLine(csvList[i]);
                    }

                    EditorUtility.DisplayDialog("���̳ʸ�,CSV ����/����", "�Ϸ�", "Ȯ��");
                }
            }
            else
            {
                EditorUtility.DisplayDialog("���̳ʸ� ����/����", $"���� : ���̺� ������ ������ �����߽��ϴ�.", "Ȯ��");
                return;
            }
            #endregion ������ ����
        }
        #endregion ���������� CSV,���̳ʸ��� ��ȯ

        #region ���������� CS�� ��ȯ
        private void ConvertExcelToCS()
        {
            //CS���� ������ �ʿ��� ������ ����
            scGenerator.SetExcelData(selectTableName, selectColumnNameList, selectColumnTypeList);

            //CS ��ũ��Ʈ
            string csText = scGenerator.ConvertExcelToCSText();

            //�����͸� ����
            using (FileStream file = File.Open(selectTableCSPath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter writer = new StreamWriter(file))
                {
                    writer.Flush();
                    writer.Write(csText);
                }

                EditorUtility.DisplayDialog("CS ����/����", "�Ϸ�", "Ȯ��");
            }
        }
        #endregion ���������� CS�� ��ȯ
    }

    #region ���������� ��ƿ��Ƽ
    public static class ExcelUtility
    {
        /// <summary> string�� �´� eDataType�� ��ȯ </summary>
        public static string ConverteDataTypeToString(eDataType type)
        {
            return type switch
            {
                eDataType.String => "string",
                eDataType.Int => "int",
                eDataType.Long => "long",
                eDataType.Bool => "bool",
                _ => string.Empty,
            };
        }

        /// <summary> string�� �´� eDataType�� ��ȯ </summary>
        public static eDataType ConvertStringToeDataType(string typeName)
        {
            return typeName switch
            {
                "string" => eDataType.String,
                "int" => eDataType.Int,
                "long" => eDataType.Long,
                "bool" => eDataType.Bool,
                _ => eDataType.None,
            };
        }

        /// <summary> ������ Ÿ������ ���� ����ȯ�ؼ� ��ȯ </summary>
        /// <param name="type"> ���� Ÿ�� </param>
        /// <param name="value"> ���̺� </param>
        /// <returns> ���� ����� �������� ������ string���� ��ȯ </returns>
        public static object GetValue(eDataType type, string value)
        {
            switch (type)
            {
                case eDataType.Int:
                    return int.Parse(value);
                case eDataType.Long:
                    return long.Parse(value);
                case eDataType.String :
                    return value;
                case eDataType.Bool :
                    return bool.Parse(value);
                default:
                    return value;
            }
        }
    }
    #endregion ���������� ��ƿ��Ƽ
}