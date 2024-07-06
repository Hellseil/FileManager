/******************************************************************************
 * ファイル	：BinaryFileData.cs
 * 目的		：
 * 名前空間	:FileManagerLibrary.BinaryFile
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

#endregion

namespace FileManagerLibrary.BinaryFile
{
	public class BinaryDataFile:IDataFile,IEnumerable<byte>
	{
		//---------------------------------------------------------------------
		#region 定数

		#endregion

		//---------------------------------------------------------------------
		#region メンバー
		private byte[] _data;
		#endregion

		//---------------------------------------------------------------------
		#region コンストラクタ＆デストラクタ

		/// <summary>
		/// BinaryFileDataを生成します
		/// </summary>
		public BinaryDataFile(string filePath):this(filePath,new byte[] { })
		{
			
		}
		public BinaryDataFile(string filePath,byte[] bytes)
		{
			this.FilePath = filePath;
			this._data = bytes;

		}
		/// <summary>
		/// BinaryFileDataを破棄します
		/// </summary>
		~BinaryDataFile()
		{

		}

		public Exception LastException { get; protected set; }

		public string FilePath {get;protected set;}


		#endregion

		//---------------------------------------------------------------------
		#region プロパティ
		public byte this[int index]
		{
			get
			{
				return this.Data[index];
			}
			set
			{
				this.Data[index] = value;
			}
		}
		public byte[] Data
		{
			get { return this._data; }
			set { this._data=value; }
		}
		public int Length =>this.Data!=null?this.Data.Length:0;
		#endregion

		//---------------------------------------------------------------------
		#region 公開メソッド
		public bool Load(bool shouldCreate = true)
		{
			bool ret=false;
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
				BinaryReader br=null;
				try
				{
					fs=new FileStream(this.FilePath, FileMode.Open);
					if (fs.Length > int.MaxValue)
					{
						// ファイルサイズが int.MaxValue よりも大きい場合、MemoryStream を使用して読み取り
						using (MemoryStream ms = new MemoryStream())
						{
							fs.CopyTo(ms);
							this.Data = ms.ToArray();
						}
					}
					else
					{
						br = new BinaryReader(fs);
						// ファイルサイズが int.MaxValue 以下の場合、直接 BinaryReader から読み取り
						this.Data = br.ReadBytes((int)fs.Length);
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
					if (br!=null)
					{
						br.Close();
						br.Dispose();
						br = null;
					}
					if (fs!=null)
					{
						fs.Close();
						fs.Dispose();
						fs = null;
					}
				}
			}
			return ret;
		}

		public bool Save(FileMode mode = FileMode.Create)
		{
			bool ret=false;

			FileStream fs = null;
			BinaryWriter bw =null;
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
				bw = new BinaryWriter(fs);
				bw.Write(this.Data);

				ret = true;
			}
			catch (Exception ex)
			{
				this.LastException = ex;
				ret=false;
			}
			finally
			{
				if (bw != null)
				{
					bw.Close();
					bw.Dispose();
					bw = null;
				}
				if (fs != null)
				{
					fs.Close();
					fs.Dispose();
					fs=null;
				}
			}


			return ret;
		}

		public void SetFilePath(string filepath)
		{
			this.FilePath = filepath;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
		public IEnumerator<byte> GetEnumerator()
		{
			return (this.Data as IEnumerable<byte>).GetEnumerator();
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
