using System;
using System.Linq;
using System.Collections.Generic;
using Box2DX.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game1
{
    public class GameWorld : IDrawable
    {
        /// <summary>
        /// list of game objects
        /// </summary>
        private List<GameObject> _gameObjects;

        private List<Action<GameObject>> _gameObjectAdded;
        private List<Action<GameObject>> _gameObjectRemoved;

        /// <summary>
        /// singleton getters
        /// </summary>
        private static GameWorld _instance = null;
        public static GameWorld Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new GameWorld();
                }

                return _instance;
            }
        }

        /// <summary>
        /// ctor
        /// </summary>
        private GameWorld()
        {
            _gameObjectAdded = new List<Action<GameObject>>();
            _gameObjectRemoved = new List<Action<GameObject>>();
            _gameObjects = new List<GameObject>();
        }

        /// <summary>
        /// adds the game object to the game world
        /// </summary>
        /// <param name="obj"></param>
        public void AddGameObject(GameObject obj)
        {
            if (!_gameObjects.Contains(obj))
            {
                _gameObjects.Add(obj);
                _gameObjectAdded.ForEach(a => a.Invoke(obj));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void RemoveGameObject(GameObject obj)
        {
            if(_gameObjects.Contains(obj))
            {
                _gameObjects.Remove(obj);
                _gameObjectRemoved.ForEach(a => a.Invoke(obj));
            }
        }

        /// <summary>
        /// Returns a list of game objects that are of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IList<T> GetGameObjects<T>()
        {
            //TODO:  this might be super inefficient
            return _gameObjects.OfType<T>().ToList();
        }

        /// <summary>
        /// registers the given callback for knowing when a game object is added
        /// </summary>
        /// <param name="a"></param>
        public void RegisterGameObjectAdded(Action<GameObject> a)
        {
            if (!_gameObjectAdded.Contains(a))
            {
                _gameObjectAdded.Add(a);
            }
        }

        /// <summary>
        /// registers the given callback for knowing when a game object is added
        /// </summary>
        /// <param name="a"></param>
        public void RegisterGameObjectRemoved(Action<GameObject> a)
        {
            if(!_gameObjectRemoved.Contains(a))
            {
                _gameObjectRemoved.Add(a);
            }
        }

        /// <summary>
        /// draw routine, draws all the registered game objects
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="cameraOrigin"></param>
        /// <param name="viewport"></param>
        public void Draw(SpriteBatch spriteBatch, Vec2 cameraOrigin, Vector2 viewport)
        {
            foreach(var obj in _gameObjects)
            {
                obj.Draw(spriteBatch, cameraOrigin, viewport);
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach(var obj in _gameObjects)
            {
                obj.Update(gameTime);
            }
        }
    }
}
