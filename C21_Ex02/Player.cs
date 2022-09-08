namespace C21_Ex02
{
    using System;

    public class Player
    {
        private ePlayerType m_PlayerType;
        private Board.eMatrixCell m_PlayerName;
        private int m_PlayerScore;

        public Player()
        {
            m_PlayerType = new ePlayerType();
        }

        public enum ePlayerType 
        { 
            Human,
            Computer
        }

        public int PlayerScore
        {
            get 
            { 
                return m_PlayerScore; 
            }

            set 
            {
                m_PlayerScore = value;
            }
        }

        public ePlayerType Type
        {
            get 
            { 
                return m_PlayerType; 
            }

            set
            {
                m_PlayerType = value;
            }
        }

        public Board.eMatrixCell Name
        {
            get 
            { 
                return m_PlayerName;
            }

            set 
            {
                m_PlayerName = value;
            }
        }
    }
}