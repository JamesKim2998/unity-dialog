using System.Collections.Generic;
using UnityEngine;
using LitJson;

using ITalkSentenceParser = IJsonDbParser<string, TalkSentenceSequence>;

public enum TalkViewType
{
    Sprite,
    Prefab,
}

public enum TalkViewPivot
{
    Left,
    Right,
    Center,
}

public class TalkView
{
    public TalkViewType type;
    public string path;
    public TalkViewPivot pivot = TalkViewPivot.Left;
    public Vector2 position;
    public string animationTrigger;
}

public class TalkSentenceSequence
{
    public TalkView view;
    [JsonIgnore]
    public DialogSentenceSequence sentences;
    public bool continueAuto;
    public string nextTalk;

    public static TalkSentenceSequence error = new TalkSentenceSequence() { sentences = DialogSentenceSequence.error, };
}

public class TalkSentenceParser : ITalkSentenceParser
{
    string ITalkSentenceParser.ParseKey(string raw) { return raw; }

    TalkSentenceSequence ITalkSentenceParser.ParseValue(JsonData raw)
    {
        var ret = raw.Convert<TalkSentenceSequence>();
        ret.sentences = DialogSentenceSequence.Parse(raw["sentences"]);
        return ret;
    }
}

public class TalkDb : JsonDb<string, TalkSentenceSequence>
{
    public static readonly TalkDb inst = new TalkDb();

    public TalkDb() : base(new TalkSentenceParser())
    {
    }

    public TalkSentenceSequence Get(string key)
    {
        return GetOrDefault(key, TalkSentenceSequence.error);
    }

    public bool TryAppendWithDefaultDirectory(string fileName, bool force)
    {
        return TryAppend("Dialog/Talk/" + fileName, force);
    }
}
