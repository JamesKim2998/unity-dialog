using System.Collections.Generic;
using System.Linq;
using LitJson;

public enum DialogPlaySpeed
{
    Slowest = 1,
    Slow = 2,
    Normal = 3,
    Fast = 4,
    Fastest = 5,
}

public class DialogSentence
{
    public string text;
    public DialogPlaySpeed speed = DialogPlaySpeed.Normal;
    public bool clear = true;

    public DialogSentence() { }

    public DialogSentence(string text)
    {
        this.text = text;
    }

    public static DialogSentence Parse(JsonData data)
    {
        if (data.IsObject)
        {
            return data.Convert<DialogSentence>();
        }
        else
        {
            return new DialogSentence((string) data);
        }
    }
}

public class DialogSentenceSequence
{
    public List<DialogSentence> sentences { get; private set; }

    public static readonly DialogSentenceSequence error;

    static DialogSentenceSequence()
    {
        error = new DialogSentenceSequence();
        error.sentences = new List<DialogSentence>
        {
            new DialogSentence("ERROR"),
        };
    }

    public static DialogSentenceSequence Parse(JsonData data)
    {
        var ret = new DialogSentenceSequence();

        if (data.IsArray)
        {
            ret.sentences = data.GetListEnum().Select<JsonData, DialogSentence>(DialogSentence.Parse).ToList();
        }
        else
        {
            ret.sentences = new List<DialogSentence>
            {
                new DialogSentence((string)data)
            };
        }

        return ret;
    }
}
