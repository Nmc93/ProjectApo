using System;
using UnityEngine;
 
[Serializable]
public class UnitWeaponTableData : TableBase
{
	public override string TableName { get => "UnitWeaponTable"; }
	public override object GetKey { get => iD; }

	public UnitWeaponTableData (int iD, string category, string label, int damage, int speed) 
	{
		this.iD = iD;
		this.category = category;
		this.label = label;
		this.damage = damage;
		this.speed = speed;
	}
	
	[SerializeField]
	private int iD;
	public int ID { get => iD; }
	
	[SerializeField]
	private string category;
	public string Category { get => category; }
	
	[SerializeField]
	private string label;
	public string Label { get => label; }
	
	[SerializeField]
	private int damage;
	public int Damage { get => damage; }
	
	[SerializeField]
	private int speed;
	public int Speed { get => speed; }
}