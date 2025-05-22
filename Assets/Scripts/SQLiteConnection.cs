using System.Runtime.InteropServices;
using System;
using UnityEngine;

public class SQLiteConnection : MonoBehaviour
{
	private IntPtr db;

	void Start()
	{
		string dbPath = Application.persistentDataPath + "/test.db";

		// Abrir o crear la base de datos
		int result = SQLiteWrapper.sqlite3_open(dbPath, out db);
		if (result != 0)
		{
			Debug.LogError("Error abriendo la base de datos.");
			return;
		}

		Debug.Log("Base de datos abierta en: " + dbPath);

		// Crear una tabla
		ExecuteSQL("CREATE TABLE IF NOT EXISTS players (id INTEGER PRIMARY KEY, name TEXT, score INTEGER);");

		// Insertar datos
		ExecuteSQL("INSERT INTO players (name, score) VALUES ('Jorge', 9001);");

		// Cerrar la base
		SQLiteWrapper.sqlite3_close(db);
		Debug.Log("Base cerrada.");
	}

	void ExecuteSQL(string sql)
	{
		IntPtr errMsg;
		int rc = SQLiteWrapper.sqlite3_exec(db, sql, IntPtr.Zero, IntPtr.Zero, out errMsg);

		if (rc != 0)
		{
			string error = Marshal.PtrToStringAnsi(errMsg);
			Debug.LogError("SQLite error: " + error);
			SQLiteWrapper.sqlite3_free(errMsg);
		}
		else
		{
			Debug.Log("SQL ejecutado correctamente.");
		}
	}
}
