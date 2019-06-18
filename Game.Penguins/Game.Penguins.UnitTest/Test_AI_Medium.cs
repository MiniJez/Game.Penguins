using Game.Penguins.Core.Interfaces.Game.Players;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Game.Penguins.UnitTest
{
	[TestClass]
	class Test_AI_Medium
	{
		[TestMethod]
		public void Test_PlacePenguin()
		{
			GameClass game = new GameClass();
			game.AddPlayer("player1", PlayerType.AIMedium);
			game.AddPlayer("player2", PlayerType.AIMedium);
			game.ChoosePlayerColor();
			game.StartGame();

			game.AI_medium.PlacePenguin(game.Board, game.CurrentPlayer, game.AIPenguins);
			game.AI_medium.PlacePenguin(game.Board, game.CurrentPlayer, game.AIPenguins);

			foreach (Cell cell in game.Board.Board)
			{
				
			}


		}
	}
}
