/******************************************************************************
 * ファイル	：ImageFileData.cs
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
using System.Drawing;
using System.IO;


#endregion

namespace FileManagerLibrary.BinaryFile
{
	public class ImageDataFile : IDataFile,IDisposable
	{
		//---------------------------------------------------------------------
		#region 定数

		#endregion

		//---------------------------------------------------------------------
		#region メンバー
		private Image _data;
		#endregion

		//---------------------------------------------------------------------
		#region コンストラクタ＆デストラクタ

		/// <summary>
		/// BinaryFileDataを生成します
		/// </summary>
		public ImageDataFile(string filePath) : this(filePath,null)
		{

		}
		public ImageDataFile(string filePath, Image image)
		{
			this.FilePath = filePath;
			this._data = image;

		}
		/// <summary>
		/// BinaryFileDataを破棄します
		/// </summary>
		~ImageDataFile()
		{
		this.Dispose();
		}

		public Exception LastException { get; protected set; }

		public string FilePath { get; protected set; }


		#endregion

		//---------------------------------------------------------------------
		#region プロパティ
		
		public Image Data
		{
			get { return this._data; }
			set { this._data=value; }
		}
		#endregion

		//---------------------------------------------------------------------
		#region 公開メソッド
		public static implicit operator Image(ImageDataFile image)
		{
			return image.Data;
		}
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
				
				try
				{
					this.Data=Image.FromFile(this.FilePath);
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
				this.Data.Save(this.FilePath);

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


		#endregion

		//---------------------------------------------------------------------
		#region 非公開メソッド

		#endregion

		//---------------------------------------------------------------------
		#region インターフェース実装

		public void Dispose()
		{
			if (this.Data!=null)
			{
			this.Data.Dispose();
			this.Data = null;
			}
		}
		#endregion

	}
}
