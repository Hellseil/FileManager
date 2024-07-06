/******************************************************************************
 * ファイル	：TextFileData.cs
 * 目的		：
 * 名前空間	:FileManagerLibrary.TextFile
 * 依存関係	：
 * 注意点	：
 * 備考		：
 * Netver	：4.8
 * 変更履歴
 *	2024/##/##	ysugi		新規作成
*******************************************************************************/


//-----------------------------------------------
#region 使用する名前空間
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;


#endregion

namespace FileManagerLibrary.TextFile
{
	public class LineTextDataFile:IDataFile,IEnumerable<string>
	{
		//---------------------------------------------------------------------
		#region 定数

		#endregion

		//---------------------------------------------------------------------
		#region メンバー
		private string _filePath;
		Collection<string> _lines; 
		#endregion

		//---------------------------------------------------------------------
		#region コンストラクタ＆デストラクタ

		/// <summary>
		/// TextFileDataを生成します
		/// </summary>
		public LineTextDataFile(string filePath):this(filePath,null)
		{
			
		}
		/// <summary>
		/// TextFileDataを生成します
		/// </summary>
		public LineTextDataFile(string filePath,string[] lines):this(filePath,lines,Encoding.UTF8)
		{
		}
		public LineTextDataFile(string filePath, string[] lines, Encoding encoding)
		{
			this._filePath = filePath;
			this._lines = new Collection<string>();
			if (lines!=null)
			{
				foreach (string line in lines)
				{
					this._lines.Add(line);
				}
			}
			this.Encod=encoding;
		}
		/// <summary>
		/// TextFileDataを破棄します
		/// </summary>
		~LineTextDataFile()
		{
		}


		#endregion

		//---------------------------------------------------------------------
		#region プロパティ
		public string this[int index]
		{
			get
			{
				return this._lines[index];
			}
			set
			{
				this._lines[index] = value;
			}
		}
		public string FilePath => this._filePath;
		public Exception LastException {get;protected set;}
		public Collection<string> Lines => this._lines;
		public int Count => this._lines.Count;

		public Encoding Encod { get; protected set; }

		#endregion

		//---------------------------------------------------------------------
		#region 公開メソッド
		public void SetEncod(Encoding encoding)
		{
			this.Encod = encoding;
		}
		public static bool Save(string filePath, string[] lines, FileMode mode = FileMode.Create)
		{
			LineTextDataFile file=new LineTextDataFile(filePath,lines);
			return file.Save(mode);
		}
		public static bool Load(string filePath,out string[] lines, bool shouldCreate = true)
		{
			LineTextDataFile file=new LineTextDataFile(filePath);
			bool ret=file.Load(shouldCreate);
			lines=null;
			if (ret)
			{
				lines=file.Lines.ToArray();
			}
			return ret;
		}
		public void SetFilePath(string filePath)
		{
			this._filePath = filePath;
		}
		public bool Load(bool shouldCreate = true)
		{
			bool ret=false;
			if (!string.IsNullOrWhiteSpace(this.FilePath))
			{
				if (!File.Exists(this.FilePath)&&shouldCreate)
				{
					if (!Directory.Exists(Path.GetDirectoryName(this.FilePath)))
					{
						Directory.CreateDirectory(Path.GetDirectoryName(this.FilePath));
					}
					File.Create(this.FilePath).Close();
				}
				if (File.Exists(this.FilePath))
				{
					FileStream fs=null;
					StreamReader sr=null;
					try
					{
						fs=new FileStream(this.FilePath, FileMode.Open);
						sr = new StreamReader(fs, this.Encod);
						string text=sr.ReadToEnd();
						string[] lines=text.Split(new[]{"\r\n","\n","\r"},StringSplitOptions.None);
						this._lines.Clear();
						foreach (string line in lines)
						{
							this.Lines.Add(line);
						}
						ret = true;
					}
					catch (Exception ex)
					{
						this.LastException = ex;
						ret = false;
					}
					finally
					{
						if (sr!=null)
						{
							sr.Close();
							sr.Dispose();
							sr = null;
						}
						if (fs!=null)
						{
							fs.Close();
							fs.Dispose();
							fs = null;
						}
					}
				}
			}
			return ret;

		}

		public bool Save(FileMode mode = FileMode.Create)
		{
			bool ret=false;

			FileStream fs = null;
			StreamWriter sw =null;
			if (!string.IsNullOrWhiteSpace(this.FilePath))
			{
				try
				{
					switch (mode)
					{
						case FileMode.Create:
						case FileMode.CreateNew:
						case FileMode.OpenOrCreate:
						case FileMode.Append:
							if (!Directory.Exists(Path.GetDirectoryName(this.FilePath)))
							{
								Directory.CreateDirectory(Path.GetDirectoryName(this.FilePath));
							}
							break;
						default:
							break;
					}
					fs=new FileStream(this.FilePath, mode);
					sw = new StreamWriter(fs,this.Encod);
					foreach (string line in this._lines)
					{
						sw.WriteLine(line);
					}

					ret = true;
				}
				catch (Exception ex)
				{
					this.LastException = ex;
					ret=false;
				}
				finally
				{
					if (sw != null)
					{
						sw.Close();
						sw.Dispose();
						sw = null;
					}
					if (fs != null)
					{
						fs.Close();
						fs.Dispose();
						fs=null;
					}
				}

			}
			return ret;
		}
		public void Insert(int index,string item)
		{
			this.Lines.Insert(index,item);
		}
		public void Add(string item)
		{
			this.Lines.Add(item);
		}
		public void Remove(string item)
		{
			this.Lines.Remove(item);
		}
		public void RemoveAt(int index)
		{
			this.Lines.RemoveAt(index);
		}
		public void Clear()
		{
			this.Lines.Clear();
		}
		public bool Contains(string item)
		{
			return this.Lines.Contains(item);
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public IEnumerator<string> GetEnumerator()
		{
			return this._lines.GetEnumerator();
		}

		#endregion

		//---------------------------------------------------------------------
		#region 非公開メソッド

		#endregion

		//---------------------------------------------------------------------
		#region インターフェース実装

		#endregion

	}
}
