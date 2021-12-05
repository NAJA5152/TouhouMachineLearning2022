using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace TouhouMachineLearningSummary.Extension
{
    public static class Extension
    {

        public static string ToJson(this object target) => JsonConvert.SerializeObject(target, Formatting.Indented);
        public static T ToObject<T>(this string Data) => JsonConvert.DeserializeObject<T>(Data);
        public static T Clone<T>(this T Object) => Object.ToJson().ToObject<T>();
        public static Sprite ToSprite(this Texture2D texture) => Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);


        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action) => enumerable.ToList().ForEach(action);
        public static void ForEach<T>(this IEnumerable<T> enumerable, Func<T> action) => enumerable.ToList().ForEach(action);
        //public static void To<T>(this T param, Action<T> action) => action(param) ;
        public static void To<T>(this T param, Action<T, object[]> action, params object[] paramas) => action(param, paramas);
        public static Color SetR(this Color color, float r) => new Color(r, color.g, color.b, color.a);
        public static Color SetG(this Color color, float g) => new Color(color.r, g, color.b, color.a);
        public static Color SetB(this Color color, float b) => new Color(color.r, color.g, b, color.a);
        public static Color SetA(this Color color, float a) => new Color(color.r, color.g, color.b, a);
        public static List<int> EnumToOneHot<T>(this T targetEnum) => Enumerable.Range(0, Enum.GetNames(typeof(T)).Length).SelectList(index => index == (int)(object)targetEnum ? 1 : 0);
        public static TEnum OneHotToEnum<TEnum>(this List<int> targetEnum) => (TEnum)(object)targetEnum.IndexOf(1);

        
        public static List<TResult> SelectList<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            List<TResult> result = new List<TResult>(source.Count());
            result.AddRange(source.Select(selector));
            return result;
        }
    }
}


