using System;
using UnityEngine;
 
[Serializable]
public class UnitSpriteTableData : TableBase
{
	public override string TableName { get => "UnitSpriteTable"; }
	public override object GetKey { get => id; }

	public UnitSpriteTableData (int id, int groupid, int unittype, int parttype, string path) 
	{
		this.id = id;
		this.groupid = groupid;
		this.unittype = unittype;
		this.parttype = parttype;
		this.path = path;
	}
	
	[SerializeField]
	private int id;
	public int ID { get => id; }
	
	[SerializeField]
	private int groupid;
	public int GroupID { get => groupid; }
	
	[SerializeField]
	private int unittype;
	public int UnitType { get => unittype; }
	
	[SerializeField]
	private int parttype;
	public int PartType { get => parttype; }
	
	[SerializeField]
	private string path;
	public string Path { get => path; }
}