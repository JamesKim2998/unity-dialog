using LitJson;
using System.Linq;

using SpeechBalloonTalk = System.Collections.Generic.List<TargetedDialogSentenceSequence>;
using ISpeechBalloonTalkParser = IJsonDbParser<string, System.Collections.Generic.List<TargetedDialogSentenceSequence>>;

public class SpeechBalloonTalkParser : ISpeechBalloonTalkParser
{
    string ISpeechBalloonTalkParser.ParseKey(string raw) { return raw; }

    SpeechBalloonTalk ISpeechBalloonTalkParser.ParseValue(JsonData raw)
    {
        return raw.GetListEnum().Select<JsonData, TargetedDialogSentenceSequence>(TargetedDialogSentenceSequence.Parse).ToList();
    }
}

public class TargetedDialogSentenceSequence
{
    public string target;
    public DialogSentenceSequence sentences;

    public static TargetedDialogSentenceSequence Parse(JsonData raw)
    {
        return new TargetedDialogSentenceSequence
        {
            target = (string) raw["target"],
            sentences = DialogSentenceSequence.Parse(raw["sentences"]),
        };
    }

    public static TargetedDialogSentenceSequence error = new TargetedDialogSentenceSequence
    {
        target = "",
        sentences = DialogSentenceSequence.error,
    };
}

public class SpeechBalloonTalkDb : JsonDb<string, SpeechBalloonTalk>
{
    public static SpeechBalloonTalkDb inst = new SpeechBalloonTalkDb();

    public SpeechBalloonTalkDb() : base(new SpeechBalloonTalkParser())
    { 
    }

    public SpeechBalloonTalk Get(string key)
    {
        return GetOrDefault(key, new SpeechBalloonTalk { TargetedDialogSentenceSequence.error });
    }

    public bool TryAppendWithDefaultDirectory(string fileName, bool force)
    {
        return TryAppend("Dialog/SpeechBalloonTalk/" + fileName, force);
    }
}
