using System;
using UnityEngine;
 
[Serializable]
public class UnitSpriteTableData : TableBase
{
	public override string TableName { get => "UnitSpriteTable"; }
	public override object GetKey { get => iD; }

	public UnitSpriteTableData (int iD, int groupID, int unitType, string category, string label) 
	{
		this.iD = iD;
		this.groupID = groupID;
		this.unitType = unitType;
		this.category = category;
		this.label = label;
	}
	
	[SerializeField]
	private int iD;
	public int ID { get => iD; }
	
	[SerializeField]
	private int groupID;
	public int GroupID { get => groupID; }
	
	[SerializeField]
	private int unitType;
	public int UnitType { get => unitType; }
	
	[SerializeField]
	private string category;
	public string Category { get => category; }
	
	[SerializeField]
	private string label;
	public string Label { get => label; }
}