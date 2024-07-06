using System;

namespace FileManagerLibrary.IniFile
{
	[AttributeUsage(AttributeTargets.Property)]
	public class KeyNameAttribute:Attribute
	{
		public KeyNameAttribute(string keyName) 
		{
			this.KeyName = keyName;
		}
		public string KeyName { get;private set; }
	}
}
