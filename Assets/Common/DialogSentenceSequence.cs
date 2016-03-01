﻿using System.Collections.Generic;
using System.Linq;
using LitJson;

namespace Dialog
{
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
        public bool Clear = true;
        public DialogPlaySpeed Speed = DialogPlaySpeed.Normal;
        public string Text;

        public DialogSentence()
        {
        }

        public DialogSentence(string text)
        {
            Text = text;
        }

        public static DialogSentence Parse(JsonData data)
        {
            return data.IsObject ? data.Convert<DialogSentence>() : new DialogSentence((string)data);
        }
    }

    public class DialogSentenceSequence
    {
        public static readonly DialogSentenceSequence Error;

        static DialogSentenceSequence()
        {
            Error = new DialogSentenceSequence
            {
                Sentences = new List<DialogSentence>
            {
                new DialogSentence("ERROR")
            }
            };
        }

        public List<DialogSentence> Sentences { get; private set; }

        public static DialogSentenceSequence Parse(JsonData data)
        {
            var ret = new DialogSentenceSequence();

            if (data.IsArray)
            {
                ret.Sentences = data.GetListEnum().Select<JsonData, DialogSentence>(DialogSentence.Parse).ToList();
            }
            else
            {
                ret.Sentences = new List<DialogSentence>
            {
                new DialogSentence((string) data)
            };
            }

            return ret;
        }
    }
}