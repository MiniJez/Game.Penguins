using System;
using System.Collections.Generic;
using Game.Penguins.Core.Interfaces.Game.GameBoard;
using Game.Penguins.Core.Interfaces.Game.Players;

namespace Game.Penguins
{
    class IA_Hard
    {
        Dictionary<string, Dictionary<string, int>> QTable = new Dictionary<string, Dictionary<string, int>>();
        const double learningRate = 0.1; // Learning Rate
        const double discountFactor = 0.9; // Discount Factor of Future Rewards
        double randomize = 0.1; // Randomization Rate on Action
        Random rnd = new Random();

        BoardClass Board = new BoardClass();
        PlayerClass CurrentPlayer;

        public string GetActualState(Cell myPenguinsCell)
        {
            int[] index = SearchIndexOfCell(myPenguinsCell);
            return index[0] + ", " + index[1];
        }

        public Dictionary<string, int> GetQTable(string stateName)
        {
            return QTable[stateName];
        }

        public string GetBestAction(string stateName, List<List<Cell>> availableDeplacement)
        {
            Dictionary<string, int> Q = GetQTable(stateName);

            int maxValue;
            string choseAction;

            if (rnd.NextDouble() < randomize)
            {
                int index1 = rnd.Next(availableDeplacement.Count);
                Cell cell = availableDeplacement[index1][rnd.Next(availableDeplacement[index1].Count)];
                int[] index = SearchIndexOfCell(cell);
                choseAction = index[0] + ", " + index[1];
                return choseAction;
            }

            for (let i = 0; i < availableActions.length; i++)
            {
                if (Q[availableActions[i]] > maxValue)
                {
                    maxValue = Q[availableActions[i]];
                    choseAction = availableActions[i];
                }
            }

            if (maxValue === 0)
            {
                choseAction = availableActions[getRandomInt(0, 4)];
            }

            return choseAction;
        }

        public void PlacePenguins(BoardClass Board, PlayerClass CurrentPlayer)
        {
            List<Cell> availableCell = GetAvailablePlacementCell(Board);

            int rndIndex = rnd.Next(availableCell.Count);

            Cell cell = availableCell[rndIndex];
            cell.CellType = CellType.FishWithPenguin;
            cell.CurrentPenguinObject = new Penguins(CurrentPlayer);
            cell.ChangeState();
        }

        public List<Cell> GetAvailablePlacementCell(BoardClass Board)
        {
            List<Cell> availableCell = new List<Cell>();

            foreach (Cell cell in Board.Board)
            {
                if (cell.FishCount == 1 && cell.CellType == CellType.Fish)
                {
                    availableCell.Add(cell);
                }
            }

            return availableCell;
        }

        public List<Cell> GetMyPenguinsCell(BoardClass Board, PlayerClass CurrentPlayer)
        {
            List<Cell> myPenguinsCell = new List<Cell>();

            foreach (Cell cell in Board.Board)
            {
                if (cell.CurrentPenguinObject != null && cell.CurrentPenguinObject.Player == CurrentPlayer)
                {
                    myPenguinsCell.Add(cell);
                }
            }

            return myPenguinsCell;
        }

