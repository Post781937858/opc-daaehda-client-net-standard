#region Copyright (c) 2011-2020 Technosoftware GmbH. All rights reserved
//-----------------------------------------------------------------------------
// Copyright (c) 2011-2020 Technosoftware GmbH. All rights reserved
// Web: https://www.technosoftware.com 
// 
// The source code in this file is covered under a dual-license scenario:
//   - Owner of a purchased license: RPL 1.5
//   - GPL V3: everybody else
//
// RPL license terms accompanied with this source code.
// See https://technosoftware.com/license/RPLv15License.txt
//
// GNU General Public License as published by the Free Software Foundation;
// version 3 of the License are accompanied with this source code.
// See https://technosoftware.com/license/GPLv3License.txt
//
// This source code is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE.
//-----------------------------------------------------------------------------
#endregion Copyright (c) 2011-2020 Technosoftware GmbH. All rights reserved

#region Using Directives
using System;
#endregion

namespace Technosoftware.DaAeHdaClient.Da
{
	/// <summary>
	/// The set of possible server states.
	/// </summary>
	public enum TsCDaServerState
	{
		/// <summary>
		/// The server state is not known.
		/// </summary>
		Unknown,

		/// <summary>
		/// The server is running normally.
		/// </summary>
		Running,

		/// <summary>
		/// The server is not functioning due to a fatal error.
		/// </summary>
		Failed,

		/// <summary>
		/// The server cannot load its configuration information.
		/// </summary>
		NoConfig,

		/// <summary>
		/// The server has halted all communication with the underlying hardware.
		/// </summary>
		Suspended,

		/// <summary>
		/// The server is disconnected from the underlying hardware.
		/// </summary>
		Test,

		/// <summary>
		/// The server cannot communicate with the underlying hardware.
		/// </summary>
		CommFault
	}
}
