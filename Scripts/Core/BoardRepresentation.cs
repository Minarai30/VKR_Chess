namespace Chess {

	public static class BoardRepresentation {
		public const string fileNames = "abcdefgh"; //имена столбцов
		public const string rankNames = "12345678"; //имена строк

		// назначение начальных и конечных индексов
		public const int a1 = 0;
		public const int b1 = 1;
		public const int c1 = 2;
		public const int d1 = 3;
		public const int e1 = 4;
		public const int f1 = 5;
		public const int g1 = 6;
		public const int h1 = 7;

		public const int a8 = 56;
		public const int b8 = 57;
		public const int c8 = 58;
		public const int d8 = 59;
		public const int e8 = 60;
		public const int f8 = 61;
		public const int g8 = 62;
		public const int h8 = 63;

		public static int RankIndex (int squareIndex) // функция для назначения строки
        {
			return squareIndex >> 3;
		}

		
		public static int FileIndex (int squareIndex) // функция для назначения столбца
        {
			return squareIndex & 0b000111;
		}

		public static int IndexFromCoord (int fileIndex, int rankIndex) // функция для определения индекса по координатам
		{
			return rankIndex * 8 + fileIndex;
		}

		public static int IndexFromCoord (Coord coord) // функция определения индекса через класс координат
		{
			return IndexFromCoord (coord.fileIndex, coord.rankIndex);
		}

		public static Coord CoordFromIndex (int squareIndex) // координаты из индекса
		{
			return new Coord (FileIndex (squareIndex), RankIndex (squareIndex));
		}

		public static bool LightSquare (int fileIndex, int rankIndex) // проверка на то, является ли клетка светлой
		{
			return (fileIndex + (rankIndex + 1)) % 2 == 0;
		}

		public static string SquareNameFromCoordinate (int fileIndex, int rankIndex) // получение имени клетки из точных координат
		{
			return fileNames[fileIndex] + "" + (rankIndex + 1);
		}

		public static string SquareNameFromIndex (int squareIndex) // получение имени клетки из индекса
		{
			return SquareNameFromCoordinate (CoordFromIndex (squareIndex));
		}

		public static string SquareNameFromCoordinate (Coord coord) //получение имени клетки из координат
		{
			return SquareNameFromCoordinate (coord.fileIndex, coord.rankIndex);
		}
	}
}