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

        public int GetLiveNeighbors(int x, int y)
        {
            int count = 0;

            // Start up
            if (Cells[x, y - 1].Alive)
            {
                count++;
            }

            // next, up one right one
            if (Cells[x + 1, y -1].Alive)
            {
                count++;
            }

            // next, right one
            if (Cells[x + 1, y].Alive)
            {
                count++;
            }

            // next, down one, right one
            if (Cells[x + 1, y - 1].Alive)
            {
                count++;
            }

            // next, down one
            if (Cells[x, y - 1].Alive)
            {
                count++;
            }

            // next, left one, down one
            if (Cells[x - 1, y - 1].Alive)
            {
                count++;
            }

            // next, left one
            if (Cells[x - 1, y].Alive)
            {
                count++;
            }

            // finally, left one, up one
            if (Cells[x - 1, y + 1].Alive)
            {
                count++;
            }

            return count;
        }
    }
}