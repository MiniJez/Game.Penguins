using Microsoft.VisualStudio.TestTools.UnitTesting;
using Game.Penguins.Core.Interfaces.Game.GameBoard;
using Game.Penguins.Core.Interfaces.Game.Players;
using Newtonsoft.Json;
using System;

namespace Game.Penguins.UnitTest
{
    [TestClass]
    public class Test_GameClass
    {
        [TestMethod]
        public void Test_NumbersOfPenguins()
        {
            GameClass game = new GameClass();

            Assert.AreEqual(game.PenguinsByPlayer, 0);

            game.AddPlayer("player1", PlayerType.AIEasy);
            game.AddPlayer("player2", PlayerType.AIEasy);
            game.StartGame();

            Assert.AreEqual(game.PenguinsByPlayer, 4);
        }

        [TestMethod]
        public void Test_AddPlayer()
        {
            GameClass game = new GameClass();

            Assert.AreEqual(game.Players.Count, 0);

            game.AddPlayer("player1", PlayerType.AIEasy);
            game.AddPlayer("player2", PlayerType.Human);
            game.AddPlayer("player2", PlayerType.AIMedium);

            Assert.AreEqual(game.Players.Count, 3);
            Assert.AreEqual(game.Players[0].PlayerType, PlayerType.AIEasy);
            Assert.AreEqual(game.Players[1].PlayerType, PlayerType.Human);
            Assert.AreEqual(game.Players[2].PlayerType, PlayerType.AIMedium);
        }

        [TestMethod]
        public void Test_ChoosePlayerColor()
        {
            GameClass game = new GameClass();
            PlayerColor color = game.ChoosePlayerColor();

            Assert.AreEqual(color, PlayerColor.Blue);

            game.AddPlayer("player1", PlayerType.AIEasy);
            color = game.ChoosePlayerColor();

            Assert.AreEqual(color, PlayerColor.Green);

            game.AddPlayer("player2", PlayerType.Human);
            color = game.ChoosePlayerColor();

            Assert.AreEqual(color, PlayerColor.Red);
        }

        [TestMethod]
        public void Test_PlacePenguins()
        {
            GameClass game = new GameClass();
            game.AddPlayer("player1", PlayerType.AIEasy);
            game.AddPlayer("player1", PlayerType.AIEasy);
            game.StartGame();

            Cell[,] oldBoard = new Cell[8, 8];
            string strOldBoard = JsonConvert.SerializeObject(game.Board.Board);
            oldBoard = JsonConvert.DeserializeObject<Cell[,]>(strOldBoard);

            do
            {
                game.PlacePenguin();
            } while (game.NextAction == NextActionType.PlacePenguin);

            bool areEquals = true;
            int compteur = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (game.Board.Board[i, j].CellType != oldBoard[i, j].CellType)
                    {
                        compteur++;
                        areEquals = false;
                    }
                }
            }

            Assert.AreEqual(compteur, 8);
            Assert.IsTrue(areEquals == false);
        }

        [TestMethod]
        public void Test_Move()
        {
            GameClass game = new GameClass();
            game.AddPlayer("player1", PlayerType.AIEasy);
            game.AddPlayer("player1", PlayerType.AIEasy);
            game.StartGame();

            do
            {
                game.PlacePenguin();
            } while (game.NextAction == NextActionType.PlacePenguin);

            Cell[,] oldBoard = new Cell[8, 8];
            string strOldBoard = JsonConvert.SerializeObject(game.Board.Board);
            oldBoard = JsonConvert.DeserializeObject<Cell[,]>(strOldBoard);

            game.Move();

            bool areEquals = true;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (game.Board.Board[i, j].CellType != oldBoard[i, j].CellType && game.Board.Board[i, j].CurrentPenguin != null)
                    {
                        areEquals = false;
                    }
                }
            }
            
            Assert.IsTrue(areEquals == false);
        }

        [TestMethod]
        public void Test_PlacePenguinsManual()
        {
            GameClass game = new GameClass();
            game.AddPlayer("player1", PlayerType.Human);
            game.AddPlayer("player1", PlayerType.Human);
            game.StartGame();

            Cell[,] oldBoard = new Cell[8, 8];
            string strOldBoard = JsonConvert.SerializeObject(game.Board.Board);
            oldBoard = JsonConvert.DeserializeObject<Cell[,]>(strOldBoard);

            Random rnd = new Random();

            do
            {
                game.PlacePenguinManual(rnd.Next(8), rnd.Next(8));
            } while (game.NextAction == NextActionType.PlacePenguin);

            bool areEquals = true;
            int compteur = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (game.Board.Board[i, j].CellType != oldBoard[i, j].CellType)
                    {
                        compteur++;
                        areEquals = false;
                    }
                }
            }

            Assert.AreEqual(compteur, 8);
            Assert.IsTrue(areEquals == false);
        }

        [TestMethod]
        public void Test_MovePenguinsManual()
        {
            GameClass game = new GameClass();
            game.AddPlayer("player1", PlayerType.Human);
            game.AddPlayer("player1", PlayerType.Human);
            game.StartGame();

            Random rnd = new Random();

            do
            {
                game.PlacePenguinManual(rnd.Next(8), rnd.Next(8));
            } while (game.NextAction == NextActionType.PlacePenguin);

            Cell[,] oldBoard = new Cell[8, 8];
            string strOldBoard = JsonConvert.SerializeObject(game.Board.Board);
            oldBoard = JsonConvert.DeserializeObject<Cell[,]>(strOldBoard);

            game.MoveManual(game.Board.Board[rnd.Next(8), rnd.Next(8)], game.Board.Board[rnd.Next(8), rnd.Next(8)]);

            bool areEquals = true;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (game.Board.Board[i, j].CellType != oldBoard[i, j].CellType)
                    {
                        areEquals = false;
                    }
                }
            }
            
            Assert.IsTrue(areEquals == false);
        }
    }
}