        public bool MovePenguins(BoardClass board, PlayerClass currentPlayer)
        {
            Board = board;
            CurrentPlayer = currentPlayer;
            Random rnd = new Random();
            List<Cell> myPenguinsCell = GetMyPenguinsCell(Board, CurrentPlayer);
            int rndIndex = rnd.Next(myPenguinsCell.Count);
            Cell start = SearchCell(myPenguinsCell[rndIndex]);
            List<List<Cell>> avalaibleDeplacement = FindAvalaibleDeplacement(start);

            string actualStateName = GetActualState(myPenguinsCell[rndIndex]);
            string action = GetBestAction(actualStateName, avalaibleDeplacement);

            if (myPenguinsCell.Count != 0)
            {
                if (avalaibleDeplacement.Count == 0)
                {
                    Cell toDelete = myPenguinsCell[rndIndex];
                    
                    CurrentPlayer.Points += toDelete.FishCount;
                    CurrentPlayer.ChangeState();

                    toDelete.CellType = CellType.Water;
                    toDelete.FishCount = 0;
                    toDelete.CurrentPenguinObject = null;
                    toDelete.ChangeState();

                    MovePenguins(Board, CurrentPlayer);
                }
                else
                {
                    Cell start = myPenguinsCell[rndIndex];

                    rndIndex = rnd.Next(availableCell.Count);
                    Cell end = availableCell[rndIndex];

                    PlayerClass currentPlayer = CurrentPlayer;
                    currentPlayer.Points += start.FishCount;
                    currentPlayer.ChangeState();

                    start.CellType = CellType.Water;
                    start.FishCount = 0;
                    start.CurrentPenguinObject = null;

                    end.CellType = CellType.FishWithPenguin;
                    end.CurrentPenguinObject = new Penguins(CurrentPlayer);

                    start.ChangeState();
                    end.ChangeState();
                }

                return false;
            }

            return true;
        }

        public List<List<Cell>> FindAvalaibleDeplacement(Cell origin)
        {
            List<List<Cell>> avalaibleDeplacement = new List<List<Cell>>();
            int[] cellIndexOrigin = SearchIndexOfCell(origin);

            avalaibleDeplacement.Add(FindAvalaibleLigne(cellIndexOrigin));
            avalaibleDeplacement.Add(FindAvalaibleDiagGauche(cellIndexOrigin));
            avalaibleDeplacement.Add(FindAvalaibleDiagDroite(cellIndexOrigin));

            avalaibleDeplacement = RemoveUnreachableCellInLigne(avalaibleDeplacement, cellIndexOrigin);
            avalaibleDeplacement = RemoveUnreachableCellInDiagGauche(avalaibleDeplacement, cellIndexOrigin);
            avalaibleDeplacement = RemoveUnreachableCellInDiagDroite(avalaibleDeplacement, cellIndexOrigin);

            return avalaibleDeplacement;
        }

        public List<List<Cell>> RemoveUnreachableCellInLigne(List<List<Cell>> avalaibleDeplacement, int[] cellIndexOrigin)
        {
            int[] rm = new int[2];
            int[] rm1 = new int[2];

            for (int i = (avalaibleDeplacement[0].Count - 1); i > 0; i--)
            {
                if (avalaibleDeplacement[0][i].CellType != CellType.Fish)
                {
                    if (SearchIndexOfCell(avalaibleDeplacement[0][i])[0] > cellIndexOrigin[0])
                    {
                        rm[0] = i;
                        rm[1] = avalaibleDeplacement[0].Count - i;
                    }
                    else if (SearchIndexOfCell(avalaibleDeplacement[0][i])[0] < cellIndexOrigin[0])
                    {
                        rm1[0] = 0;
                        rm1[1] = i + 1;
                        i = 0;
                    }
                }
            }

            avalaibleDeplacement[0].RemoveRange(rm[0], rm[1]);
            avalaibleDeplacement[0].RemoveRange(rm1[0], rm1[1]);

            return avalaibleDeplacement;
        }

        public List<List<Cell>> RemoveUnreachableCellInDiagGauche(List<List<Cell>> avalaibleDeplacement, int[] cellIndexOrigin)
        {
            int[] rm = new int[2];
            int[] rm1 = new int[2];

            for (int i = (avalaibleDeplacement[1].Count - 1); i > 0; i--)
            {
                if (avalaibleDeplacement[1][i].CellType != CellType.Fish)
                {
                    if (SearchIndexOfCell(avalaibleDeplacement[1][i])[1] > cellIndexOrigin[1])
                    {
                        rm[0] = i;
                        rm[1] = avalaibleDeplacement[1].Count - i;
                    }
                    else if (SearchIndexOfCell(avalaibleDeplacement[1][i])[1] < cellIndexOrigin[1])
                    {
                        rm1[0] = 0;
                        rm1[1] = i + 1;
                        i = 0;
                    }
                }
            }

            avalaibleDeplacement[1].RemoveRange(rm[0], rm[1]);
            avalaibleDeplacement[1].RemoveRange(rm1[0], rm1[1]);

            return avalaibleDeplacement;
        }

