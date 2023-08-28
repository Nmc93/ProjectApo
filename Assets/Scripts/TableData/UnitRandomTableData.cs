using System;
using UnityEngine;
 
[Serializable]
public class UnitRandomTableData : TableBase
{
	public override string TableName { get => "UnitRandomTable"; }
	public override object GetKey { get => id; }

	public UnitRandomTableData (int id, int unittype, string head, string hair, string backhair, string face, string facedeco, string hat, string body, string stat) 
	{
		this.id = id;
		this.unittype = unittype;
		this.head = head;
		this.hair = hair;
		this.backhair = backhair;
		this.face = face;
		this.facedeco = facedeco;
		this.hat = hat;
		this.body = body;
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
	private string face;
	public string Face { get => face; }
	
	[SerializeField]
	private string facedeco;
	public string FaceDeco { get => facedeco; }
	
	[SerializeField]
	private string hat;
	public string Hat { get => hat; }
	
	[SerializeField]
	private string body;
	public string Body { get => body; }
	
	[SerializeField]
	private string stat;
	public string Stat { get => stat; }
}