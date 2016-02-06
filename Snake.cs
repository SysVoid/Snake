using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Snake
{
    class Snake
    {

        public List<Vector2> Locations { get; private set; } = new List<Vector2>();
        public Direction Direction { get; set; } = Direction.Down;
        private int _piecesToAdd = 0;

        public Snake(int pieces)
        {
            if (pieces == 0) { pieces = 1; }

            for (int i = 0; i < pieces; i++)
            {
                int j = i + 1;
                Locations.Add(new Vector2(j*20, 20));
            }
        }

        public void Move(Direction direction)
        {
            int movementAmount = 20;
            List<Vector2> newLocations = new List<Vector2>();
            Vector2 newPieceLocation = Locations[Locations.Count - 1];

            for (int i = Locations.Count - 1; i > 0; i--)
            {
                Vector2 location = Locations[i - 1];
                newLocations.Add(location);
            }

            Vector2 newFirstVector;
            if (direction == Direction.Up)
            {
                newFirstVector = new Vector2(Locations[0].X, Locations[0].Y - movementAmount);
            } else if (direction == Direction.Down)
            {
                newFirstVector = new Vector2(Locations[0].X, Locations[0].Y + movementAmount);
            } else if (direction == Direction.Left)
            {
                newFirstVector = new Vector2(Locations[0].X - movementAmount, Locations[0].Y);
            } else if (direction == Direction.Right)
            {
                newFirstVector = new Vector2(Locations[0].X + movementAmount, Locations[0].Y);
            } else
            {
                return;
            }
            newLocations.Add(newFirstVector);
            newLocations.Reverse();

            if (Locations.Count > 0 && _piecesToAdd > 0)
            {
                // Add one every time the snake moves.
                _piecesToAdd--;
                newLocations.Add(newPieceLocation);
            }

            Locations = newLocations;
        }

        public void AddPiece()
        {
            _piecesToAdd++;
        }

    }
}
