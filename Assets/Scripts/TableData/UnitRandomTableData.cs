using System;
using UnityEngine;
 
[Serializable]
public class UnitRandomTableData : TableBase
{
	public override string TableName { get => "UnitRandomTable"; }
	public override object GetKey { get => id; }

	public UnitRandomTableData (int id, int unittype, string head, string hair, string backhair, string facedeco, string hat, string headanim, string bodyanim, string stat) 
	{
		this.id = id;
		this.unittype = unittype;
		this.head = head;
		this.hair = hair;
		this.backhair = backhair;
		this.facedeco = facedeco;
		this.hat = hat;
		this.headanim = headanim;
		this.bodyanim = bodyanim;
		this.stat = stat;
	}
	
	[SerializeField]
	private int id;
	public int ID { get => id; }
	
	[SerializeField]
	private int unittype;
	public int UnitType { get => unittype; }
	
	[SerializeField]
	private string head;
	public string Head { get => head; }
	
	[SerializeField]
	private string hair;
	public string Hair { get => hair; }
	
	[SerializeField]
	private string backhair;
	public string BackHair { get => backhair; }
	
	[SerializeField]
	private string facedeco;
	public string FaceDeco { get => facedeco; }
	
	[SerializeField]
	private string hat;
	public string Hat { get => hat; }
	
	[SerializeField]
	private string headanim;
	public string HeadAnim { get => headanim; }
	
	[SerializeField]
	private string bodyanim;
	public string BodyAnim { get => bodyanim; }
	
	[SerializeField]
	private string stat;
	public string Stat { get => stat; }
}