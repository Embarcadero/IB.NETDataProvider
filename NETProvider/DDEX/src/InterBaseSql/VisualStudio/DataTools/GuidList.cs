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

// Guids.cs
// MUST match guids.h

using System;

namespace InterBaseSql.VisualStudio.DataTools
{
    static class GuidList
    {
        public const string GuidDataToolsPkgString = "0095710D-F7DC-4FA1-8FEB-C8153AA5DF75";
        public const string GuidObjectFactoryServiceString = "EADC6C1E-17D2-42FF-9816-19850428BCF1";

        public static readonly Guid GuidDataToolsPkg = new Guid(GuidDataToolsPkgString);
        public static readonly Guid GuidObjectFactoryService = new Guid(GuidObjectFactoryServiceString);
    };
}