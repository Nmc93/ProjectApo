using System;
using UnityEngine;
 
[Serializable]
public class UnitRandomTableData : TableBase
{
	public override string TableName { get => "UnitRandomTable"; }
	public override object GetKey { get => iD; }

	public UnitRandomTableData (int iD, int unitType, string head, string hair, string backHair, string faceDeco, string hat, string headAnim, string bodyAnim, string stat) 
	{
		this.iD = iD;
		this.unitType = unitType;
		this.head = head;
		this.hair = hair;
		this.backHair = backHair;
		this.faceDeco = faceDeco;
		this.hat = hat;
		this.headAnim = headAnim;
		this.bodyAnim = bodyAnim;
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
	private string headAnim;
	public string HeadAnim { get => headAnim; }
	
	[SerializeField]
	private string bodyAnim;
	public string BodyAnim { get => bodyAnim; }
	
	[SerializeField]
	private string stat;
	public string Stat { get => stat; }
}