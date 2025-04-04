/*
 *  Visual Studio DDEX Provider for FirebirdClient
 * 
 *     The contents of this file are subject to the Initial 
 *     Developer's Public License Version 1.0 (the "License"); 
 *     you may not use this file except in compliance with the 
 *     License. You may obtain a copy of the License at 
 *     http://www.firebirdsql.org/index.php?op=doc&id=idpl
 *
 *     Software distributed under the License is distributed on 
 *     an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either 
 *     express or implied.  See the License for the specific 
 *     language governing rights and limitations under the License.
 * 
 *  Copyright (c) 2005 Carlos Guzman Alvarez
 *  Portions created by Embarcadero are Copyright (C) Embarcadero.
 *
 *  All Rights Reserved.
 *   
 *  Contributors:
 *    Jiri Cincura (jiri@cincura.net)
 */

using System;
using Microsoft.VisualStudio.Data;

namespace InterBaseSql.VisualStudio.DataTools
{
    internal class IBDataViewSupport : DataViewSupport
    {
        #region � Constructors �

        public IBDataViewSupport()
            : base("InterBaseSql.VisualStudio.DataTools.IBDataViewSupport", typeof(IBDataViewSupport).Assembly)
		{
            System.Diagnostics.Trace.WriteLine("IBDataViewSupport()");
		}

        #endregion
    }
}
