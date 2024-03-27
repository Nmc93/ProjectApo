using System;
using UnityEngine;
 
[Serializable]
public class UnitWeaponTableData : TableBase
{
	public override string TableName { get => "UnitWeaponTable"; }
	public override object GetKey { get => iD; }

	public UnitWeaponTableData (int iD, int weaponType, string path) 
	{
		this.iD = iD;
		this.weaponType = weaponType;
		this.path = path;
	}
	
	[SerializeField]
	private int iD;
	public int ID { get => iD; }
	
	[SerializeField]
	private int weaponType;
	public int WeaponType { get => weaponType; }
	
	[SerializeField]
	private string path;
	public string Path { get => path; }
}