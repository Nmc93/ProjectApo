using System;
using UnityEngine;
 
[Serializable]
public class OptionTableData : TableBase
{
	public override string TableName { get => "OptionTable"; }
	public override object GetKey { get => iD; }

	public OptionTableData (string iD, string optionValue) 
	{
		this.iD = iD;
		this.optionValue = optionValue;
	}
	
	[SerializeField]
	private string iD;
	public string ID { get => iD; }
	
	[SerializeField]
	private string optionValue;
	public string OptionValue { get => optionValue; }
}