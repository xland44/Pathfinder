using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Collections.Generic;

namespace SpaceGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D tileAtlas;
        private int[,] map = new int[50, 50];
        private MouseState _mouseState = Mouse.GetState();
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
            this.IsMouseVisible = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            tileAtlas = Content.Load<Texture2D>("sprites/tileAtlas");
            Debug.WriteLine("Game is loaded!");
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            _mouseState = Mouse.GetState(); // update mouse state
            
            //if right button pressed, "erase" tile (edit map array at specified location to be tileID "0")
            if (_mouseState.MiddleButton == ButtonState.Pressed && _mouseState.X<windowWidth && _mouseState.X>0 && _mouseState.Y < windowWidth && _mouseState.Y > 0)
            {
                int rowNo = _mouseState.Y / tileSize;
                int columnNo = _mouseState.X / tileSize;
                //below: solves array out of bounds exception when mouse clicks on the border pixels (800X or 800Y), as 800/16 is 50, but array is 0-index 50
                    map[rowNo, columnNo] = 0;
           }
            //if left button pressed, "draw" tile (edit map array at specified location to be tileID "1")
            if (_mouseState.LeftButton == ButtonState.Pressed && _mouseState.X < windowWidth && _mouseState.X > 0 && _mouseState.Y < windowWidth && _mouseState.Y > 0)
            {
                int rowNo = _mouseState.Y / tileSize;
                int columnNo = _mouseState.X / tileSize;
                map[rowNo, columnNo] = 1;
            }
            //if left button pressed, "mark" tile (edit map array at specified location to be tileID "2")
            if (_mouseState.RightButton == ButtonState.Pressed && _mouseState.X < windowWidth && _mouseState.X > 0 && _mouseState.Y < windowWidth && _mouseState.Y > 0)
            {
                int rowNo = _mouseState.Y / tileSize;
                int columnNo = _mouseState.X / tileSize;
                map[rowNo, columnNo] = 2;
                BreadthSearch(new Point(rowNo, columnNo), new Point(25,25));
            }

            base.Update(gameTime);
        }

        //gets a tile id (from the map array), and returns the relevant dimensions from the sprite atlas (e.g "2" returns a rectangle containing the third tile in the atlas)
        protected Rectangle GetDrawRectFromTileID(int i)
        {
            int totalColumns = tileAtlas.Width / tileSize; //total columns in the sprite atlas.
            int currentRow = (int)((float)i / (float)totalColumns);
            int currentColumn = i % totalColumns;
            return new Rectangle(currentColumn*tileSize, currentRow * tileSize, tileSize, tileSize);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            // TODO: Add your drawing code here
            _spriteBatch.Begin();

            for (int columnNo = 0; columnNo < gameMapColumnCount; columnNo++)
            {
                for (int rowNo = 0; rowNo < gameMapRowCount; rowNo++)
                {
                    _spriteBatch.Draw(tileAtlas, new Rectangle(columnNo * tileSize, rowNo*tileSize, tileSize, tileSize), GetDrawRectFromTileID(map[rowNo,columnNo]), Color.White);
                }
            }
            base.Draw(gameTime);
            _spriteBatch.End();
        }
        protected enum _direction { Up, Right, Down, Left }
        protected Point? GetNeighbor(_direction dir, Point nexus) // finds all adjacent squares to
        {
            //important to note: Point Nexus is a pairing of two numbers that represent a [row,column] that translate to
            //map coordinates. so in reality nexus.X represents the map Y coordinates (row#), whilst nexus.Y represents the map X coordinates (column#)!!
            if (dir == _direction.Up && nexus.X > 0)
                return new Point(nexus.X - 1, nexus.Y);
            else if (dir == _direction.Right && nexus.Y < gameMapColumnCount-1) //-1 because arrays are 0-index
                return new Point(nexus.X, nexus.Y + 1);
            else if (dir == _direction.Down && nexus.X < gameMapRowCount-1)
                return new Point(nexus.X + 1, nexus.Y);
            else if (dir == _direction.Left && nexus.Y > 0)
                return new Point(nexus.X, nexus.Y - 1);
            else return null;
        }
        protected Queue<Point> frontier = new Queue<Point>();
        protected Dictionary<Point, Point> came_from = new Dictionary<Point, Point>(); // dict keys are current tile, value is which tile it came from (https://www.redblobgames.com/pathfinding/a-star/introduction.html)
        protected Queue<Point> pathPoints = new Queue<Point>();


        protected void ClearPreviousPaths()
        {
            while(pathPoints.Count>0)
            {
                Point temp = pathPoints.Dequeue();
                if (map[temp.X, temp.Y] == 5)
                    map[temp.X, temp.Y] = 0;
            }
        }
        protected void TraceBack(Point start, Point end) // traces back a path from the endpoint to the startpoint using came_from dictionary
        {
            ClearPreviousPaths();
            pathPoints.Enqueue(new Point(end.X,end.Y));
            while (end != start)
            {
                end = came_from[end];
                map[end.X, end.Y] = 5;
                pathPoints.Enqueue(new Point(end.X, end.Y));
            }
            frontier.Clear();
            came_from.Clear();
        }
        protected void BreadthSearch(Point startlocation, Point EndLocation)
        {
            startlocation = new Point(startlocation.X, startlocation.Y);
            frontier.Enqueue(startlocation);
            while(frontier.Count>0)
            {
                Point current = frontier.Dequeue();
                for (int i = (int)_direction.Up; i <= (int)_direction.Left; i++)
                {
                    Point neighbor;
                    if (GetNeighbor((_direction)i, current) is null) // if null point (point is out of bounds of game) skip to next neighbor
                        continue;
                    else neighbor = (Point)GetNeighbor((_direction)i, current);
                    if (map[neighbor.X, neighbor.Y] == 1 || map[neighbor.X, neighbor.Y] == 2)
                        continue;

                    if(!(came_from.ContainsKey(neighbor)))
                    { 
                        came_from.Add(neighbor, current);
                        frontier.Enqueue(neighbor);
                    }

                        
                }
            }
            TraceBack(startlocation, EndLocation);
        }
    }
}
