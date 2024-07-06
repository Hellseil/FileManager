/******************************************************************************
 * ファイル	：CsvFileDataBase.cs
 * 目的		：
 * 名前空間	:FileManagerLibrary.CsvFile
 * 依存関係	：
 * 注意点	：
 * 備考		：
 * Netver	：4.8
 * 変更履歴
 *	2024/##/##	ysugi		新規作成
*******************************************************************************/


//-----------------------------------------------
#region 使用する名前空間
using FileManagerLibrary.TextFile;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
#endregion

namespace FileManagerLibrary.CsvFile
{
	public class CsvDataFile : IDataFile, IEnumerable<string>
	{
		//---------------------------------------------------------------------
		#region 定数

		#endregion

		//---------------------------------------------------------------------
		#region メンバー

		private TextDataFile _file;
		private char _delimiter=',';
		#endregion

		//---------------------------------------------------------------------
	
		#region コンストラクタ＆デストラクタ

		/// <summary>
		/// CsvFileDataBaseを生成します
		/// </summary>
		public CsvDataFile(string filePath):this(filePath,null)
		{
		}
		public CsvDataFile(string filePath,string [,] data):this(filePath,data,Encoding.UTF8)
		{
		}
		public CsvDataFile(string filePath, string[,] data,Encoding encoding)
		{
			this._file=new TextDataFile(filePath,null,encoding);
			this.Data = data;

		}


		/// <summary>
		/// CsvFileDataBaseを破棄します
		/// </summary>
		~CsvDataFile()
		{

		}
		#endregion

		//---------------------------------------------------------------------
		#region プロパティ
		public Encoding Encod
		{
			get => this._file.Encod;
		}
		public string this[int row, int column]
		{
			get
			{
				return this.Data[row, column];
			}
			set
			{
				this.Data[row, column] = value;
			}
		}
		public int RowCount
		{
			get=>this.Data.GetLength(0);
		}
		public int ColumnCount
		{
			get => this.Data.GetLength(1);
		}
		public Exception LastException { get => this._file.LastException; }

		public string FilePath
		{
			get=>this._file.FilePath;
		}

		public string[,] Data
		{
			get;
			protected set;
		}
		#endregion

