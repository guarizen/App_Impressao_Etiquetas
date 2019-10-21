using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Impressao_Etiquetas
{
    public partial class ListaRecebimento
    {
        [JsonProperty("odata.metadata")]
        public Uri OdataMetadata { get; set; }

        [JsonProperty("odata.count")]
        public long OdataCount { get; set; }

        [JsonProperty("value")]
        public List<Value> Value { get; set; }
    }

    public partial class Value
    {
        [JsonProperty("produto")]
        public long Produto { get; set; }

        [JsonProperty("cod_produto")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long CodProduto { get; set; }

        [JsonProperty("descricao1")]
        public string Descricao1 { get; set; }

        [JsonProperty("cor")]
        public string Cor { get; set; }

        [JsonProperty("estampa")]
        public string Estampa { get; set; }

        [JsonProperty("tamanho")]
        public string Tamanho { get; set; }

        [JsonProperty("ean13")]
        public string Ean13 { get; set; }

        [JsonProperty("quantidade")]
        public long Quantidade { get; set; }

        [JsonProperty("foto")]
        public string Foto { get; set; }
    }

    public partial class ListaRecebimento
    {
        public static ListaRecebimento FromJson(string json) => JsonConvert.DeserializeObject<ListaRecebimento>(json, Impressao_Etiquetas.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this ListaRecebimento self) => JsonConvert.SerializeObject(self, Impressao_Etiquetas.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}