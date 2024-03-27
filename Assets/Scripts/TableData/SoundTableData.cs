using System;
using UnityEngine;
 
[Serializable]
public class SoundTableData : TableBase
{
	public override string TableName { get => "SoundTable"; }
	public override object GetKey { get => iD; }

	public SoundTableData (int iD, string path, int soundType, bool isLoop) 
	{
		this.iD = iD;
		this.path = path;
		this.soundType = soundType;
		this.isLoop = isLoop;
	}
	
	[SerializeField]
	private int iD;
	public int ID { get => iD; }
	
	[SerializeField]
	private string path;
	public string Path { get => path; }
	
	[SerializeField]
	private int soundType;
	public int SoundType { get => soundType; }
	
	[SerializeField]
	private bool isLoop;
	public bool IsLoop { get => isLoop; }
}