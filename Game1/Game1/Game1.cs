﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Box2DX.Dynamics;
using Box2DX.Collision;
using Box2DX.Common;
using NLog;
using System.Collections.Generic;

namespace Game1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private GameObject crate;
        private World physicsWorld;
        private AABB aabb;
        private GameObject topWall;
        private GameObject bottomWall;
        private GameObject leftWall;
        private GameObject rightWall;
        private Vec2 currentCrateVelocity;
        private Logger Logger = LogManager.GetCurrentClassLogger();

        private GameData gameData;

        private Texture2D blueStar;
        private Texture2D brightMoon;
        private Texture2D brightStar;
        private Texture2D dirtPlanet;
        private Texture2D firePlanet;
        private Texture2D venusPlanet;
        private Texture2D yellowPlanet;
        private List<Texture2D> planets = new List<Texture2D>();

        public Game1(GameData data)
        {
            gameData = data;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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


            base.Initialize();
        }

        private Vec2 PhysicsVec(Vector2 gfxVector)
        {
            return new Vec2(((float)gfxVector.X) * (1.0f/gameData.PixelsPerMeter), ((float)gfxVector.Y) * (1.0f/gameData.PixelsPerMeter));
        }

        private Vector2 GraphicsVec(Vec2 physicsVec)
        {
            return new Vector2()
            {
                X = physicsVec.X * gameData.PixelsPerMeter,
                Y = physicsVec.Y * gameData.PixelsPerMeter 
            };
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
            var vTex = GraphicsVec(wallPhysicsSize);

            if(vTex.X <= 0)
            {
                vTex.X = 1;
            }
            if(vTex.Y <= 0)
            {
                vTex.Y = 1;
            }

            var texture2d = new Texture2D(graphics.GraphicsDevice, (int) vTex.X, (int) vTex.Y);
            var data = new Microsoft.Xna.Framework.Color[(int)vTex.X * (int)vTex.Y];
            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = Microsoft.Xna.Framework.Color.Chocolate;
            }

            texture2d.SetData(data);
            Logger.Info($"Wall created at ({wallBody.GetPosition().X},{wallBody.GetPosition().Y}) " + 
                $"extends to ({wallBody.GetPosition().X + wallPhysicsSize.X},{wallBody.GetPosition().Y + wallPhysicsSize.Y})");
            return new GameObject(texture2d, shape, wallBody);
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

            //create physics bounds
            aabb = new AABB();
            aabb.LowerBound = new Vec2(0, 0);
            aabb.UpperBound = new Vec2(gameData.MaxXDimension, gameData.MaxYDimension);
            physicsWorld = new World(aabb, new Vec2(0, 0), doSleep: true);

            int wallThickness = 10;
            var width = 1200;
            var height = 800;
            //top wall
            //topWall = Wall(new Vec2(10*gameData.MetersPerPixel,10* gameData.MetersPerPixel), new Vec2(width* gameData.MetersPerPixel, 10* gameData.MetersPerPixel));
            //bottom wall
            //bottomWall = Wall(new Vec2(10* gameData.MetersPerPixel, height * gameData.MetersPerPixel), new Vec2(width * gameData.MetersPerPixel, height * gameData.MetersPerPixel));
            //left wall
            //leftWall = Wall(new Vec2(10 * gameData.MetersPerPixel, 10 * gameData.MetersPerPixel), new Vec2(10 * gameData.MetersPerPixel, height * gameData.MetersPerPixel));
            //right wall
            //rightWall = Wall(new Vec2(width * gameData.MetersPerPixel, 10 * gameData.MetersPerPixel), new Vec2(width * gameData.MetersPerPixel, height * gameData.MetersPerPixel));

            var crateShapeDef = new PolygonDef();
            var cratePhysicsSize = PhysicsVec(new Vector2(crateTexture.Width, crateTexture.Height));
            crateShapeDef.SetAsBox(cratePhysicsSize.X/2, cratePhysicsSize.Y/2);
            Logger.Info($"crate size = ({cratePhysicsSize.X},{cratePhysicsSize.Y})");
            crateShapeDef.Density = 1.0f;
            crateShapeDef.Friction = 0.6f;

            var crateBodyDef = new BodyDef();
            crateBodyDef.IsBullet = true;
            var playerPosition = new Vec2(GameData.Instance.PlayerStartX, GameData.Instance.PlayerStartY);
            crateBodyDef.Position.Set(playerPosition.X, playerPosition.Y);
            var crateBody = physicsWorld.CreateBody(crateBodyDef);
            var crateShape = crateBody.CreateShape(crateShapeDef);
            crateBody.SetMassFromShapes();

            crate = new GameObject(crateTexture, crateShape, crateBody);
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

            if(Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                var cratePosition = crate.RigidBody.GetPosition();
                crate.RigidBody.ApplyImpulse(new Vec2(-.0025f, 0), new Vec2(cratePosition.X, cratePosition.Y));
            }

            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                var cratePosition = crate.RigidBody.GetPosition();
                crate.RigidBody.ApplyImpulse(new Vec2(.0025f, 0), new Vec2(cratePosition.X, cratePosition.Y));
            }

            if(Keyboard.GetState().IsKeyDown(Keys.W))
            {
                var cratePosition = crate.RigidBody.GetPosition();
                crate.RigidBody.ApplyImpulse(new Vec2(0, -0.0045f), new Vec2(cratePosition.X, cratePosition.Y));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                var cratePosition = crate.RigidBody.GetPosition();
                crate.RigidBody.ApplyImpulse(new Vec2(0, 0.0045f), new Vec2(cratePosition.X, cratePosition.Y));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                var cratePosition = crate.RigidBody.GetPosition();
                crate.RigidBody.ApplyImpulse(new Vec2(0.0045f, 0), new Vec2(cratePosition.X, cratePosition.Y));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                var cratePosition = crate.RigidBody.GetPosition();
                crate.RigidBody.ApplyImpulse(new Vec2(-0.0045f, 0), new Vec2(cratePosition.X, cratePosition.Y));
            }

            physicsWorld.Step(1.0f / 60.0f, 2,1);

            var lVelocity = crate.RigidBody.GetLinearVelocity();
            if (currentCrateVelocity != null)
            {
                if (Math.Abs(lVelocity.X) <= 0 && Math.Abs(lVelocity.Y) <= 0 && 
                    (Math.Abs(currentCrateVelocity.X) > 0 || Math.Abs(currentCrateVelocity.Y) > 0))
                {
                    //we came to a stop, log the position
                    var position = crate.RigidBody.GetPosition();
                    Logger.Info($"Crate stopped at ({position.X},{position.Y})");
                }
            }

            currentCrateVelocity = lVelocity;
            base.Update(gameTime);
        }

        private void DrawWalls(SpriteBatch spriteBatch)
        {
            /*var planetLocation = new Vector2(0, 0);
            var scale = .75f;
            var vscale = new Vector2(scale,scale);
            foreach(var planet in planets)
            {
                spriteBatch.Draw(planet, planetLocation, null, null, null, 0, vscale);
                planetLocation.X += (planet.Width * scale);
                if(planetLocation.X > 800)
                {
                    planetLocation.X = 0;
                    planetLocation.Y += 250 * scale;
                }
            }
            */
            /*var texturePosition = GraphicsVec(topWall.RigidBody.GetPosition());
            spriteBatch.Draw(topWall.Texture, texturePosition);
            texturePosition = GraphicsVec(bottomWall.RigidBody.GetPosition());
            spriteBatch.Draw(bottomWall.Texture, texturePosition);
            texturePosition = GraphicsVec(leftWall.RigidBody.GetPosition());
            spriteBatch.Draw(leftWall.Texture, texturePosition);
            texturePosition = GraphicsVec(rightWall.RigidBody.GetPosition());
            spriteBatch.Draw(rightWall.Texture, texturePosition);
            */
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);

            //figure out where the camera should be using the player position

            //FIXME: viewport doesn't need to be calculated every frame
            var viewport = new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height);

            var playerPosition = crate.RigidBody.GetPosition();

            var cameraPosition = playerPosition - new Vec2((viewport.X / 2) * GameData.Instance.MetersPerPixel,
                (viewport.Y / 2) * GameData.Instance.MetersPerPixel); 

            spriteBatch.Begin();

            Background.DrawBackground(spriteBatch, cameraPosition, viewport);

            //draw player relative to camera
            var drawPosition = new Vector2((playerPosition.X - cameraPosition.X) * GameData.Instance.PixelsPerMeter,
                (playerPosition.Y - cameraPosition.Y) * GameData.Instance.PixelsPerMeter);

            spriteBatch.Draw(crate.Texture, drawPosition, null, null, null, crate.RigidBody.GetAngle());
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
