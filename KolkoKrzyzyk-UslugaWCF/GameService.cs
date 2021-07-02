using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace WCFGame
{
    public class GameService : IGameService
    {
        const int R = 3;
        const int C = 3;
        static int[,] BoardArray = new int[R, C];
        static List<int> MoveList = new List<int>();
        const int O_Mark = 1;
        const int X_Mark = 2;
        readonly Tuple<int, int>[][] coords  =
        {
            //rows
            new[] { new Tuple<int, int> (0, 0), new Tuple<int, int> (0, 1), new Tuple<int, int>  (0, 2) }, // 0. w0 
            new[] { new Tuple<int, int> (1, 0), new Tuple<int, int> (1, 1), new Tuple<int, int>  (1, 2) }, // 1. w1   
            new[] { new Tuple<int, int> (2, 0), new Tuple<int, int> (2, 1), new Tuple<int, int>  (2, 2) }, // 2. w2
            //columns
            new[] { new Tuple<int, int> (0, 0), new Tuple<int, int> (1, 0), new Tuple<int, int>  (2, 0) }, // 3. k0
            new[] { new Tuple<int, int> (0, 1), new Tuple<int, int> (1, 1), new Tuple<int, int>  (2, 1) }, // 4. k1   
            new[] { new Tuple<int, int> (0, 2), new Tuple<int, int> (1, 2), new Tuple<int, int>  (2, 2) }, // 5. k2
            //diagonals                   
            new[] { new Tuple<int, int> (0, 0), new Tuple<int, int> (1, 1), new Tuple<int, int>  (2, 2) }, // 6. s0   
            new[] { new Tuple<int, int> (0, 2), new Tuple<int, int> (1, 1), new Tuple<int, int>  (2, 0) }  // 7. s1
        };

        public GameService()
        {
        }

        public void Start()
        {
            BoardArray = new int[R, C];   

            MoveList = new List<int>();
            for (var i=0; i<9; i++)
            {
                MoveList.Add(i);
            }
        }

        public bool Move(int row, int col, out int serverRow, out int serverCol)
        {
            serverRow = -1; serverCol = -1;

            // user's move
            if (BoardArray[row, col] == 0) BoardArray[row, col] = O_Mark;
            else return false;
            MoveList.Remove(row * C + col);

            // check draw
            if (MoveList.Count == 0) return true;

            
            // server's move
            // the decision is taken with the steps below
            // 1. If exists, take a chance to win
            // 2. If you're in danger, defense yourself (in case of two O's in a line)
            // 3. Otherwise, take the best line (most X'es, least O's in it)
            var lines = new[]
            {
                //rows
                new[] {BoardArray[0, 0], BoardArray[0, 1], BoardArray[0, 2]}, // 0. r0
                new[] {BoardArray[1, 0], BoardArray[1, 1], BoardArray[1, 2]}, // 1. r1   
                new[] {BoardArray[2, 0], BoardArray[2, 1], BoardArray[2, 2]}, // 2. r2
                //columns
                new[] {BoardArray[0, 0], BoardArray[1, 0], BoardArray[2, 0]}, // 3. c0
                new[] {BoardArray[0, 1], BoardArray[1, 1], BoardArray[2, 1]}, // 4. c1   
                new[] {BoardArray[0, 2], BoardArray[1, 2], BoardArray[2, 2]}, // 5. c2
                //diagonals
                new[] {BoardArray[0, 0], BoardArray[1, 1], BoardArray[2, 2]}, // 6. d0   
                new[] {BoardArray[0, 2], BoardArray[1, 1], BoardArray[2, 0]}  // 7. d1
            };
            

            var lwin = -1; // winning line
            var llos = -1; // danger line
            var maxX = -1; // max No of X'es 
            var lmax = -1; // line with best possible move

            var ind = 0;
            foreach (var l in lines) //Finding winning, losing and best line
            {
                var o = 0; // count O's in the line
                var x = 0; // count X'es
                var p = 0; // count empty cells
                foreach (var mark in l)
                {
                    switch (mark)
                    {
                        case O_Mark:
                            o++;
                            break;
                        case X_Mark:
                            x++;
                            break;
                        default:
                            p++;
                            break;
                    }
                }

                if (o == 2 && p==1) llos = ind;
                if (x == 2 && p==1) lwin = ind;
                if (x > maxX && p>0)
                {
                    maxX = x;
                    lmax = ind;
                } 
                ind++;
            }

            
            int line;
            // deciding on the best line
            if (lwin != -1)
            {
                line = lwin; //winning
            } else if (llos != -1)
            {
                line = llos; //line in danger
            }
            else
            {
                line = lmax; //the best line to play
            }

            var finalLineCoords = coords[line];
            Debug.WriteLine(line);
            foreach (var coord in finalLineCoords)
            {
                if(BoardArray[coord.Item1, coord.Item2] == 0)
                {
                    serverRow = coord.Item1;
                    serverCol = coord.Item2;
                    break;
                }
            }
            
            /* END OF WINNING ALGORITHM */

            BoardArray[serverRow, serverCol] = X_Mark;
            MoveList.Remove(serverRow * C + serverCol);
            return true;
        }

        public int CheckWin(int row, int column)
        {
            int mark = BoardArray[row, column];
            if (
               BoardArray[0, column] == mark && BoardArray[1, column] == mark && BoardArray[2, column] == mark // horizontally
                ) return 1;

            
            if (
               BoardArray[row, 0] == mark && BoardArray[row, 1] == mark && BoardArray[row, 2] == mark // vertically
                ) return 2;
            

            if (
                BoardArray[0, 0]==mark && BoardArray[1, 1]==mark && BoardArray[2, 2]==mark    // diagonal \
                ) return 4;
            
            
            if (
                BoardArray[0, 2]==mark && BoardArray[1, 1]==mark && BoardArray[2, 0]==mark    // diagonal /
                ) return 8;

            return 0;
        }

    }
}
