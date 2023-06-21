using UnityEngine;

namespace Chess.Game
{
    public class FirstLesson
    {
        int File;
        int Rank;
        static Coord FirstLessonCoord;
        PositionGenerator PosGen = new PositionGenerator();

        void CoordGen() //��������� ��������� ������
        {
            string PlayerPiece = PosGen.GetPlayerPiece().ToUpper(); //���������� ������ ������ � ����� ��� �������� ��������
            
            if (PlayerPiece != "P" || PlayerPiece != "B") //���� � ������ �� ����� � �� ����
            {
                File = Random.Range(0, 7);
                Rank = Random.Range(0, 7);

                FirstLessonCoord = new Coord(File, Rank); //������ ���������� ����� ����������
            }

            if (PlayerPiece == "P") //���� � ������ �����
            {
                Coord PlayerCoord = PosGen.GetPlayerCoord(); //�������� ���������� ������
                int PlayerFile = PlayerCoord.fileIndex;
                int PlayerRank = PlayerCoord.rankIndex;

                if (PosGen.GetPlayerPiece() == "P") Rank = Random.Range(PlayerRank + 1, 7); //������� ����������� ��������� ������� ������ �� ������ �����

                else Rank = Random.Range(5, PlayerRank + 1); //����������� ������� �������� ��� ������

                FirstLessonCoord = new Coord(PlayerFile, Rank); //���������� ����������
            }

            if (PlayerPiece == "B") //� ������ ����
            {
                File = Random.Range(0, 7);
                Rank = Random.Range(0, 7);

                Coord PlayerCoord = PosGen.GetPlayerCoord(); //�������� ���������� ������

                if (BoardRepresentation.LightSquare(File, Rank) == !BoardRepresentation.LightSquare(PlayerCoord.fileIndex, PlayerCoord.rankIndex)) FirstLessonCoord = new Coord(File, Rank); //������������, ����� ���� ������ ����� � ���� ������� ������ ���������
                else FirstLessonCoord = new Coord(File + 1, Rank); //���������� ����������
            }
        }
        public static Coord FirstLessonGenerator() //�����-��������� ��� �������� ������� ������ �������� ���������
        {
            
            FirstLesson CoordGen = new FirstLesson();
            CoordGen.CoordGen();
            return FirstLessonCoord;
        }

        public static bool PositionChecker(Move move) //�������� ������� ������ ������
        {
            if (move.TargetSquare == BoardRepresentation.IndexFromCoord(FirstLessonCoord)) return true; //���� ������� ������ ��������� � ������� �������, ��������� ����
            else return false; //����� �� ���������
        }
    }
}

