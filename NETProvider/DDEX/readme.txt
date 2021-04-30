Visual Studio 2017/2019 DDEX Provider for InterBase
======================================================================

Code based on code created by Sean Leyne (Broadview Software)

This project is supported by:
-----------------------------

  Embarcadero
  Jeff Overcash  
	
-----------------------------

The DDEX Provider for InterBase provides integration of InterBaseClient into Visual Studio. In order to use InterBase Client in Visual Studio, you have to perform these steps.

0. Preinstallation steps.
-------------------------
If not installed using an installer, copy files from this archive to some place (must be accessible for VS). 

1. Install InterBaseClient into the GAC.
---------------------------------------
Recommended way is to install from the installer.  Manually you can use gacutil utility to do this or to check whether it's correctly installed. The gacutil show you also the signature for assembly, that will be used later.

2. Modify machine.config file.
------------------------------
Modify it like this (for 64bit systems you have to edit "32bit version" of this file, because Visual Studio is 32bit, but there's no problem with editing the "64bit version" too):

<configuration>
  <system.data>
    <DbProviderFactories>
      ...
      <add name="InterBaseClient Data Provider" invariant="InterBaseSql.Data.InterBaseClient" description=".NET Framework Data Provider for InterBase" type="InterBaseSql.Data.InterBaseClient.InterBaseClientFactory, InterBaseSql.Data.InterBaseClient, Version=%Version%, Culture=neutral, PublicKeyToken=73f45bff97b4c31b" />
      ...
    </DbProviderFactories>
  </system.data>
</configuration>

And substitute (these informations you can find using gacutil): 
  - %Version% with the version of the provider assembly that you have in the GAC (currently 7.10.2.0).

Note:
  Notice, that in configSections there isn't signature of InterBaseClient but the signature of assembly from framework.
	There must be only one entry for the ItnerBaseSQL.Data.InterBaseClient.

3. Import registry file.
------------------------

For VS2015 and earlier 
There's a couple of *.reg files in installation. There are files for 32bit and for 64bit system, so select appropriate version for your system. There are also files in "withSDK" directory. These can be used for Visual Studio with VS SDK installed. The files not in this directory are for systems without Visual Studio SDK (it's *not* the .NET FW SDK!) and it's probably the best choice for a lot of developers. The selected registry file needs be modified to set the correct paths. To do this, substitute %Path% in the file with path for the DDEX files where you copyied them in step 0 (remember to backslash the backslash character).

For VS 2017+
Go over the documentation in ADO Driver documentation.  This outlines how to load the private registry and how to update things.

IMPORTANT: The DDEX provider didn't work with Express editions of Visual Studio.

(If you want to build the sources you will need to have C# and C++ installed and VS SDK.)

