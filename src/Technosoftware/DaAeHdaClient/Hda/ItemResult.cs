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
	/// Describes the results for an item used in a read processed or raw data request.
	/// </summary>
	[Serializable]
	public class TsCHdaItemResult : TsCHdaItem, IOpcResult
	{
		///////////////////////////////////////////////////////////////////////
		#region Fields

		private OpcResult _result = OpcResult.S_OK;

		#endregion

		///////////////////////////////////////////////////////////////////////
		#region Constructors, Destructor, Initialization

		/// <summary>
		/// Initialize object with default values.
		/// </summary>
		public TsCHdaItemResult() { }

		/// <summary>
		/// Initialize object with the specified ItemIdentifier object.
		/// </summary>
		public TsCHdaItemResult(OpcItem item) : base(item) { }

		/// <summary>
		/// Initializes object with the specified Item object.
		/// </summary>
		public TsCHdaItemResult(TsCHdaItem item) : base(item) { }

		/// <summary>
		/// Initialize object with the specified ItemResult object.
		/// </summary>
		public TsCHdaItemResult(TsCHdaItemResult item)
			: base(item)
		{
			if (item != null)
			{
				Result = item.Result;
				DiagnosticInfo = item.DiagnosticInfo;
			}
		}

		#endregion

		///////////////////////////////////////////////////////////////////////
		#region IOpcResult Members

		/// <summary>
		/// The error id for the result of an operation on an item.
		/// </summary>
		public OpcResult Result
		{
			get { return _result; }
			set { _result = value; }
		}

		/// <summary>
		/// Vendor specific diagnostic information (not the localized error text).
		/// </summary>
		public string DiagnosticInfo { get; set; }

		#endregion
	}
}
