
        class VOTYPENewtonsoftJsonConverter : global::Newtonsoft.Json.JsonConverter
        {
            public override bool CanConvert(System.Type objectType)
            {
                return objectType == typeof(VOTYPE);
            }

            public override void WriteJson(global::Newtonsoft.Json.JsonWriter writer, object value, global::Newtonsoft.Json.JsonSerializer serializer)
            {
                var id = (VOTYPE)value;
                serializer.Serialize(writer, id.Value);
            }

            public override object ReadJson(global::Newtonsoft.Json.JsonReader reader, System.Type objectType, object existingValue, global::Newtonsoft.Json.JsonSerializer serializer)
            {
                var result = serializer.Deserialize<global::System.String>(reader);
                return result is not null ? VOTYPE.__Deserialize(result) : null;
            }
        }