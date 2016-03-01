using UnityEngine;

public static class TalkManager
{
    public static bool TryPlay(string key)
    {
        var canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("no canvas.");
            return false;
        }

        var player = PrefabDb.inst.talkPlayer.Instantiate();
        player.transform.SetParent(canvas.transform, false);
        player.Play(TalkDb.inst.Get(key));
        return true;
    }
}
