﻿namespace Vogen.IntegrationTests.TestTypes.StructVos;

[ValueObject(conversions: Conversions.None, underlyingType: typeof(byte))]
public partial struct ByteVo { }

[ValueObject(conversions: Conversions.None, underlyingType: typeof(byte))]
public partial struct NoConverterByteVo { }

[ValueObject(conversions: Conversions.TypeConverter, underlyingType: typeof(byte))]
public partial struct NoJsonByteVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson, underlyingType: typeof(byte))]
public partial struct NewtonsoftJsonByteVo { }

[ValueObject(conversions: Conversions.SystemTextJson, underlyingType: typeof(byte))]
public partial struct SystemTextJsonByteVo { }

[ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(byte))]
public partial struct BothJsonByteVo { }

[ValueObject(conversions: Conversions.EfCoreValueConverter, underlyingType: typeof(byte))]
public partial struct EfCoreByteVo { }

[ValueObject(conversions: Conversions.DapperTypeHandler, underlyingType: typeof(byte))]
public partial struct DapperByteVo { }

[ValueObject(conversions: Conversions.LinqToDbValueConverter, underlyingType: typeof(byte))]
public partial struct LinqToDbByteVo { }