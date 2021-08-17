﻿/*
 *    The contents of this file are subject to the Initial
 *    Developer's Public License Version 1.0 (the "License");
 *    you may not use this file except in compliance with the
 *    License. You may obtain a copy of the License at
 *    https://github.com/FirebirdSQL/NETProvider/blob/master/license.txt.
 *
 *    Software distributed under the License is distributed on
 *    an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either
 *    express or implied. See the License for the specific
 *    language governing rights and limitations under the License.
 *
 *    All Rights Reserved.
 */

//$Authors = Jiri Cincura (jiri@cincura.net)

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FirebirdSql.Data.Common
{
	internal abstract class DatabaseBase
	{
		public Action<IscException> WarningMessage { get; set; }

		public abstract int Handle { get; }
		public int TransactionCount { get; set; }
		public string ServerVersion { get; protected set; }
		public Charset Charset { get; set; }
		public short PacketSize { get; set; }
		public short Dialect { get; set; }
		public abstract bool HasRemoteEventSupport { get; }
		public abstract bool ConnectionBroken { get; }

		public abstract void Attach(DatabaseParameterBufferBase dpb, string database, byte[] cryptKey);
		public abstract ValueTask AttachAsync(DatabaseParameterBufferBase dpb, string database, byte[] cryptKey, CancellationToken cancellationToken = default);

		public abstract void AttachWithTrustedAuth(DatabaseParameterBufferBase dpb, string database, byte[] cryptKey);
		public abstract ValueTask AttachWithTrustedAuthAsync(DatabaseParameterBufferBase dpb, string database, byte[] cryptKey, CancellationToken cancellationToken = default);

		public abstract void Detach();
		public abstract ValueTask DetachAsync(CancellationToken cancellationToken = default);

		public abstract void CreateDatabase(DatabaseParameterBufferBase dpb, string database, byte[] cryptKey);
		public abstract ValueTask CreateDatabaseAsync(DatabaseParameterBufferBase dpb, string database, byte[] cryptKey, CancellationToken cancellationToken = default);

		public abstract void CreateDatabaseWithTrustedAuth(DatabaseParameterBufferBase dpb, string database, byte[] cryptKey);
		public abstract ValueTask CreateDatabaseWithTrustedAuthAsync(DatabaseParameterBufferBase dpb, string database, byte[] cryptKey, CancellationToken cancellationToken = default);

		public abstract void DropDatabase();
		public abstract ValueTask DropDatabaseAsync(CancellationToken cancellationToken = default);

		public abstract TransactionBase BeginTransaction(TransactionParameterBuffer tpb);
		public abstract ValueTask<TransactionBase> BeginTransactionAsync(TransactionParameterBuffer tpb, CancellationToken cancellationToken = default);

		public abstract StatementBase CreateStatement();
		public abstract StatementBase CreateStatement(TransactionBase transaction);

		public abstract DatabaseParameterBufferBase CreateDatabaseParameterBuffer();

		public abstract List<object> GetDatabaseInfo(byte[] items);
		public abstract ValueTask<List<object>> GetDatabaseInfoAsync(byte[] items, CancellationToken cancellationToken = default);

		public abstract List<object> GetDatabaseInfo(byte[] items, int bufferLength);
		public abstract ValueTask<List<object>> GetDatabaseInfoAsync(byte[] items, int bufferLength, CancellationToken cancellationToken = default);

		public abstract void CloseEventManager();
		public abstract ValueTask CloseEventManagerAsync(CancellationToken cancellationToken = default);

		public abstract void QueueEvents(RemoteEvent events);
		public abstract ValueTask QueueEventsAsync(RemoteEvent events, CancellationToken cancellationToken = default);

		public abstract void CancelEvents(RemoteEvent events);
		public abstract ValueTask CancelEventsAsync(RemoteEvent events, CancellationToken cancellationToken = default);

		public abstract void CancelOperation(int kind);
		public abstract ValueTask CancelOperationAsync(int kind, CancellationToken cancellationToken = default);

		public string GetServerVersion()
		{
			var items = new byte[]
			{
				IscCodes.isc_info_firebird_version,
				IscCodes.isc_info_end
			};
			var info = GetDatabaseInfo(items, IscCodes.BUFFER_SIZE_256);
			return (string)info[info.Count - 1];
		}
		public async ValueTask<string> GetServerVersionAsync(CancellationToken cancellationToken = default)
		{
			var items = new byte[]
			{
				IscCodes.isc_info_firebird_version,
				IscCodes.isc_info_end
			};
			var info = await GetDatabaseInfoAsync(items, IscCodes.BUFFER_SIZE_256, cancellationToken).ConfigureAwait(false);
			return (string)info[info.Count - 1];
		}
	}
}
