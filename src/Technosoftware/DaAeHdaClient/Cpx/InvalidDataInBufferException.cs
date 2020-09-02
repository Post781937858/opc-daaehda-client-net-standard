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
using System.Runtime.Serialization;
#endregion

namespace Technosoftware.DaAeHdaClient.Cpx
{
	/// <summary>
	/// Raised if the data in buffer is not consistent with the schema.
	/// </summary>
	[Serializable]
	public class TsCCpxInvalidDataInBufferException : ApplicationException
	{
		private const string Default = "The data in the buffer cannot be read because it is not consistent with the schema.";
		/// <remarks/>
		public TsCCpxInvalidDataInBufferException() : base(Default) { }
		/// <remarks/>
		public TsCCpxInvalidDataInBufferException(string message) : base(Default + Environment.NewLine + message) { }
		/// <remarks/>
		public TsCCpxInvalidDataInBufferException(Exception e) : base(Default, e) { }
		/// <remarks/>
		public TsCCpxInvalidDataInBufferException(string message, Exception innerException) : base(Default + Environment.NewLine + message, innerException) { }
		/// <remarks/>
		protected TsCCpxInvalidDataInBufferException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
