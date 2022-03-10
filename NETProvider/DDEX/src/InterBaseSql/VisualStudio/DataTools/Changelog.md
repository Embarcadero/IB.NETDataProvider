# Changes for 1.0.1.0

## File Changes
- Removed FireBird.VisualStudio.DataTools.snk
- Added InterBaseSQL.VisualStudio.DataTools.snk

## InterBaseSQL.VisualStudio.DataTools.csproj
- Changed from the Fb snk file to the new InterBase snk file

## Resources.Designer.cs
- Namespace changed from FirebirdSQL to InterBaseSQL
- ResourceManager now reflect ItnerBaseSQL instead of FirebirdSQL

# Changes for 1.0.0.0

## General

	  All class names and name spaces are changed to be IB vs Fb or Fes.  This was done so that both the Fb driver and this driver it is based on will not have any naming conflicts if both used in the same application.

## Common 
- Namespaces changed from FirebirdSql.VisualStudio.DataTools to InterBaseSQL.VisualStudio.DataTools
- Use the IB named versions of classes and variable names where appropriate moved from fb to ib prefixes.	
- All files renamed from FbXxxxx to IBXxxxx with the internal class matching that same change.  
	    For example FbBackup.cs becomes IBBackup.cs and the class FbBackup is now IBBackup
- The string "FirebirdSql.Data.FirebirdClient" changed to "InterBaseSql.Data.InterBaseClient"
		
###	FbDataConnectionUIControl.Designer.cs changed to IBDataConnectionUIControl.Designer.cs
- cboServerType list now "Standalone Server" and "Embedded Server"
- cboCharset lists only IB charactersets.
		
###	FbDataViewSupport.cs now IBDataViewSupport.cs
- The string "FirebirdSql.VisualStudio.DataTools.FbDataViewSupport" now "InterBaseSql.VisualStudio.DataTools.IBDataViewSupport" in constructor
		
	
	
	
	
	
