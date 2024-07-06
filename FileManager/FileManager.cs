/******************************************************************************
 * ファイル	：FileManager.cs
 * 目的		：
 * 名前空間	:FileManagerLibrary
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
using System.IO;


#endregion

namespace FileManagerLibrary
{
	public static  class FileManager
	{
		//---------------------------------------------------------------------
		#region 定数

		#endregion

		//---------------------------------------------------------------------
		#region メンバー

		#endregion

		//---------------------------------------------------------------------
		#region コンストラクタ＆デストラクタ

		/// <summary>
		/// FileManagerを生成します
		/// </summary>
		static  FileManager()
		{

		}
		#endregion

		//---------------------------------------------------------------------
		#region プロパティ

		#endregion

		//---------------------------------------------------------------------
		#region 公開メソッド
		public static bool[] SaveAll(FileMode mode = FileMode.Create,params IDataFile[] files)
		{
			bool[] ret;
			if (files == null)
			{
				ret = new bool[0];
			}
			else
			{
				ret=Array.ConvertAll(files, (item) =>
				{
					return item.Save(mode);
				});
			}
			return ret;
		}
		public static bool[] SaveAll( params IDataFile[] files)
		{
			return SaveAll(FileMode.Create,files);
		}
		public static bool[] LoadAll(bool shouldCreate,params IDataFile[] files)
		{
			bool[] ret;
			if (files == null)
			{
				ret = new bool[0];
			}
			else
			{
				ret=Array.ConvertAll(files, (item) =>
				{
					return item.Load(shouldCreate);
				});
			}
			return ret;
		}
		public static bool[] LoadAll( params IDataFile[] files)
		{
			return LoadAll(true,files);
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
