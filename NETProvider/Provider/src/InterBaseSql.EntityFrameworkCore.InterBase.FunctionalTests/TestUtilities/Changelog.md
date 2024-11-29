# Changes for 10.0.1

## Added PsuedoGeneratorIntValues.cs

## IBTestHelpers.cs
** IBTestHelpers inherits from RelationalTestHelpers
** UseProviderOptions now returns DbContextOptionsBuilder

## IBTestStore.cs
** Added ServerVersion property
** Added ServerLessThan4 method (always returns true)
** Script now adds EF_ASCII_CHAR UDF
** AddProviderOptions sets WithExplicitParameterTypes and WithExplicitStringLiteralTypes to false (this fixes many problems, but it breaks a few other tests so needs a better place to set. 