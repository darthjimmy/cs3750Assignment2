using System;
using System.Drawing;

namespace Conway
{
    public static class Utils
    {
        private static readonly Random rand = new Random();

        public static Color CombineColors(params Color[] colors)
        {
            byte r = new byte();
            byte g = new byte();
            byte b = new byte();

            foreach(Color color in colors)
            {
                r += color.R;
                g += color.G;
                b += color.B;
            }

            r /= (byte) colors.Length;
            g /= (byte) colors.Length;
            b /= (byte) colors.Length;

            return Color.FromArgb(r, g, b);
        }

        public static GameBoard FillBoard(int size, int numCells)
        {
            GameBoard board = new GameBoard(size);
            for (int i = 0; i < numCells; i++)
            {
                int x = rand.Next(1, board.Size - 2); // we want to skip the last cell in the row
                int y = rand.Next(1, board.Size - 2); // we want to skip the last cell in a column

                board.Cells[x, y].Alive = true;
            }
            return board;
        }
    }
}