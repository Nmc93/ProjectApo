using System;
using UnityEngine;
 
[Serializable]
public class UnitStatTableData : TableBase
{
	public override string TableName { get => "UnitStatTable"; }
	public override object GetKey { get => id; }

	public UnitStatTableData (int id, int unittype, int maxhp, int minhp, int maxdmg, int mindmg, int maxdef, int mindef, int maxattspeed, int minattspeed, int maxmovespeed, int minmovespeed, int maxsearchsize, int minsearchsize) 
	{
		this.id = id;
		this.unittype = unittype;
		this.maxhp = maxhp;
		this.minhp = minhp;
		this.maxdmg = maxdmg;
		this.mindmg = mindmg;
		this.maxdef = maxdef;
		this.mindef = mindef;
		this.maxattspeed = maxattspeed;
		this.minattspeed = minattspeed;
		this.maxmovespeed = maxmovespeed;
		this.minmovespeed = minmovespeed;
		this.maxsearchsize = maxsearchsize;
		this.minsearchsize = minsearchsize;
	}
	
	[SerializeField]
	private int id;
	public int ID { get => id; }
	
	[SerializeField]
	private int unittype;
	public int UnitType { get => unittype; }
	
	[SerializeField]
	private int maxhp;
	public int MaxHp { get => maxhp; }
	
	[SerializeField]
	private int minhp;
	public int MinHp { get => minhp; }
	
	[SerializeField]
	private int maxdmg;
	public int MaxDmg { get => maxdmg; }
	
	[SerializeField]
	private int mindmg;
	public int MinDmg { get => mindmg; }
	
	[SerializeField]
	private int maxdef;
	public int MaxDef { get => maxdef; }
	
	[SerializeField]
	private int mindef;
	public int MinDef { get => mindef; }
	
	[SerializeField]
	private int maxattspeed;
	public int MaxAttSpeed { get => maxattspeed; }
	
	[SerializeField]
	private int minattspeed;
	public int MinAttSpeed { get => minattspeed; }
	
	[SerializeField]
	private int maxmovespeed;
	public int MaxMoveSpeed { get => maxmovespeed; }
	
	[SerializeField]
	private int minmovespeed;
	public int MinMoveSpeed { get => minmovespeed; }
	
	[SerializeField]
	private int maxsearchsize;
	public int MaxSearchSize { get => maxsearchsize; }
	
	[SerializeField]
	private int minsearchsize;
	public int MinSearchSize { get => minsearchsize; }
}