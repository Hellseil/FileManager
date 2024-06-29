using System;
using System.IO;

namespace FileManagerLibrary
{
	public interface IDataFile
	{
		/// <summary>
		/// 最後に発生したエラー
		/// </summary>
		Exception LastException { get; }
		/// <summary>
		/// ファイルパス
		/// </summary>
		string FilePath { get; }
		/// <summary>
		/// ファイルパス設定
		/// </summary>
		/// <param name="filepath"></param>
		void SetFilePath(string filepath);
		/// <summary>
		/// 読込
		/// </summary>
		/// <param name="shouldCreate"></param>
		/// <returns></returns>
		bool Load(bool shouldCreate=true);
		/// <summary>
		/// 保存
		/// </summary>
		/// <param name="mode"></param>
		/// <returns></returns>
		bool Save(FileMode mode = FileMode.Create);
	}
}