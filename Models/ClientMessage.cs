
using System.Drawing;

namespace Conway
{
    public class Location
    {
        public int X { get; set;}
        public int Y { get; set; }
    }

    public class ClientMessage
    {
        public string Command { get; set; }
        
        public string Color { get; set; }

        public int X { get; set; }
        public int Y { get; set; }
    }
}