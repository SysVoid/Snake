using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Snake
{
    public class SnakeGame : Game
    {

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Snake _snake;

        private Color[] _snakeColor = new Color[] { Color.LimeGreen };
        private Texture2D _snakeSquare;

        private Color[] _snakeHeadColor = new Color[] { Color.Orange };
        private Texture2D _snakeHeadSquare;

        private Color[] _foodColor = new Color[] { Color.Yellow };
        private Texture2D _foodSquare;

        private double _lastMovement = 0;
        private double _gameDelay = 100;

        private List<Vector2> _food = new List<Vector2>();

        public SnakeGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            _snake = new Snake(1);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Snake head
            _snakeHeadSquare = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            _snakeHeadSquare.SetData(_snakeHeadColor);

            // Snake pieces
            _snakeSquare = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            _snakeSquare.SetData(_snakeColor);

            // Food
            _foodSquare = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            _foodSquare.SetData(_foodColor);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            } else if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                _snake.Direction = Direction.Up;
            } else if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                _snake.Direction = Direction.Down;
            } else if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                _snake.Direction = Direction.Left;
            } else if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                _snake.Direction = Direction.Right;
            }

            if (_food.Count < 5)
            {
                Random random = new Random(DateTime.Now.Millisecond);
                Vector2 foodVector = new Vector2(random.Next(1, 38)*20, random.Next(1, 22)*20);

                if (!_food.Contains(foodVector))
                {
                    _food.Add(foodVector);
                }
            }

            Window.Title = "Snake [Score: " + (_snake.Locations.Count - 1) + "]";

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            double time = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
            if (time - _lastMovement >= _gameDelay)
            {
                _snake.Move(_snake.Direction);
                _lastMovement = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
            }

            _spriteBatch.Begin();
            foreach (Vector2 location in _food)
            {
                int pieceX = (int) location.X;
                int pieceY = (int) location.Y;

                _spriteBatch.Draw(_foodSquare, new Rectangle(pieceX, pieceY, 20, 20), _foodColor[0]);
            }

            bool isFirst = true;
            foreach (Vector2 location in _snake.Locations)
            {
                int pieceX = (int) location.X;
                int pieceY = (int) location.Y;

                if (pieceX < 0 || pieceX > Window.ClientBounds.Width)
                {
                    Exit();
                } else if (pieceY < 0 || pieceY > Window.ClientBounds.Height)
                {
                    Exit();
                }

                Rectangle rectangle = new Rectangle(pieceX, pieceY, 20, 20);

                if (isFirst)
                {
                    _spriteBatch.Draw(_snakeHeadSquare, rectangle, _snakeHeadColor[0]);
                    
                    bool nom = false;
                    foreach (Vector2 food in _food)
                    {
                        if (food.Equals(location))
                        {
                            nom = true;
                        }
                    }

                    if (nom)
                    {
                        _food.Remove(location);
                        _snake.AddPiece();
                    }
                } else
                {
                    _spriteBatch.Draw(_snakeSquare, rectangle, _snakeColor[0]);
                }

                isFirst = false;
            }
            _spriteBatch.End();

            foreach (Vector2 location in _snake.Locations)
            {
                int piecesAtLocation = 0;
                foreach (Vector2 location2 in _snake.Locations)
                {
                    if (location.Equals(location2))
                    {
                        piecesAtLocation++;
                    }

                    if (piecesAtLocation >= 2)
                    {
                        Exit();
                    }
                }
            }

            base.Draw(gameTime);
        }

    }
}
