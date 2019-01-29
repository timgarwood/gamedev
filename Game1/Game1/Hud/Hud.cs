using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Game1.Hud
{
    public class Hud
    {
        /// <summary>
        /// singleton
        /// </summary>
        private static Hud _instance = null;

        /// <summary>
        /// the hud components
        /// </summary>
        private List<HudComponent> _hudComponents;

        /// <summary>
        /// oh hey, a singleton!
        /// </summary>
        public static Hud Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new Hud();
                }

                return _instance;
            }
        }

        /// <summary>
        /// ctor
        /// </summary>
        private Hud()
        {
            _hudComponents = new List<HudComponent>();
            _viewport = Vector2.Zero;
        }

        /// <summary>
        /// window resized method
        /// </summary>
        /// <param name="viewport"></param>
        public void OnWindowResized(Vector2 viewport)
        {
            foreach(var hudComponent in _hudComponents)
            {
                hudComponent.OnWindowResized(viewport);
            }
        }
    }
}
