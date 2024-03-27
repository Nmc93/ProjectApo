using System;
using UnityEngine;
 
[Serializable]
public class UnitSpriteTableData : TableBase
{
	public override string TableName { get => "UnitSpriteTable"; }
	public override object GetKey { get => iD; }

	public UnitSpriteTableData (int iD, int groupID, int unitType, int partType, string path) 
	{
		this.iD = iD;
		this.groupID = groupID;
		this.unitType = unitType;
		this.partType = partType;
		this.path = path;
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
	private int partType;
	public int PartType { get => partType; }
	
	[SerializeField]
	private string path;
	public string Path { get => path; }
}