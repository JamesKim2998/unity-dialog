using LitJson;

namespace Dialog
{
    using DialogSentenceSequenceJsonDbParser = IJsonDbParser<string, DialogSentenceSequence>;

    public class SpeechBalloonDbParser : DialogSentenceSequenceJsonDbParser
    {
        string DialogSentenceSequenceJsonDbParser.ParseKey(string raw)
        {
            return raw;
        }

        DialogSentenceSequence DialogSentenceSequenceJsonDbParser.ParseValue(JsonData raw)
        {
            return DialogSentenceSequence.Parse(raw);
        }
    }

    public class SpeechBalloonDb : JsonDb<string, DialogSentenceSequence>
    {
        public static SpeechBalloonDb Inst = new SpeechBalloonDb();

        public SpeechBalloonDb() : base(new SpeechBalloonDbParser())
        {
        }

        public DialogSentenceSequence Get(string key)
        {
            return GetOrDefault(key, DialogSentenceSequence.Error);
        }

        public bool TryAppendWithDefaultDirectory(string fileName, bool force)
        {
            return TryAppend(Config.Inst.DefaultSpeechBalloonDbDirectory + fileName, force);
        }
    }
}