namespace C21_Ex02
{
    using System;

    public class Game
    {
        public const int k_MaxSize = 8;
        public const int k_MinSize = 4;
        private readonly Player[] r_PlayersArray;
        private int m_NumOfTurns;
        private bool m_IsEndGame;
        private bool m_ExtraGameCheck;
        private Board m_GameBoard;
        private Board.eMatrixCell? m_WinnerOfTheGame = null;
        private eGameMode? m_PlayMode = null;

        public Game()
        {
            r_PlayersArray = new Player[2] { new Player(), new Player() };
            r_PlayersArray[0].Type = Player.ePlayerType.Human;
            r_PlayersArray[0].Name = Board.eMatrixCell.FirstPlayer;
            r_PlayersArray[1].Name = Board.eMatrixCell.SecondPlayer;
        }

        public enum eGameMode
        {
            Computer,
            TwoPlayers
        }

        public eGameMode? Mode
        {
            get { return m_PlayMode; }
            set { m_PlayMode = value; }
        }

        public bool ExtraGame
        {
            get { return m_ExtraGameCheck; }
            set { m_ExtraGameCheck = value; }
        }

        public bool EndGame
        {
            get { return m_IsEndGame; }
            set { m_IsEndGame = value; }
        }

        public Board.eMatrixCell? Winner => m_WinnerOfTheGame;

        public int GetBoardNumRows()
        {
            return m_GameBoard.Rows;
        }

        public int GetBoardNumCols()
        {
            return m_GameBoard.Columns;
        }

        public void InitGameBoard(int i_Rows, int i_Cols)
        {
            m_GameBoard = new Board(i_Rows, i_Cols);
        }

        public void SendQuitMsg()
        {
            Board.eMatrixCell playerName = GetCurrentPlayerName();
            r_PlayersArray[((int)playerName) % 2].PlayerScore++;
            m_ExtraGameCheck = true;
        }

        public void BeginExtraGame()
        {
            m_GameBoard = new Board(m_GameBoard.Rows, m_GameBoard.Columns);
            m_IsEndGame = false;
            m_WinnerOfTheGame = null;
            m_NumOfTurns = 0;
            m_ExtraGameCheck = false;
        }

        public Board.eMatrixCell GetCurrentPlayerName()
        {
            return r_PlayersArray[m_NumOfTurns % 2].Name;
        }

        public Player.ePlayerType GetCurrentPlayerType()
        {
            return r_PlayersArray[m_NumOfTurns % 2].Type;
        }

        public bool DimensionsValidation(int i_Rows, int i_Cols)
        {
            if (!((i_Cols <= k_MaxSize && i_Cols >= k_MinSize) &&
                (i_Rows >= k_MinSize && i_Rows <= k_MaxSize)))
            {
                return false;
            }

            return true;
        }

        public bool ColumnNumberValidation(int i_columnNumber)
        {
            if (!((i_columnNumber <= m_GameBoard.Columns) && (i_columnNumber >= 1)))
            {
                return false;
            }

            return true;
        }

        public Board.eMatrixCell GetCellContent(int i_Row, int i_Col)
        {
            return m_GameBoard.GetCellValue(i_Row, i_Col);
        }

        public void SetPlayersTypes()
        {
            if (m_PlayMode == eGameMode.Computer)
            {
                r_PlayersArray[1].Type = Player.ePlayerType.Computer;
            }
            else
            {
                r_PlayersArray[1].Type = Player.ePlayerType.Human;
            }
        }

        public bool TryToInsertCoin(int i_ColumnNum)
        {
            bool colAvailable;
            Player.ePlayerType playerType = GetCurrentPlayerType();

            // if the player type is computer
            if (playerType == Player.ePlayerType.Computer)
            {
                // generate a number   
                Random num = new Random();
                i_ColumnNum = num.Next(0, m_GameBoard.Columns);
            }

            // check if the selected column is free 
            if (Board.eMatrixCell.Empty == m_GameBoard.GetCellValue(0, i_ColumnNum))
            {
                colAvailable = true;
            }
            else
            {
                colAvailable = false;
            }

            if (colAvailable)
            {
                // insert the coin into selected column
                InsertCoin(i_ColumnNum);
            }

            // return the status of the insertion (fail or succeed) 
            return colAvailable;
        }

