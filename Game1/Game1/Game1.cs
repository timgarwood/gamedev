using Microsoft.Xna.Framework;
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
        //private Joint crateJoint;
        //private Body crateAnchor;
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
        private Texture2D positionTexture;
        private Texture2D upperBoundTexture;
        private Texture2D lowerBoundTexture;
        private List<Texture2D> planets = new List<Texture2D>();

        private Vector2 viewport;

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

            //FIXME: viewport doesn't need to be calculated every frame
            viewport = new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height);

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

            //var texture2d = new Texture2D(graphics.GraphicsDevice, (int) vTex.X, (int) vTex.Y);
            //var data = new Microsoft.Xna.Framework.Color[(int)vTex.X * (int)vTex.Y];
            //for (int i = 0; i < data.Length; ++i)
           // {
           //     data[i] = Microsoft.Xna.Framework.Color.Chocolate;
           // }

           // texture2d.SetData(data);
            Logger.Info($"Wall created at ({wallBody.GetPosition().X},{wallBody.GetPosition().Y}) " + 
                $"extends to ({wallBody.GetPosition().X + wallPhysicsSize.X},{wallBody.GetPosition().Y + wallPhysicsSize.Y})");
            return new GameObject(null, shape, wallBody);
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

            //create physics bounds
            aabb = new AABB();
            aabb.LowerBound = new Vec2(0, 0);
            aabb.UpperBound = new Vec2(gameData.MaxXDimension, gameData.MaxYDimension);
            physicsWorld = new World(aabb, new Vec2(0, 0), doSleep: true);

            var width = 1200;
            var height = 800;
            //top wall
            topWall = Wall(new Vec2(0.1f, 0.1f), new Vec2(gameData.MaxXDimension-1,0.1f));
            //bottom wall
            bottomWall = Wall(new Vec2(0.1f, gameData.MaxYDimension - 1), new Vec2(gameData.MaxXDimension - 1, gameData.MaxYDimension - 1));
            //left wall
            leftWall = Wall(new Vec2(0.1f,0.1f), new Vec2(0.1f,gameData.MaxYDimension-1));
            //right wall
            rightWall = Wall(new Vec2(gameData.MaxXDimension - 1, 0.1f), new Vec2(gameData.MaxXDimension - 1, gameData.MaxYDimension - 1));

            var crateShapeDef = new PolygonDef();
            var cratePhysicsSize = PhysicsVec(new Vector2(crateTexture.Width, crateTexture.Height));
            crateShapeDef.Vertices = new Vec2[4];
            crateShapeDef.Vertices[0] = new Vec2( -(cratePhysicsSize.X / 2),  -(cratePhysicsSize.Y / 2));
            crateShapeDef.Vertices[1] = new Vec2((cratePhysicsSize.X / 2),  -(cratePhysicsSize.Y / 2));
            crateShapeDef.Vertices[2] = new Vec2((cratePhysicsSize.X / 2), (cratePhysicsSize.Y / 2));
            crateShapeDef.Vertices[3] = new Vec2(-(cratePhysicsSize.X / 2), (cratePhysicsSize.Y / 2));
            //crateShapeDef.Vertices[0] = new Vec2(-cratePhysicsSize.X,-cratePhysicsSize.Y);
            //crateShapeDef.Vertices[1] = new Vec2(0,  -cratePhysicsSize.Y);
            //crateShapeDef.Vertices[2] = new Vec2(0, 0);
            //crateShapeDef.Vertices[3] = new Vec2(-cratePhysicsSize.X, 0);
            crateShapeDef.VertexCount = 4;
            //crateShapeDef.SetAsBox(cratePhysicsSize.X/2, cratePhysicsSize.Y/2, new Vec2(0f,0f), 0);
            Logger.Info($"crate size = ({cratePhysicsSize.X},{cratePhysicsSize.Y})");
            crateShapeDef.Density = 1.0f;
            crateShapeDef.Friction = 1.6f;

            var crateBodyDef = new BodyDef();
            crateBodyDef.IsBullet = true;
            var playerPosition = new Vec2(GameData.Instance.PlayerStartX, GameData.Instance.PlayerStartY);
            crateBodyDef.Position.Set(playerPosition.X, playerPosition.Y);
            var crateBody = physicsWorld.CreateBody(crateBodyDef);
            var crateShape = crateBody.CreateShape(crateShapeDef);
            crateBody.SetMassFromShapes();

            //var anchorBodyDef = new BodyDef();
            //anchorBodyDef.IsBullet = true;
            //anchorBodyDef.Position.Set(GameData.Instance.PlayerStartX + cratePhysicsSize.X / 2, GameData.Instance.PlayerStartY + cratePhysicsSize.Y / 2);
            //crateAnchor = physicsWorld.CreateBody(anchorBodyDef);

            //var jointDef = new RevoluteJointDef();
            //jointDef.Body1 = crateBody;
            //jointDef.Body2 = crateAnchor;
            //jointDef.LocalAnchor2 = new Vec2(0, 0);
            //jointDef.LocalAnchor1 = new Vec2(cratePhysicsSize.X / 2, cratePhysicsSize.Y / 2);
            //var offset = 0;
            //jointDef.Initialize(crateAnchor, crateBody, new Vec2(anchorBodyDef.Position.X + offset, anchorBodyDef.Position.Y + offset));
            //jointDef.UpperAngle = (float)(2 * System.Math.PI);
            //jointDef.LowerAngle = (float)(2 * System.Math.PI);
            //jointDef.CollideConnected = false;
            //jointDef.MotorSpeed = 1.0f;
            //jointDef.MaxMotorTorque = 1.0f;
            //jointDef.EnableMotor = true;
            //crateJoint = physicsWorld.CreateJoint(jointDef);

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
        /// converts degrees to radians
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        private double ToRadians(double degrees)
        {
            return degrees * System.Math.PI / 180.0;
        }

        /// <summary>
        /// Takes an angle in degrees and converts to a x,y distance vector
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private Vec2 RotationToVec2(float r)
        {
            //TODO:  is there a better way to do this?
            r = r % 360;

            if(r < 0)
            {
                r = 360 + r;
            }

            var rotation = Math.Abs(r);
            int xSign, ySign;
            double x, y;

            //break rotation down into right triangles 
            if(rotation >= 0 && rotation <= 90)
            {
                xSign = 1;
                ySign = -1;
                if(rotation + 45 > 90)
                {
                    x = System.Math.Cos(ToRadians(90 - rotation));
                    y = System.Math.Sin(ToRadians(90 - rotation));
                }
                else
                {
                    x = System.Math.Sin(ToRadians(rotation));
                    y = System.Math.Cos(ToRadians(rotation));
                }
            }
            else if(rotation > 90 && rotation <= 180)
            {
                xSign = 1;
                ySign = 1;
                if(rotation + 45 > 180)
                {
                    x = System.Math.Sin(ToRadians(180 - rotation));
                    y = System.Math.Cos(ToRadians(180 - rotation));
                }
                else
                {
                    x = System.Math.Cos(ToRadians(rotation - 90));
                    y = System.Math.Sin(ToRadians(rotation - 90));
                }
            }
            else if(rotation > 180 && rotation <= 270)
            {
                xSign = -1;
                ySign = 1;
                if(rotation + 45 > 270)
                {
                    x = System.Math.Cos(ToRadians(270- rotation));
                    y = System.Math.Sin(ToRadians(270- rotation));
                }
                else
                {
                    x = System.Math.Sin(ToRadians(rotation - 180));
                    y = System.Math.Cos(ToRadians(rotation - 180));
                }
            }
            else
            {
                xSign = -1;
                ySign = -1;
                if(rotation + 45 > 360)
                {
                    x = System.Math.Sin(ToRadians(360 - rotation));
                    y = System.Math.Cos(ToRadians(360 - rotation));
                }
                else
                {
                    x = System.Math.Cos(ToRadians(rotation - 270));
                    y = System.Math.Sin(ToRadians(rotation - 270));
                }
            }

            return new Vec2((float)x * xSign, (float)y * ySign);
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

            var degrees = (crate.RigidBody.GetAngle() * 180 / System.Math.PI) % 360;

            if(Keyboard.GetState().IsKeyDown(Keys.W))
            {
                //zero out torque
                //crate.RigidBody.SetAngularVelocity(0);

                var maxImpulse = 0.0045f;
                var impulseVec = RotationToVec2((float)(crate.RigidBody.GetAngle() * 180 / System.Math.PI));
                Logger.Info($"impulse = ({impulseVec.X},{impulseVec.Y})");
                crate.RigidBody.ApplyImpulse(new Vec2(impulseVec.X * maxImpulse, impulseVec.Y * maxImpulse), crate.RigidBody.GetPosition());
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                //zero out torque
                //crate.RigidBody.SetAngularVelocity(0);

                var maxImpulse = 0.0045f;
                var impulseVec = RotationToVec2((float)(crate.RigidBody.GetAngle() * 180 / System.Math.PI));
                Logger.Info($"impulse = ({impulseVec.X},{impulseVec.Y})");
                crate.RigidBody.ApplyImpulse(new Vec2(-impulseVec.X * maxImpulse, -impulseVec.Y * maxImpulse), crate.RigidBody.GetPosition());

            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                crate.RigidBody.ApplyTorque(.001f);
                Logger.Info(crate.RigidBody.GetAngle());
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                crate.RigidBody.ApplyTorque(-.001f);
                Logger.Info(crate.RigidBody.GetAngle());
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
            var playerPosition = crate.RigidBody.GetPosition();
            var boundingBox = crate.BoundingBox;

            var cameraPosition = playerPosition - new Vec2((viewport.X / 2) * GameData.Instance.MetersPerPixel,
                (viewport.Y / 2) * GameData.Instance.MetersPerPixel);

            var angle = crate.RigidBody.GetAngle();

            spriteBatch.Begin();

            Background.DrawBackground(spriteBatch, cameraPosition, viewport);

            //draw player relative to camera
            var texturePosition= new Vector2((playerPosition.X - cameraPosition.X) * GameData.Instance.PixelsPerMeter,
                (playerPosition.Y - cameraPosition.Y) * GameData.Instance.PixelsPerMeter);
            var bodyPosition = new Vector2((crate.RigidBody.GetPosition().X - cameraPosition.X) * GameData.Instance.PixelsPerMeter,
                (crate.RigidBody.GetPosition().Y - cameraPosition.Y)* GameData.Instance.PixelsPerMeter);
            var upperBound = new Vector2((crate.BoundingBox.UpperBound.X - cameraPosition.X) * GameData.Instance.PixelsPerMeter,
                (crate.BoundingBox.UpperBound.Y - cameraPosition.Y) * GameData.Instance.PixelsPerMeter);
            var lowerBound = new Vector2((crate.BoundingBox.LowerBound.X - cameraPosition.X) * GameData.Instance.PixelsPerMeter,
                (crate.BoundingBox.LowerBound.Y - cameraPosition.Y) * GameData.Instance.PixelsPerMeter);

            spriteBatch.Draw(crate.Texture, texturePosition, null, null, rotation:angle, origin: new Vector2(crate.Texture.Width/2, crate.Texture.Height/2));
            spriteBatch.Draw(positionTexture, bodyPosition);
            spriteBatch.Draw(upperBoundTexture, upperBound);
            spriteBatch.Draw(lowerBoundTexture, lowerBound);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
