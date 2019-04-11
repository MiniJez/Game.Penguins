using Game.Penguins.Core.Interfaces.Game.Actions;
using Game.Penguins.Core.Interfaces.Game.GameBoard;
using Game.Penguins.Core.Interfaces.Game.Players;
using System;
using System.Collections.Generic;

namespace Game.Penguins
{
    class FakeClasses : IGame
    {
		private List<PlayerColor> availableColor;
        public FakeClasses()
        {
            Board = new FakeBoard();
            Players = new List<IPlayer>();

			// init the list of available colors with all colors
			availableColor = new List<PlayerColor>();
			for (int i = 0; i < 4; i++)
			{
				availableColor.Add((PlayerColor)i);
			}
		}

        /// <summary>
        /// Current state of the board
        /// </summary>
        public IBoard Board { get; }

        /// <summary>
        /// Next action that the UI must manage
        /// </summary>
        public NextActionType NextAction { get; }

        /// <summary>
        /// Get informations about the current user that needs to play
        /// </summary>
        public IPlayer CurrentPlayer { get; }

        /// <summary>
        /// Informations about players
        /// </summary>
        public IList<IPlayer> Players { get; }

        /// <summary>
        /// Fired when the state has changed (Current player, ...)
        /// </summary>
        public event EventHandler StateChanged;

        /// <summary>
        /// Add a new player on the game
        /// Must be called before StartGame()
        /// </summary>
        /// <param name="playerName"></param>
        /// <param name="playerType"></param>
        public IPlayer AddPlayer(string playerName, PlayerType playerType)
        {
			Random r = new Random();
			//PlayerColor color = new PlayerColor();

			Random random = new Random();	// new random
			int number = random.Next(0, availableColor.Count-1);	// new random between 0 and the length of availableColor
			PlayerColor tmpColor = new PlayerColor();	// new temp color
			tmpColor = availableColor[number];	//	the temp color take ine value in the list
			availableColor.RemoveAt(number);	//	the value is removed in the list
			
			IPlayer player = new FakePlayer(playerName, playerType, tmpColor);
            Players.Add(player);
            return player;
        }

        /// <summary>
        /// Start the game
        /// Do not forget to add players before strting the game
        /// </summary>
        public void StartGame()
        {

        }

        /// <summary>
        /// Place a penguin for the current user
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void PlacePenguinManual(int x, int y)
        {

        }

        /// <summary>
        /// Call the AI to place his penguin
        /// </summary>
        /// <param name="playerName"></param>
        /// <param name="playerType"></param>
        public void PlacePenguin()
        {

        }

        /// <summary>
        /// Execute a move for the current user if it's a human
        /// </summary>
        /// <param name="player"></param>
        /// <param name="action"></param>
        public void MoveManual(ICell origin, ICell destination)
        {

        }

        /// <summary>
        /// Execute a move for an AI
        /// </summary>
        /// <param name="player"></param>
        /// <param name="action"></param>
        public void Move()
        {

        }
    }
}