        public void InsertCoin(int i_ColumnNum)
        {
            bool findWinner = false;

            // get the number of coins in the selected col
            int countOfCoinsInCol = m_GameBoard.GetCoinsCountInCol(i_ColumnNum);

            // get the available row index that we can insert the coin
            int RowIndex = m_GameBoard.Rows - countOfCoinsInCol - 1;
            Board.eMatrixCell playerName = GetCurrentPlayerName();

            // insert the coin at the available position
            m_GameBoard.SetCellValue(RowIndex, i_ColumnNum, playerName);
            Ex02.ConsoleUtils.Screen.Clear();
            m_ExtraGameCheck = findWinner = SearchWinner(RowIndex, i_ColumnNum);
            countOfCoinsInCol++;
            m_GameBoard.SetCoinsCountInCol(i_ColumnNum, countOfCoinsInCol);
            if (findWinner)
            {
                r_PlayersArray[(int)playerName - 1].PlayerScore++;
                m_WinnerOfTheGame = playerName;
            }
            else
            {
                m_ExtraGameCheck = m_GameBoard.IsBoardFull();
            }

            m_NumOfTurns++;
        }

        public bool SearchWinner(int i_LastInsertedRow, int i_LastInsertedCol)
        {
            bool isWinnerFound = false;

            // get num of rows in the board
            int numOfRows = GetBoardNumRows();

            // get num of cols in the board
            int numOfCols = GetBoardNumCols();

            // the row index that we inserted the coin
            int currentRow = i_LastInsertedRow;

            // the column that we inserted the coin 
            int currentCol = i_LastInsertedCol;

            Board.eMatrixCell playerName = GetCurrentPlayerName();
            /// horizontalCheck 
            if (!isWinnerFound)
            {
                isWinnerFound = HorizontalCheck(numOfCols, currentRow, playerName);
            }

            if (!isWinnerFound)
            {
                isWinnerFound = VerticalCheck(numOfRows, currentCol, playerName);
            }

            if (!isWinnerFound)
            {
                isWinnerFound = CheckDiagonalLeftToRight(numOfRows, numOfCols, playerName);
            }

            if (!isWinnerFound)
            {
                isWinnerFound = CheckDiagonalRightToTopLeft(numOfRows, numOfCols, playerName);
            }

            return isWinnerFound;
        }

        public bool HorizontalCheck(int i_NumOfCols, int i_CurrentRow, Board.eMatrixCell i_Player)
        {
            int sequenceCount = 0;
            for (int col = 0; col < i_NumOfCols; col++)
            {
                // increment the col index each time
                if (m_GameBoard.CheckIfPlayerCoinInCell(i_CurrentRow, col, i_Player))
                {
                    sequenceCount++;
                }
                else
                {
                    sequenceCount = 0;
                }

                if (sequenceCount >= 4)
                {
                    return true;
                }
            }

            return false;
        }

        public bool VerticalCheck(int i_NumOfRows, int i_CurrentCol, Board.eMatrixCell i_Player)
        {
            int sequenceCount = 0;
            for (int row = 0; row < i_NumOfRows; row++)
            {
                // increment the row index each time
                if (m_GameBoard.CheckIfPlayerCoinInCell(row, i_CurrentCol, i_Player))
                {
                    sequenceCount++;
                }
                else
                {
                    sequenceCount = 0;
                }

                if (sequenceCount >= 4)
                {
                    return true;
                }
            }

            return false;
        }

