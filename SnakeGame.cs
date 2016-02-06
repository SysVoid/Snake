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

        // Food
        private Color[] _virusColour = new Color[] { Color.Red };
        private Texture2D _virusSquare;

        // Slow down the movement.
        private double _lastMovement = 0;
        private double _gameDelay = 100;

        // The food!
        private List<Vector2> _food = new List<Vector2>();

        // The viruses!
        private List<Vector2> _viruses = new List<Vector2>();

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

            // Virus
            _virusSquare = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            _virusSquare.SetData(_virusColour);
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
                // Also do the same for viruses.
                if (!_food.Contains(foodVector) && !_viruses.Contains(foodVector))
                {
                    // Nope? Add it.
                    _food.Add(foodVector);
                }
            }

            // Add viruses if there aren't any.
            if (_viruses.Count == 0)
            {
                MoveViruses();
            }

            Window.Title = "Snake [Score: " + (_snake.Locations.Count - 1) + "]";

            base.Update(gameTime);
        }

        private void MoveViruses()
        {
            // Make randomness!
            Random random = new Random();
            Vector2 virusVector1 = new Vector2(random.Next(1, MAX_X) * SNAKE_SIZE, random.Next(1, MAX_Y) * SNAKE_SIZE);
            Vector2 virusVector2 = new Vector2(virusVector1.X + SNAKE_SIZE, virusVector1.Y);
            Vector2 virusVector3 = new Vector2(virusVector1.X + SNAKE_SIZE, virusVector1.Y + SNAKE_SIZE);
            Vector2 virusVector4 = new Vector2(virusVector1.X, virusVector1.Y + SNAKE_SIZE);

            // Check if a piece of food exists at one of those locations already.
            if (!_food.Contains(virusVector1) && !_food.Contains(virusVector2) && !_food.Contains(virusVector3) && !_food.Contains(virusVector4))
            {
                _viruses.Clear();
                _viruses.Add(virusVector1);
                _viruses.Add(virusVector2);
                _viruses.Add(virusVector3);
                _viruses.Add(virusVector4);
            }
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

        public bool EatVirus()
        {
            foreach (Vector2 virus in _viruses)
            {
                if (virus.Equals(_snake.Locations[0]))
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

            _spriteBatch.Begin();
            // Only let the snake move every _gameDelay amount of ms.
            if (time - _lastMovement >= _gameDelay)
            {
                // Good, move.
                _snake.Move(_snake.Direction);

                // Set the new movement time.
                _lastMovement = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;

                // Only do food things and stuff when they move.
                // Can the snake eat? Is its head on food?
                if (EatFood())
                {
                    // Yep, remove the food and add a piece to the tail!
                    _food.Remove(_snake.Locations[0]);
                    _snake.AddPiece();

                    // Move the virus if they are divisible by 5.
                    if (_snake.Locations.Count % 5 == 0)
                    {
                        MoveViruses();
                    }
                }

                // Has the snake run into a virus?
                if (EatVirus())
                {
                    // Yep, darn. Let's remove a piece.
                    if (_snake.Locations.Count > 1)
                    {
                        _snake.Locations.RemoveAt(_snake.Locations.Count - 1);
                    }

                    // Move the virus.
                    MoveViruses();
                }
            }

            // Render all food.
            foreach (Vector2 location in _food)
            {
                _spriteBatch.Draw(_foodSquare, new Rectangle((int) location.X, (int) location.Y, SNAKE_SIZE, SNAKE_SIZE), _foodColor[0]);
            }

            // Render all viruses.
            foreach (Vector2 location in _viruses)
            {
                _spriteBatch.Draw(_virusSquare, new Rectangle((int) location.X, (int) location.Y, SNAKE_SIZE, SNAKE_SIZE), _virusColour[0]);
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
            _spriteBatch.End();

            // Is it dead?
            CheckForDeath();

            base.Draw(gameTime);
        }

    }
}
