using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Snake
{
    class Snake
    {

        public List<Vector2> Locations { get; private set; } = new List<Vector2>();
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
                Locations.Add(new Vector2(j*SnakeGame.SNAKE_SIZE, SnakeGame.SNAKE_SIZE));
            }
        }

        public void Move(Direction direction)
        {
            // TODO: Improve this. Remove 1, add 1.

            List<Vector2> newLocations = new List<Vector2>();
            Vector2 newPieceLocation = Locations[Locations.Count - 1];

            for (int i = Locations.Count - 1; i > 0; i--)
            {
                Vector2 location = Locations[i - 1];
                newLocations.Add(location);
            }

            Vector2 newFirstVector;
            switch (direction)
            {
                case Direction.Up:
                    newFirstVector = new Vector2(Locations[0].X, Locations[0].Y - SnakeGame.SNAKE_SIZE);
                    break;
                case Direction.Down:
                    newFirstVector = new Vector2(Locations[0].X, Locations[0].Y + SnakeGame.SNAKE_SIZE);
                    break;
                case Direction.Left:
                    newFirstVector = new Vector2(Locations[0].X - SnakeGame.SNAKE_SIZE, Locations[0].Y);
                    break;
                case Direction.Right:
                    newFirstVector = new Vector2(Locations[0].X + SnakeGame.SNAKE_SIZE, Locations[0].Y);
                    break;
                default:
                    throw new ArgumentException("Direction should be valid!");
            }
            newLocations.Add(newFirstVector);
            newLocations.Reverse();

            if (_piecesToAdd > 0)
            {
                // Add one every time the snake moves.
                _piecesToAdd--;
                newLocations.Add(newPieceLocation);
            }

            Locations.Clear();
            foreach (Vector2 newLocation in newLocations)
            {
                Locations.Add(newLocation);
            }
        }

        public void AddPiece()
        {
            // AddPiece() just queues a piece to add for the next render/movement.
            _piecesToAdd++;
        }

    }
}