        public List<List<Cell>> RemoveUnreachableCellInDiagDroite(List<List<Cell>> avalaibleDeplacement, int[] cellIndexOrigin)
        {
            int[] rm = new int[2];
            int[] rm1 = new int[2];

            for (int i = (avalaibleDeplacement[2].Count - 1); i > 0; i--)
            {
                if (avalaibleDeplacement[2][i].CellType != CellType.Fish)
                {
                    if (SearchIndexOfCell(avalaibleDeplacement[2][i])[1] < cellIndexOrigin[1])
                    {
                        rm[0] = i;
                        rm[1] = avalaibleDeplacement[2].Count - i;
                    }
                    else if (SearchIndexOfCell(avalaibleDeplacement[2][i])[1] > cellIndexOrigin[1])
                    {
                        rm1[0] = 0;
                        rm1[1] = i + 1;
                        i = 0;
                    }
                }
            }

            avalaibleDeplacement[2].RemoveRange(rm[0], rm[1]);
            avalaibleDeplacement[2].RemoveRange(rm1[0], rm1[1]);

            return avalaibleDeplacement;
        }

        public List<Cell> FindAvalaibleDiagGauche(int[] cellIndexOrigin)
        {
            List<Cell> deplacementDiagGauche = new List<Cell>();

            for (int i = 0; i < Board.Board.GetLength(0); i++)
            {
                for (int j = 0; j < Board.Board.GetLength(1); j++)
                {
                    if ((j % 2 == 0 && cellIndexOrigin[1] % 2 == 0) || (j % 2 == 1 && cellIndexOrigin[1] % 2 == 1))
                    {
                        if (j < cellIndexOrigin[1])
                        {
                            if (i == cellIndexOrigin[0] - ((cellIndexOrigin[1] - j) / 2))
                            {
                                deplacementDiagGauche.Add((Cell)Board.Board[i, j]);
                            }
                        }
                        else if (j > cellIndexOrigin[1])
                        {
                            if (i == cellIndexOrigin[0] + ((j - cellIndexOrigin[1]) / 2))
                            {
                                deplacementDiagGauche.Add((Cell)Board.Board[i, j]);
                            }
                        }
                    }
                    else if ((j % 2 == 1 && cellIndexOrigin[1] % 2 == 0))
                    {
                        if (j < cellIndexOrigin[1])
                        {
                            if (i == cellIndexOrigin[0] - (Decimal.Floor((cellIndexOrigin[1] - j) / 2) + 1))
                            {
                                deplacementDiagGauche.Add((Cell)Board.Board[i, j]);
                            }
                        }
                        else if (j > cellIndexOrigin[1])
                        {
                            if (i == cellIndexOrigin[0] + (Decimal.Floor((j - cellIndexOrigin[1]) / 2)))
                            {
                                deplacementDiagGauche.Add((Cell)Board.Board[i, j]);
                            }
                        }
                    }
                    else if ((j % 2 == 0 && cellIndexOrigin[1] % 2 == 1))
                    {
                        if (j < cellIndexOrigin[1])
                        {
                            if (i == cellIndexOrigin[0] - (Decimal.Floor((cellIndexOrigin[1] - j) / 2)))
                            {
                                deplacementDiagGauche.Add((Cell)Board.Board[i, j]);
                            }
                        }
                        else if (j > cellIndexOrigin[1])
                        {
                            if (i == cellIndexOrigin[0] + (Decimal.Floor((j - cellIndexOrigin[1]) / 2) + 1))
                            {
                                deplacementDiagGauche.Add((Cell)Board.Board[i, j]);
                            }
                        }
                    }
                }
            }

            return deplacementDiagGauche;
        }

