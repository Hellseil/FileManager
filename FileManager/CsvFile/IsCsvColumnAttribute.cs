/******************************************************************************
 * ファイル	：IsCsvColumnAttribute.cs
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
using System;

#endregion

namespace FileManagerLibrary.CsvFile
{
[AttributeUsage(AttributeTargets.Property)]
	public class IsCsvColumnAttribute:Attribute
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
		/// IsCsvColumnAttributeを生成します
		/// </summary>
		public IsCsvColumnAttribute(bool isColumn=true)
		{
			this.IsColumn=isColumn;
		}

		/// <summary>
		/// IsCsvColumnAttributeを破棄します
		/// </summary>
		~IsCsvColumnAttribute()
		{

		}
		#endregion

		//---------------------------------------------------------------------
		#region プロパティ
		public bool IsColumn
		{
			get; private set;
		}
		#endregion

		//---------------------------------------------------------------------
		#region 公開メソッド

		#endregion

		//---------------------------------------------------------------------
		#region 非公開メソッド

		#endregion

		//---------------------------------------------------------------------
		#region インターフェース実装

		#endregion

	}
}
