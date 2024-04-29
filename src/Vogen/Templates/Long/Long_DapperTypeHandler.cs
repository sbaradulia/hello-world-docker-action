﻿
        public class DapperTypeHandler : global::Dapper.SqlMapper.TypeHandler<VOTYPE>
        {
            public override void SetValue(global::System.Data.IDbDataParameter parameter, VOTYPE value)
            {
                parameter.DbType = global::System.Data.DbType.Int64;
                parameter.Value = value.Value;
            }

            public override VOTYPE Parse(object value)
            {
                return value switch
                {
                    global::System.Int64 longValue => VOTYPE.Deserialize(longValue),
                    global::System.Int32 intValue => VOTYPE.Deserialize(intValue),
                    global::System.Int16 shortValue => VOTYPE.Deserialize(shortValue),
                    global::System.String stringValue when  !global::System.String.IsNullOrEmpty(stringValue) && long.TryParse(stringValue, out var result) => VOTYPE.Deserialize(result),
                    _ => throw new global::System.InvalidCastException($"Unable to cast object of type {value.GetType()} to VOTYPE"),
                };
            }
        }