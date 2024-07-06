using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace FileManagerLibrary.IniFile
{
	public abstract class IniDataFileBase:IDataFile
	{
		
		#region API32から引用(kernel32.dll)
		// ini ﾌｧｲﾙの読み込み用の関数(GetPrivateProfileString)の宣言部分
		[DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileString")]
		private static extern uint GetPrivateProfileString(string lpApplicationName, string lpKeyName, string lpDefault, System.Text.StringBuilder lpReturnedString, uint nSize, string lpFileName);

		// ini ﾌｧｲﾙの書き込み用の関数(_WritePrivateProfileString)の宣言部分
		[DllImport("kernel32.dll", EntryPoint = "_WritePrivateProfileString")]
		private static extern uint _WritePrivateProfileString(string lpApplicationName, string lpKeyName, string lpString, string lpFileName);

		// ini ﾌｧｲﾙの書き換え用の関数(_WritePrivateProfileSection)の宣言部分
		[DllImport("kernel32.dll", EntryPoint = "_WritePrivateProfileSection")]
		private static extern bool _WritePrivateProfileSection(string lpApplicationName, string lpString, string lpFileName);
		#endregion
		private const string DATETIME_FORMAT = "yyyy/MM/dd HH/mm/ss.fff";
		public abstract string			FilePath { get; protected set; }
		public IniDataFileBase() 
		{

		}
		public Exception LastException { get; protected set; }
		public abstract bool Load(bool shouldCreate = true);

		public abstract bool Save(FileMode mode = FileMode.Create);
		public void SetFilePath(string filePath)
		{
			this.FilePath = filePath;
		}
		protected string _GetKeyName(string propertyName)
		{
			string ret = "";
			Type myClassType = this.GetType();
			PropertyInfo info=myClassType.GetProperty(propertyName);
			KeyNameAttribute customAttributes =info.GetCustomAttribute<KeyNameAttribute>();
			if (customAttributes != null)
			{
				ret=customAttributes.KeyName;
			}
			else
			{
				throw new NullReferenceException();
			}
			return ret;
		}
		#region リード
		/// <summary>
		/// データを読み取ります
		/// </summary>
		/// <param name="section">セクション</param>
		/// <param name="key">キー</param>
		/// <param name="readValue">取得値</param>
		/// <param name="defaultValue">規定値</param>
		/// <returns>読み込みに成功したか</returns>
		protected bool _Read(string section, string key,out string readValue , string defaultValue="")
		{
			var ret = false;
			var sb = new StringBuilder(){
				Capacity = 256
			} ;
			try
			{
				GetPrivateProfileString(section, key, defaultValue, sb, (uint)sb.Capacity, this.FilePath);

				ret = true;
			}
			catch (Exception ex)
			{
				this.LastException = ex;
				ret = false;
			}
			finally
			{
				readValue = sb.ToString();
				sb.Clear();
				sb = null;
			}
			return ret;
		}
		/// <summary>
		/// データを読み取ります
		/// </summary>
		/// <param name="section">セクション</param>
		/// <param name="key">キー</param>
		/// <param name="readValue">取得値</param>
		/// <param name="defaultValue">規定値</param>
		/// <returns>読み込みに成功したか</returns>
		protected bool _Read(string section, string key, out int readValue, int defaultValue = 0)
		{
			var ret = false;
			readValue = defaultValue;
			if (_Read(section, key, out var text,  defaultValue.ToString()) && int.TryParse(text, out readValue))
			{
				ret = true;
			}
			return ret;
		}
		/// <summary>
		/// データを読み取ります
		/// </summary>
		/// <param name="section">セクション</param>
		/// <param name="key">キー</param>
		/// <param name="readValue">取得値</param>
		/// <param name="defaultValue">規定値</param>
		/// <returns>読み込みに成功したか</returns>
		protected bool _Read(string section, string key, out uint readValue, uint defaultValue = 0)
		{
			var ret = false;
			readValue = defaultValue;
			if (_Read(section, key, out var text,  defaultValue.ToString()) && uint.TryParse(text, out readValue))
			{
				ret = true;
			}
			return ret;
		}
		/// <summary>
		/// データを読み取ります
		/// </summary>
		/// <param name="section">セクション</param>
		/// <param name="key">キー</param>
		/// <param name="readValue">取得値</param>
		/// <param name="defaultValue">規定値</param>
		/// <returns>読み込みに成功したか</returns>
		protected bool _Read(string section, string key, out long readValue, long defaultValue = 0)
		{
			var ret = false;
			readValue = defaultValue;
			if (_Read(section, key, out var text,  defaultValue.ToString()) && long.TryParse(text, out readValue))
			{
				ret = true;
			}
			return ret;
		}
		/// <summary>
		/// データを読み取ります
		/// </summary>
		/// <param name="section">セクション</param>
		/// <param name="key">キー</param>
		/// <param name="readValue">取得値</param>
		/// <param name="defaultValue">規定値</param>
		/// <returns>読み込みに成功したか</returns>
		protected bool _Read(string section, string key, out ulong readValue, ulong defaultValue = 0)
		{
			var ret = false;
			readValue = defaultValue;
			if (_Read(section, key, out var text,defaultValue.ToString()) && ulong.TryParse(text, out readValue))
			{
				ret = true;
			}
			return ret;
		}
		/// <summary>
		/// データを読み取ります
		/// </summary>
		/// <param name="section">セクション</param>
		/// <param name="key">キー</param>
		/// <param name="readValue">取得値</param>
		/// <param name="defaultValue">規定値</param>
		/// <returns>読み込みに成功したか</returns>
		protected bool _Read(string section, string key, out byte readValue, byte defaultValue = 0)
		{
			var ret = false;
			readValue = defaultValue;
			if (_Read(section, key, out var text,  defaultValue.ToString()) && byte.TryParse(text, out readValue))
			{
				ret = true;
			}
			return ret;
		}
		/// <summary>
		/// データを読み取ります
		/// </summary>
		/// <param name="section">セクション</param>
		/// <param name="key">キー</param>
		/// <param name="readValue">取得値</param>
		/// <param name="defaultValue">規定値</param>
		/// <returns>読み込みに成功したか</returns>
		protected bool _Read(string section, string key, out short readValue, short defaultValue = 0)
		{
			var ret = false;
			readValue = defaultValue;
			if (_Read(section, key, out var text,  defaultValue.ToString()) && short.TryParse(text, out readValue))
			{
				ret = true;
			}
			return ret;
		}
		/// <summary>
		/// データを読み取ります
		/// </summary>
		/// <param name="section">セクション</param>
		/// <param name="key">キー</param>
		/// <param name="readValue">取得値</param>
		/// <param name="defaultValue">規定値</param>
		/// <returns>読み込みに成功したか</returns>
		protected bool _Read(string section, string key, out ushort readValue, ushort defaultValue = 0)
		{
			var ret = false;
			readValue = defaultValue;
			if (_Read(section, key, out var text, defaultValue.ToString()) && ushort.TryParse(text, out readValue))
			{
				ret = true;
			}
			return ret;
		}
		/// <summary>
		/// データを読み取ります
		/// </summary>
		/// <param name="section">セクション</param>
		/// <param name="key">キー</param>
		/// <param name="readValue">取得値</param>
		/// <param name="defaultValue">規定値</param>
		/// <returns>読み込みに成功したか</returns>
		protected bool _Read(string section, string key, out sbyte readValue, sbyte defaultValue = 0)
		{
			var ret = false;
			readValue = defaultValue;
			if (_Read(section, key, out var text, defaultValue.ToString()) && sbyte.TryParse(text, out readValue))
			{
				ret = true;
			}
			return ret;
		}
		/// <summary>
		/// データを読み取ります
		/// </summary>
		/// <param name="section">セクション</param>
		/// <param name="key">キー</param>
		/// <param name="readValue">取得値</param>
		/// <param name="defaultValue">規定値</param>
		/// <returns>読み込みに成功したか</returns>
		protected bool _Read(string section, string key, out float readValue, float defaultValue = 0)
		{
			var ret = false;
			readValue = defaultValue;
			if (_Read(section, key, out var text,  defaultValue.ToString()) && float.TryParse(text, out readValue))
			{
				ret = true;
			}
			return ret;
		}
		/// <summary>
		/// データを読み取ります
		/// </summary>
		/// <param name="section">セクション</param>
		/// <param name="key">キー</param>
		/// <param name="readValue">取得値</param>
		/// <param name="defaultValue">規定値</param>
		/// <returns>読み込みに成功したか</returns>
		protected bool _Read(string section, string key, out double readValue, double defaultValue = 0)
		{
			var ret = false;
			readValue = defaultValue;
			if (_Read(section, key, out var text,  defaultValue.ToString()) && double.TryParse(text, out readValue))
			{
				ret = true;
			}
			return ret;
		}
		/// <summary>
		/// データを読み取ります
		/// </summary>
		/// <param name="section">セクション</param>
		/// <param name="key">キー</param>
		/// <param name="readValue">取得値</param>
		/// <param name="defaultValue">規定値</param>
		/// <returns>読み込みに成功したか</returns>
		protected bool _Read(string section, string key, out decimal readValue, decimal defaultValue = 0)
		{
			var ret = false;
			readValue = defaultValue;
			if (_Read(section, key, out var text, defaultValue.ToString()) && decimal.TryParse(text, out readValue))
			{
				ret = true;
			}
			return ret;
		}
		/// <summary>
		/// データを読み取ります
		/// </summary>
		/// <param name="section">セクション</param>
		/// <param name="key">キー</param>
		/// <param name="readValue">取得値</param>
		/// <param name="defaultValue">規定値</param>
		/// <returns>読み込みに成功したか</returns>
		protected bool _Read(string section, string key, out bool readValue, bool defaultValue = default)
		{
			var ret = false;
			readValue = defaultValue;
			if (this._Read(section, key, out var text, defaultValue.ToString()) && bool.TryParse(text, out readValue))
			{
				ret = true;
			}
			return ret;
		}
		/// <summary>
		/// データを読み取ります
		/// </summary>
		/// <param name="section">セクション</param>
		/// <param name="key">キー</param>
		/// <param name="readValue">取得値</param>
		/// <param name="defaultValue">規定値</param>
		/// <returns>読み込みに成功したか</returns>
		protected bool _Read(string section, string key, out char readValue, char defaultValue = default)
		{
			var ret = false;
			readValue = defaultValue;
			if (this._Read(section, key, out var text, defaultValue.ToString()) && char.TryParse(text, out readValue))
			{
				ret = true;
			}
			return ret;
		}
		/// <summary>
		/// データを読み取ります
		/// </summary>
		/// <param name="section">セクション</param>
		/// <param name="key">キー</param>
		/// <param name="readValue">取得値</param>
		/// <param name="defaultValue">規定値</param>
		/// <returns>読み込みに成功したか</returns>
		protected bool _Read<TEnum>(string section, string key, out TEnum readValue, TEnum defaultValue=default) where TEnum : struct,Enum
		{
			var ret = false;
			readValue = defaultValue;
			if (_Read(section, key, out var text, defaultValue.ToString()) && Enum.TryParse<TEnum>(text, out readValue))
			{
				ret = true;
			}
			return ret;
		}
		/// <summary>
		/// データを読み取ります
		/// </summary>
		/// <param name="section">セクション</param>
		/// <param name="key">キー</param>
		/// <param name="readValue">取得値</param>
		/// <param name="defaultValue">規定値</param>
		/// <returns>読み込みに成功したか</returns>
		protected virtual bool _Read(string section, string key, out DateTime readValue, DateTime defaultValue = default,string dateTimeFormat=DATETIME_FORMAT)
		{
			var ret = false;
			readValue = defaultValue;
			if (_Read(section, key, out var text, defaultValue.ToString(dateTimeFormat)) && DateTime.TryParseExact(text,dateTimeFormat,null,System.Globalization.DateTimeStyles.None ,out readValue))
			{
				ret = true;
			}
			return ret;
		}

		//protected virtual bool _Read<T>(string section, string key, ref IEnumerable<T> objects)
		//{
		//	if (objects!=null)
		//	{
		//	typeof(T)
		//	}
		//	else
		//	{
		//		this.LastException=new ArgumentException();
		//	}

		//}
		//protected virtual bool _ReadObject<T>(string section, string key,out T readValue,T defaultValue=default)
		//{
		//	var ret=true;
		//	var type=typeof(T);
		//	readValue=defaultValue;
		//	object obj=null;
			
		//	switch (defaultValue)
		//	{
		//		case int val: { ret=this._Read(section, key, out var outval, val); obj=outval; } break;
		//		case uint val: { ret=this._Read(section, key, out var outval, val); obj=outval; } break;
		//		case long val: { ret=this._Read(section, key, out var outval, val); obj=outval; } break;
		//		case ulong val: { ret=this._Read(section, key, out var outval, val); obj=outval; } break;
		//		case short val: { ret=this._Read(section, key, out var outval, val); obj=outval; } break;
		//		case ushort val: { ret=this._Read(section, key, out var outval, val); obj=outval; } break;
		//		case byte val: { ret=this._Read(section, key, out var outval, val); obj=outval; } break;
		//		case sbyte val: { ret=this._Read(section, key, out var outval, val); obj=outval; } break;
		//		case float val: { ret=this._Read(section, key, out var outval, val); obj=outval; } break;
		//		case double val: { ret=this._Read(section, key, out var outval, val); obj=outval; } break;
		//		case decimal val: { ret=this._Read(section, key, out var outval, val); obj=outval; } break;
		//		case bool val: { ret=this._Read(section, key, out var outval, val); obj=outval; } break;
		//		case char val: { ret=this._Read(section, key, out var outval, val); obj=outval; } break;
		//		case string val: { ret=this._Read(section, key, out var outval, val); obj=outval; } break;
		//		case DateTime val: { ret=this._Read(section, key, out var outval, val); obj=outval; } break;
		//		default:
		//			if (defaultValue==null)
		//			{
		//				if (typeof(T).Equals(typeof(string)))
		//				{
		//					string text;
		//					ret=this._Read(section, key, out text);
		//					obj=text;
		//				}
		//				else
		//					ret=false;
		//			}
		//			else if (defaultValue is Enum enumdef)
		//			{
		//				if (_Read(section, key, out var text, defaultValue.ToString()))
		//				{
		//					foreach (var item in enumdef.GetType().GetFields())
		//					{
		//						if (item.Name==text)
		//						{
		//							obj= item.GetValue(enumdef);
		//							ret=true;
		//							break;
		//						}
		//					}
		//				}

		//			}
		//			break;

		//	}
		//	if (obj==null)
		//		readValue=default;
		//	else
		//		readValue=(T)obj;
		//	return ret;
		//}
		//private static class TYPES
		//{
		//public static readonly Type INT=typeof(int);
		//	public static readonly Type UINT=typeof(uint);
		//	public static readonly Type LONG=typeof(long);
		//	public static readonly Type ULONG=typeof(ulong);
		//	public static readonly Type SHORT=typeof(short);
		//	public static readonly Type SBYTE=typeof(byte);
		//	public static readonly Type BYTE=typeof(sbyte);
		//	public static readonly Type FLOAT=typeof(float);
		//	public static readonly Type DOUBLE=typeof(double);
		//	public static readonly Type DECIMAL=typeof(decimal);
		//	public static readonly Type STRING=typeof(string);
		//public static readonly Type BOOLEAN=typeof(bool);
		//public static readonly Type OBJECT=typeof(object);
		//public static readonly Type ENUM=typeof(Enum);
		//public static readonly Type DATETIME=typeof(DateTime);
		//public static readonly Type ARRAY=typeof(IEnumerable);
	
		//}
		#endregion
		#region _Write
		/// <summary>
		/// 値を書き込みます
		/// </summary>
		/// <param name="section">セクション</param>
		/// <param name="key">キー</param>
		/// <param name="value">書き込み値</param>
		/// <returns>書き込みが成功したか</returns>
		protected bool _Write(string section, string key, string value)
		{
			var ret= false;
			try
			{
				_WritePrivateProfileString(section, key, value, this.FilePath);
				ret = true;
			}
			catch (Exception ex)
			{
				this.LastException = ex;
				ret = false;
			}
			finally
			{
			}
			return ret;
		}
		
		/// <summary>
		/// 値を書き込みます
		/// </summary>
		/// <param name="section">セクション</param>
		/// <param name="key">キー</param>
		/// <param name="value">書き込み値</param>
		/// <param name="format">フォーマット</param>
		/// <returns>書き込みが成功したか</returns>
		protected bool _Write(string section, string key, int value, string format="")
		{
			return _Write(section, key, value.ToString(format));
		}
		
		/// <summary>
		/// 値を書き込みます
		/// </summary>
		/// <param name="section">セクション</param>
		/// <param name="key">キー</param>
		/// <param name="value">書き込み値</param>
		/// <param name="format">フォーマット</param>
		/// <returns>書き込みが成功したか</returns>
		protected bool _Write(string section, string key, uint value, string format="")
		{
			return _Write(section, key, value.ToString(format));
		}
		/// <summary>
		/// 値を書き込みます
		/// </summary>
		/// <param name="section">セクション</param>
		/// <param name="key">キー</param>
		/// <param name="value">書き込み値</param>
		/// <param name="format">フォーマット</param>
		/// <returns>書き込みが成功したか</returns>
		protected bool _Write(string section, string key, long value, string format="")
		{
			return _Write(section, key, value.ToString(format));
		}
		/// <summary>
		/// 値を書き込みます
		/// </summary>
		/// <param name="section">セクション</param>
		/// <param name="key">キー</param>
		/// <param name="value">書き込み値</param>
		/// <param name="format">フォーマット</param>
		/// <returns>書き込みが成功したか</returns>
		protected bool _Write(string section, string key, ulong value, string format="")
		{
			return _Write(section, key, value.ToString(format));
		}
		/// <summary>
		/// 値を書き込みます
		/// </summary>
		/// <param name="section">セクション</param>
		/// <param name="key">キー</param>
		/// <param name="value">書き込み値</param>
		/// <param name="format">フォーマット</param>
		/// <returns>書き込みが成功したか</returns>
		protected bool _Write(string section, string key, byte value, string format="")
		{
			return _Write(section, key, value.ToString(format));
		}
		/// <summary>
		/// 値を書き込みます
		/// </summary>
		/// <param name="section">セクション</param>
		/// <param name="key">キー</param>
		/// <param name="value">書き込み値</param>
		/// <param name="format">フォーマット</param>
		/// <returns>書き込みが成功したか</returns>
		protected bool _Write(string section, string key, sbyte value, string format="")
		{
			return _Write(section, key, value.ToString(format));
		}
		/// <summary>
		/// 値を書き込みます
		/// </summary>
		/// <param name="section">セクション</param>
		/// <param name="key">キー</param>
		/// <param name="value">書き込み値</param>
		/// <param name="format">フォーマット</param>
		/// <returns>書き込みが成功したか</returns>
		protected bool _Write(string section, string key, float value, string format="")
		{
			return _Write(section, key, value.ToString(format));
		}
		/// <summary>
		/// 値を書き込みます
		/// </summary>
		/// <param name="section">セクション</param>
		/// <param name="key">キー</param>
		/// <param name="value">書き込み値</param>
		/// <param name="format">フォーマット</param>
		/// <returns>書き込みが成功したか</returns>
		protected bool _Write(string section, string key, double value, string format="")
		{
			return _Write(section, key, value.ToString(format));
		}
		/// <summary>
		/// 値を書き込みます
		/// </summary>
		/// <param name="section">セクション</param>
		/// <param name="key">キー</param>
		/// <param name="value">書き込み値</param>
		/// <param name="format">フォーマット</param>
		/// <returns>書き込みが成功したか</returns>
		protected bool _Write(string section, string key, decimal value, string format="")
		{
			return _Write(section, key, value.ToString(format));
		}
		/// <summary>
		/// 値を書き込みます
		/// </summary>
		/// <param name="section">セクション</param>
		/// <param name="key">キー</param>
		/// <param name="value">書き込み値</param>
		/// <param name="format">フォーマット</param>
		/// <returns>書き込みが成功したか</returns>
		protected bool _Write(string section, string key, bool value)
		{
			return _Write(section, key, value.ToString());
		}

		/// <summary>
		/// 値を書き込みます
		/// </summary>
		/// <param name="section">セクション</param>
		/// <param name="key">キー</param>
		/// <param name="value">書き込み値</param>
		/// <param name="format">フォーマット</param>
		/// <returns>書き込みが成功したか</returns>
		protected bool _Write(string section, string key, char value)
		{
			return _Write(section, key, value.ToString());
		}
		/// <summary>
		/// 値を書き込みます
		/// </summary>
		/// <param name="section">セクション</param>
		/// <param name="key">キー</param>
		/// <param name="value">書き込み値</param>
		/// <returns>書き込みが成功したか</returns>
		protected bool _Write<TEnum>(string section, string key,TEnum value) where TEnum : struct,Enum
		{
			return _Write(section, key,Enum.GetName(typeof(TEnum), value));
		}
		/// <summary>
		/// 値を書き込みます
		/// </summary>
		/// <param name="section">セクション</param>
		/// <param name="key">キー</param>
		/// <param name="value">書き込み値</param>
		/// <param name="format">フォーマット</param>
		/// <returns>書き込みが成功したか</returns>
		protected bool _Write(string section, string key, DateTime value, string format=DATETIME_FORMAT)
		{
			return _Write(section, key, value.ToString(format));
		}
		#endregion
	}
}
