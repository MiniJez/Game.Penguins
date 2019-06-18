using Game.Penguins.Core.Interfaces.Game.GameBoard;
using Game.Penguins.Core.Interfaces.Game.Players;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Penguins
{
	public class GameClass : IGame
	{
		public GameClass()
		{
			Players = new List<IPlayer>();
			Board = new BoardClass();
			AI_easy = new AI_Easy();
			AI_medium = new AI_Medium();
		}

		public Dictionary<IPlayer, List<Cell>> AIPenguins { get; set; }

		public IBoard Board { get; set; }

		public NextActionType NextAction { get; set; }

		public IPlayer CurrentPlayer { get; set; }

		public AI_Easy AI_easy { get; set; }

		public AI_Medium AI_medium{ get; set; }

		public IList<IPlayer> Players { get; set; }

		public event EventHandler StateChanged;

		public int PenguinsByPlayer { get; set; }

		public int Turn { get; set; }

		/// <summary>
		/// define the number of penguin by player
		/// </summary>
		/// <returns></returns>
		public int NumberOfPenguins()
		{
			if (Players.Count() == 2)
			{
				PenguinsByPlayer = 4;
			}
			else if (Players.Count() == 3)
			{
				PenguinsByPlayer = 3;
			}
			else if (Players.Count() == 4)
			{
				PenguinsByPlayer = 2;
			}

			return PenguinsByPlayer;
		}

		/// <summary>
		/// Add a player to the game
		/// </summary>
		/// <param name="playerName"></param>
		/// <param name="playerType"></param>
		/// <returns></returns>
		public IPlayer AddPlayer(string playerName, PlayerType playerType)
		{
			PlayerColor color = ChoosePlayerColor();

			IPlayer player = new PlayerClass(playerType, color, playerName);
			Players.Add(player);
			if (AIPenguins == null)
			{
				AIPenguins = new Dictionary<IPlayer, List<Cell>>();
			}
			if (player.PlayerType != PlayerType.Human)
			{
				AIPenguins.Add(player, new List<Cell>());
			}

			return player;
		}

		/// <summary>
		/// Choose the color for a player
		/// </summary>
		/// <returns></returns>
		public PlayerColor ChoosePlayerColor()
		{
			if (Players.Count() == 0)
			{
				return PlayerColor.Blue;
			}
			else if (Players.Count() == 1)
			{
				return PlayerColor.Green;
			}
			else if (Players.Count() == 2)
			{
				return PlayerColor.Red;
			}
			else
			{
				return PlayerColor.Yellow;
			}
		}

		/// <summary>
		/// Move a non human player
		/// </summary>
		public void Move()
		{

			bool endGame = true;
			foreach (var item in Players)
			{
				if (item.Penguins > 0)
				{
					endGame = false;
				}
			}


			if (CurrentPlayer.PlayerType == PlayerType.AIEasy)
			{
				endGame = AI_easy.MovePenguins((BoardClass)Board, (PlayerClass)CurrentPlayer);
			}
			else if (CurrentPlayer.PlayerType == PlayerType.AIMedium)
			{
				AI_medium.Move(Board, CurrentPlayer, AIPenguins);
			}

			if (!endGame)
			{
				NextAction = NextActionType.MovePenguin;
				NextPlayer();
			}
			else
			{
				NextPlayer();
				endGame = false;

				while (!endGame)
				{
					endGame = AI_easy.MovePenguins((BoardClass)Board, (PlayerClass)CurrentPlayer);
				}

				NextAction = NextActionType.Nothing;
			}


			StateChanged(this, null);
		}

		/// <summary>
		/// Move a human player
		/// </summary>
		/// <param name="origin"></param>
		/// <param name="destination"></param>
		public void MoveManual(ICell origin, ICell destination)
		{
			Cell start = SearchCell(origin);
			Cell end = SearchCell(destination);
			List<List<Cell>> avalaibleDeplacement = FindAvalaibleDeplacement(start, end);

			if (start.CurrentPenguin != null && start.CurrentPenguin.Player == CurrentPlayer && end.CellType == CellType.Fish && IsInAvalaibleDeplacement(avalaibleDeplacement, destination))
			{
				PlayerClass currentPlayer = (PlayerClass)CurrentPlayer;
				currentPlayer.Points += start.FishCount;
				currentPlayer.ChangeState();

				start.CellType = CellType.Water;
				start.FishCount = 0;
				start.CurrentPenguin = null;

				end.CellType = CellType.FishWithPenguin;
				end.CurrentPenguin = new Penguins(CurrentPlayer);

				NextAction = NextActionType.MovePenguin;
				NextPlayer();
				StateChanged(this, null);
				start.ChangeState();
				end.ChangeState();
			}
		}

		/// <summary>
		/// Check if a cell is available
		/// </summary>
		/// <param name="avalaibleDeplacement"></param>
		/// <param name="destination"></param>
		/// <returns></returns>
		public bool IsInAvalaibleDeplacement(List<List<Cell>> avalaibleDeplacement, ICell destination)
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
		/// Search a cell on the board
		/// </summary>
		/// <param name="cellToFind"></param>
		/// <returns></returns>
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

			throw new Exception("Error during cell search");
		}

		/// <summary>
		/// Search the index of a cell
		/// </summary>
		/// <param name="cell"></param>
		/// <returns></returns>
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="origin"></param>
		/// <param name="dest"></param>
		/// <returns></returns>
		public List<List<Cell>> FindAvalaibleDeplacement(Cell origin, Cell dest)
		{
			List<List<Cell>> avalaibleDeplacement = new List<List<Cell>>();
			int[] cellIndexOrigin = SearchIndexOfCell(origin);

			avalaibleDeplacement.Add(FindAvalaibleLine(cellIndexOrigin));
			avalaibleDeplacement.Add(FindAvalaibleDiagLeft(cellIndexOrigin));
			avalaibleDeplacement.Add(FindAvalaibleDiagRight(cellIndexOrigin));

			avalaibleDeplacement = RemoveUnreachableCellInLine(avalaibleDeplacement, cellIndexOrigin);
			avalaibleDeplacement = RemoveUnreachableCellInDiagLeft(avalaibleDeplacement, cellIndexOrigin);
			avalaibleDeplacement = RemoveUnreachableCellInDiagRight(avalaibleDeplacement, cellIndexOrigin);

			return avalaibleDeplacement;
		}

		/// <summary>
		/// Remove unreachable cells in availabe cell list
		/// </summary>
		/// <param name="avalaibleDeplacement"></param>
		/// <param name="cellIndexOrigin"></param>
		/// <returns></returns>
		public List<List<Cell>> RemoveUnreachableCellInLine(List<List<Cell>> avalaibleDeplacement, int[] cellIndexOrigin)
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

		/// <summary>
		/// Remove unreachable cells in availabe cell list
		/// </summary>
		/// <param name="avalaibleDeplacement"></param>
		/// <param name="cellIndexOrigin"></param>
		/// <returns></returns>
		public List<List<Cell>> RemoveUnreachableCellInDiagLeft(List<List<Cell>> avalaibleDeplacement, int[] cellIndexOrigin)
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

		/// <summary>
		/// Remove unreachable cells in availabe cell list
		/// </summary>
		/// <param name="avalaibleDeplacement"></param>
		/// <param name="cellIndexOrigin"></param>
		/// <returns></returns>
		public List<List<Cell>> RemoveUnreachableCellInDiagRight(List<List<Cell>> avalaibleDeplacement, int[] cellIndexOrigin)
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

		/// <summary>
		/// Find available cell and add it to the list
		/// </summary>
		/// <param name="cellIndexOrigin"></param>
		/// <returns></returns>
		public List<Cell> FindAvalaibleDiagLeft(int[] cellIndexOrigin)
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
		/// Find available cell and add it to the list
		/// </summary>
		/// <param name="cellIndexOrigin"></param>
		/// <returns></returns>
		public List<Cell> FindAvalaibleDiagRight(int[] cellIndexOrigin)
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

		/// <summary>
		/// Find available cell and add it to the list
		/// </summary>
		/// <param name="cellIndexOrigin"></param>
		/// <returns></returns>
		public List<Cell> FindAvalaibleLine(int[] cellIndexOrigin)
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
		/// Place penguin for non human player
		/// </summary>
		public void PlacePenguin()
		{
			if (CurrentPlayer.PlayerType == PlayerType.AIEasy)
			{
				AI_easy.PlacePenguins((BoardClass)Board, (PlayerClass)CurrentPlayer);
			}
			else if (CurrentPlayer.PlayerType == PlayerType.AIMedium)
			{
				AI_medium.PlacePenguin(Board, CurrentPlayer, AIPenguins);
				NextAction = NextActionType.PlacePenguin;
				if (Turn == Players.Count() * PenguinsByPlayer)
				{
					NextAction = NextActionType.MovePenguin;
				}
			}

			NextAction = NextActionType.PlacePenguin;
			if (Turn == Players.Count() * PenguinsByPlayer)
			{
				NextAction = NextActionType.MovePenguin;
			}

			Turn++;
			NextPlayer();
			StateChanged.Invoke(this, null);
		}

		/// <summary>
		/// place penguin for human player
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void PlacePenguinManual(int x, int y)
		{
			Cell cell = (Cell)Board.Board[x, y];

			if (cell.FishCount == 1 && cell.CurrentPenguin == null)
			{
				cell.CellType = CellType.FishWithPenguin;
				cell.CurrentPenguin = new Penguins(CurrentPlayer);

				NextAction = NextActionType.PlacePenguin;
				if (Turn == Players.Count() * PenguinsByPlayer)
				{
					NextAction = NextActionType.MovePenguin;
				}

				Turn++;
				NextPlayer();
				cell.ChangeState();
				StateChanged.Invoke(this, null);
			}
		}

		/// <summary>
		/// Init the list of penguin for each player
		/// </summary>
		public void InitAiPenguins()
		{
			AIPenguins = new Dictionary<IPlayer, List<Cell>>();
			foreach (var item in Players)
			{
				if (item.PlayerType != PlayerType.Human)
				{
					AIPenguins.Add(item, new List<Cell>());
				}
			}
		}

		/// <summary>
		/// Start the game
		/// </summary>
		public void StartGame()
		{
			Turn = 1;
			CurrentPlayer = Players[0];
			PenguinsByPlayer = NumberOfPenguins();
			NextAction = NextActionType.PlacePenguin;
			StateChanged.Invoke(this, null);
		}

		/// <summary>
		/// Change player
		/// </summary>
		public void NextPlayer()
		{
			int nextPlayerIndex = Players.IndexOf(CurrentPlayer) + 1;

			if (nextPlayerIndex == Players.Count())
			{
				nextPlayerIndex = 0;
			}

			CurrentPlayer = Players[nextPlayerIndex];
		}
	}
}
