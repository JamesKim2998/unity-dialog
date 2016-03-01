using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class RichTextStroller
{
    public bool isSetuped { get { return orgText != null; } }
    public bool isEnded { get { return (orgText == null) || (_idx == orgText.Length); } }

    public string orgText { get; private set; }
    private int _idx;

    private int _base;
    public string text { get; private set; }
    public int visualLength { get; private set; }

    private string _header = "";
    private List<string> _tagHeads = new List<string>();
    private List<string> _tagTails = new List<string>();

    public RichTextStroller()
    {
        text = "";
    }

    public bool Start(string text)
    {
        if (isSetuped)
        {
            Debug.LogError("try Start again.");
            return false;
        }

        orgText = text;
        return true;
    }

    public void StopAndReset()
    {
        if (!isSetuped)
        {
            Debug.LogError("try StopAndReset again.");
            return;
        }

        _idx = 0;
        orgText = null;

        _tagHeads.Clear();
        _tagTails.Clear();

        Rebase();
    }

    public bool Next()
    {
        if (!isSetuped)
        {
            Debug.LogError("not setuped yet.");
            return false;
        }

        if (isEnded)
            return false;

        _idx += ProceedOneVisualChracter();
        text = BuildText(MakeRaw());
        ++visualLength;

        return true;
    }

    public bool Rebase()
    {
        _base = _idx;
        text = "";
        visualLength = 0;

        _header = !_tagHeads.Empty() ? MakeHead(ref _tagHeads) : "";

        return true;
    }

    private int ProceedOneVisualChracter()
    {
        var offset = 0;

        do
        {
            var hasTag = SplitTag(orgText.Substring(_idx + offset));
            if (!hasTag.HasValue)
            {
                ++offset;
                break;
            }

            var tag = hasTag.Value;
            if (!tag.end)
            {
                _tagHeads.Add(orgText.Substring(_idx + offset, tag.offset));
                _tagTails.Add(tag.tag);
            }
            else
            {
                Debug.Assert(!_tagHeads.Empty() && !_tagTails.Empty());
                _tagHeads.RemoveBack();
                _tagTails.RemoveBack();
            }

            offset += tag.offset;
            if (orgText.Length == _idx + offset)
                break;

        } while (true);

        return offset;
    }

    private string MakeRaw() { return orgText.Substring(_base, _idx - _base); }

    private string BuildText(string txt)
    {
        if (string.IsNullOrEmpty(_header) && _tagTails.Empty())
            return txt;

        var builder = new StringBuilder(orgText.Length);

        if (!string.IsNullOrEmpty(_header))
            builder.Append(_header);

        builder.Append(txt);

        if (!_tagTails.Empty())
            builder.Append(MakeTail(ref _tagTails));

        return builder.ToString();
    }

    struct Tag
    {
        public string tag;
        public int offset;
        public bool end;
    }

    private static Tag? SplitTag(string txt)
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

                return new Tag
                {
                    tag = txt.Substring(tagStart, tagEnd - tagStart),
                    offset = i + 1,
                    end = isEnd,
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
        var txt = new StringBuilder(tags.Count * 45);
        foreach (var tag in tags)
            txt.Append(tag);
        return txt.ToString();
    }

    private static string MakeTail(ref List<string> tags)
    {
        Debug.Assert(!tags.Empty());
        var txt = new StringBuilder(tags.Count * 5);
        foreach (var tag in tags.GetReverseEnum())
            txt.Append("</").Append(tag).Append('>');
        return txt.ToString();
    }
}
