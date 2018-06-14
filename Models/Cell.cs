using System.Drawing;

namespace Conway
{
    public class Cell
    {
        public Color Color { get; set; }
        public bool Alive { get; set; }

        public Cell()
        {
            Color = Color.Gray;
            Alive = false;
        }

        // copy constructor
        public Cell(Cell oldCell)
        {
            Color = oldCell.Color;
            Alive = oldCell.Alive;
        }
    }
}