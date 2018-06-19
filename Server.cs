using System.Net.Sockets;
using System.Net;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Conway;

namespace Conway
{
    public class Server {
        private bool _running;
        private GameBoard _lastBoard;

        public Server(int size, int numAlive)
        {
            _running = false;
            _lastBoard = Utils.FillBoard(size, numAlive);
        }

        public void Stop()
        {
            _running = false;
        }

        public GameBoard GetBoard()
        {
            if (!_running)
            {
                Task.Run(() => Run());
            }

            return _lastBoard;
        }

        public void Run()
        {
            GameBoard curBoard;

            _running = true;

            while (_running) // only break when killed...
            {
                // start of our tick
                curBoard = new GameBoard(_lastBoard);

                // check each of our cells
                for (int i = 0; i < curBoard.Size; i++)
                {
                    for (int j = 0; j < curBoard.Size; j++)
                    {
                        // Meat and potatoes of our game logic right here...
                        int neighbors = 0;
                        // RULES!

                        if (_lastBoard.Cells[i,j].Alive)
                        {
                            neighbors = _lastBoard.GetLiveNeighbors(i, j);
                            // Any live cell with fewer than two live neighbors dies
                            if (neighbors < 2)
                            {
                                curBoard.Cells[i,j].Alive = false;
                            }
                            else if (neighbors == 2 || neighbors == 3)
                            {
                                // Any live cell with two or three live neighbors lives
                                curBoard.Cells[i,j].Alive = true;
                            }
                            else if (neighbors > 3)
                            {
                                // Any live cell with more than three live neighbors dies
                                curBoard.Cells[i,j].Alive = false;
                            }
                        }
                        else
                        {
                            neighbors = _lastBoard.GetLiveNeighbors(i, j);

                            // Any dead cell with exactly three live neighbors becomes a live cell
                            if (neighbors == 3)
                            {
                                curBoard.Cells[i,j].Alive = true;
                            }
                        }
                    }
                }

                // send the results of our tick to the client (curBoard)

                _lastBoard = new GameBoard(curBoard);
                // end of our tick
            }
        }
    }
}