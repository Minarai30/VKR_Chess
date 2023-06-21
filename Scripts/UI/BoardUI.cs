using System.Collections;
using UnityEngine;

namespace Chess.Game {
	public class BoardUI : MonoBehaviour 
	{
		public PieceTheme pieceTheme; //загружаем изображения фигур
		public BoardTheme boardTheme; //загружаем тему доски
		public bool showLegalMoves; //переключатель показа верхних ходов

		public bool whiteIsBottom = true; //отображение игрока в нижней части

		//подключение рендереров изображений
		MeshRenderer[, ] squareRenderers;
		SpriteRenderer[, ] squarePieceRenderers;

		//вспомогательные объекты для отображения сделанных ходов
		Move lastMadeMove;
		MoveGenerator moveGenerator;

		//константы для отображения фигур
		const float pieceDepth = -0.1f;
		const float pieceDragDepth = -0.2f;

		void Awake () //при начале новой игры запускаем генератор ходов и создаём отображение доски
		{
			moveGenerator = new MoveGenerator ();
			CreateBoardUI ();
		}

		public void HighlightLegalMoves (Board board, Coord fromSquare) //метод для отображения возможных ходов
		{
			if (showLegalMoves) //проверка на то, включена ли эта опция
			{
				var moves = moveGenerator.GenerateMoves (board); //генерируем возможные ходы

				for (int i = 0; i < moves.Count; i++) //проходим по сгенерированным ходам 
				{
					Move move = moves[i]; //сохраняем ходы в массив
					if (move.StartSquare == BoardRepresentation.IndexFromCoord (fromSquare)) //забираем те, которые начинаются из клетки с выбранной фигурой
					{
						Coord coord = BoardRepresentation.CoordFromIndex (move.TargetSquare); //получаем координаты клеток, в которые можно сделать ход
						SetSquareColour (coord, boardTheme.lightSquares.legal, boardTheme.darkSquares.legal); //подсвечиваем эти клетки
					}
				}
			}
		}

		public void DragPiece (Coord pieceCoord, Vector2 mousePos) //визуализация переноса фигур
		{
			squarePieceRenderers[pieceCoord.fileIndex, pieceCoord.rankIndex].transform.position = new Vector3 (mousePos.x, mousePos.y, pieceDragDepth);
		}

		public void ResetPiecePosition (Coord pieceCoord) //возвращение фигуры на место в случае попытки некорректного хода
		{
			Vector3 pos = PositionFromCoord (pieceCoord.fileIndex, pieceCoord.rankIndex, pieceDepth);
			squarePieceRenderers[pieceCoord.fileIndex, pieceCoord.rankIndex].transform.position = pos;
		}

		public void SelectSquare (Coord coord) //выбор клетки
		{
			SetSquareColour (coord, boardTheme.lightSquares.selected, boardTheme.darkSquares.selected);
		}

		public void DeselectSquare (Coord coord) //отмена выбора клетки
		{
			ResetSquareColours ();
		}

		public bool TryGetSquareUnderMouse (Vector2 mouseWorld, out Coord selectedCoord) //получение клетки, на которую кликнули
		{
			//определение столбцов и строк
			int file = (int) (mouseWorld.x + 4);
			int rank = (int) (mouseWorld.y + 4);
			if (!whiteIsBottom) //если игрок не снизу, инвертируем столбцы и строки
			{
				file = 7 - file;
				rank = 7 - rank;
			}
			selectedCoord = new Coord (file, rank); //формируем координаты кликнутой клетки
			return file >= 0 && file < 8 && rank >= 0 && rank < 8; //возвращаем то, корректна ли выделенная клетка
		}

		public void UpdatePosition (Board board) //обновление позиций после хода
		{
			for (int rank = 0; rank < 8; rank++) 
			{
				for (int file = 0; file < 8; file++) 
				{
					Coord coord = new Coord (file, rank); //создаем координаты
					int piece = board.Square[BoardRepresentation.IndexFromCoord (coord.fileIndex, coord.rankIndex)]; //ищем фигуру
					squarePieceRenderers[file, rank].sprite = pieceTheme.GetPieceSprite (piece); //вставляем её изображение
					squarePieceRenderers[file, rank].transform.position = PositionFromCoord (file, rank, pieceDepth);
				}
			}

		}

		public void OnMoveMade (Board board, Move move) //во время совершения хода
		{
			lastMadeMove = move; //записываем ход как последний совершенный

			UpdatePosition (board); //обновляем позиции
			ResetSquareColours (); //сбрасываем подсветки
		}

