using UnityEngine;

namespace Chess.Game {
	[CreateAssetMenu (menuName = "Theme/Pieces")] //указываем на папку с изображениями фигур
	public class PieceTheme : ScriptableObject 
	{
		//создаём классы для белых и чёрных фигур
		public PieceSprites whitePieces;
		public PieceSprites blackPieces;

		public Sprite GetPieceSprite (int piece) //метод для вывода изображения для фигуры
		{
			PieceSprites pieceSprites = Piece.IsColour (piece, Piece.White) ? whitePieces : blackPieces; //определяем цвет

			switch (Piece.PieceType (piece)) // проходим по фигурам
			{
				case Piece.Pawn: //пешка
					return pieceSprites.pawn;
				case Piece.Rook: //ладья
					return pieceSprites.rook;
				case Piece.Knight: //конь
					return pieceSprites.knight;
				case Piece.Bishop: //офицер
					return pieceSprites.bishop;
				case Piece.Queen: //ферзь
					return pieceSprites.queen;
				case Piece.King: //король
					//return null;
					return pieceSprites.king;
				default: //если не смогли определить фигуру
					if (piece != 0) 
					{
						Debug.Log (piece); //если в клетке была фигура, то выводим её название
					}
					return null; //ничего не выводим
			}
		}

		[System.Serializable]
		public class PieceSprites //класс для изображений фигур
		{
			public Sprite pawn, rook, knight, bishop, queen, king; //объявляем изображения для каждого типа фигур

			public Sprite this [int i] //формируем массив изображений
			{
				get 
				{
					return new Sprite[] { pawn, rook, knight, bishop, queen, king }[i];
				}
			}
		}
	}
}