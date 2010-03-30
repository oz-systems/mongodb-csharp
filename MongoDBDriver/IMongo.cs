using System;

namespace MongoDB.Driver
{
	public interface IMongo
	{
		/// <summary>
		/// Gets the connection string.
		/// </summary>
		/// <value>The connection string.</value>
		string ConnectionString { get; }
		/// <summary>
		/// Gets the named database.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		Database GetDatabase(String name);
		/// <summary>
		/// Gets the <see cref="MongoDB.Driver.Database"></see> with the specified name.
		/// </summary>
		/// <value></value>
		Database this[String name] { get; }
		/// <summary>
		/// Connects this instance.
		/// </summary>
		/// <returns></returns>
		Boolean Connect();
		/// <summary>
		/// Disconnects this instance.
		/// </summary>
		/// <returns></returns>
		Boolean Disconnect();
	}
}
