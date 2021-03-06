using Microsoft.VisualStudio.TestTools.UnitTesting;
using Game.Penguins.Core.Interfaces.Game.GameBoard;
using Game.Penguins.Core.Interfaces.Game.Players;
using System.Collections.Generic;


namespace Game.Penguins.AI.UnitTests
{
    [TestClass]
    public class Test_AI_Easy
    {
        [TestMethod]
        public void Test_PlacePenguins()
        {
            GameClass game = new GameClass();
            game.AddPlayer("player1", PlayerType.AIEasy);
            game.AddPlayer("player2", PlayerType.AIEasy);
            game.ChoosePlayerColor();
            game.StartGame();

            int Initcompteur = 0;
            int CompareCompteur = 0;

            foreach (Cell cell in game.Board.Board)
            {
                if (cell.CellType == CellType.FishWithPenguin && cell.CurrentPenguin.Player == game.CurrentPlayer)
                    Initcompteur++;
            }

            game.AI_easy.PlacePenguins((BoardClass)game.Board, (PlayerClass)game.CurrentPlayer);

            foreach (Cell cell in game.Board.Board)
            {
                if (cell.CellType == CellType.FishWithPenguin && cell.CurrentPenguin.Player == game.CurrentPlayer)
                    CompareCompteur++;
            }

            Assert.IsTrue(CompareCompteur == Initcompteur + 1);
        }

        [TestMethod]
        public void Test_GetAvailablePlacementCell()
        {
            GameClass game = new GameClass();
            game.AddPlayer("player1", PlayerType.AIEasy);
            game.AddPlayer("player2", PlayerType.AIEasy);
            game.ChoosePlayerColor();
            game.StartGame();

            List<Cell> cellList = game.AI_easy.GetAvailablePlacementCell((BoardClass)game.Board);

            foreach (Cell cell in cellList)
            {
                Assert.IsTrue(cell.CellType == CellType.Fish);
                Assert.IsTrue(cell.FishCount == 1);
            }
        }

        [TestMethod]
        public void Test_GetMyPenguinsCell()
        {
            GameClass game = new GameClass();
            game.AddPlayer("player1", PlayerType.AIEasy);
            game.AddPlayer("player2", PlayerType.AIEasy);
            game.ChoosePlayerColor();
            game.StartGame();

            List<Cell> cellList = game.AI_easy.GetMyPenguinsCell((BoardClass)game.Board, (PlayerClass)game.CurrentPlayer);

            foreach (Cell cell in cellList)
            {
                Assert.IsTrue(cell.CellType == CellType.FishWithPenguin);
                Assert.IsTrue(cell.CurrentPenguin.Player == game.CurrentPlayer);
            }
        }

        [TestMethod]
        public void Test_SearchIndexOfCell()
        {
            GameClass game = new GameClass();
            Cell cell = (Cell)game.Board.Board[2, 5];

            int[] index = game.AI_easy.SearchIndexOfCell(cell, (BoardClass)game.Board);

            Assert.AreEqual(2, index[0]);
            Assert.AreEqual(5, index[1]);
        }

        [TestMethod]
        public void Test_FindAvailableCellNeighbor()
        {
            GameClass game = new GameClass();
            Cell start = (Cell)game.Board.Board[2, 5];

            List<Cell> listNeighbor = game.AI_easy.FindAvailableCellNeighbor(start, (BoardClass)game.Board);
            List<int[]> index = new List<int[]>();

            foreach (Cell cell in listNeighbor)
            {
                index.Add(game.AI_easy.SearchIndexOfCell(cell, (BoardClass)game.Board));
            }

            foreach (int[] ind in index)
            {
                Assert.IsTrue(ind[0] == 1 || ind[0] == 2 || ind[0] == 3);
                Assert.IsTrue(ind[1] == 4 || ind[1] == 5 || ind[1] == 6);
            }

            start = (Cell)game.Board.Board[0, 0];
            listNeighbor = game.AI_easy.FindAvailableCellNeighbor(start, (BoardClass)game.Board);
            index.RemoveRange(0, index.Count);

            foreach (Cell cell in listNeighbor)
            {
                index.Add(game.AI_easy.SearchIndexOfCell(cell, (BoardClass)game.Board));
            }

            foreach (int[] ind in index)
            {
                Assert.IsTrue(ind[0] == 0 || ind[0] == 1);
                Assert.IsTrue(ind[1] == 0 || ind[1] == 1);
            }
        }

        [TestMethod]
        public void Test_MovePenguins()
        {
            GameClass game = new GameClass();
            game.AddPlayer("player1", PlayerType.AIEasy);
            game.AddPlayer("player2", PlayerType.AIEasy);
            game.ChoosePlayerColor();
            game.StartGame();

            Assert.IsTrue(game.AI_easy.MovePenguins((BoardClass)game.Board, (PlayerClass)game.CurrentPlayer));

            game.AI_easy.PlacePenguins((BoardClass)game.Board, (PlayerClass)game.CurrentPlayer);

            Assert.IsFalse(game.AI_easy.MovePenguins((BoardClass)game.Board, (PlayerClass)game.CurrentPlayer));

            List<Cell> listCellPenguins = game.AI_easy.GetMyPenguinsCell((BoardClass)game.Board, (PlayerClass)game.CurrentPlayer);
            List<Cell> listNeighbor = game.AI_easy.FindAvailableCellNeighbor(listCellPenguins[0], (BoardClass)game.Board);
            int compteur = listNeighbor.Count - 1;

            for (int i = 0; i < compteur; i++)
            {
                listNeighbor[i].CellType = CellType.Water;
                listNeighbor[i].FishCount = 0;
            }

            int[] indexToCompare = game.AI_easy.SearchIndexOfCell(listNeighbor[listNeighbor.Count - 1], (BoardClass)game.Board);
            game.AI_easy.MovePenguins((BoardClass)game.Board, (PlayerClass)game.CurrentPlayer);
            listCellPenguins = game.AI_easy.GetMyPenguinsCell((BoardClass)game.Board, (PlayerClass)game.CurrentPlayer);
            int[] indexPenguin = game.AI_easy.SearchIndexOfCell(listCellPenguins[listCellPenguins.Count - 1], (BoardClass)game.Board);

            Assert.AreEqual(indexPenguin[0], indexToCompare[0]);
            Assert.AreEqual(indexPenguin[1], indexToCompare[1]);
        }
    }
}
