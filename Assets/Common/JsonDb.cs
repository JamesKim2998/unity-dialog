using System.Collections;
using System.Collections.Generic;
using LitJson;

public interface IJsonDbParser<out Key, out Value>
{
    Key ParseKey(string raw);
    Value ParseValue(JsonData raw);
}

public class JsonDb<Key, Value> : IEnumerable<KeyValuePair<Key, Value>>
{
    private readonly Dictionary<Key, Value> _db = new Dictionary<Key, Value>();
    private readonly IJsonDbParser<Key, Value> _parser;

    public JsonDb(IJsonDbParser<Key, Value> parser)
    {
        _parser = parser;
    }

    public IEnumerator<KeyValuePair<Key, Value>> GetEnumerator()
    {
        return _db.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public Value GetOrDefault(Key key, Value defaultValue)
    {
        return _db.GetOrDefault(key, defaultValue);
    }

    public bool TryAppend(string filePath, bool force)
    {
        JsonData loadedData;
        if (!JsonHelper.LoadFromResources(filePath, out loadedData))
            return false;

        foreach (var kv in loadedData.GetDictEnum())
        {
            var key = _parser.ParseKey(kv.Key);
            var elem = _parser.ParseValue(kv.Value);
            if (!force) _db.TryAdd(key, elem);
            else _db[key] = elem;
        }

        return true;
    }
}