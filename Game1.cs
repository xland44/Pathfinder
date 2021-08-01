using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D tileAtlas;
        //game properties
        public const int tileSize = 16;
        public const int windowWidth = 800;
        public const int windowHeight = 800;
        public const int gameMapRowCount = windowHeight / tileSize;
        public const int gameMapColumnCount = windowWidth / tileSize; // number of t
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
           
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 800;
            _graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            tileAtlas = Content.Load<Texture2D>("sprites/tileAtlas");
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            for (int x = 0; x < gameMapColumnCount; x++)
            {
                for (int y = 0; y < gameMapRowCount; y++)
                {
                    _spriteBatch.Draw(tileAtlas, new Rectangle(x * gameMapColumnCount, y * gameMapRowCount,40,40),new Rectangle(0,0,tileSize,tileSize),Color.White);
                }
            }
            base.Draw(gameTime);
            _spriteBatch.End();
        }

    }
}
