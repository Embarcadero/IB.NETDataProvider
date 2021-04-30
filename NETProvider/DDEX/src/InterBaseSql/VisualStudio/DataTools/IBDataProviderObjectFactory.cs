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
using System.Runtime.InteropServices;
using Microsoft.Data.ConnectionUI;
using Microsoft.VisualStudio.Data;
using Microsoft.VisualStudio.Data.AdoDotNet;

namespace InterBaseSql.VisualStudio.DataTools
{
    [Guid(GuidList.GuidObjectFactoryServiceString)]
    internal class IBDataProviderObjectFactory : AdoDotNetProviderObjectFactory
    {
        #region · Constructors ·

        public IBDataProviderObjectFactory() : base()
        {
            System.Diagnostics.Trace.WriteLine("IBDataProviderObjectFactory()");
        }

        #endregion

        #region · Methods ·

        public override object CreateObject(Type objectType)
        {
            System.Diagnostics.Trace.WriteLine(String.Format("IBDataProviderObjectFactory::CreateObject({0})", objectType.FullName));

            if (objectType == typeof(DataConnectionSupport))
            {
                return new IBDataConnectionSupport();
            }
            else if (objectType == typeof(IDataConnectionUIControl) || objectType == typeof(DataConnectionUIControl))
            {
                return new IBDataConnectionUIControl();
            }
            else if (objectType == typeof(IDataConnectionProperties) || objectType == typeof(DataConnectionProperties))
            {
                return new IBDataConnectionProperties();
            }

            return base.CreateObject(objectType);
        }
        
        #endregion
    }
}
