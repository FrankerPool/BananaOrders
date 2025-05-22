using System;
using System.Runtime.InteropServices;
using System.Text;

public static class SQLiteWrapper
{
	private const string SQLITE_DLL = "sqlite3";

	[DllImport(SQLITE_DLL)]
	public static extern int sqlite3_open(string filename, out IntPtr db);

	[DllImport(SQLITE_DLL)]
	public static extern int sqlite3_close(IntPtr db);

	[DllImport(SQLITE_DLL)]
	public static extern int sqlite3_exec(
		IntPtr db, string sql,
		IntPtr callback, IntPtr arg,
		out IntPtr errMsg);

	[DllImport(SQLITE_DLL)]
	public static extern void sqlite3_free(IntPtr ptr);
}