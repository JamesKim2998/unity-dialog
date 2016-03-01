using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class RichTextStroller
{
    private int _base;
    private string _header = "";
    private int _idx;
    private List<string> _tagHeads = new List<string>();
    private List<string> _tagTails = new List<string>();

    public RichTextStroller()
    {
        Text = "";
    }

    public bool IsSetuped
    {
        get { return OrgText != null; }
    }

    public bool IsEnded
    {
        get { return (OrgText == null) || (_idx == OrgText.Length); }
    }

    public string OrgText { get; private set; }
    public string Text { get; private set; }
    public int VisualLength { get; private set; }

    public bool Start(string text)
    {
        if (IsSetuped)
        {
            Debug.LogError("try Start again.");
            return false;
        }

        OrgText = text;
        return true;
    }

    public void StopAndReset()
    {
        if (!IsSetuped)
        {
            Debug.LogError("try StopAndReset again.");
            return;
        }

        _idx = 0;
        OrgText = null;

        _tagHeads.Clear();
        _tagTails.Clear();

        Rebase();
    }

    public bool Next()
    {
        if (!IsSetuped)
        {
            Debug.LogError("not setuped yet.");
            return false;
        }

        if (IsEnded)
            return false;

        _idx += ProceedOneVisualChracter();
        Text = BuildText(MakeRaw());
        ++VisualLength;

        return true;
    }

    public bool Rebase()
    {
        _base = _idx;
        Text = "";
        VisualLength = 0;

        _header = !_tagHeads.Empty() ? MakeHead(ref _tagHeads) : "";

        return true;
    }

    private int ProceedOneVisualChracter()
    {
        var offset = 0;

        do
        {
            var hasTag = SplitTag(OrgText.Substring(_idx + offset));
            if (!hasTag.HasValue)
            {
                ++offset;
                break;
            }

            var tag = hasTag.Value;
            if (!tag.End)
            {
                _tagHeads.Add(OrgText.Substring(_idx + offset, tag.Offset));
                _tagTails.Add(tag.Tag);
            }
            else
            {
                Debug.Assert(!_tagHeads.Empty() && !_tagTails.Empty());
                _tagHeads.RemoveBack();
                _tagTails.RemoveBack();
            }

            offset += tag.Offset;
            if (OrgText.Length == _idx + offset)
                break;
        } while (true);

        return offset;
    }

    private string MakeRaw()
    {
        return OrgText.Substring(_base, _idx - _base);
    }

    private string BuildText(string txt)
    {
        if (string.IsNullOrEmpty(_header) && _tagTails.Empty())
            return txt;

        var builder = new StringBuilder(OrgText.Length);

        if (!string.IsNullOrEmpty(_header))
            builder.Append(_header);

        builder.Append(txt);

        if (!_tagTails.Empty())
            builder.Append(MakeTail(ref _tagTails));

        return builder.ToString();
    }

    private static TagRange? SplitTag(string txt)
    {
        Debug.Assert(!string.IsNullOrEmpty(txt));

        if (txt.Length < 3)
            return null;

        if (txt[0] != '<')
            return null;

        var isEnd = (txt[1] == '/');
        var tagStart = isEnd ? 2 : 1;
        var tagEnd = 0;

        for (var i = tagStart; i != txt.Length; ++i)
        {
            if (tagEnd == 0)
            {
                if (!char.IsLower(txt[i]))
                    tagEnd = i;
            }

            if (txt[i] == '>')
            {
                Debug.Assert((tagEnd != 0) && (tagEnd != tagStart));

                return new TagRange
                {
                    Tag = txt.Substring(tagStart, tagEnd - tagStart),
                    Offset = i + 1,
                    End = isEnd
                };
            }
        }

        Debug.LogError("> not found.");
        return null;
    }

    private static string MakeHead(ref List<string> tags)
    {
        Debug.Assert(!tags.Empty());
        if (tags.Count == 1)
            return tags[0];
        var txt = new StringBuilder(tags.Count*45);
        foreach (var tag in tags)
            txt.Append(tag);
        return txt.ToString();
    }

    private static string MakeTail(ref List<string> tags)
    {
        Debug.Assert(!tags.Empty());
        var txt = new StringBuilder(tags.Count*5);
        foreach (var tag in tags.GetReverseEnum())
            txt.Append("</").Append(tag).Append('>');
        return txt.ToString();
    }

    private struct TagRange
    {
        public bool End;
        public int Offset;
        public string Tag;
    }
}