        public bool CheckDiagonalLeftToRight(int i_NumOfRows, int i_NumOfCols, Board.eMatrixCell i_Player)
        {
            if (DiagonalLeftToRightCase1(i_NumOfRows, i_NumOfCols, i_Player) ||
                DiagonalLeftToRightCase2(i_NumOfRows, i_NumOfCols, i_Player))
            {
                return true;
            }

            return false;
        }

        public bool CheckDiagonalRightToTopLeft(int i_NumOfRows, int i_NumOfCols, Board.eMatrixCell i_Player)
        {
            if (DiagonalRightToTopLeftCase1(i_NumOfRows, i_NumOfCols, i_Player) ||
                DiagonalRightToTopLeftCase2(i_NumOfRows, i_NumOfCols, i_Player))
            {
                return true;
            }

            return false;
        }

        public bool DiagonalRightToTopLeftCase1(int i_NumOfRows, int i_NumOfCols, Board.eMatrixCell i_Player)
        {
            int sequenceCount = 0;

            for (int row = i_NumOfRows - 1; row >= i_NumOfRows - 4; row--)
            {
                int rowPosition = row;
                for (int column = 0; column < i_NumOfCols && rowPosition < i_NumOfRows && rowPosition >= 0; column++, rowPosition--)
                {
                    Board.eMatrixCell currentValue = GetCellContent(rowPosition, column);
                    if (currentValue == i_Player)
                    {
                        sequenceCount++;
                    }
                    else
                    {
                        sequenceCount = 0;
                    }

                    if (sequenceCount >= 4)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool DiagonalRightToTopLeftCase2(int i_NumOfRows, int i_NumOfCols, Board.eMatrixCell i_Player)
        {
            int sequenceCount = 0;

            for (int col = 1; col <= i_NumOfCols; col++)
            {
                int colPosition = col;
                for (int row = i_NumOfRows - 1; row < i_NumOfRows && colPosition < i_NumOfCols && colPosition >= 1; row--, colPosition++)
                {
                    Board.eMatrixCell currentValue = GetCellContent(row, colPosition);
                    if (currentValue == i_Player)
                    {
                        sequenceCount++;
                    }
                    else
                    {
                        sequenceCount = 0;
                    }

                    if (sequenceCount >= 4)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool DiagonalLeftToRightCase1(int i_NumOfRows, int i_NumOfCols, Board.eMatrixCell i_Player)
        {
            int sequenceCount = 0;

            for (int row = 0; row <= i_NumOfRows - 4; row++)
            {
                int rowPosition = row;
                for (int column = 0; column < i_NumOfCols && rowPosition < i_NumOfRows; column++, rowPosition++)
                {
                    Board.eMatrixCell currentValue = GetCellContent(rowPosition, column);
                    if (currentValue == i_Player)
                    {
                        sequenceCount++;
                    }
                    else
                    {
                        sequenceCount = 0;
                    }

                    if (sequenceCount >= 4)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool DiagonalLeftToRightCase2(int i_NumOfRows, int i_NumOfCols, Board.eMatrixCell i_Player)
        {
            int sequenceCount = 0;

            for (int col = 1; col <= i_NumOfCols - 4; col++)
            {
                int colPosition = col;
                for (int row = 0; row < i_NumOfRows && colPosition < i_NumOfCols; row++, colPosition++)
                {
                    Board.eMatrixCell currentValue = GetCellContent(row, colPosition);
                    if (currentValue == i_Player)
                    {
                        sequenceCount++;
                    }
                    else
                    {
                        sequenceCount = 0;
                    }

                    if (sequenceCount >= 4)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool IsGameBoardFull()
        {
            return m_GameBoard.IsBoardFull();
        }

        public int[] GetScores()
        {
            int[] playersScore = new int[2];
            playersScore[0] = r_PlayersArray[0].PlayerScore;
            playersScore[1] = r_PlayersArray[1].PlayerScore;

            return playersScore;
        }
    }
}