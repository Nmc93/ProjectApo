using System;
using UnityEngine;
 
[Serializable]
public class UnitRandomTableData : TableBase
{
	public override string TableName { get => "UnitRandomTable"; }
	public override object GetKey { get => iD; }

	public UnitRandomTableData (int iD, int unitType, string head, string face, string hair, string backHair, string faceDeco, string hat, string headLib, string bodyLib, string stat) 
	{
		this.iD = iD;
		this.unitType = unitType;
		this.head = head;
		this.face = face;
		this.hair = hair;
		this.backHair = backHair;
		this.faceDeco = faceDeco;
		this.hat = hat;
		this.headLib = headLib;
		this.bodyLib = bodyLib;
		this.stat = stat;
	}
	
	[SerializeField]
	private int iD;
	public int ID { get => iD; }
	
	[SerializeField]
	private int unitType;
	public int UnitType { get => unitType; }
	
	[SerializeField]
	private string head;
	public string Head { get => head; }
	
	[SerializeField]
	private string face;
	public string Face { get => face; }
	
	[SerializeField]
	private string hair;
	public string Hair { get => hair; }
	
	[SerializeField]
	private string backHair;
	public string BackHair { get => backHair; }
	
	[SerializeField]
	private string faceDeco;
	public string FaceDeco { get => faceDeco; }
	
	[SerializeField]
	private string hat;
	public string Hat { get => hat; }
	
	[SerializeField]
	private string headLib;
	public string HeadLib { get => headLib; }
	
	[SerializeField]
	private string bodyLib;
	public string BodyLib { get => bodyLib; }
	
	[SerializeField]
	private string stat;
	public string Stat { get => stat; }
}