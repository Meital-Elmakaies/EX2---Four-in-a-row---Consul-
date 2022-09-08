namespace C21_Ex02
{
    using System;
    using System.Text;

    public class ApplicationUI
    {
        private readonly Game r_CurrentGame;

        public ApplicationUI()
        {
            r_CurrentGame = new Game();
        }

        public void BeginGame()
        {
            GetDimensionsFromUser();
            ChoosePlayMode();
            BuildBoard();

            while (!r_CurrentGame.EndGame)
            {
                AskUserToInsertCoin();
                if (r_CurrentGame.ExtraGame)
                {
                    PrintSummaryAndScores();
                    AskUserForExtraGame();
                }
            }
        }

        public void GetDimensionsFromUser()
        {
            bool goodInput = false;
            bool validSize = false;
            int rows = 0, cols = 0;

            while (!goodInput || !validSize)
            {
                // ask the user for number of rows
                Console.WriteLine("Enter number of rows for the board: ");
                string rowStr = Console.ReadLine();

                // ask the user for number of cols
                Console.WriteLine("Enter number of columns for the board: ");
                string colStr = Console.ReadLine();

                goodInput = int.TryParse(rowStr, out rows);
                goodInput = int.TryParse(colStr, out cols);

                if (!goodInput)
                {
                    Console.WriteLine("Invalid input ... Try again");
                }
                else
                {
                    validSize = r_CurrentGame.DimensionsValidation(rows, cols);
                    if (!validSize)
                    {
                        Console.WriteLine("Invalid size ... Choose size between 4-8 ");
                    }
                }
            }

            r_CurrentGame.InitGameBoard(rows, cols);
        }

        public void ChoosePlayMode()
        {
            Game.eGameMode gameMode;
            Console.WriteLine("Play With another player --> y  |  Play with Computer --> n");
            char input = GetAnswerFromUser();

            if (input.Equals('y'))
            {
                gameMode = Game.eGameMode.TwoPlayers;
            }
            else
            {
                gameMode = Game.eGameMode.Computer;
            }

            r_CurrentGame.Mode = gameMode;
            r_CurrentGame.SetPlayersTypes();
        }

        public char GetAnswerFromUser()
        {
            bool validAns = false;
            char answer = ' ';

            while (!validAns)
            {
                bool goodInput = char.TryParse(Console.ReadLine(), out answer);
                if (goodInput)
                {
                    if (answer.Equals('y') || answer.Equals('n'))
                    {
                        validAns = true;
                    }
                    else
                    {
                        Console.WriteLine("Invalid Input ... enter only (y/n)");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Input ... enter only (y/n)");
                }
            }

            return answer;
        }

        public void BuildBoard()
        {
            Ex02.ConsoleUtils.Screen.Clear();
            StringBuilder gameBoard = new StringBuilder();
            int numOfColsInBoard = r_CurrentGame.GetBoardNumCols();
            int numOfRowsInBoard = r_CurrentGame.GetBoardNumRows();

            // building the game board matrix 
            for (int colIndex = 1; colIndex <= numOfColsInBoard; colIndex++)
            {
                gameBoard.Append(string.Format("  {0}  ", colIndex));
            }

            gameBoard.Append("\n");

            for (int i = 0; i < numOfRowsInBoard; i++)
            {
                for (int j = 0; j < numOfColsInBoard; j++)
                {
                    Board.eMatrixCell value = r_CurrentGame.GetCellContent(i, j);

                    switch (value)
                    {
                        case Board.eMatrixCell.FirstPlayer:
                            gameBoard.Append("| X  ");
                            break;
                        case Board.eMatrixCell.SecondPlayer:
                            gameBoard.Append("| O  ");
                            break;
                        default:
                            gameBoard.Append("|    ");
                            break;
                    }
                }

                gameBoard.Append("|\n");
                for (int k = 0; k < numOfColsInBoard; k++)
                {
                    gameBoard.Append("=====");
                }

                gameBoard.Append("=\n");
            }

            Console.WriteLine(gameBoard);
        }

        public void ReadColumnToInsert()
        {
            Player.ePlayerType playerType = r_CurrentGame.GetCurrentPlayerType();
            bool colAvailable = false;
            int columnNumber = 0;

            while (!r_CurrentGame.ExtraGame && !colAvailable)
            {
                // if the player type is human ask him for column
                if (Player.ePlayerType.Computer != playerType)
                {
                    columnNumber = GetColNumberFromUser();
                    columnNumber--;
                }

                if (!r_CurrentGame.ExtraGame)
                {
                    colAvailable = r_CurrentGame.TryToInsertCoin(columnNumber);
                    if (!colAvailable)
                    {
                        Console.WriteLine("This column is full... insert to another column");
                    }
                }
            }
        }

        public void AskUserForExtraGame()
        {
            Console.WriteLine("Do You Want To Play An Extra Game? (y/n)");
            char input = GetAnswerFromUser();

            if (input.Equals('y'))
            {
                r_CurrentGame.BeginExtraGame();
                BuildBoard();
            }
            else
            {
                r_CurrentGame.EndGame = true;
            }
        }

        public void AskUserToInsertCoin()
        {
            // Get the name of the player 
            Board.eMatrixCell currentPlayerName = r_CurrentGame.GetCurrentPlayerName();

            if (currentPlayerName.Equals(Board.eMatrixCell.FirstPlayer))
            {
                Console.WriteLine("Player 1, enter a column to insert the coin");
            }
            else if (currentPlayerName.Equals(Board.eMatrixCell.SecondPlayer))
            {
                Console.WriteLine("Player 2, enter a column to insert the coin");
            }

            ReadColumnToInsert();
            BuildBoard();
        }

        public void PrintSummaryAndScores()
        {
            int[] playersScores = r_CurrentGame.GetScores();
            string winnerOfTheGameMsg = " ";

            if (r_CurrentGame.Winner == null)
            {
                if (r_CurrentGame.IsGameBoardFull())
                {
                    winnerOfTheGameMsg = "Draw - No Winner";
                }
            }
            else
            {
                if (r_CurrentGame.Winner == Board.eMatrixCell.FirstPlayer)
                {
                    winnerOfTheGameMsg = "The Winner is: Player 1";
                }

                if (r_CurrentGame.Winner == Board.eMatrixCell.SecondPlayer)
                {
                    winnerOfTheGameMsg = "The Winner is: Player 2";
                }
            }

            Console.WriteLine(winnerOfTheGameMsg);
            string showMsg = string.Format(
                "Results:\nPlayer1\t\tPlayer2\n======================\n {0} \t\t " + "{1}", 
                playersScores[0].ToString(),
                playersScores[1].ToString());
            Console.WriteLine(showMsg);
        }

        public int GetColNumberFromUser()
        {
            string colStr;
            bool validAns = false;
            int colNumber = 0;

            while (!validAns)
            {
                // read input from user 
                colStr = Console.ReadLine();

                // try to parse it to int
                bool goodInput = int.TryParse(colStr, out colNumber);
                if (goodInput)
                {
                    // if it is valid integer and in the right range  
                    if (r_CurrentGame.ColumnNumberValidation(colNumber))
                    {
                        validAns = true;
                    }
                    else
                    {
                        Console.WriteLine("Invalid Input ... must be in range of columns");
                    }
                }
                else
                {
                    // check if it's the quit kew - 'Q'
                    if (char.TryParse(colStr, out char answer))
                    {
                        if (answer.Equals('Q'))
                        {
                            validAns = true;
                            r_CurrentGame.SendQuitMsg();
                        }
                        else
                        {
                            Console.WriteLine("Invalid Input ... enter only integer number");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid Input ... enter only integer number");
                    }
                }
            }

            return colNumber;
        }
    }
}