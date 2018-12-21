﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Box2DX.Dynamics;
using Box2DX.Collision;
using Box2DX.Common;

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

        public Game1()
        {
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
            this.Window.AllowUserResizing = true;
            this.Window.Title = "Game1";

            base.Initialize();
        }

        private Vec2 PhysicsVec(Vector2 gfxVector)
        {
            return new Vec2(((float)gfxVector.X) / 225f, ((float)gfxVector.Y) / 255f);
        }

        private Vector2 GraphicsVec(Vec2 physicsVec)
        {
            return new Vector2()
            {
                X = physicsVec.X * 225f,
                Y = physicsVec.Y * 225f
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
        private GameObject Wall(float x, float y, float w, float h)
        {
            // Define the ground body.
            var wallBodyDef = new BodyDef();
            var wallPhysicsLocation = PhysicsVec(new Vector2(x, y));
            wallBodyDef.Position.Set(wallPhysicsLocation.X, wallPhysicsLocation.Y);

            // Call the body factory which creates the wall box shape.
            // The body is also added to the world.
            var wallBody = physicsWorld.CreateBody(wallBodyDef);

            // Define the wall box shape.
            var wallShapeDef = new PolygonDef();

            // The extents are the half-widths of the box.
            var wallPhysicsSize = PhysicsVec(new Vector2(w,h));
            wallShapeDef.SetAsBox(wallPhysicsSize.X, wallPhysicsSize.Y);

            // Add the ground shape to the ground body.
            wallBody.CreateShape(wallShapeDef);

            var vTex = GraphicsVec(wallPhysicsSize);
            var texture2d = new Texture2D(graphics.GraphicsDevice, (int) vTex.X, (int) vTex.Y);
            var data = new Microsoft.Xna.Framework.Color[(int)vTex.X * (int)vTex.Y];
            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = Microsoft.Xna.Framework.Color.Chocolate;
            }

            texture2d.SetData(data);
            return new GameObject(texture2d, wallBody);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            var crateTexture = Content.Load<Texture2D>("cratesmall");

            aabb = new AABB();
            aabb.LowerBound = new Vec2(-10, -10);
            aabb.UpperBound = new Vec2(10, 10);
            physicsWorld = new World(aabb, new Vec2(0, .98f), doSleep: true);

            //top wall
            topWall = Wall(0, 0, Window.ClientBounds.Width, 10);
            //bottom wall
            bottomWall = Wall(0, Window.ClientBounds.Height, Window.ClientBounds.Width, 10);
            //left wall
            leftWall = Wall(0, 0, 10, Window.ClientBounds.Height);
            //right wall
            rightWall = Wall(Window.ClientBounds.Width, 0, 10, Window.ClientBounds.Height);

            var crateShapeDef = new PolygonDef();
            var cratePhysicsSize = PhysicsVec(new Vector2(crateTexture.Width, crateTexture.Height));
            crateShapeDef.SetAsBox(cratePhysicsSize.X, cratePhysicsSize.Y);
            crateShapeDef.Density = 1.0f;
            crateShapeDef.Friction = 0.3f;

            var crateBodyDef = new BodyDef();
            var centerOfScreen = PhysicsVec(new Vector2(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2));
            crateBodyDef.Position.Set(centerOfScreen.X, centerOfScreen.Y);
            var crateBody = physicsWorld.CreateBody(crateBodyDef);
            crateBody.CreateShape(crateShapeDef);
            crateBody.SetMassFromShapes();

            crate = new GameObject(crateTexture, crateBody);
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
                crate.RigidBody.ApplyImpulse(new Vec2(1, 1), new Vec2(cratePosition.X, cratePosition.Y + 5));
            }

            physicsWorld.Step(1.0f / 60.0f, 2, 1);

            base.Update(gameTime);
        }

        private void DrawWalls(SpriteBatch spriteBatch)
        {
            var texturePosition = GraphicsVec(topWall.RigidBody.GetPosition());
            spriteBatch.Draw(topWall.Texture, texturePosition);
            texturePosition = GraphicsVec(bottomWall.RigidBody.GetPosition());
            spriteBatch.Draw(bottomWall.Texture, texturePosition);
            texturePosition = GraphicsVec(leftWall.RigidBody.GetPosition());
            spriteBatch.Draw(leftWall.Texture, texturePosition);
            texturePosition = GraphicsVec(rightWall.RigidBody.GetPosition());
            spriteBatch.Draw(rightWall.Texture, texturePosition);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);


            var texturePosition = GraphicsVec(crate.RigidBody.GetPosition());

            spriteBatch.Begin();
            DrawWalls(spriteBatch);
            spriteBatch.Draw(crate.Texture, texturePosition);
            spriteBatch.End();

            // TODO: Add your drawing code here


            base.Draw(gameTime);
        }
    }
}
