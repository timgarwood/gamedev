﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Box2DX.Dynamics;
using Box2DX.Collision;
using Box2DX.Common;
using NLog;
using System.Collections.Generic;
using System.IO;
using Game1.Fonts;
using Game1.Weapons;
using Game1.Hud;
using Game1.Animations;
using Game1.Menu;
using Game1.GameMode;
using Game1.Physics;
using System;
using Math = System.Math;

namespace Game1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        /// <summary>
        /// game states
        /// </summary>
        private enum GameStates
        {
            Normal,
            Paused,
            WaitingForRespawn,
            GameOver
        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private Player player;
        private World physicsWorld;
        private AABB aabb;
        private GameObject topWall;
        private GameObject bottomWall;
        private GameObject leftWall;
        private GameObject rightWall;
        private Logger Logger = LogManager.GetCurrentClassLogger();

        private GameData gameData;

        private Texture2D blueStar;
        private Texture2D brightMoon;
        private Texture2D brightStar;
        private Texture2D dirtPlanet;
        private Texture2D firePlanet;
        private Texture2D venusPlanet;
        private Texture2D yellowPlanet;
        private Texture2D positionTexture;
        private Texture2D upperBoundTexture;
        private Texture2D lowerBoundTexture;
        private List<Texture2D> planets = new List<Texture2D>();

        private Vector2 viewport;

        private AlienFactory _alienFactory;
        private WeaponFactory _weaponsFactory;
        private FontFactory _fontFactory;
        private AnimationFactory _animationFactory;
        private MenuFactory _menuFactory;

        private GameMode.GameMode CurrentGameMode { get; set; }

        private GameStates GameState { get; set; }

        private TimeSpan RespawnWaitTime { get; set; } = TimeSpan.FromSeconds(3);

        private TimeSpan LastRespawnTime { get; set; }

        public Game1(GameData data)
        {
            gameData = data;
            graphics = new GraphicsDeviceManager(this);

            //create physics bounds
            aabb = new AABB();
            aabb.LowerBound = new Vec2(0, 0);
            aabb.UpperBound = new Vec2(gameData.MaxXDimension, gameData.MaxYDimension);
            physicsWorld = new World(aabb, new Vec2(0, 0), doSleep: false);
            physicsWorld.SetContactListener(new GameContactListener());
            Content.RootDirectory = "Content";
            this.TargetElapsedTime = System.TimeSpan.FromSeconds(1f / gameData.Fps);

            _animationFactory = new AnimationFactory(Content);
            _alienFactory = new AlienFactory(physicsWorld, Content, _animationFactory);
            _weaponsFactory = new WeaponFactory(physicsWorld, Content);
            _fontFactory = new FontFactory(Content);
            _menuFactory = new MenuFactory(_fontFactory);

            Window.ClientSizeChanged += OnResize;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Window.AllowUserResizing = true;
            Window.Title = "Game1";

            float xpix = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            float ypix = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            viewport = new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height);

            //load up aliens
            using (var stream = new FileStream("./AlienDefinitions.json", FileMode.Open))
            {
                _alienFactory.Load(stream);
            }

            //load up weapons
            using(var stream = new FileStream("./Weapons/WeaponDefinitions.json", FileMode.Open))
            {
                _weaponsFactory.Load(stream);
            }

            //load up fonts
            using (var stream = new FileStream("./Fonts/FontDefinitions.json", FileMode.Open))
            {
                _fontFactory.Load(stream);
            }

            //load up animations
            using (var stream = new FileStream("./Animations/AnimationDefinitions.json", FileMode.Open))
            {
                _animationFactory.Load(stream);
            }

            //load up menus
            using (var stream = new FileStream("./Menu/MenuDefinitions.json", FileMode.Open))
            {
                _menuFactory.Load(stream);
            }

            //FIXME
            var hud = new Hud.Hud(Content, GraphicsDevice);

            //load up the HUD
            using (var stream = new FileStream("./Hud/HudDefinition.json", FileMode.Open))
            {
                Hud.Hud.Instance.Load(stream);
            }

            Hud.Hud.Instance.OnWindowResized(viewport);


            base.Initialize();
        }

        /// <summary>
        /// callback for game window being resized
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnResize(object sender, System.EventArgs args)
        {
            viewport = new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height);

            Hud.Hud.Instance.OnWindowResized(viewport);
        }

        /// <summary>
        /// Creates a wall
        /// </summary>
        /// <param name="x">x position in pixels</param>
        /// <param name="y">y position in pixels</param>
        /// <param name="w">width of wall in pixels</param>
        /// <param name="h">height of wall in pixels</param>
        /// <returns></returns>
        private GameObject Wall(Vec2 topLeft, Vec2 bottomRight)
        {
            // Define the ground body.
            var wallBodyDef = new BodyDef();
            wallBodyDef.Position.Set(topLeft.X, topLeft.Y);

            // Call the body factory which creates the wall box shape.
            // The body is also added to the world.
            var wallBody = physicsWorld.CreateBody(wallBodyDef);

            // Define the wall box shape.
            var wallShapeDef = new PolygonDef();
            wallShapeDef.Friction = 0.3f;
            wallShapeDef.Density = 1.0f;

            // The extents are the half-widths of the box.
            var wallPhysicsSize = new Vec2(Math.Abs(bottomRight.X - topLeft.X), Math.Abs(bottomRight.Y - topLeft.Y));
            if(wallPhysicsSize.X <= 0)
            {
                wallPhysicsSize.X = 1*gameData.MetersPerPixel;
            }
            if(wallPhysicsSize.Y <= 0)
            {
                wallPhysicsSize.Y = 1*gameData.MetersPerPixel;
            }

            wallShapeDef.SetAsBox(wallPhysicsSize.X, wallPhysicsSize.Y);

            // Add the ground shape to the ground body.
            var shape = wallBody.CreateShape(wallShapeDef);
            var vTex = GameUtils.GraphicsVec(wallPhysicsSize);

            if(vTex.X <= 0)
            {
                vTex.X = 1;
            }
            if(vTex.Y <= 0)
            {
                vTex.Y = 1;
            }

            //var texture2d = new Texture2D(graphics.GraphicsDevice, (int) vTex.X, (int) vTex.Y);
            //var data = new Microsoft.Xna.Framework.Color[(int)vTex.X * (int)vTex.Y];
            //for (int i = 0; i < data.Length; ++i)
           // {
           //     data[i] = Microsoft.Xna.Framework.Color.Chocolate;
           // }

           // texture2d.SetData(data);
            Logger.Info($"Wall created at ({wallBody.GetPosition().X},{wallBody.GetPosition().Y}) " + 
                $"extends to ({wallBody.GetPosition().X + wallPhysicsSize.X},{wallBody.GetPosition().Y + wallPhysicsSize.Y})");
            return new GameObject(physicsWorld, null, shape, wallBody, 0);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            blueStar = Content.Load<Texture2D>("sprites/planets/blue-star-transparent");
            brightMoon = Content.Load<Texture2D>("sprites/planets/bright-moon-transparent");
            brightStar = Content.Load<Texture2D>("sprites/planets/bright-star-transparent");
            dirtPlanet = Content.Load<Texture2D>("sprites/planets/dirt-planet-transparent");
            firePlanet = Content.Load<Texture2D>("sprites/planets/fire-planet-transparent");
            venusPlanet = Content.Load<Texture2D>("sprites/planets/venus-planet-transparent");
            yellowPlanet = Content.Load<Texture2D>("sprites/planets/yellow-planet-transparent");
            positionTexture = new Texture2D(graphics.GraphicsDevice, 5, 5);
            var data = new Microsoft.Xna.Framework.Color[5*5];
            for (int i = 0; i < data.Length; ++i)
             {
                 data[i] = Microsoft.Xna.Framework.Color.Red;
             }

            positionTexture.SetData(data);

            upperBoundTexture = new Texture2D(graphics.GraphicsDevice, 5, 5);
            lowerBoundTexture = new Texture2D(graphics.GraphicsDevice, 5, 5);
            var upperData = new Microsoft.Xna.Framework.Color[5 * 5];
            var lowerData = new Microsoft.Xna.Framework.Color[5 * 5];
            for (int i = 0; i < upperData.Length; ++i)
            {
                upperData[i] = Microsoft.Xna.Framework.Color.Yellow;
                lowerData[i] = Microsoft.Xna.Framework.Color.Orange;
            }

            upperBoundTexture.SetData(upperData);
            lowerBoundTexture.SetData(lowerData);

            for (var i = 0; i < 10; ++i)
            {
                planets.Add(blueStar);
                planets.Add(brightMoon);
                planets.Add(brightStar);
                planets.Add(dirtPlanet);
                planets.Add(firePlanet);
                planets.Add(venusPlanet);
                planets.Add(yellowPlanet);
            }

            Background.GenerateBackground(planets.ToArray(), gameData);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            var crateTexture = Content.Load<Texture2D>("sprites/ships/ship1small");


            //top wall
            topWall = Wall(new Vec2(0.1f, 0.1f), new Vec2(gameData.MaxXDimension-1,0.1f));
            //bottom wall
            bottomWall = Wall(new Vec2(0.1f, gameData.MaxYDimension - 1), new Vec2(gameData.MaxXDimension - 1, gameData.MaxYDimension - 1));
            //left wall
            leftWall = Wall(new Vec2(0.1f,0.1f), new Vec2(0.1f,gameData.MaxYDimension-1));
            //right wall
            rightWall = Wall(new Vec2(gameData.MaxXDimension - 1, 0.1f), new Vec2(gameData.MaxXDimension - 1, gameData.MaxYDimension - 1));

            var crateShapeDef = new PolygonDef();
            var cratePhysicsSize = GameUtils.PhysicsVec(new Vector2(crateTexture.Width, crateTexture.Height));
            crateShapeDef.Vertices = new Vec2[4];
            crateShapeDef.Vertices[0] = new Vec2( -(cratePhysicsSize.X / 2),  -(cratePhysicsSize.Y / 2));
            crateShapeDef.Vertices[1] = new Vec2((cratePhysicsSize.X / 2),  -(cratePhysicsSize.Y / 2));
            crateShapeDef.Vertices[2] = new Vec2((cratePhysicsSize.X / 2), (cratePhysicsSize.Y / 2));
            crateShapeDef.Vertices[3] = new Vec2(-(cratePhysicsSize.X / 2), (cratePhysicsSize.Y / 2));
            crateShapeDef.VertexCount = 4;

            Logger.Info($"crate size = ({cratePhysicsSize.X},{cratePhysicsSize.Y})");
            crateShapeDef.Density = gameData.PlayerDensity;
            crateShapeDef.Friction = gameData.PlayerFriction;
            crateShapeDef.Filter.CategoryBits = CollisionCategory.Player;
            crateShapeDef.Filter.MaskBits = (ushort) (CollisionCategory.Alien | CollisionCategory.AlienProjectile);

            var crateBodyDef = new BodyDef();
            crateBodyDef.IsBullet = true;
            var playerPosition = new Vec2(GameData.Instance.PlayerStartX, GameData.Instance.PlayerStartY);
            crateBodyDef.Position.Set(playerPosition.X, playerPosition.Y);
            var crateBody = physicsWorld.CreateBody(crateBodyDef);
            var crateShape = crateBody.CreateShape(crateShapeDef);
            crateBody.SetMassFromShapes();

            player = new Player(physicsWorld, crateTexture, positionTexture, upperBoundTexture, lowerBoundTexture, crateShape, crateBody, _animationFactory);
            GameWorld.Instance.AddGameObject(player);


            /* _alienFactory.Create("Alien1", new Vec2(rand.Next(0, (int)gameData.MaxXDimension), rand.Next(0, (int)gameData.MaxYDimension)));
             _alienFactory.Create("Alien2", new Vec2(rand.Next(0, (int)gameData.MaxXDimension), rand.Next(0, (int)gameData.MaxYDimension)));
             _alienFactory.Create("Alien3", new Vec2(rand.Next(0, (int)gameData.MaxXDimension), rand.Next(0, (int)gameData.MaxYDimension)));
             _alienFactory.Create("Alien4", new Vec2(rand.Next(0, (int)gameData.MaxXDimension), rand.Next(0, (int)gameData.MaxYDimension)));
             _alienFactory.Create("Alien5", new Vec2(rand.Next(0, (int)gameData.MaxXDimension), rand.Next(0, (int)gameData.MaxYDimension)));
             _alienFactory.Create("Alien6", new Vec2(rand.Next(0, (int)gameData.MaxXDimension), rand.Next(0, (int)gameData.MaxYDimension)));
             _alienFactory.Create("Alien7", new Vec2(rand.Next(0, (int)gameData.MaxXDimension), rand.Next(0, (int)gameData.MaxYDimension)));
             _alienFactory.Create("Alien8", new Vec2(rand.Next(0, (int)gameData.MaxXDimension), rand.Next(0, (int)gameData.MaxYDimension)));
             */

            //TODO:  add this back in when pixels per meter is at 250
            
            
            /*_alienFactory.Create("Alien2", new Vec2(rand.Next(40, 45), rand.Next(40, 45)));
            _alienFactory.Create("Alien3", new Vec2(rand.Next(40, 45), rand.Next(40, 45)));
            _alienFactory.Create("Alien4", new Vec2(rand.Next(40, 45), rand.Next(40, 45)));
            _alienFactory.Create("Alien5", new Vec2(rand.Next(40, 45), rand.Next(40, 45)));
            _alienFactory.Create("Alien6", new Vec2(rand.Next(40, 45), rand.Next(40, 45)));
            _alienFactory.Create("Alien7", new Vec2(rand.Next(40, 45), rand.Next(40, 45)));
            _alienFactory.Create("Alien8", new Vec2(rand.Next(40, 45), rand.Next(40, 45)));
            */
            
            CurrentGameMode = new GameMode.BasicGameMode(GameWorld.Instance, _animationFactory, _alienFactory, player);
            CurrentGameMode.Initialize();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (GameState == GameStates.Normal)
            {
                var modeState = CurrentGameMode.Update(gameTime);
                physicsWorld.Step(1.0f / 120.0f, 1,1);
                if(modeState != GameModeStatus.Continue)
                {
                    LastRespawnTime = gameTime.TotalGameTime;
                    GameState = GameStates.WaitingForRespawn;
                }
            }
            else if (GameState == GameStates.WaitingForRespawn)
            {
                var timeWaited = gameTime.TotalGameTime - LastRespawnTime;
                if(timeWaited >= RespawnWaitTime)
                {
                    GameState = GameStates.Normal;
                    Player.Instance.Reset();
                    CurrentGameMode.Initialize();
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);

            spriteBatch.Begin();

            var cameraPosition = player.CalculateCamera(viewport);

            //TODO:  implement camera logic
            Background.DrawBackground(spriteBatch, cameraPosition, viewport);

            GameWorld.Instance.Draw(spriteBatch, cameraPosition, viewport);

            Hud.Hud.Instance.Draw(spriteBatch, viewport);

            var font1 = _fontFactory.GetFont("Default");
            var font2 = _fontFactory.GetFont("Default2");
//            font1.DrawString(spriteBatch, "new game", new Vector2(400,400));
//            font2.DrawString(spriteBatch, "credits", new Vector2(400,450));
//            font2.DrawString(spriteBatch, "quit", new Vector2(400,500));

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
