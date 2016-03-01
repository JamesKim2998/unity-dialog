using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LitJson;
using UnityEngine;

public static class JsonHelper
{
	private static string MakeMessageWrongType(JsonType expected, JsonType matches)
	{
		return "JsonData has wrong type.\n"
			   + "expected: " + expected + ", matches: " + matches;
	}

	public static IEnumerable<JsonData> GetListEnum(this JsonData data)
	{
		return data.Cast<JsonData>();
	}

	public static IEnumerable<KeyValuePair<string, JsonData>> GetDictEnum(this JsonData data)
	{
		return data.Cast<KeyValuePair<string, JsonData>>();
	}

    public static bool Contains(this JsonData thiz, string key)
    {
		return thiz.Keys.Contains(key);
    }

	public static bool TryGet(this JsonData data, string key, out JsonData val)
	{
		if (!data.Keys.Contains(key))
		{
			Debug.LogWarning("key not exists: " + key);
			val = null;
			return false;
		}

		val = data[key];
		return true;
	}

	public static bool TryGet(this JsonData data, string key, out string val)
	{
		JsonData ret;

		if (data.TryGet(key, out ret))
		{
			if (ret.IsString)
			{
				val = (string)ret;
				return true;
			}
			else
			{
				Debug.LogError(MakeMessageWrongType(JsonType.String, ret.GetJsonType()));
			}
		}

		val = string.Empty;
		return false;
	}

	public static bool TryGet(this JsonData thiz, string key, out bool val)
	{
		JsonData data;
		if (thiz.TryGet(key, out data))
		{
			if (data.IsBoolean)
			{
				val = (bool)data;
			}
			else
			{
				Debug.LogError(MakeMessageWrongType(JsonType.Boolean, data.GetJsonType()));
			}
		}

		val = default(bool);
		return false;
	}

	public static bool TryGet(this JsonData thiz, string key, out int val)
	{
		JsonData data;

		if (thiz.TryGet(key, out data))
		{
			if (data.IsNatural)
			{
				val = (int)data;
				return true;
			}
			else
			{
				Debug.LogError(MakeMessageWrongType(JsonType.Natural, data.GetJsonType()));
			}
		}

		val = default(int);
		return false;
	}

	public static bool TryGet(this JsonData thiz, string key, out float val)
	{
		JsonData data;

		if (thiz.TryGet(key, out data))
		{
			if (data.IsNatural)
			{
				val = (int)data;
				return true;
			}
			else if (data.IsReal)
			{
				val = (float)data;
				return true;
			}
			else
			{
				Debug.LogError(MakeMessageWrongType(JsonType.Real, data.GetJsonType()));
			}
		}

		val = default(float);
		return false;
	}

	public static bool BoolOrDefault(this JsonData thiz, string key, bool _default = false)
	{
		bool ret;
		return thiz.TryGet(key, out ret) ? ret : _default;
	}

	public static int IntOrDefault(this JsonData thiz, string key, int _default = 0)
	{
		int ret;
		return thiz.TryGet(key, out ret) ? ret : _default;
	}

	public static float FloatOrDefault(this JsonData thiz, string key, float _default = 0)
	{
		float ret;
		return thiz.TryGet(key, out ret) ? ret : _default;
	}

	public static string StringOrDefault(this JsonData thiz, string key, string _default = "")
	{
		string ret;
		return thiz.TryGet(key, out ret) ? ret : _default;
	}

    private static T ConvertForReflection<T>(this JsonData thiz)
    {
        return thiz.Convert<T>();
    }

    public static T Convert<T>(this JsonData thiz)
    {
        if (thiz == null)
            return default(T);

        if (thiz.IsNatural)
            return (T)(object)(int)thiz;

        if (thiz.IsString)
        {
            var t = typeof(T);
            if (t == typeof(string))
            {
                return (T)(object)(string)thiz;
            }
            else if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof (Nullable<>))
            {
                var valueType = t.GetGenericArguments()[0];
                var ctor = t.GetConstructor(new[] { valueType });
                return (T)ctor.Invoke(new[] { Convert(thiz, valueType) });
            }
            else if (t.IsEnum)
            {
                return (T)Enum.Parse(t, thiz.ToString());
            }
            else
            {
                return JsonMapper.ToObject<T>(thiz.ToJson());
            }
        }

        if (thiz.IsBoolean)
            return (T)(object)(bool)thiz;
        if (thiz.IsReal)
            return (T)(object)(float)thiz;
        return JsonMapper.ToObject<T>(thiz.ToJson());
    }

    public static object Convert(this JsonData thiz, Type t)
    {
        if (thiz == null) return null;
        var convertMethod = typeof(JsonHelper).GetMethod("ConvertForReflection", BindingFlags.NonPublic | BindingFlags.Static);
        var genericMethod = convertMethod.MakeGenericMethod(t);
        return genericMethod.Invoke(null, new object[] { thiz });
    }

    public static JsonData ToObject<T>(T data)
    {
        if (data == null)
            return null;

        try
        {
            if (data.GetType().IsEnum)
            {
                return new JsonData(data.ToString());
            }
            else
            {
                return new JsonData(data);
            }
        }
        catch (Exception)
        {
            return JsonMapper.ToObject(JsonMapper.ToJson(data));
        }
    }

	public static bool Load<T>(string path, out T data)
	{
		try
		{
			using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
			using (var reader = new StreamReader(fs))
			{
				data = JsonMapper.ToObject<T>(new JsonReader(reader));
				return true;
			}
		}
		catch (Exception e)
		{
			Debug.LogException(e);
			data = default(T);
			return false;
		}
	}

	public static bool LoadFromResources<T>(string path, out T data)
	{
		var text = Resources.Load<TextAsset>(path);
		if (text == null)
		{
			Debug.LogWarning("text asset on " + path + " does not exists.");
			data = default(T);
			return false;
		}

		using (TextReader reader = new StringReader(text.text))
			data = JsonMapper.ToObject<T>(reader);

		return true;
	}

	public static bool Save<T>(string path, T data)
	{
		try
		{
			using (var fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
			using (var writer = new StreamWriter(fs))
			{
				JsonMapper.ToJson(data, new JsonWriter(writer));
				return true;
			}
		}
		catch (Exception e)
		{
			Debug.LogException(e);
			return false;
		}
	}
}