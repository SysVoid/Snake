using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Snake
{
    class Snake
    {

        public readonly List<Vector2> Locations = new List<Vector2>();
        public Direction Direction { get; set; } = Direction.Down;
        private int _piecesToAdd = 0;

        public Snake(int pieces)
        {
            if (pieces <= 0) { pieces = 1; }

            for (int i = 0; i < pieces; i++)
            {
                // +1 because I don't want snakes to start on the left edge of the map.
                // I want a "clearance" column.
                int j = i + 1;
                Locations.Add(new Vector2(j * SnakeGame.SNAKE_SIZE, SnakeGame.SNAKE_SIZE));
            }
        }

        public void Move(Direction direction)
        {
            Vector2 headLocation = Locations[0];
            Vector2 newPieceLocation = Locations[Locations.Count - 1];

            Vector2 newFirstVector;
            switch (direction)
            {
                case Direction.Up:
                    newFirstVector = new Vector2(headLocation.X, headLocation.Y - SnakeGame.SNAKE_SIZE);
                    break;
                case Direction.Down:
                    newFirstVector = new Vector2(headLocation.X, headLocation.Y + SnakeGame.SNAKE_SIZE);
                    break;
                case Direction.Left:
                    newFirstVector = new Vector2(headLocation.X - SnakeGame.SNAKE_SIZE, headLocation.Y);
                    break;
                case Direction.Right:
                    newFirstVector = new Vector2(headLocation.X + SnakeGame.SNAKE_SIZE, headLocation.Y);
                    break;
                default:
                    throw new ArgumentException("Direction should be valid!");
            }

            // Re-add head location
            Locations.Reverse();
            Locations.Add(newFirstVector);
            Locations.Reverse();

            if (_piecesToAdd == 0)
            {
                // Remove extra bit if there's nothing to add
                Locations.RemoveAt(Locations.Count - 1);
            } else
            {
                _piecesToAdd--;
            }
        }

        public void AddPiece()
        {
            // AddPiece() just queues a piece to add for the next render/movement.
            _piecesToAdd++;
        }

    }
}
