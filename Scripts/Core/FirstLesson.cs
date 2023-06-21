using UnityEngine;

namespace Chess.Game
{
    public class FirstLesson
    {
        int File;
        int Rank;
        static Coord FirstLessonCoord;
        PositionGenerator PosGen = new PositionGenerator();

        void CoordGen() //генерация координат задачи
        {
            string PlayerPiece = PosGen.GetPlayerPiece().ToUpper(); //превращаем фигуру игрока в белую для простоты проверок
            
            if (PlayerPiece != "P" || PlayerPiece != "B") //если у игрока не пешка и не слон
            {
                File = Random.Range(0, 7);
                Rank = Random.Range(0, 7);

                FirstLessonCoord = new Coord(File, Rank); //просто генерируем любые координаты
            }

            if (PlayerPiece == "P") //если у игрока пешка
            {
                Coord PlayerCoord = PosGen.GetPlayerCoord(); //забираем координаты игрока
                int PlayerFile = PlayerCoord.fileIndex;
                int PlayerRank = PlayerCoord.rankIndex;

                if (PosGen.GetPlayerPiece() == "P") Rank = Random.Range(PlayerRank + 1, 7); //убираем возможность генерации целевой клетки за спиной пешки

                else Rank = Random.Range(5, PlayerRank + 1); //инвертируем прошлую операцию для чёрных

                FirstLessonCoord = new Coord(PlayerFile, Rank); //генерируем координаты
            }

            if (PlayerPiece == "B") //у игрока слон
            {
                File = Random.Range(0, 7);
                Rank = Random.Range(0, 7);

                Coord PlayerCoord = PosGen.GetPlayerCoord(); //забираем координаты игрока

                if (BoardRepresentation.LightSquare(File, Rank) == !BoardRepresentation.LightSquare(PlayerCoord.fileIndex, PlayerCoord.rankIndex)) FirstLessonCoord = new Coord(File, Rank); //контролируем, чтобы цвет клетки слона и цвет целевой клетки совпадали
                else FirstLessonCoord = new Coord(File + 1, Rank); //генерируем координаты
            }
        }
        public static Coord FirstLessonGenerator() //метод-посредник для передачи целевой клетки игровому менеджеру
        {
            
            FirstLesson CoordGen = new FirstLesson();
            CoordGen.CoordGen();
            return FirstLessonCoord;
        }

        public static bool PositionChecker(Move move) //проверка позиции фигуры игрока
        {
            if (move.TargetSquare == BoardRepresentation.IndexFromCoord(FirstLessonCoord)) return true; //если позиция игрока совпадает с целевой клеткой, завершаем урок
            else return false; //иначе не завершаем
        }
    }
}

