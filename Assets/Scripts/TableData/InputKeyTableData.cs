using System;
using UnityEngine;
 
[Serializable]
public class InputKeyTableData : TableBase
{
	public override string TableName { get => "InputKeyTable"; }
	public override object GetKey { get => iD; }

	public InputKeyTableData (string iD, string keyString) 
	{
		this.iD = iD;
		this.keyString = keyString;
	}
	
	[SerializeField]
	private string iD;
	public string ID { get => iD; }
	
	[SerializeField]
	private string keyString;
	public string KeyString { get => keyString; }
}