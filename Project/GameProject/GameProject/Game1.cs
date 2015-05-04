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

namespace GameProject
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // game objects. Using inheritance would make this
        // easier, but inheritance isn't a GDD 1200 topic
        Burger burger;
        List<TeddyBear> bears = new List<TeddyBear>();
        static List<Projectile> projectiles = new List<Projectile>();
        List<Explosion> explosions = new List<Explosion>();

        // projectile and explosion sprites. Saved so they don't have to
        // be loaded every time projectiles or explosions are created
        static Texture2D frenchFriesSprite;
        static Texture2D teddyBearProjectileSprite;
        static Texture2D explosionSpriteStrip;

        // scoring support
        int score = 0;
        string scoreString = GameConstants.SCORE_PREFIX + 0;

        // health support
        string healthString = GameConstants.HEALTH_PREFIX + 
            GameConstants.BURGER_INITIAL_HEALTH;
        bool burgerDead = false;

        // text display support
        SpriteFont font;

        // sound effects
        SoundEffect burgerDamage;
        SoundEffect burgerDeath;
        SoundEffect burgerShot;
        SoundEffect explosion;
        SoundEffect teddyBounce;
        SoundEffect teddyShot;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // set resolution
            graphics.PreferredBackBufferWidth = GameConstants.WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = GameConstants.WINDOW_HEIGHT;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            RandomNumberGenerator.Initialize();

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

            // load audio content

            // load sprite font

            // load projectile and explosion sprites
            frenchFriesSprite = Content.Load<Texture2D>("frenchfries");
            teddyBearProjectileSprite = Content.Load<Texture2D>("teddybearprojectile");
            explosionSpriteStrip = Content.Load<Texture2D>("explosion");

            // add initial game objects
            burger = new Burger(Content, "burger", graphics.PreferredBackBufferWidth / 2, (graphics.PreferredBackBufferHeight / 8) * 7, null);
            //add bears until max number of bears is reached
            while (bears.Count < GameConstants.MAX_BEARS)
            {
                SpawnBear();
            }

            // set initial health and score strings
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            // get current mouse state and update burger
            MouseState mouse = Mouse.GetState();
            burger.Update(gameTime, mouse);

            // update other game objects
            foreach (TeddyBear bear in bears)
            {
                bear.Update(gameTime);
            }
            foreach (Projectile projectile in projectiles)
            {
                projectile.Update(gameTime);
            }
            foreach (Explosion explosion in explosions)
            {
                explosion.Update(gameTime);
            }

            // check and resolve collisions between teddy bears
            for (int i = 0; i < bears.Count; i++)
            {
                for (int j = i + 1; j < bears.Count; j++)
                {
                    //two bears are active and they collide
                    if (bears[i].CollisionRectangle.Intersects(bears[j].CollisionRectangle) && bears[i].Active && bears[j].Active)
                    {
                        //use the collision utils to get the info
                        CollisionResolutionInfo collisionInfoItem = CollisionUtils.CheckCollision(gameTime.ElapsedGameTime.Milliseconds, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, bears[i].Velocity, bears[i].DrawRectangle, bears[j].Velocity, bears[j].DrawRectangle);

                        //there was a collision to resolve
                        if (collisionInfoItem != null)
                        {
                            //collision put a bear outside the window, deactivate it
                            if (collisionInfoItem.FirstOutOfBounds)
                            {
                                bears[i].Active = false;
                            }
                            else
                            {
                                //change the draw rectangle and direction of the bear according to the util function
                                bears[i].Velocity = collisionInfoItem.FirstVelocity;
                                bears[i].DrawRectangle = collisionInfoItem.FirstDrawRectangle;
                            }

                            //collision put a bear outside the window, deactivate it
                            if (collisionInfoItem.SecondOutOfBounds)
                            {
                                bears[j].Active = false;
                            }
                            else
                            {
                                //change the draw rectangle and direction of the bear according to the util function
                                bears[j].Velocity = collisionInfoItem.SecondVelocity;
                                bears[j].DrawRectangle = collisionInfoItem.SecondDrawRectangle;
                            }
                        }
                    }
                }
            }

                // check and resolve collisions between burger and teddy bears
            foreach (TeddyBear bear in bears)
            {
                if (bear.CollisionRectangle.Intersects(burger.CollisionRectangle) && bear.Active)
                {
                    burger.Health -= GameConstants.BEAR_DAMAGE;
                    bear.Active = false;
                    explosions.Add(new Explosion(explosionSpriteStrip, bear.Location.X, bear.Location.Y));
                }
            }

                // check and resolve collisions between burger and projectiles
            foreach (Projectile projectile in projectiles)
            {
                if (projectile.Type == ProjectileType.TeddyBear && projectile.Active && projectile.CollisionRectangle.Intersects(burger.CollisionRectangle))
                {
                    burger.Health -= GameConstants.TEDDY_BEAR_PROJECTILE_DAMAGE;
                    projectile.Active = false;
                }
            }

                // check and resolve collisions between teddy bears and projectiles
                foreach (TeddyBear bear in bears)
                {
                    foreach (Projectile projectile in projectiles)
                    {
                        //active french fry projectile hits an active bear actor
                        if (projectile.Type == ProjectileType.FrenchFries && bear.CollisionRectangle.Intersects(projectile.CollisionRectangle) && projectile.Active && bear.Active)
                        {
                            bear.Active = false;
                            projectile.Active = false;
                            //play the explosion at the bear's location
                            explosions.Add(new Explosion(explosionSpriteStrip, bear.Location.X, bear.Location.Y));
                        }
                    }
                }

            // clean out inactive teddy bears and add new ones as necessary
            for (int i = bears.Count - 1; i >= 0; i--)
            {
                if (!bears[i].Active)
                {
                    bears.RemoveAt(i);
                }
            }
            //later, we'll add bears up to the max again
            while (bears.Count < GameConstants.MAX_BEARS)
            {
                SpawnBear();
            }

            // clean out inactive projectiles
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                if (!projectiles[i].Active)
                {
                    projectiles.RemoveAt(i);
                }
            }

            // clean out finished explosions
            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                if (explosions[i].Finished)
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

            spriteBatch.Begin();

            // draw game objects
            burger.Draw(spriteBatch);
            foreach (TeddyBear bear in bears)
            {
                bear.Draw(spriteBatch);
            }
            foreach (Projectile projectile in projectiles)
            {
                projectile.Draw(spriteBatch);
            }
            foreach (Explosion explosion in explosions)
            {
                explosion.Draw(spriteBatch);
            }

            // draw score and health

            spriteBatch.End();

            base.Draw(gameTime);
        }

        #region Public methods

        /// <summary>
        /// Gets the projectile sprite for the given projectile type
        /// </summary>
        /// <param name="type">the projectile type</param>
        /// <returns>the projectile sprite for the type</returns>
        public static Texture2D GetProjectileSprite(ProjectileType type)
        {
            //return the matching sprite to the type, or nothing if the types don't match
            if (type == ProjectileType.FrenchFries)
            {
                return frenchFriesSprite;
            }
            else if (type == ProjectileType.TeddyBear)
            {
                return teddyBearProjectileSprite;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Adds the given projectile to the game
        /// </summary>
        /// <param name="projectile">the projectile to add</param>
        public static void AddProjectile(Projectile projectile)
        {
            //add a projectile to the list
            projectiles.Add(projectile);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Spawns a new teddy bear at a random location
        /// </summary>
        private void SpawnBear()
        {
            // generate random location
            int locationX = GetRandomLocation(GameConstants.SPAWN_BORDER_SIZE, graphics.PreferredBackBufferWidth - 2 * GameConstants.SPAWN_BORDER_SIZE);
            int locationY = GetRandomLocation(GameConstants.SPAWN_BORDER_SIZE, graphics.PreferredBackBufferHeight - 2 * GameConstants.SPAWN_BORDER_SIZE);

            // generate random velocity
            float speed = GameConstants.MIN_BEAR_SPEED + RandomNumberGenerator.NextFloat(GameConstants.BEAR_SPEED_RANGE);
            float angle = RandomNumberGenerator.NextFloat((float)(Math.PI * 2));
            Vector2 velocity = new Vector2(speed * (float)Math.Cos((double)angle), speed * (float)Math.Sin((double)angle));

            // create new bear
            TeddyBear newBear = new TeddyBear(Content, "teddybear", locationX, locationY, velocity, null, null);

            // make sure we don't spawn into a collision
            List<Rectangle> collisionRectangles = GetCollisionRectangles();

            while(!CollisionUtils.IsCollisionFree(newBear.DrawRectangle, collisionRectangles)) {
                newBear.X = GetRandomLocation(GameConstants.SPAWN_BORDER_SIZE, graphics.PreferredBackBufferWidth - 2 * GameConstants.SPAWN_BORDER_SIZE);
                newBear.Y = GetRandomLocation(GameConstants.SPAWN_BORDER_SIZE, graphics.PreferredBackBufferHeight - 2 * GameConstants.SPAWN_BORDER_SIZE);
            }

            // add new bear to list
            bears.Add(newBear);

        }

        /// <summary>
        /// Gets a random location using the given min and range
        /// </summary>
        /// <param name="min">the minimum</param>
        /// <param name="range">the range</param>
        /// <returns>the random location</returns>
        private int GetRandomLocation(int min, int range)
        {
            return min + RandomNumberGenerator.Next(range);
        }

        /// <summary>
        /// Gets a list of collision rectangles for all the objects in the game world
        /// </summary>
        /// <returns>the list of collision rectangles</returns>
        private List<Rectangle> GetCollisionRectangles()
        {
            List<Rectangle> collisionRectangles = new List<Rectangle>();
            collisionRectangles.Add(burger.CollisionRectangle);
            foreach (TeddyBear bear in bears)
            {
                collisionRectangles.Add(bear.CollisionRectangle);
            }
            foreach (Projectile projectile in projectiles)
            {
                collisionRectangles.Add(projectile.CollisionRectangle);
            }
            foreach (Explosion explosion in explosions)
            {
                collisionRectangles.Add(explosion.CollisionRectangle);
            }
            return collisionRectangles;
        }

        /// <summary>
        /// Checks to see if the burger has just been killed
        /// </summary>
        private void CheckBurgerKill()
        {

        }

        #endregion
    }
}
