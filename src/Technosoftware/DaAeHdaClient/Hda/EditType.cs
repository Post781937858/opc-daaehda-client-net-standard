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

namespace Technosoftware.DaAeHdaClient.Hda
{
	/// <summary>
	/// The types of modifications that can be applied to an item.
	/// </summary>
	public enum TsCHdaEditType
	{
		/// <summary>
		/// The item was inserted.
		/// </summary>
		Insert = 1,

		/// <summary>
		/// The item was replaced.
		/// </summary>
		Replace = 2,

		/// <summary>
		/// The item was inserted or replaced during an insert/replace operation.
		/// </summary>
		InsertReplace = 3,

		/// <summary>
		/// The item was deleted.
		/// </summary>
		Delete = 4
	}
}
