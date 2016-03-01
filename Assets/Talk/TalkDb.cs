﻿using LitJson;
using UnityEngine;
using ITalkSentenceParser = IJsonDbParser<string, TalkSentenceSequence>;

public enum TalkViewType
{
    Sprite,
    Prefab
}

public enum TalkViewPivot
{
    Left,
    Right,
    Center
}

public class TalkView
{
    public string AnimationTrigger;
    public string Path;
    public TalkViewPivot Pivot = TalkViewPivot.Left;
    public Vector2 Position;
    public TalkViewType Type;
}

public class TalkSentenceSequence
{
    public static TalkSentenceSequence Error = new TalkSentenceSequence {Sentences = DialogSentenceSequence.Error};
    public bool ContinueAuto;
    public string NextTalk;
    [JsonIgnore] public DialogSentenceSequence Sentences;
    public TalkView View;
}

public class TalkSentenceParser : ITalkSentenceParser
{
    string ITalkSentenceParser.ParseKey(string raw)
    {
        return raw;
    }

    TalkSentenceSequence ITalkSentenceParser.ParseValue(JsonData raw)
    {
        var ret = raw.Convert<TalkSentenceSequence>();
        ret.Sentences = DialogSentenceSequence.Parse(raw["sentences"]);
        return ret;
    }
}

public class TalkDb : JsonDb<string, TalkSentenceSequence>
{
    public static readonly TalkDb Inst = new TalkDb();

    public TalkDb() : base(new TalkSentenceParser())
    {
    }

    public TalkSentenceSequence Get(string key)
    {
        return GetOrDefault(key, TalkSentenceSequence.Error);
    }

    public bool TryAppendWithDefaultDirectory(string fileName, bool force)
    {
        return TryAppend("Dialog/Talk/" + fileName, force);
    }
}