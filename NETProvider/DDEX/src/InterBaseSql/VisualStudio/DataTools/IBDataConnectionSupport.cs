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
using Microsoft.VisualStudio.Data.AdoDotNet;

namespace InterBaseSql.VisualStudio.DataTools
{
    internal class IBDataConnectionSupport : AdoDotNetConnectionSupport
    {
        #region · Constructors ·

        public IBDataConnectionSupport() 
            : base("InterBaseSql.Data.InterBaseClient")
        {
            System.Diagnostics.Trace.WriteLine("IBDataConnectionSupport()");
        }

        #endregion

        #region · Protected Methods ·

        protected override DataSourceInformation CreateDataSourceInformation()
        {
            System.Diagnostics.Trace.WriteLine("IBDataConnectionSupport::CreateDataSourceInformation()");

            return new IBDataSourceInformation(base.Site as DataConnection);
        }

        protected override DataObjectIdentifierConverter CreateObjectIdentifierConverter()
        {
            return new IBDataObjectIdentifierConverter(base.Site as DataConnection);
        }

        protected override object GetServiceImpl(Type serviceType)
        {
            System.Diagnostics.Trace.WriteLine(String.Format("IBDataConnectionSupport::GetServiceImpl({0})", serviceType.FullName));

            if (serviceType == typeof(DataViewSupport))
            {
                return new IBDataViewSupport();
            }
            else if (serviceType == typeof(DataObjectSupport))
            {
                return new IBDataObjectSupport();
            }
            else if (serviceType == typeof(DataObjectIdentifierResolver))
            {
                return new IBDataObjectIdentifierResolver(base.Site as DataConnection);
            }

            return base.GetServiceImpl(serviceType);
        }

        #endregion
    }
}
