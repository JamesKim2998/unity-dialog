using System;
using UnityEngine;

public interface IDialogTextPlayerSource
{
    int count { get; }
    DialogSentence this[int i] { get; }
}

public class DialogTextPlayerSource : IDialogTextPlayerSource
{
    private readonly DialogSentenceSequence _sentenceSequence;

    public DialogTextPlayerSource(DialogSentenceSequence sentenceSequence)
    {
        _sentenceSequence = sentenceSequence;
    }

    int IDialogTextPlayerSource.count { get { return _sentenceSequence.sentences.Count; } }
    DialogSentence IDialogTextPlayerSource.this[int i] { get { return _sentenceSequence.sentences[i]; } }
}

public class DialogTextPlayer
{
    public const float SentenceProceedDelay = 1;

    public bool isPlaying { get { return currentSentenceIndex < _source.count; } }
    public bool isDone { get { return !isPlaying; } }
    public string text { get; private set; }
    private string _oldText;

    public int currentSentenceIndex { get; private set; }
    public Action<int> onSentenceProceed;

    private readonly IDialogTextPlayerSource _source;
    private readonly RichTextStroller _richTextStroller = new RichTextStroller();

    private const float _lettersPerSecond = 6;
    private float _numberOfLettersToShow;
    private float _timeLeftToProceed = SentenceProceedDelay;

    public DialogTextPlayer(IDialogTextPlayerSource source)
    {
        text = "";
        _source = source;
        _richTextStroller.Start(_source[0].text);
    }

    public void Update(float dt)
    {
        if (isDone) return;
        var canAppendText = UpdateNumberOfCharactersToBeAppended(dt);
        if (canAppendText)
        {
            AppendSentence();
        }
        else
        {
            ProceedSentenceWithTimer(dt);
        }
    }

    private bool UpdateNumberOfCharactersToBeAppended(float dt)
    {
        var sentence = _source[currentSentenceIndex];
        _numberOfLettersToShow += (int) sentence.speed * _lettersPerSecond * dt;
        var newLetterIndex = _richTextStroller.text.Length + (int) _numberOfLettersToShow;
        return newLetterIndex <= sentence.text.Length;
    }

    private void AppendSentence()
    {
        Debug.Assert(!isDone, "already done.");
        for (var i = 0; i < (int)_numberOfLettersToShow; ++i)
            _richTextStroller.Next();
        text = _oldText + _richTextStroller.text;
        _numberOfLettersToShow -= (int)_numberOfLettersToShow;
    }

    private void ProceedSentenceWithTimer(float dt)
    {
        if (_timeLeftToProceed <= 0)
        {
            ProceedSentence();
            _timeLeftToProceed = SentenceProceedDelay;
        }

        _timeLeftToProceed -= dt;
    }

    private void ProceedSentence()
    {
        var oldSentence = _source[currentSentenceIndex];
        ++currentSentenceIndex;
        _numberOfLettersToShow = 0;
        if (oldSentence.clear)
        {
            _oldText = "";
        }
        else
        {
            _oldText = text;
        }
        _richTextStroller.StopAndReset();
        if (!isDone) _richTextStroller.Start(_source[currentSentenceIndex].text);
        onSentenceProceed.CheckAndCall(currentSentenceIndex);
    }
}
