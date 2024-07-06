/******************************************************************************
 * ファイル	：CsvHedderAttribute.cs
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
	public class CsvHeaderTextAttribute:Attribute
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
		/// CsvHedderAttributeを生成します
		/// </summary>
		public CsvHeaderTextAttribute(string headerText)
		{
			this.HeaderText = headerText;
		}

		/// <summary>
		/// CsvHedderAttributeを破棄します
		/// </summary>
		~CsvHeaderTextAttribute()
		{

		}
		#endregion

		//---------------------------------------------------------------------
		#region プロパティ
		public string HeaderText
		{
			get;private set;
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
