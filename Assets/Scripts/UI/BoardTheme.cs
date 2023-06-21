using UnityEngine;

namespace Chess.Game {
	[CreateAssetMenu (menuName = "Theme/Board")] //указываем на папку с темами доски
	public class BoardTheme : ScriptableObject 
	{
		//классы для цветов клеток
		public SquareColours lightSquares;
		public SquareColours darkSquares;

		[System.Serializable]
		public struct SquareColours //структура для различных подсветок клеток - например, подсветки возможных ходов фигуры или сделанного хода
		{
			public Color normal; //нет подсветки
			public Color legal; //отображение возможных ходов
			public Color selected; //отображение клетки с выбранной фигурой
			public Color moveFromHighlight; //подсветка клетки, из которой началось движение
			public Color moveToHighlight; //подсветка клетки, на которой закончилось движение
		}
	}
}