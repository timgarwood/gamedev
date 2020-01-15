using Microsoft.Xna.Framework;
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
using Game1.Animations;
using Game1.Menu;
using Game1.GameMode;
using Game1.Physics;
using System;
using Game1.Pickups;
using Newtonsoft.Json;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Microsoft.Xna.Framework.Content;

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
        private Player Player;
        private Logger Logger = LogManager.GetCurrentClassLogger();

        private List<Texture2D> planets = new List<Texture2D>();

        private Vector2 viewport;

        private GameMode.GameMode CurrentGameMode { get; set; }

        private GameStates GameState { get; set; }

        private TimeSpan RespawnWaitTime { get; set; } = TimeSpan.FromSeconds(3);

        private TimeSpan LastRespawnTime { get; set; }

        private WindsorContainer Container { get; set; }
        private World PhysicsWorld { get; set; }

        private Background Background { get; set; }

        private GameWorld GameWorld { get; set; }

        private Hud.Hud Hud { get; set; }

        public Game1() 
        {
            graphics = new GraphicsDeviceManager(this);

            Container = new WindsorContainer();
            
            Content.RootDirectory = "Content";

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
            
            var gameData = JsonConvert.DeserializeObject<GameData>(File.ReadAllText("./GameData.json"));
            this.TargetElapsedTime = System.TimeSpan.FromSeconds(1f / gameData.Fps);

            var gameUtils = new GameUtils(gameData);

            //create physics bounds
            var aabb = new AABB();
            aabb.LowerBound = new Vec2(0, 0);
            aabb.UpperBound = new Vec2(gameData.MaxXDimension, gameData.MaxYDimension);
            PhysicsWorld = new World(aabb, new Vec2(0, 0), doSleep: false);
            PhysicsWorld.SetContactListener(new GameContactListener());

            Container.Register(Component.For<GraphicsDevice>().Instance(GraphicsDevice).LifestyleSingleton());
            Container.Register(Component.For<GameData>().Instance(gameData).LifestyleSingleton());
            Container.Register(Component.For<GameUtils>().Instance(gameUtils).LifestyleSingleton());
            Container.Register(Component.For<GameWorld>().ImplementedBy<GameWorld>().LifestyleSingleton());
            Container.Register(Component.For<World>().Instance(PhysicsWorld).LifestyleSingleton());
            Container.Register(Component.For<ContentManager>().Instance(Content).LifestyleSingleton());
            Container.Register(Component.For<FontFactory>().ImplementedBy<FontFactory>().LifestyleSingleton());
            Container.Register(Component.For<MenuFactory>().ImplementedBy<MenuFactory>().LifestyleSingleton());
            Container.Register(Component.For<AnimationFactory>().ImplementedBy<AnimationFactory>().LifestyleSingleton());
            Container.Register(Component.For<WeaponFactory>().ImplementedBy<WeaponFactory>().LifestyleSingleton());
            Container.Register(Component.For<AlienFactory>().ImplementedBy<AlienFactory>().LifestyleSingleton());
            Container.Register(Component.For<WeaponInventory>().ImplementedBy<WeaponInventory>().LifestyleSingleton());
            Container.Register(Component.For<Background>().ImplementedBy<Background>().LifestyleSingleton());
            Container.Register(Component.For<WallFactory>().ImplementedBy<WallFactory>().LifestyleSingleton());
            Container.Register(Component.For<Hud.Hud>().ImplementedBy<Hud.Hud>());
            Container.Register(Component.For<BasicGameMode>().ImplementedBy<BasicGameMode>());
            Container.Register(Component.For<PickupFactory>().ImplementedBy<PickupFactory>());
            Container.Register(Component.For<PlayerFactory>().ImplementedBy<PlayerFactory>().LifestyleSingleton());
            
            float xpix = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            float ypix = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            viewport = new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height);
            
            var crateTexture = Content.Load<Texture2D>("sprites/ships/ship1small");
            Player = Container.Resolve<PlayerFactory>().CreatePlayer(crateTexture);
            Container.Register(Component.For<Player>().Instance(Player).LifestyleSingleton());

            //load up aliens
            using (var stream = new FileStream("./AlienDefinitions.json", FileMode.Open))
            {
                Container.Resolve<AlienFactory>().Load(stream);
            }

            //load up weapons
            using(var stream = new FileStream("./Weapons/WeaponDefinitions.json", FileMode.Open))
            {
                Container.Resolve<WeaponFactory>().Load(stream);
            }

            //load up pickups
            using(var stream = new FileStream("./Pickups/PickupDefinitions.json", FileMode.Open))
            {
                Container.Resolve<PickupFactory>().Load(stream);
            }

            //load up fonts
            using (var stream = new FileStream("./Fonts/FontDefinitions.json", FileMode.Open))
            {
                Container.Resolve<FontFactory>().Load(stream);
            }

            //load up animations
            using (var stream = new FileStream("./Animations/AnimationDefinitions.json", FileMode.Open))
            {
                Container.Resolve<AnimationFactory>().Load(stream);
            }

            //load up menus
            using (var stream = new FileStream("./Menu/MenuDefinitions.json", FileMode.Open))
            {
                Container.Resolve<MenuFactory>().Load(stream);
            }

            //load up the HUD
            using (var stream = new FileStream("./Hud/HudDefinition.json", FileMode.Open))
            {
                var hud = Container.Resolve<Hud.Hud>();
                hud.Load(stream);
                hud.OnWindowResized(viewport);
            }

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
            Hud.OnWindowResized(viewport);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Container.Resolve<Background>().GenerateBackground(new[] { Content.Load<Texture2D>("sprites/backgrounds/redsand1") });
            Container.Resolve<WallFactory>().CreateWalls();


            Background = Container.Resolve<Background>();
            GameWorld = Container.Resolve<GameWorld>();
            Hud = Container.Resolve<Hud.Hud>();

            CurrentGameMode = Container.Resolve<BasicGameMode>();
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
                PhysicsWorld.Step(1.0f / 120.0f, 1,1);
                if(Player.LivesRemaining <= 0)
                {
                    Exit();
                }

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
                    Player.Reset();
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

            var cameraPosition = Player.CalculateCamera(viewport);

            Background.DrawBackground(spriteBatch, cameraPosition, viewport);

            GameWorld.Draw(spriteBatch, cameraPosition, viewport);

            Hud.Draw(spriteBatch, viewport);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