		//---------------------------------------------------------------------
		#region 公開メソッド
		public void SetEncod(Encoding encoding)
		{
			this._file.SetEncod ( encoding);
		}
		public void SetFilePath(string filePath)
		{
			this._file.SetFilePath(filePath);
		}
		/// <inheritdoc/>
		public bool Load(bool shouldCreate = true)
		{
			bool ret=this._file.Load(shouldCreate);
			if (ret)
			{
				List<List<string>> list = ParseCsv(this._file.Text,out int maxCnt);
				this.Data=new string[list.Count, maxCnt];
				for (int ii = 0; ii<list.Count; ii++)
				{
					List<string> row=list[ii];
					int column =row.Count;
					for (int jj = 0; jj<maxCnt; jj++)
					{
						this.Data[ii,jj]=jj<column?row[jj]:"";
					}
				}
			}

			return ret;
		}
		public bool Save(FileMode mode = FileMode.Create)
		{
			return Save (false,mode);
		}
		public bool Save(bool notOutputNull=false, FileMode mode = FileMode.Create)
		{
			var text=new  StringBuilder();
			Func<string[,],int,string> getLine;
			if (notOutputNull)
			{
				getLine= new Func<string[,],int, string>((fields,lineNo) =>
				{
					var line=new StringBuilder();	
					bool fast=true;
					for (int ii = 0; ii<fields.GetLength(1); ii++)
					{
						if (!fast) 
						line.Append(this._delimiter);
						else
						fast=false;
						line.Append($"\"{fields[lineNo,ii].Replace("\"","\"\"")}\"");
						bool end=true;
						for (int jj = ii+1; jj<fields.GetLength(1); jj++)
						{
							if (!string.IsNullOrEmpty(fields[lineNo, jj]))
							{
								end=false;
								break;
							}
						}
						if (end)
							break;
					}
					return line.ToString();
				});
			}
			else
			{
				getLine= new Func<string[,],int, string>((fields,lineNo) =>
				{
					var line=new StringBuilder();
					bool fast=true;
					for (int ii = 0; ii<fields.GetLength(1); ii++)
					{
						if (!fast) line.Append(this._delimiter);
						else 
						fast=false;
						line.Append($"\"{fields[lineNo, ii].Replace("\"", "\"\"")}\"");
					}
					return line.ToString();
				});
			}
			for (int ii = 0; ii<this.Data.GetLength(0); ii++)
			{
				text.Append(getLine(this.Data, ii) + Environment.NewLine); ;
			}
			this._file.Text = text.ToString();
			return this._file.Save(mode);
		}
		public List<List<string>> ParseCsv(string text,out int maxcolumn)
		{
			var lines = new List<List<string>>();
			var fields = new List<string>();
			bool insideQuotes = false;
			int start = 0;
			maxcolumn=-1;
			for (int ii = 0; ii < text.Length; ii++)
			{
				if (text[ii] == '\"')
				{
					if (!insideQuotes||ii+1<text.Length&&text[ii+1]!='\"')
					{
						insideQuotes = !insideQuotes;
					}
					else
					{
						ii++;
					}
				}
				else if (!insideQuotes)
				{
					if (text[ii] == this._delimiter)
					{
						string field;
						field= text.Substring(start, ii - start);
						fields.Add(this._TrimTextData(field));
						start = ii + 1;
					}
					else if (text[ii]=='\r')
					{
						string field;
						field= text.Substring(start, ii - start);
						fields.Add(this._TrimTextData(field));
						lines.Add(fields);
						if (ii+1<text.Length&&text[ii+1]=='\n')
						{
							ii++;
						}
						if(fields.Count>maxcolumn)
							maxcolumn = fields.Count;	
						start = ii + 1;
						fields=new List<string>();
					}
					else if (text[ii]=='\n')
					{
						string field;
						field= text.Substring(start, ii - start);
						fields.Add(this._TrimTextData(field));
						lines.Add(fields);
						if (ii+1<text.Length&&text[ii+1]=='\r')
						{
							ii++;
						}
						if (fields.Count>maxcolumn)
							maxcolumn = fields.Count;
						start = ii + 1;
						fields=new List<string>();
					}
				}
			}

			// 最後のフィールドを処理
			string lastField = text.Substring(start);
			if (!string.IsNullOrEmpty(lastField)||fields.Count>0)
			{
				fields.Add(this._TrimTextData(lastField));
				lines.Add(fields);
				if (fields.Count>maxcolumn)
					maxcolumn = fields.Count;
			}
			
			
			return lines;
		}
		#endregion

		//---------------------------------------------------------------------
		#region 非公開メソッド
		private string _TrimTextData(string text)
		{
			if (text.Length>0&& text[0]=='\"'&&text[text.Length-1]=='\"')
				return text.Substring(1, text.Length-1-1).Replace("\"\"", "\"");
			return text;
		}


		#endregion

		//---------------------------------------------------------------------
		#region インターフェース実装
		public IEnumerator<string> GetEnumerator()
		{
			return (IEnumerator<string>)this.Data.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.Data.GetEnumerator();
		}
		#endregion

	}

	public class CsvDataFile<T> : IDataFile where T :class,new ()
	{
		//---------------------------------------------------------------------
		#region 定数
		public enum HeaderTypes
		{
			OneLine,
			MultiLine,
		}
		#endregion
		#region 
		protected struct HeaderData
		{
			public string Name;
			public int Order;
			public PropertyInfo Property;
			public string Format;
		}
		#endregion
		//---------------------------------------------------------------------
		#region メンバー

		private TextDataFile _file;
		private char _delimiter=',';
		protected Dictionary<string,HeaderData> _dicHeaderNameAndInfo;
		protected string[] _alignmentHeader;
		protected HeaderTypes _headerType=HeaderTypes.MultiLine;
		#endregion

		//---------------------------------------------------------------------
		#region コンストラクタ＆デストラクタ

		/// <summary>
		/// CsvFileDataBaseを生成します
		/// </summary>
		public CsvDataFile(string filePath) : this(filePath, null)
		{
		}
		public CsvDataFile(string filePath, T[] data) : this(filePath, data, Encoding.UTF8)
		{
		}
		public CsvDataFile(string filePath, T[] data, Encoding encoding)
		{
			this._file=new TextDataFile(filePath,null,encoding);
			this.Data = data;
			this._dicHeaderNameAndInfo= new Dictionary<string,HeaderData>();
			PropertyInfo[] infos=typeof(T).GetProperties();
			foreach (PropertyInfo info in infos)
			{
				IEnumerable<Attribute> atts= info.GetCustomAttributes();
				int order=-1;
				string format=null;
				string header=info.Name;
				bool isCol=true;
				foreach (Attribute att in atts)
				{
					switch (att)
					{
						case CsvHeaderTextAttribute headerTextAtt:
							header=headerTextAtt.HeaderText;
							break;
						case IsCsvColumnAttribute isCsvColumnAtt:
							isCol=isCsvColumnAtt.IsColumn;
							break;
						case CsvFormatAttribute formatAtt:
							format=formatAtt.Format;
							break;
						case CsvColumnOrderAttribute orderAtt:
							order=orderAtt.OrderNum;
							break;
					}
				}
				if (isCol)
				{
					this._dicHeaderNameAndInfo.Add(header, new HeaderData()
					{
						Format=format,
						Name=header,
						Order=order,
						Property=info,
					});
				}
			}
			var list=new List<HeaderData>(this._dicHeaderNameAndInfo.Values);
			list.Sort((a, b) =>
			{
				return a.Order.CompareTo(b.Order);
			});
			this._alignmentHeader=Array.ConvertAll<HeaderData,string>(list.ToArray(),(HeaderData item) =>item.Name);
		}


		/// <summary>
		/// CsvFileDataBaseを破棄します
		/// </summary>
		~CsvDataFile()
		{

		}
		#endregion

		//---------------------------------------------------------------------
		#region プロパティ
		public T this[int row]
		{
			get
			{
				return this.Data[row];
			}
			set
			{
				this.Data[row] = value;
			}
		}
		public int Length
		{
			get => this.Data.Length;
		}
		public Exception LastException { get; protected set; }

		public string FilePath
		{
			get => this._file.FilePath;
		}

		public T[] Data
		{
			get;
			set;
		}
		#endregion

		//---------------------------------------------------------------------
		#region 公開メソッド

		public void SetFilePath(string filePath)
		{
			this._file.SetFilePath(filePath);
		}
		public bool Load(bool shouldCreate = true)
		{
			return Load(true,null,shouldCreate);
		}
		/// <summary>
		/// ファイルを読み込みます。
		/// </summary>
		/// <param name="readFileHasHeader">ヘッダが読込ファイルに存在するか</param>
		/// <param name="header">
		/// readFileHasHeaderがfalseの場合有効
		/// 読込ファイルのヘッダ構成
		/// readFileHasHeaderがfalseかつNullの場合デフォルトで読み込む
		/// </param>
		/// <param name="shouldCreate"></param>
		/// <returns></returns>
		public bool Load(bool readFileHasHeader, string[] header, bool shouldCreate = true)
		{
			bool ret=this._file.Load(shouldCreate);
			if (ret)
			{
				if (readFileHasHeader)
					header=null;
				if (readFileHasHeader==false&&header==null)
					header=this._alignmentHeader;
				List<T> list = this._ParseCsv(header,this._file.Text);
				this.Data=list.ToArray();
			}
			else
			{
				this.LastException=this._file.LastException;
			}
			return ret;
		}
		public bool Save(FileMode mode = FileMode.Create)
		{
			return Save( true, mode);
		}
		public bool Save(bool headerOutput, FileMode mode = FileMode.Create)
		{
			return Save(headerOutput,this._alignmentHeader, mode);
		}
		public bool Save(bool headerOutput, string[] header, FileMode mode = FileMode.Create)
		{
			var text=new StringBuilder();
			if (headerOutput)
			{
				for (int ii = 0; ii<header.Length; ii++)
				{
					if (ii!=0) text.Append(this._delimiter);
					if (this._headerType==HeaderTypes.OneLine)
						text.Append(header[ii]);
					else
						text.Append($"\"{header[ii]}\"");
				}
				text.Append(Environment.NewLine);
			}
			for (int ii = 0; ii<this.Data.Length; ii++)
			{
				for (int jj = 0; jj<header.Length; jj++)
				{
					if (jj!=0)text.Append(this._delimiter);
					if (this._dicHeaderNameAndInfo.TryGetValue(header[jj], out HeaderData info))
					{
						Type type=info.Property.PropertyType;
						object value= info.Property.GetValue(this.Data[ii]);
						
						if (value!=null)
						{
							string valText="";
							if (info.Format==null)
							{
								if (value is DateTime) valText=$"\"{((DateTime)value).ToString()}\"";
								else if (value is DateTime?) valText=$"\"{((DateTime?)value).Value.ToString()}\"";
								else if (value is string strval) valText=$"\"{strval.Replace("\"","\"\"")}\"";
								else valText=value.ToString();
							}
							else
							{
								if (value is int) valText=((int)value).ToString(info.Format);
								else if (value is int?) valText=((int?)value).Value.ToString(info.Format);
								else if (value is uint) valText=((uint)value).ToString(info.Format);
								else if (value is uint?) valText=((uint?)value).Value.ToString(info.Format);
								else if (value is byte) valText=((byte)value).ToString(info.Format);
								else if (value is byte?) valText=((byte?)value).Value.ToString(info.Format);
								else if (value is sbyte) valText=((sbyte)value).ToString(info.Format);
								else if (value is sbyte?) valText=((sbyte?)value).Value.ToString(info.Format);
								else if (value is short) valText=((short)value).ToString(info.Format);
								else if (value is short?) valText=((short?)value).Value.ToString(info.Format);
								else if (value is ushort) valText=((ushort)value).ToString(info.Format);
								else if (value is ushort?) valText=((ushort?)value).Value.ToString(info.Format);
								else if (value is long) valText=((long)value).ToString(info.Format);
								else if (value is long?) valText=((long?)value).Value.ToString(info.Format);
								else if (value is ulong) valText=((ulong)value).ToString(info.Format);
								else if (value is ulong?) valText=((ulong?)value).Value.ToString(info.Format);
								else if (value is float) valText=((float)value).ToString(info.Format);
								else if (value is float?) valText=((float?)value).Value.ToString(info.Format);
								else if (value is double) valText=((double)value).ToString(info.Format);
								else if (value is double?) valText=((double?)value).Value.ToString(info.Format);
								else if (value is decimal) valText=((decimal)value).ToString(info.Format);
								else if (value is decimal?) valText=((decimal?)value).Value.ToString(info.Format);
								else if (value is DateTime) valText=$"\"{((DateTime)value).ToString(info.Format)}\"";
								else if (value is DateTime?) valText=$"\"{((DateTime?)value).Value.ToString(info.Format)}\"";
								else if (value is string strval) valText=$"\"{strval.Replace("\"", "\"\"")}\"";
								else throw new ArgumentException();
							}
							text.Append(valText);

						}
					}

				}
				text.Append(Environment.NewLine);
			}
			this._file.Text = text.ToString();
			bool ret=this._file.Save(mode);
			this.LastException=this._file.LastException;
			return ret;
		}
		/// <summary>
		/// CSVテキストをデータリストに変換します。
		/// </summary>
		/// <param name="header"></param>
		/// <param name="text"></param>
		/// <returns></returns>
		private List<T> _ParseCsv(string[] header, string text)
		{
			var lines = new List<T>();
			var fields=new List<string>();
			var addData = new T();
			bool insideQuotes = false;
			int fieldIndex=0;
			int start = 0;
			for (int ii = 0; ii < text.Length; ii++)
			{
				if (text[ii] == '\"'&& (header!=null||this._headerType!=HeaderTypes.OneLine))
				{
					if (!insideQuotes||ii+1<text.Length&&text[ii+1]!='\"')
					{
						insideQuotes = !insideQuotes;
					}
					else
					{
						ii++;
					}
				}
				else if (!insideQuotes)
				{
					if (text[ii] == this._delimiter)
					{
						string field;
						field= text.Substring(start, ii - start);
						if (header==null)
						{
							if (this._headerType==HeaderTypes.OneLine)
								fields.Add(field);
							else
								fields.Add(this._CsvTextToDataText(field));
						}
						else
						{
							if (fieldIndex<header.Length&& this._dicHeaderNameAndInfo.TryGetValue(header[fieldIndex], out HeaderData info))
							{
								_SetData(field, ref addData, ref info);
							}
							fieldIndex++;
						}
						start = ii + 1;
					}
					else if (text[ii]=='\r')
					{
						string field;
						field= text.Substring(start, ii - start);
						if (header==null)
						{
							if (this._headerType==HeaderTypes.OneLine)
								fields.Add(field);
							else
								fields.Add(this._CsvTextToDataText(field));
							if (header==null)
								header=fields.ToArray();
						}
						else
						{
							if (fieldIndex<header.Length&& this._dicHeaderNameAndInfo.TryGetValue(header[fieldIndex], out HeaderData info))
							{
								_SetData(field, ref addData, ref info);
							}
							lines.Add(addData);
						}
						if (ii+1<text.Length&&text[ii+1]=='\n')
						{
							ii++;
						}
						fieldIndex=0;
						start = ii + 1;
						addData=new T();
					}
					else if (text[ii]=='\n')
					{
						string field;
						field= text.Substring(start, ii - start);
						if (header==null)
						{
							if (this._headerType==HeaderTypes.OneLine)
								fields.Add(field);
							else
								fields.Add(this._CsvTextToDataText(field));
							if (header==null)
								header=fields.ToArray();
						}
						else
						{
							if (fieldIndex<header.Length&& this._dicHeaderNameAndInfo.TryGetValue(header[fieldIndex], out HeaderData info))
							{
								this._SetData(field, ref addData, ref info);
							}
							lines.Add(addData);
						}

						if (ii+1<text.Length&&text[ii+1]=='\r')
						{
							ii++;
						}
						start = ii + 1;
						fieldIndex=0;
						addData=new T();
					}
				}
			}

			// 最後のフィールドを処理
			string lastField = text.Substring(start);
			if (!string.IsNullOrEmpty(lastField)||fieldIndex!=0)
			{
				if (fieldIndex<header.Length&& this._dicHeaderNameAndInfo.TryGetValue(header[fieldIndex], out HeaderData info))
				{
					_SetData(lastField, ref addData, ref info);
				}
				lines.Add(addData);
			}


			return lines;
		}
		#endregion

		//---------------------------------------------------------------------
		#region 非公開メソッド
		/// <summary>
		/// CSV表記のテキストデータを保管用テキストに変換します。
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		private string _CsvTextToDataText(string text)
		{
			if (text.Length>0&& text[0]=='\"'&&text[text.Length-1]=='\"')
				return text.Substring(1, text.Length-1-1).Replace("\"\"", "\"");
			return text;
		}
		private void _SetData(string field ,ref T data,ref HeaderData info)
		{
			Type type = info.Property.PropertyType;
			if (field.Equals(""))
			{
				if (typeof(Nullable).IsAssignableFrom(type))
				{
					info.Property.SetValue(data, null);
				}
				else
				{
					info.Property.SetValue(data, default);
				}
			}
			else
			{
				if (type == typeof(int)) 
					info.Property.SetValue(data, int.Parse(field));
				else if (type == typeof(int?)) 
					info.Property.SetValue(data, int.Parse(field));
				else if (type == typeof(uint)) 
					info.Property.SetValue(data, uint.Parse(field));
				else if (type == typeof(uint?)) 
					info.Property.SetValue(data, uint.Parse(field));
				else if (type == typeof(byte)) 
					info.Property.SetValue(data, byte.Parse(field));
				else if (type == typeof(byte?))
					info.Property.SetValue(data, byte.Parse(field));
				else if (type == typeof(sbyte)) 
					info.Property.SetValue(data, sbyte.Parse(field));
				else if (type == typeof(sbyte?))
					info.Property.SetValue(data, sbyte.Parse(field));
				else if (type == typeof(short))
					info.Property.SetValue(data, short.Parse(field));
				else if (type == typeof(short?))
					info.Property.SetValue(data, short.Parse(field));
				else if (type == typeof(ushort)) 
					info.Property.SetValue(data, ushort.Parse(field));
				else if (type == typeof(ushort?)) 
					info.Property.SetValue(data, ushort.Parse(field));
				else if (type == typeof(long)) 
					info.Property.SetValue(data, long.Parse(field));
				else if (type == typeof(long?)) 
					info.Property.SetValue(data, long.Parse(field));
				else if (type == typeof(ulong)) 
					info.Property.SetValue(data, ulong.Parse(field));
				else if (type == typeof(ulong?)) 
					info.Property.SetValue(data, ulong.Parse(field));
				else if (type == typeof(float)) 
					info.Property.SetValue(data, float.Parse(field));
				else if (type == typeof(float?)) 
					info.Property.SetValue(data, float.Parse(field));
				else if (type == typeof(double)) 
					info.Property.SetValue(data, double.Parse(field));
				else if (type == typeof(double?)) 
					info.Property.SetValue(data, double.Parse(field));
				else if (type == typeof(decimal)) 
					info.Property.SetValue(data, decimal.Parse(field));
				else if (type == typeof(decimal?))
					info.Property.SetValue(data, decimal.Parse(field));
				else if (type == typeof(DateTime))
				{
					if (info.Format != null)
						info.Property.SetValue(data, DateTime.ParseExact(_CsvTextToDataText(field), info.Format, null));
					else
						info.Property.SetValue(data, DateTime.Parse(_CsvTextToDataText(field)));
				}
				else if (type == typeof(DateTime?))
				{
					if (info.Format != null)
						info.Property.SetValue(data, DateTime.ParseExact(_CsvTextToDataText(field), info.Format, null));
					else
						info.Property.SetValue(data, DateTime.Parse(_CsvTextToDataText(field)));
				}
				else if (type == typeof(string))
					info.Property.SetValue(data, _CsvTextToDataText(field));
				else 
					throw new ArgumentException();

			}
		}

		#endregion

		//---------------------------------------------------------------------
		#region インターフェース実装

		#endregion

	}
}
