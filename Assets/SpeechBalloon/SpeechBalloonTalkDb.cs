using System.Linq;
using LitJson;

namespace Dialog
{
    using SpeechBalloonTalk = System.Collections.Generic.List<TargetedDialogSentenceSequence>;
    using ISpeechBalloonTalkParser = IJsonDbParser<string, System.Collections.Generic.List<TargetedDialogSentenceSequence>>;

    public class SpeechBalloonTalkParser : ISpeechBalloonTalkParser
    {
        string ISpeechBalloonTalkParser.ParseKey(string raw)
        {
            return raw;
        }

        SpeechBalloonTalk ISpeechBalloonTalkParser.ParseValue(JsonData raw)
        {
            return raw.GetListEnum().Select<JsonData, TargetedDialogSentenceSequence>(TargetedDialogSentenceSequence.Parse).ToList();
        }
    }

    public class TargetedDialogSentenceSequence
    {
        public static TargetedDialogSentenceSequence Error = new TargetedDialogSentenceSequence
        {
            Target = "",
            Sentences = SentenceSequence.Error
        };

        public SentenceSequence Sentences;
        public string Target;

        public static TargetedDialogSentenceSequence Parse(JsonData raw)
        {
            return new TargetedDialogSentenceSequence
            {
                Target = (string)raw["Target"],
                Sentences = SentenceSequence.Parse(raw["Sentences"])
            };
        }
    }

    public class SpeechBalloonTalkDb : JsonDb<string, SpeechBalloonTalk>
    {
        public static SpeechBalloonTalkDb Inst = new SpeechBalloonTalkDb();

        public SpeechBalloonTalkDb() : base(new SpeechBalloonTalkParser())
        {
        }

        public SpeechBalloonTalk Get(string key)
        {
            return GetOrDefault(key, new SpeechBalloonTalk { TargetedDialogSentenceSequence.Error });
        }

        public bool TryAppendWithDefaultDirectory(string fileName, bool force)
        {
            return TryAppend("Dialog/SpeechBalloonTalk/" + fileName, force);
        }
    }
}