        public List<Cell> FindAvalaibleDiagDroite(int[] cellIndexOrigin)
        {
            List<Cell> deplacementDiagDroite = new List<Cell>();

            for (int i = 0; i < Board.Board.GetLength(0); i++)
            {
                for (int j = 0; j < Board.Board.GetLength(1); j++)
                {
                    if ((j % 2 == 0 && cellIndexOrigin[1] % 2 == 0) || (j % 2 == 1 && cellIndexOrigin[1] % 2 == 1))
                    {
                        if (j < cellIndexOrigin[1])
                        {
                            if (i == cellIndexOrigin[0] + ((cellIndexOrigin[1] - j) / 2))
                            {
                                deplacementDiagDroite.Add((Cell)Board.Board[i, j]);
                            }
                        }
                        else if (j > cellIndexOrigin[1])
                        {
                            if (i == cellIndexOrigin[0] - ((j - cellIndexOrigin[1]) / 2))
                            {
                                deplacementDiagDroite.Add((Cell)Board.Board[i, j]);
                            }
                        }
                    }
                    else if ((j % 2 == 1 && cellIndexOrigin[1] % 2 == 0))
                    {
                        if (j < cellIndexOrigin[1])
                        {
                            if (i == cellIndexOrigin[0] + (Decimal.Floor((cellIndexOrigin[1] - j) / 2)))
                            {
                                deplacementDiagDroite.Add((Cell)Board.Board[i, j]);
                            }
                        }
                        else if (j > cellIndexOrigin[1])
                        {
                            if (i == cellIndexOrigin[0] - (Decimal.Floor((j - cellIndexOrigin[1]) / 2) + 1))
                            {
                                deplacementDiagDroite.Add((Cell)Board.Board[i, j]);
                            }
                        }
                    }
                    else if ((j % 2 == 0 && cellIndexOrigin[1] % 2 == 1))
                    {
                        if (j < cellIndexOrigin[1])
                        {
                            if (i == cellIndexOrigin[0] + (Decimal.Floor((cellIndexOrigin[1] - j) / 2) + 1))
                            {
                                deplacementDiagDroite.Add((Cell)Board.Board[i, j]);
                            }
                        }
                        else if (j > cellIndexOrigin[1])
                        {
                            if (i == cellIndexOrigin[0] - (Decimal.Floor((j - cellIndexOrigin[1]) / 2)))
                            {
                                deplacementDiagDroite.Add((Cell)Board.Board[i, j]);
                            }
                        }
                    }
                }
            }

            deplacementDiagDroite = deplacementDiagDroite.OrderByDescending(e => SearchIndexOfCell(e)[1]).ToList();
            return deplacementDiagDroite;
        }

        public List<Cell> FindAvalaibleLigne(int[] cellIndexOrigin)
        {
            List<Cell> deplacementLigne = new List<Cell>();

            for (int i = 0; i < Board.Board.GetLength(0); i++)
            {
                for (int j = 0; j < Board.Board.GetLength(1); j++)
                {
                    if (j == cellIndexOrigin[1] && i != cellIndexOrigin[0])
                    {
                        deplacementLigne.Add((Cell)Board.Board[i, j]);
                    }
                }
            }

            return deplacementLigne;
        }

        public Cell SearchCell(ICell cellToFind)
        {
            for (int i = 0; i < Board.Board.GetLength(0); i++)
            {
                for (int j = 0; j < Board.Board.GetLength(1); j++)
                {
                    if (Board.Board[i, j] == cellToFind)
                    {
                        return (Cell)Board.Board[i, j];
                    }
                }
            }

            return null;
        }

        public int[] SearchIndexOfCell(Cell cell)
        {
            int[] index = new int[2];

            for (int i = 0; i < Board.Board.GetLength(0); i++)
            {
                for (int j = 0; j < Board.Board.GetLength(1); j++)
                {
                    if (Board.Board[i, j] == cell)
                    {
                        index[0] = i;
                        index[1] = j;

                        return index;
                    }
                }
            }

            return null;
        }
    }
}
