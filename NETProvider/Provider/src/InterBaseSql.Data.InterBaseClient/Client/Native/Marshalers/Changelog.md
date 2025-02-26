# Changes for 10.0.0

## JaggedArrayMarshaler.cs removed

## XsqldaMarshaler.cs
* GetIntPtr replaced with IntPtr.Add
* MarshalManagedToNative - fixed a bug with boolean data types used in parameters (the array size should be 2 not 1)
* GetIntPtr removed

# Changes for 7.14.0

## XsqldaMarsher.cs
*   Added passing the CharSet and DB to some procedures to better support Dialect 1

# Changes for 7.10.2

##  General - 	
*	  All classnames and namespaces are changed to be IB vs FB or Fes.  This was done so that both the FB driver this driver is based on and this one will not have any naming conflict if both used in the same application.

##  Common 
*    Namespace changed from FirebirdSql.Data.Client.Native.Marshalers to InterBaseSql.Data.Client.Native.Marshalers
*	  Uses InterBaseSql.Data.Common instead of FirebirdSql.Data.Common
*		In general all SQLDA/SQLVar code now defaults to V2 with specific V1 versions re-added where needed.  
	
##	Added ArrayDescMarshal_V2.cs For support of the V2 array descriptor
	
##	ArrayDescMarshaler.cs
*	  added CleanUpNativeData2 to clean up the Native V2 of arrays
*    added MarshalManagedToNative2 to Marshal between a V2 Array structure and the Array Descriptor
*		added MarshalManagedToNative overload for the new ArrayDescMarshal_V2 type
		
##	Added CTimeStructure.cs to work with hte CTimeStructue of hte interbase DateTime API calls.	
	
##	Added JaggedArrayMarshaler.cs for API support of Events.
	
##	XsqldaMarshaler.cs (Generally added V2 suppport as primary with the old V1 versions getting renamed with V1 in their naming structure) 
*	  Added support of both V1 and V2 of XSQLDA structure.
*		Added V2 support to CleanUpNativeData
*		Added V2 support to MarshalManagedToNative
*		MarshalManagedToNative renamed to MarshalManagedToNative_V1
*		Added new MarshalManagedToNative that supports V2
*		Renamed MarshalXSQLVARNativeToManaged to MarshalXSQLVARV1NativeToManaged
*		Added new MarshalXSQLVARNativeToManaged that supports V2
*		Added V2 support ofr XSQLDA to MarshalNativeToManaged 
*		MarshalXSQLVARNativeToManaged changed signature for V1 version of XSQLVAR
*		Added overloaded MarshalXSQLVARNativeToManaged for V2 version of XSQLVar
*		ComputeLength supports both V1 and V2 versions of XSQLVar and params now include a version parameter
*		GetBytes removed Firebird specific datatype and changed signature to support V1 version of XSQLVar
*		Added overloaded GetBytes to support V2 od XSQLVar
*		GetStringBuffer now used for V2 and the Stringbuffer returned is 68 bytes
*		Added (renamed) GetStringBuffer_V1 which is hte old 32 byte string buffer routine used with V1 structures
		
##	XSQLVAR.cs
*    Semantically changed from representing a V1 version of hte XSQLVar to the V2 (IB surrent)
*    Added sqlprecision field
*    Sizes of the byte arrays increased from 32 to 68

##  XSQLVAR_V1.cs It the old XSQLVAR.cs renamed.
		
	
	