		void HighlightMove (Move move) //подсветка совершенного хода
		{
			if (!((move.StartSquare == 0 && move.TargetSquare == 1) || (move.StartSquare == 1 && move.TargetSquare == 0))) //подсвечиваем только те клетки, которые участвовали в ходе
			{
				SetSquareColour(BoardRepresentation.CoordFromIndex(move.StartSquare), boardTheme.lightSquares.moveFromHighlight, boardTheme.darkSquares.moveFromHighlight); //реализуем подсветку в обе стороны
				SetSquareColour(BoardRepresentation.CoordFromIndex(move.TargetSquare), boardTheme.lightSquares.moveToHighlight, boardTheme.darkSquares.moveToHighlight);
			}
		}

		void CreateBoardUI () {
			//подключаем шейдеры и рендереры
			Shader squareShader = Shader.Find ("Unlit/Color");
			squareRenderers = new MeshRenderer[8, 8];
			squarePieceRenderers = new SpriteRenderer[8, 8];

			for (int rank = 0; rank < 8; rank++) 
			{
				for (int file = 0; file < 8; file++) 
				{
					// создание клеток
					Transform square = GameObject.CreatePrimitive (PrimitiveType.Quad).transform; //создаём примитив-квадрат
					square.parent = transform;
					square.name = BoardRepresentation.SquareNameFromCoordinate (file, rank); //указываем имя клетки
					square.position = PositionFromCoord (file, rank, 0); //её координаты
					Material squareMaterial = new Material (squareShader); //назначаем клетке материал

					squareRenderers[file, rank] = square.gameObject.GetComponent<MeshRenderer> (); //визуализируем
					squareRenderers[file, rank].material = squareMaterial; //применяем материал

					//вывод фигур
					SpriteRenderer pieceRenderer = new GameObject ("Piece").AddComponent<SpriteRenderer> (); //подключаем рендерер изображений
					pieceRenderer.transform.parent = square; //указываем клетку как родительский объект
					pieceRenderer.transform.position = PositionFromCoord (file, rank, pieceDepth); //получаем позицию
					pieceRenderer.transform.localScale = Vector3.one * 100 / (2000 / 6f); //указываем размер фигуры
					squarePieceRenderers[file, rank] = pieceRenderer; //выводим её
				}
			}

			ResetSquareColours ();
		}

		void ResetSquarePositions () //метод сброса позиций
		{
			for (int rank = 0; rank < 8; rank++) 
			{
				for (int file = 0; file < 8; file++) 
				{
					squareRenderers[file, rank].transform.position = PositionFromCoord (file, rank, 0); //сбрасываем клетки
					squarePieceRenderers[file, rank].transform.position = PositionFromCoord (file, rank, pieceDepth); //и фигуры
				}
			}

			if (!lastMadeMove.IsInvalid) //если последний ход был корректен
			{
				HighlightMove (lastMadeMove); //подсвечиваем его
			}
		}

		public void SetPerspective (bool whitePOV) //установка перспективы игрока. Изначально игрок находится снизу
		{
			whiteIsBottom = whitePOV;
			ResetSquarePositions ();

		}

		public void ResetSquareColours (bool highlight = true) //метод сброса подсветок
		{
			for (int rank = 0; rank < 8; rank++) 
			{
				for (int file = 0; file < 8; file++) 
				{
					SetSquareColour (new Coord (file, rank), boardTheme.lightSquares.normal, boardTheme.darkSquares.normal); //восстанавливаем цвета
				}
			}
			if (highlight) //оставляем подсветку последнего хода, если он верный
			{
				if (!lastMadeMove.IsInvalid) 
				{
					HighlightMove (lastMadeMove);
				}
			}
		}

		void SetSquareColour (Coord square, Color lightCol, Color darkCol) //установка цвета клетки
		{
			squareRenderers[square.fileIndex, square.rankIndex].material.color = (square.IsLightSquare ()) ? lightCol : darkCol;
		}

		public Vector3 PositionFromCoord (int file, int rank, float depth = 0) //получение позиции из координат
		{
			if (whiteIsBottom) 
			{
				return new Vector3 (-3.5f + file, -3.5f + rank, depth);
			}
			return new Vector3 (-3.5f + 7 - file, 7 - rank - 3.5f, depth);

		}

		public Vector3 PositionFromCoord (Coord coord, float depth = 0) //альтернативное получение позиции через класс с координатами
		{
			return PositionFromCoord (coord.fileIndex, coord.rankIndex, depth);
		}

	}
}