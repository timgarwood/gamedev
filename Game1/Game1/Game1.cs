using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using Box2DX.Dynamics;
//using Box2DX.Collision;
//using Box2DX.Common;
using NLog;
using ChipmunkSharp;
using System; 

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

        private Logger Logger = LogManager.GetCurrentClassLogger();
        private static float PixelsPerMeterX;
        private static float PixelsPerMeterY;

        // physics parameters
        /*private World physicsWorld;
        private AABB aabb;
        private Vec2 currentCrateVelocity;
        */

        private GameObject topWall;
        private GameObject bottomWall;
        private GameObject leftWall;
        private GameObject rightWall;

        private cpSpace physicsWorld;
        private cpVect currentCrateVelocity;

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

        /*private Vec2 PhysicsVec(Vector2 gfxVector)
        {
            return new Vec2(gfxVector.X * (1.0f/PixelsPerMeterX), gfxVector.Y * (1.0f/PixelsPerMeterY));
        }

        private Vector2 GraphicsVec(Vec2 physicsVec)
        {
            return new Vector2()
            {
                X = physicsVec.X * PixelsPerMeterX,
                Y = physicsVec.Y * PixelsPerMeterY 
            };
        }
        */

        private float PhysicsScalarX(float graphicsX)
        {
            return graphicsX * (1.0f / PixelsPerMeterX);
        }

        private float PhysicsScalarY(float graphicsY)
        {
            return graphicsY * (1.0f / PixelsPerMeterY);
        }

        private cpVect PhysicsVec(Vector2 gfxVector)
        {
            return new cpVect(gfxVector.X * (1.0f / PixelsPerMeterX), gfxVector.Y * (1.0f / PixelsPerMeterY));
        }

        private Vector2 GraphicsVec(cpVect physicsVec)
        {
            return new Vector2()
            {
                X = physicsVec.x * PixelsPerMeterX,
                Y = physicsVec.y * PixelsPerMeterY
            };
        }

        private float GraphicsScalarX(float x)
        {
            return x * PixelsPerMeterX;
        }

        private float GraphicsScalarY(float x)
        {
            return x * PixelsPerMeterY;
        }

        private GameObject Wall(cpVect topLeft, cpVect bottomRight)
        {
            var wallBody = physicsWorld.GetStaticBody();
            physicsWorld.AddBody(wallBody);
            var wallShape = physicsWorld.AddShape(new cpSegmentShape(wallBody, topLeft,bottomRight, 0));
            wallShape.SetFriction(1.0f);
            wallShape.SetElasticity(0.0f);

            var textureWidth = GraphicsScalarX(Math.Abs(bottomRight.x - topLeft.x));
            var textureHeight = GraphicsScalarY(Math.Abs(bottomRight.y - topLeft.y));
            if(textureWidth <= 0)
            {
                textureWidth = 1;
            }
            if(textureHeight <= 0)
            {
                textureHeight = 1;
            }

            var texture2d = new Texture2D(graphics.GraphicsDevice, (int)textureWidth, (int)textureHeight);
            var data = new Color[(int)textureWidth * (int)textureHeight];
            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = Color.Chocolate;
            }

            texture2d.SetData(data);
            //Logger.Info($"Wall created with bounds {wallShape.GetA()} => {wallShape.GetB()}");
            return new GameObject(texture2d, wallShape, wallShape.body);
        }

        private GameObject Crate(Texture2D crateTexture)
        {
            var centerOfScreen = new cpVect(50,50);
            var cratePhysicsSize = PhysicsVec(new Vector2(crateTexture.Width, crateTexture.Height));
            var mass = 3;
            var radius = cratePhysicsSize.x / 2;
            //todo: fix moment of inertia for polygons
            
            var crateBody = physicsWorld.AddBody(new cpBody(mass, cp.MomentForBox(mass,cratePhysicsSize.x,cratePhysicsSize.y)));
            crateBody.SetPosition(centerOfScreen);
            crateBody.SetAngle(1.0f);
            var crateShape = physicsWorld.AddShape(cpPolyShape.BoxShape(crateBody, cratePhysicsSize.x, cratePhysicsSize.y, 0)) as cpPolyShape;
            crateShape.SetFriction(0.7f);
            crateShape.SetElasticity(0.5f);
            Logger.Info($"crate size = ({cratePhysicsSize.x},{cratePhysicsSize.y})");
            return new GameObject(crateTexture, crateShape, crateBody);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            float xpix = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            float ypix = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            //calculate pixels-per-meter for x and y
            //lets say a meter is some number of x-pixels
            PixelsPerMeterX = 2;
            PixelsPerMeterY = PixelsPerMeterX * (ypix / xpix);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            var crateTexture = Content.Load<Texture2D>("cratesmall");

            //create physics bounds
            /*aabb = new AABB();
            aabb.LowerBound = new Vec2(-10, -10);
            aabb.UpperBound = new Vec2(10, 10);
            physicsWorld = new World(aabb, new Vec2(0, .98f), true);
            */

            physicsWorld = new cpSpace();
            physicsWorld.SetGravity(new cpVect(0, 150));
            physicsWorld.SetSleepTimeThreshold(0.5f);
            physicsWorld.SetCollisionSlop(0.0f);
            physicsWorld.SetSleepTimeThreshold(0.5f);

            //top wall
            topWall = Wall(new cpVect(10,10), new cpVect(400,10));
            //bottom wall
            bottomWall = Wall(new cpVect(10,240), new cpVect(400,240));
            //left wall
            leftWall = Wall(new cpVect(10,10), new cpVect(10, 240));
            //right wall
            rightWall = Wall(new cpVect(400, 10), new cpVect(400,240));

            crate = Crate(crateTexture);
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

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                var cratePosition = crate.Body.GetPosition();
                crate.Body.ApplyImpulse(new cpVect(5f, 5f), new cpVect(cratePosition.x, cratePosition.y + 5));
            }
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                var cratePosition = crate.Body.GetPosition();
                crate.Body.ApplyImpulse(new cpVect(-5f, -5f), new cpVect(cratePosition.x, cratePosition.y + 5));
            }

            //physicsWorld.Step(1.0f / 60.0f, 100, 100);
            physicsWorld.Step(1.0f / 30.0f);
            
            var lVelocity = crate.Body.GetVelocity();
            if (currentCrateVelocity != null)
            {
                if (Math.Abs(lVelocity.x) <= 0 && Math.Abs(lVelocity.y) <= 0 &&
                    (Math.Abs(currentCrateVelocity.x) > 0 || Math.Abs(currentCrateVelocity.y) > 0))
                {
                    //we came to a stop, log the position
                    var position = crate.Body.GetPosition();
                    Logger.Info($"Crate stopped at ({position.x},{position.y})");
                }
            }

            currentCrateVelocity = lVelocity;
            base.Update(gameTime);
        }

        private void DrawWalls(SpriteBatch spriteBatch)
        {
            var texturePosition = GraphicsVec(new cpVect(topWall.Shape.GetBB().l, topWall.Shape.GetBB().b));
            spriteBatch.Draw(topWall.Texture, texturePosition);
            texturePosition = GraphicsVec(new cpVect(bottomWall.Shape.GetBB().l, bottomWall.Shape.GetBB().b));
            spriteBatch.Draw(bottomWall.Texture, texturePosition);
            texturePosition = GraphicsVec(new cpVect(leftWall.Shape.GetBB().l, leftWall.Shape.GetBB().b));
            spriteBatch.Draw(leftWall.Texture, texturePosition);
            texturePosition = GraphicsVec(new cpVect(rightWall.Shape.GetBB().l, rightWall.Shape.GetBB().b));
            spriteBatch.Draw(rightWall.Texture, texturePosition);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            var texturePosition = GraphicsVec(crate.Body.GetPosition());
            var origin = new Vector2(crate.Texture.Width / 2, crate.Texture.Height / 2);

            spriteBatch.Begin();
            DrawWalls(spriteBatch);
            spriteBatch.Draw(crate.Texture, texturePosition, null, null, origin, crate.Body.GetAngle());
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
