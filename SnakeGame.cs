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

        // The snake
        private Snake _snake;

        // Snake's tail
        private Color[] _snakeColor = new Color[] { Color.LimeGreen };
        private Texture2D _snakeSquare;

        // Snake's head
        private Color[] _snakeHeadColor = new Color[] { Color.Orange };
        private Texture2D _snakeHeadSquare;

        // Food
        private Color[] _foodColor = new Color[] { Color.Yellow };
        private Texture2D _foodSquare;

        // Slow down the movement.
        private double _lastMovement = 0;
        private double _gameDelay = 100;

        // The food!
        private List<Vector2> _food = new List<Vector2>();

        // The size of the snake
        public const int SNAKE_SIZE = 20;

        // The maximum amount of times I can safely *SNAKE_SIZE before it goes off map. [X]
        public const int MAX_X = 38;

        // The maximum amount of times I can safely *SNAKE_SIZE before it goes off map. [Y]
        public const int MAX_Y = 22;

        public SnakeGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // Create the snake with 1 piece.
            // If you're modifying the game, you can set this to anything over 0.
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
            // Get the keyboard's current state.
            KeyboardState keyboard = Keyboard.GetState();

            // Handle key pressed. Esc = exit, up/down/left/right = move.
            if (keyboard.IsKeyDown(Keys.Escape))
            {
                Exit();
            } else if (keyboard.IsKeyDown(Keys.Up))
            {
                _snake.Direction = Direction.Up;
            } else if (keyboard.IsKeyDown(Keys.Down))
            {
                _snake.Direction = Direction.Down;
            } else if (keyboard.IsKeyDown(Keys.Left))
            {
                _snake.Direction = Direction.Left;
            } else if (keyboard.IsKeyDown(Keys.Right))
            {
                _snake.Direction = Direction.Right;
            }

            // Add new food if there aren't already 5.
            if (_food.Count < 5)
            {
                // Make randomness!
                Random random = new Random();
                Vector2 foodVector = new Vector2(random.Next(1, MAX_X) * SNAKE_SIZE, random.Next(1, MAX_Y) * SNAKE_SIZE);

                // Check if that piece of food exists already.
                if (!_food.Contains(foodVector))
                {
                    // Nope? Add it.
                    _food.Add(foodVector);
                }
            }

            Window.Title = "Snake [Score: " + (_snake.Locations.Count - 1) + "]";

            base.Update(gameTime);
        }

        private void CheckForDeath()
        {
            foreach (Vector2 location in _snake.Locations)
            {
                int piecesAtLocation = 0;
                foreach (Vector2 location2 in _snake.Locations)
                {
                    if (location.Equals(location2))
                    {
                        piecesAtLocation++;
                    }

                    // It's dead if 2 snake pieces are at the same place.
                    if (piecesAtLocation >= 2)
                    {
                        Exit();
                    }
                }
            }
        }

        public bool EatFood()
        {
            foreach (Vector2 food in _food)
            {
                if (food.Equals(_snake.Locations[0]))
                {
                    return true;
                }
            }
            return false;
        }

        protected override void Draw(GameTime gameTime)
        {
            // Reset the game's display.
            GraphicsDevice.Clear(Color.Black);
            
            // Get the current UNIX time in milliseconds.
            double time = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;

            // Only let the snake move every _gameDelay amount of ms.
            if (time - _lastMovement >= _gameDelay)
            {
                // Good, move.
                _snake.Move(_snake.Direction);

                // Set the new movement time.
                _lastMovement = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
            }

            _spriteBatch.Begin();
            // Render all food.
            foreach (Vector2 location in _food)
            {
                _spriteBatch.Draw(_foodSquare, new Rectangle((int) location.X, (int) location.Y, SNAKE_SIZE, SNAKE_SIZE), _foodColor[0]);
            }

            bool head = true;

            // Render each bit of the snake.
            foreach (Vector2 location in _snake.Locations)
            {
                int pieceX = (int) location.X;
                int pieceY = (int) location.Y;

                // Are we out of bounds? Exit.
                if (pieceX < 0 || pieceX > Window.ClientBounds.Width)
                {
                    Exit();
                } else if (pieceY < 0 || pieceY > Window.ClientBounds.Height)
                {
                    Exit();
                }

                // Prepare the rectangle.
                Rectangle rectangle = new Rectangle(pieceX, pieceY, SNAKE_SIZE, SNAKE_SIZE);

                if (head)
                {
                    // Draw the head.
                    // The head has slightly different properties.
                    _spriteBatch.Draw(_snakeHeadSquare, rectangle, _snakeHeadColor[0]);

                    // The next bit cannot be a head.
                    head = false;
                } else
                {
                    // Draw the tail.
                    _spriteBatch.Draw(_snakeSquare, rectangle, _snakeColor[0]);
                }
            }

            // Can the snake eat? Is its head on food?
            if (EatFood())
            {
                // Yep, remove the food and add a piece to the tail!
                _food.Remove(_snake.Locations[0]);
                _snake.AddPiece();
            }
            _spriteBatch.End();

            // Is it dead?
            CheckForDeath();

            base.Draw(gameTime);
        }

    }
}
