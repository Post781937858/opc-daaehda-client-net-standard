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
	/// The value of an attribute at a point in time.
	/// </summary>
	[Serializable]
	public class TsCHdaAttributeValue : ICloneable
	{
		///////////////////////////////////////////////////////////////////////
		#region Fields
		
		private DateTime _timestamp = DateTime.MinValue;

		#endregion

		///////////////////////////////////////////////////////////////////////
		#region Properties

		/// <summary>
		/// The value of the data.
		/// </summary>
		public object Value { get; set; }

		/// <summary>
		/// The timestamp associated with the value.
		/// The <see cref="LicenseHandler.TimeAsUTC">OpcBase.TimeAsUTC</see> property defines
		/// the time format (UTC or local time).
		/// </summary>
		public DateTime Timestamp
		{
			get { return _timestamp; }
			set { _timestamp = value; }
		}

		#endregion

		///////////////////////////////////////////////////////////////////////
		#region ICloneable Members

		/// <summary>
		/// Creates a deep copy of the object.
		/// </summary>
		public virtual object Clone()
		{
			Technosoftware.DaAeHdaClient.Hda.TsCHdaAttributeValue clone = (Technosoftware.DaAeHdaClient.Hda.TsCHdaAttributeValue)MemberwiseClone();
			clone.Value = Technosoftware.DaAeHdaClient.OpcConvert.Clone(Value);
			return clone;
		}

		#endregion
	}
}
