using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
    public class FilteredKeyListener
    {
        public FilteredKeyListener(Keys[] trackKeys)
        {
            TrackKeys = trackKeys;
        }

        private Keys[] TrackKeys { get; set; }

        private class KeyInfo
        {
            public DateTime KeyDownTime { get; set; }

            public void Reset()
            {
                KeyDownTime = DateTime.MinValue;
            }
        }

        private Dictionary<Keys, KeyInfo> KeyInfos { get; set; } = new Dictionary<Keys, KeyInfo>();

        private KeyInfo GetKeyInfo(Keys key)
        {
            if(!KeyInfos.ContainsKey(key))
            {
                KeyInfos[key] = new KeyInfo
                {
                    KeyDownTime = DateTime.MinValue
                };
            }

            return KeyInfos[key];
        }

        public void Update(GameTime gameTime)
        {
            foreach(var key in TrackKeys)
            {
                KeyInfo info = GetKeyInfo(key);
                if(Keyboard.GetState().IsKeyDown(key))
                {
                    info.KeyDownTime = DateTime.UtcNow;
                }
            }
        }

        public bool WasKeyPressed(Keys key)
        {
            var info = GetKeyInfo(key);

            return Keyboard.GetState().IsKeyUp(key) && info.KeyDownTime > DateTime.MinValue;
        }

        public void ResetKey(Keys key)
        {
            var info = GetKeyInfo(key);
            info.Reset();
        }
    }
}
