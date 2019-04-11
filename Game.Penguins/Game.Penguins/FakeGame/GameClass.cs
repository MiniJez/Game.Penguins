﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Penguins.Core.Interfaces.Game.GameBoard;
using Game.Penguins.Core.Interfaces.Game.Players;

namespace Game.Penguins
{
    class GameClass : IGame
    {
        public GameClass()
        {
            Players = new List<IPlayer>();
            Board = new BoardClass();
        }

        public IBoard Board { get; set; }

        public NextActionType NextAction { get; set; }

        public IPlayer CurrentPlayer { get; set; }

        public IList<IPlayer> Players { get; set; }

        public event EventHandler StateChanged;

        public int PenguinsByPlayer { get; set; }

        public int Turn { get; set; }

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

        public IPlayer AddPlayer(string playerName, PlayerType playerType)
        {
            PlayerColor color = ChoosePlayerColor();

            IPlayer player = new PlayerClass(playerType, color, playerName);
            Players.Add(player);

            return player;
        }

        PlayerColor ChoosePlayerColor()
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

        public void Move()
        {
            throw new NotImplementedException();
        }

        public void MoveManual(ICell origin, ICell destination)
        {
            throw new NotImplementedException();
        }

        public void PlacePenguin()
        {
            throw new NotImplementedException();
        }

        public void PlacePenguinManual(int x, int y)
        {
            Cell cell = (Cell)Board.Board[x, y];
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

        public void StartGame()
        {
            Turn = 1;
            CurrentPlayer = Players[0];
            PenguinsByPlayer = NumberOfPenguins();
            NextAction = NextActionType.PlacePenguin;
            StateChanged.Invoke(this, null);
        }

        public void NextPlayer()
        {
            int nextPlayerIndex = Players.IndexOf(CurrentPlayer) + 1;

            if(nextPlayerIndex == Players.Count())
            {
                nextPlayerIndex = 0;
            }

            CurrentPlayer = Players[nextPlayerIndex];
        }
    }
}
