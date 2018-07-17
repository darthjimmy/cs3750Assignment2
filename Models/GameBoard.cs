using System.Collections.Generic;
using System.Drawing;
using Newtonsoft.Json;

namespace Conway
{
    public class GameBoard
    {
        [JsonIgnore]
        public int Size { get; set; }
        public Cell[,] Cells { get; set; }

        public GameBoard(int size)
        {
            Size = size;

            // create an empty array
            Cells = new Cell[Size, Size];

            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    Cells[i, j] = new Cell();
                }
            }
        }

        // copy constructor
        public GameBoard(GameBoard oldBoard)
        {
            Size = oldBoard.Size;
            Cells = new Cell[Size, Size];
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    Cells[i, j] = new Cell(oldBoard.Cells[i, j]);
                }
            }
        }

        public int GetLiveNeighbors(int x, int y, out List<Color> colors)
        {
            int count = 0;

            colors = new List<Color>();

            // Start up
            if (Cells[x, y - 1].Alive)
            {
                count++;
                colors.Add(Color.FromName(Cells[x, y - 1].Color));
            }

            // next, up one right one
            if (Cells[x + 1, y - 1].Alive)
            {
                count++;
                colors.Add(Color.FromName(Cells[x + 1, y - 1].Color));
            }

            // next, right one
            if (Cells[x + 1, y].Alive)
            {
                count++;
                colors.Add(Color.FromName(Cells[x + 1, y].Color));
            }

            // next, down one, right one
            if (Cells[x + 1, y + 1].Alive)
            {
                count++;
                colors.Add(Color.FromName(Cells[x + 1, y + 1].Color));
            }

            // next, down one
            if (Cells[x, y + 1].Alive)
            {
                count++;
                colors.Add(Color.FromName(Cells[x, y + 1].Color));
            }

            // next, left one, down one
            if (Cells[x - 1, y + 1].Alive)
            {
                count++;
                colors.Add(Color.FromName(Cells[x - 1, y + 1].Color));
            }

            // next, left one
            if (Cells[x - 1, y].Alive)
            {
                count++;
                colors.Add(Color.FromName(Cells[x - 1, y].Color));
            }

            // finally, left one, up one
            if (Cells[x - 1, y - 1].Alive)
            {
                count++;
                colors.Add(Color.FromName(Cells[x - 1, y - 1].Color));
            }

            return count;
        }
    }
}