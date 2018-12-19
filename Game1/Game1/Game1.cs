using Microsoft.Xna.Framework;
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

        private void AddWall(float x, float y, float w, float h)
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
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            var crateTexture = Content.Load<Texture2D>("crate");

            aabb = new AABB();
            aabb.LowerBound = new Vec2(-10, -10);
            aabb.UpperBound = new Vec2(10, 10);
            physicsWorld = new World(aabb, new Vec2(0, .98f), doSleep: true);

            var wallDim = PhysicsVec(new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height));

            //top wall
            AddWall(0, 0, wallDim.X, 0.5f);
            //bottom wall
            AddWall(0, wallDim.Y, wallDim.X, 0.5f);
            //left wall
            AddWall(0, 0, 0.5f, wallDim.Y);
            //right wall
            AddWall(0, wallDim.X, 0.5f, wallDim.Y);

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

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);

            var texturePosition = GraphicsVec(crate.RigidBody.GetPosition());

            spriteBatch.Begin();
            spriteBatch.Draw(crate.Texture, texturePosition);
            spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
