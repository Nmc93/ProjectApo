using System;
using UnityEngine;
 
[Serializable]
public class UnitStatTableData : TableBase
{
	public override string TableName { get => "UnitStatTable"; }
	public override object GetKey { get => iD; }

	public UnitStatTableData (int iD, int unitType, int maxHp, int minHp, int maxDmg, int minDmg, int maxDef, int minDef, int maxAttSpeed, int minAttSpeed, int maxMoveSpeed, int minMoveSpeed, int maxReactionSpeed, int minxReactionSpeed, int maxDetectionRange, int minDetectionRange) 
	{
		this.iD = iD;
		this.unitType = unitType;
		this.maxHp = maxHp;
		this.minHp = minHp;
		this.maxDmg = maxDmg;
		this.minDmg = minDmg;
		this.maxDef = maxDef;
		this.minDef = minDef;
		this.maxAttSpeed = maxAttSpeed;
		this.minAttSpeed = minAttSpeed;
		this.maxMoveSpeed = maxMoveSpeed;
		this.minMoveSpeed = minMoveSpeed;
		this.maxReactionSpeed = maxReactionSpeed;
		this.minxReactionSpeed = minxReactionSpeed;
		this.maxDetectionRange = maxDetectionRange;
		this.minDetectionRange = minDetectionRange;
	}
	
	[SerializeField]
	private int iD;
	public int ID { get => iD; }
	
	[SerializeField]
	private int unitType;
	public int UnitType { get => unitType; }
	
	[SerializeField]
	private int maxHp;
	public int MaxHp { get => maxHp; }
	
	[SerializeField]
	private int minHp;
	public int MinHp { get => minHp; }
	
	[SerializeField]
	private int maxDmg;
	public int MaxDmg { get => maxDmg; }
	
	[SerializeField]
	private int minDmg;
	public int MinDmg { get => minDmg; }
	
	[SerializeField]
	private int maxDef;
	public int MaxDef { get => maxDef; }
	
	[SerializeField]
	private int minDef;
	public int MinDef { get => minDef; }
	
	[SerializeField]
	private int maxAttSpeed;
	public int MaxAttSpeed { get => maxAttSpeed; }
	
	[SerializeField]
	private int minAttSpeed;
	public int MinAttSpeed { get => minAttSpeed; }
	
	[SerializeField]
	private int maxMoveSpeed;
	public int MaxMoveSpeed { get => maxMoveSpeed; }
	
	[SerializeField]
	private int minMoveSpeed;
	public int MinMoveSpeed { get => minMoveSpeed; }
	
	[SerializeField]
	private int maxReactionSpeed;
	public int MaxReactionSpeed { get => maxReactionSpeed; }
	
	[SerializeField]
	private int minxReactionSpeed;
	public int MinxReactionSpeed { get => minxReactionSpeed; }
	
	[SerializeField]
	private int maxDetectionRange;
	public int MaxDetectionRange { get => maxDetectionRange; }
	
	[SerializeField]
	private int minDetectionRange;
	public int MinDetectionRange { get => minDetectionRange; }
}