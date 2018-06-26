using System.Drawing;

namespace Conway
{
    public class Cell
    {
        private Color _color;
        public string Color
        { 
            get
            {
                return ColorTranslator.ToHtml(_color);
            }
            set
            {
                _color = ColorTranslator.FromHtml(value);
            }
        }
        public bool Alive { get; set; }

        public Cell()
        {
            _color = System.Drawing.Color.Gray;
            Alive = false;
        }

        // copy constructor
        public Cell(Cell oldCell)
        {
            if (oldCell == null)
                oldCell = new Cell();

            Color = oldCell.Color;
            Alive = oldCell.Alive;
        }
    }
}