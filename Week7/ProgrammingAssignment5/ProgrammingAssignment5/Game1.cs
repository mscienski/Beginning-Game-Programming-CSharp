using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ProgrammingAssignment5
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //window settings
        const int WINDOW_WIDTH = 800;
        const int WINDOW_HEIGHT = 600;

        //sprites
        static Texture2D mineSprite;
        static Texture2D teddyBearSprite;
        static Texture2D explosionSpriteStrip;

        //mines
        List<TeddyMineExplosion.Mine> mines = new List<TeddyMineExplosion.Mine>();

        //teddy bears
        List<TeddyMineExplosion.TeddyBear> teddyBears = new List<TeddyMineExplosion.TeddyBear>();

        //explosions
        static List<TeddyMineExplosion.Explosion> explosions = new List<TeddyMineExplosion.Explosion>();

        //clicks
        bool leftClickStarted = false;
        bool leftButtonReleased = true;

        //timers
        int elapsedSpawnMilliseconds = 0;

        //delays
        int spawnDelayMilliseconds = 0;

        //random
        Random rand = new Random();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //set the window
            graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            //set the mouse cursor
            IsMouseVisible = true;
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

            // TODO: use this.Content to load your game content here
            //load the textures
            mineSprite = Content.Load<Texture2D>("mine");
            teddyBearSprite = Content.Load<Texture2D>("teddybear");
            explosionSpriteStrip = Content.Load<Texture2D>("explosion");

            spawnDelayMilliseconds = GetRandomSpawnDelay();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            //get a new mouse state
            MouseState mouse = Mouse.GetState();
            //left click has started
            if (mouse.LeftButton == ButtonState.Pressed && leftButtonReleased) {
                leftClickStarted = true;
                leftButtonReleased = false;
            } else if (mouse.LeftButton == ButtonState.Released) {
                //left click has finished
                leftButtonReleased = true;

                if (leftClickStarted) {
                    leftClickStarted = false;
                    //add a new mine at the mouse location
                    mines.Add(new TeddyMineExplosion.Mine(mineSprite, mouse.X, mouse.Y));
                }
            }

            //update the spawn timer
            elapsedSpawnMilliseconds += gameTime.ElapsedGameTime.Milliseconds;
            if (elapsedSpawnMilliseconds >= spawnDelayMilliseconds)
            {
                //reset the spawn timer
                elapsedSpawnMilliseconds = 0;
                //get a new random spawn delay for next time
                spawnDelayMilliseconds = GetRandomSpawnDelay();
                //spawn a teddy bear
                teddyBears.Add(new TeddyMineExplosion.TeddyBear(teddyBearSprite, GetRandomTeddyBearVelocity(), graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight));
            }


            foreach (TeddyMineExplosion.TeddyBear bear in teddyBears)
            {
                //update the existing bears
                bear.Update(gameTime);

                foreach (TeddyMineExplosion.Mine mine in mines)
                {
                    //check for collisions with mines
                    if (bear.CollisionRectangle.Intersects(mine.CollisionRectangle))
                    {
                        //deactivate the bear and mine
                        bear.Active = false;
                        mine.Active = false;
                        //add the explosion at the mine location
                        explosions.Add(new TeddyMineExplosion.Explosion(explosionSpriteStrip, mine.CollisionRectangle.Center.X, mine.CollisionRectangle.Center.Y));
                    }
                }
            }

            //update the explosions
            foreach (TeddyMineExplosion.Explosion explosion in explosions)
            {
                explosion.Update(gameTime);
            }

            //remove inactive bears
            for (int i = teddyBears.Count - 1; i >= 0; i--)
            {
                if (!teddyBears[i].Active)
                {
                    teddyBears.RemoveAt(i);
                }
            }

            //remove inactive mines
            for (int i = mines.Count - 1; i >= 0; i--)
            {
                if (!mines[i].Active)
                {
                    mines.RemoveAt(i);
                }
            }

            //remove explosions that have finished playing
            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                if (!explosions[i].Playing)
                {
                    explosions.RemoveAt(i);
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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            //draw the mines
            foreach(TeddyMineExplosion.Mine mine in mines) {
                mine.Draw(spriteBatch);
            }

            //draw the bears
            foreach (TeddyMineExplosion.TeddyBear bear in teddyBears)
            {
                bear.Draw(spriteBatch);
            }

            //draw the explosions
            foreach (TeddyMineExplosion.Explosion explosion in explosions)
            {
                explosion.Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }


        /// <summary>
        /// gets a random spawn delay between 1 and 3 seconds, or 1000 and 3000 milliseconds
        /// </summary>
        /// <returns>int a random number between 1000 and 3000, multiples of 1000</returns>
        private int GetRandomSpawnDelay()
        {
            return rand.Next(1, 4) * 1000;
        }

        /// <summary>
        /// gets a random teddy bear velocity between -0.5 and 0.5
        /// </summary>
        /// <returns>Vector2 a random vector</returns>
        private Vector2 GetRandomTeddyBearVelocity()
        {
            return new Vector2((float)(-0.5 + rand.NextDouble()), (float)(-0.5 + rand.NextDouble()));
        }
    }
}
