/*  Copyright (C) 2008 - 2012 Jordan Marr

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library. If not, see <http://www.gnu.org/licenses/>. */

using System.Linq;
using System.Data.Common;
using System.Data.SqlClient;
using Moq;

namespace Marr.Data.TestHelper
{
	/// <summary>
	/// Initializes a new DataMapper instance with a stubbed result set and mocked internals.
	/// </summary>
	public static class StubDataMapperFactory
	{
		/// <summary>
		/// Creates a DataMapper that can be used to test queries.
		/// </summary>
		/// <param name="rs">The stubbed record set.</param>
		/// <returns>Returns a StubDataMapper.</returns>
		public static IDataMapper CreateForQuery(params StubResultSet[] resultSets)
		{
			var readers = resultSets.Select(rs => new StubDataReader(rs)).ToArray();

			//var parameters = MockRepository.GenerateMock<DbParameterCollection>();
			//var command = MockRepository.GenerateMock<DbCommand>();
			//var connection = MockRepository.GenerateMock<DbConnection>();
		    var dbFactory = new Mock<DbProviderFactory>();

			foreach (var reader in readers)
			{
				var parameters = new Mock<DbParameterCollection>();
			    parameters.Setup(p => p.Add(null)).Returns(1);

				var command = new Mock<DbCommand>();
			    command.Setup(c => c.ExecuteReader()).Returns(reader);
			    command.Setup(c => c.Parameters).Returns(parameters.Object);
			    command.Setup(c => c.CreateParameter()).Returns(new SqlParameter());

				var connection = new Mock<DbConnection>();
			    connection.Setup(c => c.CreateCommand()).Returns(command.Object);

			    command.Setup(c => c.Connection).Returns(connection.Object);

				//var dbFactory = MockRepository.GenerateMock<DbProviderFactory>();
			    dbFactory.Setup(f => f.CreateConnection()).Returns(connection.Object);
			}

			return new StubDataMapper(dbFactory.Object);
		}

		/// <summary>
		/// Creates a DataMapper that can be used to test updates.
		/// </summary>
		/// <returns>Returns a StubDataMapper.</returns>
		public static IDataMapper CreateForUpdate()
		{
		    var parameters = new Mock<DbParameterCollection>();

		    var command = new Mock<DbCommand>();
		    command.Setup(c => c.Parameters).Returns(parameters.Object);
			command.Setup(c => c.ExecuteNonQuery()).Returns(1);
			command
				.Setup(c => c.CreateParameter())
				.Returns(new Mock<DbParameter>().Object);

			var connection = new Mock<DbConnection>();
			connection.Setup(c => c.CreateCommand()).Returns(command.Object);

			command.Setup(c => c.Connection).Returns(connection.Object);

			var dbFactory = new Mock<DbProviderFactory>();
			dbFactory.Setup(f => f.CreateConnection()).Returns(connection.Object);

			return new StubDataMapper(dbFactory.Object);
		}

		/// <summary>
		/// Creates a DataMapper that can be used to test inserts.
		/// </summary>
		/// <returns>Returns a StubDataMapper.</returns>
		public static IDataMapper CreateForInsert()
		{
		    var parameters = new Mock<DbParameterCollection>();

		    var command = new Mock<DbCommand>();
		    command.Setup(c => c.Parameters).Returns(parameters.Object);
		    command
		        .Setup(c => c.CreateParameter())
		        .Returns(new Mock<DbParameter>().Object);

		    var connection = new Mock<DbConnection>();
		    connection.Setup(c => c.CreateCommand()).Returns(command.Object);

		    command.Setup(c => c.Connection).Returns(connection.Object);

		    var dbFactory = new Mock<DbProviderFactory>();
		    dbFactory.Setup(f => f.CreateConnection()).Returns(connection.Object);

		    return new StubDataMapper(dbFactory.Object);
        }
	}
}
