using System;
using UnityEngine;
 
[Serializable]
public class StringTableData : TableBase
{
	public override string TableName { get => "StringTable"; }
	public override object GetKey { get => iD; }

	public StringTableData (string iD, string text) 
	{
		this.iD = iD;
		this.text = text;
	}
	
	[SerializeField]
	private string iD;
	public string ID { get => iD; }
	
	[SerializeField]
	private string text;
	public string Text { get => text; }
}