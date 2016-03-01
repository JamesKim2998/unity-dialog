using LitJson;

namespace Dialog
{
    using SentenceSequenceJsonDbParser = IJsonDbParser<string, SentenceSequence>;

    public class SpeechBalloonDbParser : SentenceSequenceJsonDbParser
    {
        string SentenceSequenceJsonDbParser.ParseKey(string raw)
        {
            return raw;
        }

        SentenceSequence SentenceSequenceJsonDbParser.ParseValue(JsonData raw)
        {
            return SentenceSequence.Parse(raw);
        }
    }

    public class SpeechBalloonDb : JsonDb<string, SentenceSequence>
    {
        public static SpeechBalloonDb Inst = new SpeechBalloonDb();

        public SpeechBalloonDb() : base(new SpeechBalloonDbParser())
        {
        }

        public SentenceSequence Get(string key)
        {
            return GetOrDefault(key, SentenceSequence.Error);
        }

        public bool TryAppendWithDefaultDirectory(string fileName, bool force)
        {
            return TryAppend(Config.Inst.DefaultSpeechBalloonDbDirectory + fileName, force);
        }
    }
}