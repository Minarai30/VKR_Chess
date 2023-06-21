public class PieceList {
	public int[] occupiedSquares; //массив занятых клеток
	int[] map; //битовая карта клеток
	int numPieces; //количество фигур

	public PieceList (int maxPieceCount = 16) //конструктор списка фигур
	{
		occupiedSquares = new int[maxPieceCount]; //создаем массив занятых фигур
		map = new int[64]; //создаем битовую карту клеток
		numPieces = 0; //количество фигур
	}

	public int Count //считаем количество фигур
	{
		get 
		{
			return numPieces;
		}
	}

	public void AddPieceAtSquare (int square) //добавление фигуры в клетку
	{
		occupiedSquares[numPieces] = square; //добавляем клетку в список занятых
		map[square] = numPieces; //вставляем в битовую карту фигуру
		numPieces++; //увеличиваем количество фигур в списке
	}

	public void RemovePieceAtSquare (int square) //удаление фигуры из клетки
	{
		int pieceIndex = map[square]; //забираем индекс из карты
		occupiedSquares[pieceIndex] = occupiedSquares[numPieces - 1]; //двигаем массив на единицу
		map[occupiedSquares[pieceIndex]] = pieceIndex; //обновляем карту
		numPieces--; //уменьшаем количество фигур
	}

	public void MovePiece (int startSquare, int targetSquare) //метода совершения хода
	{
		int pieceIndex = map[startSquare]; //забираем индекс из карты
		occupiedSquares[pieceIndex] = targetSquare; //меняем занятую клетку
		map[targetSquare] = pieceIndex; //вставляем индекс в новую клетку
	}

	public int this [int index] => occupiedSquares[index]; //метод получения индекса

}