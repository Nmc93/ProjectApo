using System;
using UnityEngine;
 
[Serializable]
public class UnitWeaponTableData : TableBase
{
	public override string TableName { get => "UnitWeaponTable"; }
	public override object GetKey { get => id; }

	public UnitWeaponTableData (int id, int weapontype, string path) 
	{
		this.id = id;
		this.weapontype = weapontype;
		this.path = path;
	}
	
	[SerializeField]
	private int id;
	public int ID { get => id; }
	
	[SerializeField]
	private int weapontype;
	public int WeaponType { get => weapontype; }
	
	[SerializeField]
	private string path;
	public string Path { get => path; }
}