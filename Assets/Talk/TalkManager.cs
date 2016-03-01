using UnityEngine;

namespace Dialog
{
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

            var player = Config.Inst.TalkPlayer.Instantiate();
            player.transform.SetParent(canvas.transform, false);
            player.Play(TalkDb.Inst.Get(key));
            return true;
        }
    }
}