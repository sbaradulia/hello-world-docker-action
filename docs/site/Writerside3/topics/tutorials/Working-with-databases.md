# Working with databases

<note>
This topic is incomplete and is being improved.
</note>

Vogen has converters and serializers for databases, including:

* Dapper
* EFCore
* [LINQ to DB](https://github.com/linq2db/linq2db)

They are controlled by the `Conversions` enum. The following specifies Newtonsoft.Json and System.Text.Json converters:

```c#
[ValueObject(conversions: Conversions.NewtonsoftJson | Conversions.SystemTextJson, underlyingType: typeof(float))]
public readonly partial struct Celsius { }
```

If you don't want any conversions, then specify `Conversions.None`.

If you want your own conversion, then again specify none, and implement them yourself, just like any other type.  But be aware that even serializers will get the same compilation errors for `new` and `default` when trying to create VOs.

If you want to use Dapper, remember to register it—something like this:

```c#
SqlMapper.AddTypeHandler(new Customer.DapperTypeHandler());
```

See the examples folder for more information.
