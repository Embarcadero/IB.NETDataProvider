/* Use these 'wrappers' to put functionality into
   any of your databases. */

/* Date/Time routines */

declare external function EF_UTCCurrentTime
  returns
  timestamp /* free_it */
  entry_point 'UTCCurrentTime' module_name 'EntityFrameworkUDF';

declare external function EF_DateAdd
  cstring(7),
  numeric(18, 0),
	timestamp
  returns
  timestamp /* free_it */
  entry_point 'DateAdd' module_name 'EntityFrameworkUDF';

declare external function EF_DateDiff
  cstring(12),
  timestamp,
	timestamp
  returns
  numeric(18,0) by value /* free_it */
  entry_point 'DateDiff' module_name 'EntityFrameworkUDF';

/* Mathematical functions */

declare external function EF_Abs
  double precision
  returns
  double precision by value
  entry_point 'fn_Abs' module_name 'EntityFrameworkUDF';

declare external function EF_Ceiling
  double precision
  returns
  numeric(18,0) by value
  entry_point 'fn_Ceiling' module_name 'EntityFrameworkUDF';

declare external function EF_Floor
  double precision
  returns
  numeric(18, 0) by value
  entry_point 'fn_Floor' module_name 'EntityFrameworkUDF';

declare external function EF_Round
  double precision,
	integer
  returns
  double precision by value
  entry_point 'fn_Round' module_name 'EntityFrameworkUDF';

declare external function EF_Power
  double precision,
	double precision
  returns
  double precision by value
  entry_point 'fn_Power' module_name 'EntityFrameworkUDF';
	
declare external function EF_Trunc
  double precision,
	integer
  returns
  double precision by value
  entry_point 'fn_Truncate' module_name 'EntityFrameworkUDF';

declare external function EF_BitAnd
  numeric(18, 0),
	numeric(18, 0)
  returns
  NUMERIC(18, 0) by value
  entry_point 'fn_BitAnd' module_name 'EntityFrameworkUDF';

declare external function EF_BitNot
	numeric(18, 0)
  returns
  NUMERIC(18, 0) by value
  entry_point 'fn_BitNot' module_name 'EntityFrameworkUDF';

declare external function EF_BitOr
  numeric(18, 0),
	numeric(18, 0)
  returns
  NUMERIC(18, 0) by value
  entry_point 'fn_BitOr' module_name 'EntityFrameworkUDF';

declare external function EF_BitXor
  numeric(18, 0),
	numeric(18, 0)
  returns
  NUMERIC(18, 0) by value
  entry_point 'fn_BitXor' module_name 'EntityFrameworkUDF';

declare external function EF_Mod
  numeric(18, 0),
	numeric(18, 0)
  returns
  NUMERIC(18, 0) by value
  entry_point 'fn_Mod' module_name 'EntityFrameworkUDF';

declare external function EF_BINSHL
  numeric(18, 0),
	numeric(18, 0)
  returns
  NUMERIC(18, 0) by value
  entry_point 'fn_BinSHL' module_name 'EntityFrameworkUDF';

declare external function EF_BINSHR
  numeric(18, 0),
	numeric(18, 0)
  returns
  NUMERIC(18, 0) by value
  entry_point 'fn_BinSHR' module_name 'EntityFrameworkUDF';

/* String functions */

declare external function EF_Reverse
  cstring(2048)
  returns cstring(2048) /* free_it */
  entry_point 'Reverse' module_name 'EntityFrameworkUDF';

declare external function EF_Position
  cstring(2048),
	cstring(2048),
	integer
  returns integer by value /* free_it */
  entry_point 'Position' module_name 'EntityFrameworkUDF';

declare external function EF_Length
  cstring(2048)
  returns integer by value /* free_it */
  entry_point 'StringLength' module_name 'EntityFrameworkUDF';

declare external function EF_Lower
  cstring(2048)
  returns cstring(2048) /* free_it */
  entry_point 'ToLower' module_name 'EntityFrameworkUDF';

declare external function EF_Trim
  cstring(8),
  cstring(2048)
  returns cstring(2048) /* free_it */
  entry_point 'Trim' module_name 'EntityFrameworkUDF';

declare external function EF_Left
  cstring(2048),
	integer
  returns cstring(2048) /* free_it */
  entry_point 'Left' module_name 'EntityFrameworkUDF';

declare external function EF_Right
  cstring(2048),
	integer
  returns cstring(2048) /* free_it */
  entry_point 'Right' module_name 'EntityFrameworkUDF';

declare external function EF_Replace
  cstring(2048),
	cstring(2048),
	cstring(2048)
  returns cstring(2048) /* free_it */
  entry_point 'Replace' module_name 'EntityFrameworkUDF';

declare external function EF_SubStr
  cstring(2048),
	integer,
	integer
  returns cstring(2048) /* free_it */
  entry_point 'SubStr' module_name 'EntityFrameworkUDF';

declare external function EF_NewGUID
  returns cstring(16) character set OCTETS /* free_it */
  entry_point 'NewGuid' module_name 'EntityFrameworkUDF';

declare external function EF_UUID_TO_CHAR
  cstring(16) character set OCTETS
  returns cstring(36)  /* free_it */
  entry_point 'UUID_TO_CHAR' module_name 'EntityFrameworkUDF';
	
declare external function EF_CHAR_TO_UUID
  cstring(36) 
  returns cstring(16) character set OCTETS /* free_it */
  entry_point 'CHAR_TO_UUID' module_name 'EntityFrameworkUDF';	



