﻿
        class VOTYPESystemTextJsonConverter : global::System.Text.Json.Serialization.JsonConverter<VOTYPE>
        {
            public override VOTYPE Read(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
            {
#if NET5_0_OR_GREATER
__NORMAL__                return VOTYPE.Deserialize(
__NORMAL__                    options.NumberHandling == global::System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString && reader.TokenType == global::System.Text.Json.JsonTokenType.String
__NORMAL__                    ? global::System.Single.Parse(reader.GetString(), global::System.Globalization.NumberStyles.Any, global::System.Globalization.CultureInfo.InvariantCulture)
__NORMAL__                    : reader.GetSingle()
__NORMAL__                );
#else
__NORMAL__                return VOTYPE.Deserialize(reader.GetSingle());
#endif
__STRING__                return VOTYPE.Deserialize(global::System.Single.Parse(reader.GetString(), global::System.Globalization.NumberStyles.Any, global::System.Globalization.CultureInfo.InvariantCulture));
            }

            public override void Write(System.Text.Json.Utf8JsonWriter writer, VOTYPE value, global::System.Text.Json.JsonSerializerOptions options)
            {
__NORMAL__                writer.WriteNumberValue(value.Value);
__STRING__                writer.WriteStringValue(value.Value.ToString(global::System.Globalization.CultureInfo.InvariantCulture));
            }
            
#if NET6_0_OR_GREATER
            public override VOTYPE ReadAsPropertyName(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
            {
                return VOTYPE.Deserialize(global::System.Single.Parse(reader.GetString(), global::System.Globalization.NumberStyles.Any, global::System.Globalization.CultureInfo.InvariantCulture));
            }

            public override void WriteAsPropertyName(System.Text.Json.Utf8JsonWriter writer, VOTYPE value, global::System.Text.Json.JsonSerializerOptions options)
            {
                writer.WritePropertyName(value.Value.ToString(global::System.Globalization.CultureInfo.InvariantCulture));
            }
#endif            
        }