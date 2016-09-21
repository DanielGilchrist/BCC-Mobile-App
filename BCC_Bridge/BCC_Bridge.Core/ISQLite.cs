using System;
using SQLite.Net;

namespace BCC_Bridge.Core
{
	public interface ISQLite
	{
		SQLiteConnection GetConnection();
	}
}
