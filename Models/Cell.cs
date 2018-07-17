using System.Drawing;
using Newtonsoft.Json;

namespace Conway
{
    public class Cell
    {
        private Color _color;
        private bool _alive;
        
        public void SetColor(Color color)
        {
            _color = color;
        }

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

        [JsonIgnore]
        public bool Alive 
        { 
            get
            {
                return _alive;
            }
            set
            {
                _alive = value;
                if (!value)
                    _color = System.Drawing.Color.White;
            }
        }

        public Cell()
        {
            _color = System.Drawing.Color.White;
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