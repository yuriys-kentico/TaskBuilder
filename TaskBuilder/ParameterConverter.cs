﻿using System;
using Newtonsoft.Json;
using TaskBuilder.Models.Function;

namespace TaskBuilder
{
    internal class ParameterConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(FieldParameter);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize(reader, objectType);
        }

        public override void WriteJson(JsonWriter writer, dynamic value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.Value);
        }
    }
}