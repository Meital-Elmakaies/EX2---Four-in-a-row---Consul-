namespace C21_Ex02
{
    public class Board
    {
        private readonly int[] r_CoinsCountInCol;
        private readonly eMatrixCell[,] r_MatrixBoard;
        private int m_Rows;
        private int m_Columns;

        public Board(int i_Rows, int i_Columns)
        {
            r_CoinsCountInCol = new int[i_Columns];
            r_MatrixBoard = new eMatrixCell[i_Rows, i_Columns];
            m_Columns = i_Columns;
            m_Rows = i_Rows;
        }

        public enum eMatrixCell
        {
            Empty,
            FirstPlayer,
            SecondPlayer,
        }

        public int Rows
        {
            get
            {
                return m_Rows;
            }

            set
            {
                m_Rows = value;
            }
        }

        public int Columns
        {
            get
            {
                return m_Columns;
            }

            set
            {
                m_Columns = value;
            }
        }

        public bool IsBoardFull()
        {
            for (int i = 0; i < m_Columns; i++)
            {
                if (r_MatrixBoard[0, i] == eMatrixCell.Empty)
                {
                    return false;
                }
            }

            return true;
        }

        public void SetCellValue(int i_Row, int i_Column, eMatrixCell i_Value)
        {
            r_MatrixBoard[i_Row, i_Column] = i_Value;
        }

        public eMatrixCell GetCellValue(int i_Row, int i_Column)
        {
            return r_MatrixBoard[i_Row, i_Column];
        }

        public bool CellValidation(int i_Row, int i_Column)
        {
            bool isCellValid = false;
            if (i_Row < m_Rows && i_Column < m_Columns)
            {
                if (i_Row >= 0 && i_Column >= 0)
                {
                    isCellValid = true;
                }
            }

            return isCellValid;
        }

        public void SetCoinsCountInCol(int i_Col, int i_Count)
        {
            r_CoinsCountInCol[i_Col] = i_Count;
        }

        public int GetCoinsCountInCol(int i_Column)
        {
            return r_CoinsCountInCol[i_Column];
        }

        public bool CheckIfPlayerCoinInCell(int i_row, int i_Column, eMatrixCell i_Player)
        {
            bool playerCoinInCell = false;
            bool isValidCell = CellValidation(i_row, i_Column);

            if (isValidCell)
            {
                playerCoinInCell = i_Player == GetCellValue(i_row, i_Column);
            }

            return playerCoinInCell;
        }
    }
}
