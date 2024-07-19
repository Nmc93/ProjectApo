using System;
using UnityEngine;
 
[Serializable]
public class UnitWeaponTableData : TableBase
{
	public override string TableName { get => "UnitWeaponTable"; }
	public override object GetKey { get => iD; }

	public UnitWeaponTableData (int iD, int weaponType, string path, int damage, int speed) 
	{
		this.iD = iD;
		this.weaponType = weaponType;
		this.path = path;
		this.damage = damage;
		this.speed = speed;
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
	
	[SerializeField]
	private int damage;
	public int Damage { get => damage; }
	
	[SerializeField]
	private int speed;
	public int Speed { get => speed; }
}