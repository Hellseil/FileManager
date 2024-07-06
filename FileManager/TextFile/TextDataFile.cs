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
using System.IO;
using System.Linq;
using System.Text;



#endregion

namespace FileManagerLibrary.TextFile
{
	public class TextDataFile : IDataFile, IEnumerable<char>
	{
		//---------------------------------------------------------------------
		#region 定数

		#endregion

		//---------------------------------------------------------------------
		#region メンバー
		private string _filePath;
		private string _text;
		#endregion

		//---------------------------------------------------------------------
		#region コンストラクタ＆デストラクタ

		/// <summary>
		/// TextFileDataを生成します
		/// </summary>
		public TextDataFile(string filePath):this(filePath,"")
		{
		}
		/// <summary>
		/// TextFileDataを生成します
		/// </summary>
		public TextDataFile(string filePath,string text):this(filePath,text,Encoding.UTF8)
		{
		}
		/// <summary>
		/// TextFileDataを生成します
		/// </summary>
		public TextDataFile(string filePath, string text,Encoding encoding)
		{
			this._filePath = filePath;
			this._text= text;
			this.Encod=encoding;
		}
		/// <summary>
		/// TextFileDataを破棄します
		/// </summary>
		~TextDataFile()
		{
		}


		#endregion

		//---------------------------------------------------------------------
		#region プロパティ
		public char this[int index]
		{
			get
			{
				return this._text[index];
			}
		}

		public Encoding Encod
		{
			get;
			protected set;
		}
		public string FilePath => this._filePath;
		public string Text
		{
		get { return this._text; }
		set { this._text = value; }
		}
		public Exception LastException { get; protected set; }
		public int Length => this._text.Length;

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
		public static bool Load(string filePath, out string[] lines, bool shouldCreate = true)
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
						sr = new StreamReader(fs,this.Encod);
						this.Text=sr.ReadToEnd();
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
					if (this.Text!=null)
						sw.Write(this.Text);

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

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public IEnumerator<char> GetEnumerator()
		{
			return this.Text.GetEnumerator();
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
