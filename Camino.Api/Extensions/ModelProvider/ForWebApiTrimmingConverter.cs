using System;
using Camino.Core.Helpers;
using Newtonsoft.Json;

namespace Camino.Api.Extensions.ModelProvider
{
    public class ForWebApiTrimmingConverter : JsonConverter
    {
        public override bool CanConvert(Type objType)
        {
            return objType == typeof(string);
        }
        public override object ReadJson(JsonReader objJsonReader, Type objType, object obj,
            JsonSerializer objJsonSerializer)
        {
            if (objJsonReader.TokenType == JsonToken.String)
            {
                if (objJsonReader.Value != null)
                {
                    return (objJsonReader.Value as string).Trim().ConvertUnicodeString();
                }
            }
            return objJsonReader.Value;
        }
        public override void WriteJson(JsonWriter objJsonWriter, object obj, JsonSerializer
            objJsonSerializer)
        {
            string str = (string)obj;
            if (str == null)
            {
                objJsonWriter.WriteNull();
            }
            else
            {
                objJsonWriter.WriteValue(str.Trim().ConvertUnicodeString());
            }

        }
    }
}