using Game.Penguins.Core.Interfaces.Game.GameBoard;
using Game.Penguins.Core.Interfaces.Game.Players;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Game.Penguins
{
	public class AI_Medium
	{
		private Random r { get; }

		public AI_Medium()
		{
			r = new Random();
		}

		/// <summary>
		/// Search the provided cell in the board
		/// </summary>
		/// <param name="Board"></param>
		/// <param name="cellToFind"></param>
		/// <returns></returns>
		private Cell SearchCell(IBoard Board, ICell cellToFind)
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
		
		/// <summary>
		/// Check if the cell is available for move
		/// </summary>
		/// <param name="avalaibleDeplacement"></param>
		/// <param name="destination"></param>
		/// <returns></returns>
		private bool IsInAvalaibleDeplacement(List<List<Cell>> avalaibleDeplacement, ICell destination)
		{
			for (int i = 0; i < avalaibleDeplacement.Count; i++)
			{
				if (avalaibleDeplacement[i].Exists(e => e == destination))
				{
					return true;
				}
			}

			return false;
		}
		
		/// <summary>
		/// Find available deplacement for a cell
		/// </summary>
		/// <param name="Board"></param>
		/// <param name="origin"></param>
		/// <param name="dest"></param>
		/// <returns></returns>
		private List<List<Cell>> FindAvalaibleDeplacement(IBoard Board, Cell origin)
		{
			List<List<Cell>> avalaibleDeplacement = new List<List<Cell>>();
			int[] cellIndexOrigin = SearchIndexOfCell(Board, origin);

			avalaibleDeplacement.Add(FindAvalaibleLine(Board, cellIndexOrigin));
			avalaibleDeplacement.Add(FindAvalaibleDiagLeft(Board, cellIndexOrigin));
			avalaibleDeplacement.Add(FindAvalaibleDiagDroite(Board, cellIndexOrigin));

			avalaibleDeplacement = RemoveUnreachableCellInLine(Board, avalaibleDeplacement, cellIndexOrigin);
			avalaibleDeplacement = RemoveUnreachableCellInDiagLeft(Board, avalaibleDeplacement, cellIndexOrigin);
			avalaibleDeplacement = RemoveUnreachableCellInDiagRight(Board, avalaibleDeplacement, cellIndexOrigin);

			return avalaibleDeplacement;
		}
		
		/// <summary>
		/// Search for the index of a cell in the board
		/// </summary>
		/// <param name="Board"></param>
		/// <param name="cell"></param>
		/// <returns></returns>
		private int[] SearchIndexOfCell(IBoard Board, Cell cell)
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
		
		/// <summary>
		/// Remove an unreachable cell in available cell list for a line
		/// </summary>
		/// <param name="Board"></param>
		/// <param name="avalaibleDeplacement"></param>
		/// <param name="cellIndexOrigin"></param>
		/// <returns></returns>
		public List<List<Cell>> RemoveUnreachableCellInLine(IBoard Board, List<List<Cell>> avalaibleDeplacement, int[] cellIndexOrigin)
		{
			int[] rm = new int[2];
			int[] rm1 = new int[2];

			for (int i = (avalaibleDeplacement[0].Count - 1); i > 0; i--)
			{
				if (avalaibleDeplacement[0][i].CellType != CellType.Fish)
				{
					if (SearchIndexOfCell(Board, avalaibleDeplacement[0][i])[0] > cellIndexOrigin[0])
					{
						rm[0] = i;
						rm[1] = avalaibleDeplacement[0].Count - i;
					}
					else if (SearchIndexOfCell(Board, avalaibleDeplacement[0][i])[0] < cellIndexOrigin[0])
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
		
		/// <summary>
		/// Remove an unreachable cell in available cell list for the first diag
		/// </summary>
		/// <param name="Board"></param>
		/// <param name="avalaibleDeplacement"></param>
		/// <param name="cellIndexOrigin"></param>
		/// <returns></returns>
		private List<List<Cell>> RemoveUnreachableCellInDiagLeft(IBoard Board, List<List<Cell>> avalaibleDeplacement, int[] cellIndexOrigin)
		{
			int[] rm = new int[2];
			int[] rm1 = new int[2];

			for (int i = (avalaibleDeplacement[1].Count - 1); i > 0; i--)
			{
				if (avalaibleDeplacement[1][i].CellType != CellType.Fish)
				{
					if (SearchIndexOfCell(Board, avalaibleDeplacement[1][i])[1] > cellIndexOrigin[1])
					{
						rm[0] = i;
						rm[1] = avalaibleDeplacement[1].Count - i;
					}
					else if (SearchIndexOfCell(Board, avalaibleDeplacement[1][i])[1] < cellIndexOrigin[1])
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
		
		/// <summary>
		/// Remove an unreachable cell in available cell list for the second diag
		/// </summary>
		/// <param name="Board"></param>
		/// <param name="avalaibleDeplacement"></param>
		/// <param name="cellIndexOrigin"></param>
		/// <returns></returns>
		private List<List<Cell>> RemoveUnreachableCellInDiagRight(IBoard Board, List<List<Cell>> avalaibleDeplacement, int[] cellIndexOrigin)
		{
			int[] rm = new int[2];
			int[] rm1 = new int[2];

			for (int i = (avalaibleDeplacement[2].Count - 1); i > 0; i--)
			{
				if (avalaibleDeplacement[2][i].CellType != CellType.Fish)
				{
					if (SearchIndexOfCell(Board, avalaibleDeplacement[2][i])[1] < cellIndexOrigin[1])
					{
						rm[0] = i;
						rm[1] = avalaibleDeplacement[2].Count - i;
					}
					else if (SearchIndexOfCell(Board, avalaibleDeplacement[2][i])[1] > cellIndexOrigin[1])
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
		
		/// <summary>
		/// find available cell in first diag
		/// </summary>
		/// <param name="Board"></param>
		/// <param name="cellIndexOrigin"></param>
		/// <returns></returns>
		private List<Cell> FindAvalaibleDiagDroite(IBoard Board, int[] cellIndexOrigin)
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

			deplacementDiagDroite = deplacementDiagDroite.OrderByDescending(e => SearchIndexOfCell(Board, e)[1]).ToList();
			return deplacementDiagDroite;
		}
		
		/// <summary>
		/// find available cell in second diag
		/// </summary>
		/// <param name="Board"></param>
		/// <param name="cellIndexOrigin"></param>
		/// <returns></returns>
		private List<Cell> FindAvalaibleDiagLeft(IBoard Board, int[] cellIndexOrigin)
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
		
		/// <summary>
		/// find available cell in line
		/// </summary>
		/// <param name="Board"></param>
		/// <param name="cellIndexOrigin"></param>
		/// <returns></returns>
		private List<Cell> FindAvalaibleLine(IBoard Board, int[] cellIndexOrigin)
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
		
		/// <summary>
		/// Move the penguin
		/// </summary>
		/// <param name="Board"></param>
		/// <param name="CurrentPlayer"></param>
		/// <param name="AIPenguins"></param>
		public void Move(IBoard Board, IPlayer CurrentPlayer, Dictionary<IPlayer, List<Cell>> AIPenguins)
		{
			if (CurrentPlayer.Penguins > 0)
			{
				bool movePenguins = false, check;
				PlayerClass currentPlayer = (PlayerClass)CurrentPlayer;

				while (!movePenguins)
				{
					Cell start;
					Cell end;
					List<List<Cell>> avalaibleDeplacement;
					do
					{
						int random = r.Next(0, CurrentPlayer.Penguins - 1);
						start = SearchCell(Board, AIPenguins[CurrentPlayer][random]);
						end = SearchCell(Board, Board.Board[r.Next(0, 8), r.Next(0, 8)]);
						avalaibleDeplacement = FindAvalaibleDeplacement(Board, start);
						check = RemovePenguins(Board, start, currentPlayer, AIPenguins);
					} while (!check);



					if (start.CurrentPenguin.Player == CurrentPlayer && end.CellType == CellType.Fish && IsInAvalaibleDeplacement(avalaibleDeplacement, end))
					{
						currentPlayer.Points += start.FishCount;
						currentPlayer.ChangeState();

						start.CellType = CellType.Water;
						start.FishCount = 0;
						start.CurrentPenguin = null;

						end.CellType = CellType.FishWithPenguin;
						end.CurrentPenguin = new Penguins(CurrentPlayer);

						start.ChangeState();
						end.ChangeState();
						AIPenguins[currentPlayer].Remove(start);
						if (RemovePenguins(Board, end, currentPlayer, AIPenguins))
						{
							AIPenguins[currentPlayer].Add(end);
						}
						movePenguins = true;
					}
				}
			}
		}
		
		/// <summary>
		/// Place the penguin
		/// </summary>
		/// <param name="Board"></param>
		/// <param name="CurrentPlayer"></param>
		/// <param name="AIPenguins"></param>
		public void PlacePenguin(IBoard Board, IPlayer CurrentPlayer, Dictionary<IPlayer, List<Cell>> AIPenguins)
		{
			List<Cell> AvailableCell = new List<Cell>();

			foreach (Cell tempcell in Board.Board)
			{
				if (tempcell.FishCount == 1)
				{
					AvailableCell.Add(tempcell);
				}
			}
			bool token = false;
			do
			{
				/// Choix aleatoire de la case.
				int index = r.Next(0, AvailableCell.Count());
				Cell cell = AvailableCell[index];
				if (cell.CurrentPenguin == null)
				{
					cell.CellType = CellType.FishWithPenguin;
					cell.CurrentPenguin = new Penguins(CurrentPlayer);

					AIPenguins[CurrentPlayer].Add(cell);
					cell.ChangeState();
					token = true;
				}
				else
				{
					AvailableCell.Remove(cell);
				}


			} while (!token);


		}
		
		/// <summary>
		/// Check if a penguins is surrounded by water and remove it
		/// </summary>
		/// <param name="Board"></param>
		/// <param name="origin"></param>
		/// <param name="currentPlayer"></param>
		/// <param name="AIPenguins"></param>
		/// <returns></returns>
		private bool RemovePenguins(IBoard Board, Cell origin, PlayerClass currentPlayer, Dictionary<IPlayer, List<Cell>> AIPenguins)
		{
			int[] originIndex = SearchIndexOfCell(Board, origin);
			int x = originIndex[0];
			int y = originIndex[1];
			bool token = false;
			if (y > 0 && !token)
			{
				if (Board.Board[x, y - 1].CellType != CellType.FishWithPenguin && Board.Board[x, y - 1].CellType != CellType.Water) token = true;
			}
			if (y < 7 && !token)
			{
				if (Board.Board[x, y + 1].CellType != CellType.FishWithPenguin && Board.Board[x, y + 1].CellType != CellType.Water) token = true;
			}
			if (x < 7 && !token)
			{
				if (Board.Board[x + 1, y].CellType != CellType.FishWithPenguin && Board.Board[x + 1, y].CellType != CellType.Water) token = true;
			}
			if (x > 0 && !token)
			{
				if (Board.Board[x - 1, y].CellType != CellType.FishWithPenguin && Board.Board[x - 1, y].CellType != CellType.Water) token = true;
			}
			if (y % 2 != 0 && !token) /// line number is odd
			{
				if (x < 7 && y > 0)
				{
					if (Board.Board[x + 1, y - 1].CellType != CellType.FishWithPenguin && Board.Board[x + 1, y - 1].CellType != CellType.Water) token = true;
				}
				if (x < 7 && y < 7 && !token)
				{
					if (Board.Board[x + 1, y + 1].CellType != CellType.FishWithPenguin && Board.Board[x + 1, y + 1].CellType != CellType.Water) token = true;
				}
			}
			if (y % 2 == 0 && !token) /// line number is even
			{
				if (x > 0 && y > 0)
				{
					if (Board.Board[x - 1, y - 1].CellType != CellType.FishWithPenguin && Board.Board[x - 1, y - 1].CellType != CellType.Water) token = true;
				}
				if (x > 0 && y < 7 && !token)
				{
					if (Board.Board[x - 1, y + 1].CellType != CellType.FishWithPenguin && Board.Board[x - 1, y + 1].CellType != CellType.Water) token = true;
				}

			}
			if (!token)
			{
				currentPlayer.Penguins--;
				AIPenguins[currentPlayer].Remove(origin);
				origin.CellType = CellType.Water;
				origin.FishCount = 0;
				origin.CurrentPenguin = null;
				origin.ChangeState();
			}
			return token;

		}
	}
}
