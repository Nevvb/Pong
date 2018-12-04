using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Pong_Example
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D boardTex;
        Texture2D ballTex;

        SpriteFont font;

        Rectangle boardRightRect;
        Rectangle boardLeftRect;
        Rectangle ballRect;

        Vector2 velocity;

        int boardSpeed = 2;
        int ballSpeed = 6;

        int scoreLeft;
        int scoreRight;

        float timer = 0;

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
            //Initialize rectangles and ball's velocity
            boardRightRect = new Rectangle(748, 222, 32, 128);
            boardLeftRect = new Rectangle(20, 222, 32, 128);
            ballRect = new Rectangle(390, 230, 32, 32);

            velocity = new Vector2(ballSpeed/2, ballSpeed/2);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Load Textures and fonts
            boardTex = Content.Load<Texture2D>("board");
            ballTex = Content.Load<Texture2D>("Ball");

            font = Content.Load<SpriteFont>("Font");

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

            //Call methods That controls the boards and the ball
            MoveBall(gameTime);
            MoveLeftBoard();
            MoveRightBoard();

            base.Update(gameTime);
        }


        private void MoveBall(GameTime gameTime)
        {
            //Check if the ball touches the roof
            if(ballRect.Y <= 0 && velocity.Y < 0)
            {
                velocity.Y *= -1;
            }
            //Check if the ball touches the floor
            else if(ballRect.Bottom >= 480 && velocity.Y > 0)
            {
                velocity.Y *= -1;
            }

            //Check if ball collides with the left board
            if (ballRect.Intersects(boardLeftRect) && velocity.X < 0)
            {
                //Check if the ball should bounce up, forward or down

                float delta = (ballRect.Y - boardLeftRect.Y) * 3;
                int angle = (int)delta / boardLeftRect.Height;

                ballSpeed++;

                switch (angle)
                {
                    case 0:
                        velocity = new Vector2(ballSpeed / 2, -ballSpeed / 2);
                        break;
                    case 1:
                        velocity = new Vector2(ballSpeed, 0);
                        break;
                    case 2:
                        velocity = new Vector2(ballSpeed / 2, ballSpeed / 2);
                        break;
                }
            }
            //Check if the ball collides right board
            else if (ballRect.Intersects(boardRightRect) && velocity.X > 0)
            {
                //Check if the ball should bounce up, forward or down

                float delta = (ballRect.Y - boardRightRect.Y) * 3;
                int angle = (int)delta / boardRightRect.Height;

                ballSpeed++;

                switch (angle)
                {
                    case 0:
                        velocity = new Vector2(-ballSpeed / 2, -ballSpeed / 2);
                        break;
                    case 1:
                        velocity = new Vector2(-ballSpeed, 0);
                        break;
                    case 2:
                        velocity = new Vector2(-ballSpeed / 2, ballSpeed / 2);
                        break;
                }

            }

            //Check if the ball leaves the left side of the arena
            if(ballRect.Right <= 0)
            {   
                //Start counting to 3000 millisecs (3 sec), give score and reset the ball.
                timer += gameTime.ElapsedGameTime.Milliseconds;
                if(timer >= 3000)
                {
                    ballRect.X = 390;
                    ballRect.Y = 230;
                    timer = 0;
                    scoreRight++;
                    ballSpeed = 6;
                    velocity = new Vector2(ballSpeed / 2, ballSpeed / 2);
                }
            }
            //Check if the ball leaves the right side of the arena
            else if (ballRect.Left >= 800)
            {
                //Start counting to 3000 millisecs (3 sec), give score and reset the ball.
                timer += gameTime.ElapsedGameTime.Milliseconds;
                if (timer >= 3000)
                {
                    ballRect.X = 390;
                    ballRect.Y = 230;
                    timer = 0;
                    scoreLeft++;
                    ballSpeed = 6;
                    velocity = new Vector2(ballSpeed / 2, ballSpeed / 2);
                }
            }

            //Apply velocity to the ball
            ballRect.X += (int)velocity.X;
            ballRect.Y += (int)velocity.Y;
        }

        //Check for user input and move the left board
        private void MoveLeftBoard()
        {
            if(Keyboard.GetState().IsKeyDown(Keys.S) && boardLeftRect.Bottom < 480)
            {
                boardLeftRect.Y += boardSpeed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.W) && boardLeftRect.Y > 0)
            {
                boardLeftRect.Y -= boardSpeed;
            }
        }

        //Controlled by human
        /*private void MoveRightBoard()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Down) && boardRightRect.Bottom < 480)
            {
                boardRightRect.Y += boardSpeed;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up) && boardRightRect.Y > 0)
            {
                boardRightRect.Y -= boardSpeed;
            }
        }*/

        //Controlled by AI
        private void MoveRightBoard()
        {

            int direction = 0;

            //Check if the computer should move the board up or down
            if(ballRect.Y < boardRightRect.Top)
            {
                direction = 1;
            }

            if (ballRect.Y > boardRightRect.Bottom)
            {
                direction = -1;
            }

            //Move the board and make sure it doesn't leave the arena
            if (direction == -1 && boardRightRect.Bottom < 480)
            {
                boardRightRect.Y += boardSpeed;
            }
            if (direction == 1 && boardRightRect.Y > 0)
            {
                boardRightRect.Y -= boardSpeed;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            Vector2 textSize = font.MeasureString("Score") * 3;

            //Draw all the textures and text. (Text is overly complicated at this point, you don't need to know what everything does)
            spriteBatch.DrawString(font, "Score", new Vector2(400 - textSize.X/2, 100), Color.Black, 0f, Vector2.Zero, 3f, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, scoreLeft.ToString(), new Vector2(250, 180), Color.Black, 0f, Vector2.Zero, 3f, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, "" + scoreRight, new Vector2(550, 180), Color.Black, 0f, Vector2.Zero, 3f, SpriteEffects.None, 1f);

            spriteBatch.Draw(boardTex, boardLeftRect, Color.White);
            spriteBatch.Draw(boardTex, boardRightRect, Color.White);
            spriteBatch.Draw(ballTex, ballRect, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
