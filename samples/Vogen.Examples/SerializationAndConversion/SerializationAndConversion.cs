﻿// ReSharper disable UnusedVariable
#pragma warning disable CS0219
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ServiceStack.Text;
using Vogen.Examples.Types;
using NewtonsoftJsonSerializer = Newtonsoft.Json.JsonConvert;
using SystemTextJsonSerializer = System.Text.Json.JsonSerializer;

namespace Vogen.Examples.SerializationAndConversion
{
    public class SerializationAndConversionExamples : IScenario
    {
        private static readonly DateTimeOffset _date1 = new DateTimeOffset(1970, 6, 10, 14, 01, 02, TimeSpan.Zero) + TimeSpan.FromTicks(12345678);

        public Task Run()
        {
            SerializeWithNewtonsoftJson();
            SerializeWithSystemTextJson();
            SerializeWithServiceStackTextJson();

            return Task.CompletedTask;
        }

        public void SerializeWithServiceStackTextJson()
        {
            SsdtDateTimeOffsetVo orig = SsdtDateTimeOffsetVo.From(_date1);

            string json = JsonSerializer.SerializeToString(orig);
            
            var deserialised = JsonSerializer.DeserializeFromString<SsdtDateTimeOffsetVo>(json);
            
            Debug.Assert(deserialised.Value == orig.Value); 
        }

        public void SerializeWithNewtonsoftJson()
        {
            var g1 = NewtonsoftJsonDateTimeOffsetVo.From(_date1);

            string serialized = NewtonsoftJsonSerializer.SerializeObject(g1);
            string serializedString = NewtonsoftJsonSerializer.SerializeObject(g1.Value);

            var deserializedVo =
                NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonDateTimeOffsetVo>(serializedString);
        }

        public void SerializeWithSystemTextJson()
        {
            var foo = SystemTextJsonDateTimeOffsetVo.From(_date1);

            string serializedFoo = SystemTextJsonSerializer.Serialize(foo);
            string serializedString = SystemTextJsonSerializer.Serialize(foo.Value);

            var deserializedVo = SystemTextJsonSerializer.Deserialize<SystemTextJsonDateTimeOffsetVo>(serializedString);
        }
    }
}