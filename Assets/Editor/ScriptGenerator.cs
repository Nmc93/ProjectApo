using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExcelEdit;

namespace ExcelEdit
{
    public class ScriptGenerator
    {
        #region Const Template

        /// <summary> Ŭ���� �̸� ���ø�<br/> $CSName$ : ���̺��̸�</summary>
        const string csNameTemplate = "$CSName$Data";
        /// <summary> Ŭ���� ���� ���ø� <br/> $CSName$ : �̸� <br/> $TblName$ : ���̺��̸�<br/> $GetKey$ : Ű�� <br/> $Const$ : ������ <br/> $Values$ : ������ ������Ƽ</summary>
        const string csBodyTemplate = "using System;\nusing UnityEngine;\n \n[Serializable]\npublic class $CSName$ : TableBase\n{\n\t$TblName$\n\t$GetKey$\n\n$Const$\n$Values$\n}";

        /// <summary>���̺� �̸� ������Ƽ <br/> $Name$ : �̸� </summary>
        const string csTblNameTemplate = "public override string TableName { get => \"$Name$\"; }";
        /// <summary> Ű�� <br/> $Key$ : Ű�� </summary>
        const string csGetKeyTemplat = "public override object GetKey { get => $Key$; }";

        /// <summary> ������ ������Ƽ <br/> $Type$ : Ÿ��<br/> $LowName$ : �ҹ��� �����̸�<br/> $Name$ : ���� �̸�) </summary>
        const string csParamTemplate = "\n\t[SerializeField]\n\tprivate $Type$ $LowName$;\n\tpublic $Type$ $Name$ { get => $LowName$; }";

        /// <summary> ������ <br/> $CSName$ : Ŭ���� �̸�<br/> $Params$ : �Ű�����<br/> $Value$ : ������ </summary>
        const string csConstBody = "\tpublic $CSName$ ($Params$) \n\t{\n$Value$\n\t}";
        /// <summary> ������ �Ű�����<br/> $Type$ : ���� Ÿ��<br/> $LowName$ : ���� �̸� </summary>
        const string csConstParam = "$Type$ $LowName$";
        /// <summary> ������ ������ ����<br/> $Name$ : name </summary>
        const string csConstvalue = "this.$LowName$ = $LowName$;";

        #endregion Const Template

        #region ���̺� ���� ����
        /// <summary> ���̺� �̸� </summary>
        private string tableName;
        /// <summary> ���̺� �� �̸� </summary>
        private List<string> columnNameList = new List<string>();
        /// <summary> ���̺� �� Ÿ�� </summary>
        private List<eDataType> columnTypeList = new List<eDataType>();
        #endregion  ���̺� ���� ����

        #region ���������� CS�� ��ȯ

        /// <summary> ������ ���� </summary>
        public void SetExcelData(string tableName, List<string> columnNameList, List<eDataType> columnTypeList)
        {
            this.tableName = tableName;
            this.columnNameList = columnNameList;
            this.columnTypeList = columnTypeList;
        }

        /// <summary> ������ CS ���Ϸ� ���� </summary>
        public string ConvertExcelToCSText()
        {
            //1. Ŭ���� �̸�
            string tableCSName = csNameTemplate.Replace("$CSName$", tableName);

            //2. ������ ������Ƽ
            string cValue = string.Empty;
            #region ������ ������Ƽ ����

            for (int i = 0; i < columnNameList.Count; ++i)
            {
                if (string.IsNullOrEmpty(cValue))
                {
                    if (columnTypeList[i] != eDataType.None)
                    {
                        //ex : private int a;
                        //     public int A { get => a; }
                        cValue = string.Format("\t{0}", csParamTemplate
                        .Replace("$Type$", ExcelUtility.ConverteDataTypeToString(columnTypeList[i]))
                        .Replace("$LowName$", columnNameList[i].ToLower())
                        .Replace("$Name$", columnNameList[i]));
                    }
                }
                else
                {
                    if (columnTypeList[i] != eDataType.None)
                    {
                        //ex : private int a;
                        //     public int A { get => a; }
                        //     private int b;
                        //     public int B { get => b; }
                        cValue = string.Format("{0}\n\t{1}", cValue, csParamTemplate
                        .Replace("$Type$", ExcelUtility.ConverteDataTypeToString(columnTypeList[i]))
                        .Replace("$LowName$", columnNameList[i].ToLower())
                        .Replace("$Name$", columnNameList[i]));
                    }
                }
            }

            #endregion ������ ������Ƽ ����

            //3. ������ ���� - �������� �Ű������� ������ ����
            string constString1 = string.Empty;
            string constString2 = string.Empty;
            string cConstBody = string.Empty;
            #region ������ ����
            for (int i = 0; i < columnNameList.Count; ++i)
            {
                if (string.IsNullOrEmpty(constString1))
                {
                    if (columnTypeList[i] != eDataType.None)
                    {
                        //ex : int a
                        constString1 = csConstParam
                            .Replace("$Type$", ExcelUtility.ConverteDataTypeToString(columnTypeList[i]))
                            .Replace("$LowName$", columnNameList[i].ToLower());
                        //ex : this.a = a;
                        constString2 = string.Format("\t\t{0}", csConstvalue
                            .Replace("$LowName$", columnNameList[i].ToLower()));
                    }
                }
                else
                {
                    if (columnTypeList[i] != eDataType.None)
                    {
                        //ex : int a,int b
                        constString1 = string.Format("{0}, {1}", constString1, csConstParam
                            .Replace("$Type$", ExcelUtility.ConverteDataTypeToString(columnTypeList[i]))
                            .Replace("$LowName$", columnNameList[i].ToLower()));
                        //ex : this.a = a;
                        //     this.b = b;
                        constString2 = string.Format("{0}\n\t\t{1}", constString2, csConstvalue
                            .Replace("$LowName$", columnNameList[i].ToLower()));
                    }
                }
            }

            //3-1 ������ ����
            cConstBody = csConstBody
                .Replace("$CSName$", tableCSName)
                .Replace("$Params$", constString1)
                .Replace("$Value$", constString2);

            #endregion ������ ����

            //���̺� �̸� ������Ƽ ����
            string tblName = csTblNameTemplate.Replace("$Name$", tableName);
            //Ű�� ������Ƽ ����
            string getKey = csGetKeyTemplat.Replace("$Key$", columnNameList[0].ToLower());

            //4. Ŭ���� ���� ����
            string cBody = csBodyTemplate
                .Replace("$CSName$", tableCSName)
                .Replace("$TblName$", tblName)
                .Replace("$GetKey$", getKey)
                .Replace("$Const$", cConstBody)
                .Replace("$Values$", cValue);
            
            return cBody;
        }
        #endregion ���������� CS�� ��ȯ��
    }
}