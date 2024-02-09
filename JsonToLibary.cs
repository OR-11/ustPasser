//https://yaspage.com/cs-json-to-dictionary/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace ustPasser
{
    public class JsonToLibary
    {
        // JSON文字列をDictionary<string, dynamic>型に変換するメソッド
        public static Dictionary<string, dynamic> ParseJson(string json)
        {
            // とりあえずJSON文字列をDictionary<string, JsonElement>型に変換
            var dic = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

            // JsonElementから値を取り出してdynamic型に入れてDictionary<string, dynamic>型で返す
            return dic.Select(d => new { key = d.Key, value = ParseJsonElement(d.Value) })
                .ToDictionary(a => a.key, a => a.value);
        }

        // JsonElementの型を調べて変換するメソッド
        private static dynamic ParseJsonElement(JsonElement elem)
        {
            // データの種類によって値を取得する処理を変える
            return elem.ValueKind switch
            {
                JsonValueKind.String => elem.GetString(),
                JsonValueKind.Number => elem.GetDecimal(),
                JsonValueKind.False => false,
                JsonValueKind.True => true,
                JsonValueKind.Array => elem.EnumerateArray().Select(e => ParseJsonElement(e)).ToList(),
                JsonValueKind.Null => null,
                JsonValueKind.Object => ParseJson(elem.GetRawText()),
                _ => throw new NotSupportedException(),
            };
        }
    }
}
