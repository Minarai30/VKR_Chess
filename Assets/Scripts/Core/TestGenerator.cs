using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess.Game
{
    public class TestGenerator
    {
        List<string> Test; //�������������� ������ �������� �����

        List<int> GenQuestionOrder() //����� ��� ������������ �������� �����
        {
            int TestVariant = Random.Range(1, 6); //������������� 6 ��������� ��������� ���� ��������
            List<int> QuestionOrder = new List<int>(); //������ � �������� �������� � �����

            switch (TestVariant) //��������� ������� �������� �������� ��������
            {
                case 1:
                    QuestionOrder.Add(1);
                    QuestionOrder.Add(2);
                    QuestionOrder.Add(3);
                    break;
                case 2:
                    QuestionOrder.Add(1);
                    QuestionOrder.Add(3);
                    QuestionOrder.Add(2);
                    break;
                case 3:
                    QuestionOrder.Add(2);
                    QuestionOrder.Add(1);
                    QuestionOrder.Add(3);
                    break;
                case 4:
                    QuestionOrder.Add(2);
                    QuestionOrder.Add(3);
                    QuestionOrder.Add(1);
                    break;
                case 5:
                    QuestionOrder.Add(3);
                    QuestionOrder.Add(1);
                    QuestionOrder.Add(2);
                    break;
                case 6:
                    QuestionOrder.Add(3);
                    QuestionOrder.Add(2);
                    QuestionOrder.Add(1);
                    break;
            }

            return QuestionOrder; //���������� �������
        }

        public List<string> GenTest(int TestIndex) //��������� ����� � ���������
        {
            Test = new List<string>(); //�������������� ����� ����
            List<int> TestOrder = GenQuestionOrder(); //�������� �������

            switch (TestIndex) //���������� ������� � ����������� �� ���������� ������� �����
            {
                case 1:
                    for (int i = 0; i <= 2; i++)
                    {
                        Test.Add(GenTest1Position(TestOrder[i]));
                    }

                    return Test;
                case 2:
                    for (int i = 0; i <= 2; i++)
                    {
                        Test.Add(GenTest2Position(TestOrder[i]));
                    }

                    return Test;
                case 3:
                    for (int i = 0; i <= 2; i++)
                    {
                        Test.Add(GenTest3Position(TestOrder[i]));
                    }

                    return Test;
                case 4:
                    for (int i = 0; i <= 2; i++)
                    {
                        Test.Add(GenTest4Position(TestOrder[i]));
                    }

                    return Test;
                case 5:
                    for (int i = 0; i <= 2; i++)
                    {
                        Test.Add(GenTest5Position(TestOrder[i]));
                    }

                    return Test;

            }
            for (int i = 0; i <= 2; i++) //���� �� �����-�� ������� �� ���������� ������������� ����, ���������� ������
            {
                Test.Add(GenTest1Position(TestOrder[i]));
            }

            return Test;
        }

        string GenTest1Position(int Question) //��������� ������� �����
        {
            switch (Question)
            {
                case 1:
                    return "8/8/8/8/4r3/8/8/Q7 w KQkq - 0 1|������ ���������� ����� ������ FEN ��� ���� �����.|3r4, q7|4r3, Q7|3R4, Q7|4r3, q7|2";
                case 2:
                    return "8/8/8/8/b/8/8/8 w KQkq - 0 1|����� ������ ����� � �4?|�����|������|�����|����|4";
                case 3:
                    return "8/8/8/PpPpPpPp/8/8/8/8 w KQkq - 0 1|������ ���������� ������� ��� 5 ������.|pPpPpPpP|PpPpPpPp|���|���|2";
                default:
                    Debug.Log("Unable to generate test position!");
                    break;
            }
            return "8/8/8/8/4r3/8/8/Q7 w KQkq - 0 1|������ ���������� ����� ������ FEN ��� ���� �����.|3r4, q7|4r3, Q7|3R4, Q7|4r3, q7|2"; //���������� ������ ������ � ������ ������
        }

        string GenTest2Position(int Question) //�������
        {
            switch (Question)
            {
                case 1:
                    return "8/r3P3/3K3r/1p6/5q2/8/8/8 w KQkq - 0 1|����� �� ��� ������ ������� �� ����?|��|���|���|���|1";
                case 2:
                    return "8/8/8/8/7q/5nP1/6R1/5B1K w KQkq - 0 1|��� �������, ��� ����� ��� ���� �����. ��� ���?|��|���|���|���|2";
                case 3:
                    return "r1n5/8/8/K1q5/8/8/8/8 w KQkq - 0 1|����� �� ������ ��� ������� ���, ����� �� �������� ���?|��|���|���|���|1";
                default:
                    Debug.Log("Unable to generate test position!");
                    break;
            }
            return "8/r3P3/3K3r/1p6/5q2/8/8/8 w KQkq - 0 1|����� �� ��� ������ ������� �� ����?|��|���|���|���|1";
        }

        string GenTest3Position(int Question) //��������
        {
            switch (Question)
            {
                case 1:
                    return "8/1kp4p/1pp5/3p1R2/1P6/QPKPP3/2P3q1/5b1r w KQkq - 0 1|������ �������� ����� ����� ��������� ����� ��� � ��� ����?|����� � �����|����� � �����|���|���|1";
                case 2:
                    return "1k6/1p6/p5p1/5p2/1PQ2Pn1/8/P1P3P1/4qB1K w KQkq - 0 1|����� ��� ���� �� �� ������, ����� ��������� ��� �����?|�e1-e2, �e2-e1|�e1-h4, �h4-h3|�g4-f2, ��1-h4|��1-f2, �f2xg2|2";
                case 3:
                    return "1k6/p4p2/6b1/Pp2p1p1/PQ1N4/2P1q3/1P6/3K4 w KQkq - 0 1|׸���� ������� ��� �f3-e3, � ����� ���1-d1. ��� ������ ��������� ��� ���?|�e3-d2|�g6-h5|���|���|2";
                default:
                    Debug.Log("Unable to generate test position!");
                    break;
            }
            return "8/1kp4p/1pp5/3p1R2/1P6/QPKPP3/2P3q1/5b1r w KQkq - 0 1|������ �������� ����� ����� ��������� ����� ��� � ��� ����?|����� � �����|����� � �����|���|���|1";
        }

        string GenTest4Position(int Question) //���������
        {
            switch (Question)
            {
                case 1:
                    return "8/k4p/8/7K/8/8/8/8 w KQkq - 0 1|���� �� ����� �����?|��|���|���|���|2";
                case 2:
                    return "8/8/8/2KB4/8/3k5/8/8 w KQkq - 0 1|��-�����, ��� ����� ��������. ��� �������?|�����|������|���|���|1";
                case 3:
                    return "8/8/2PK4/1PKP4/6B1/2P5/5B2/8 w KQkq - 0 1|�������� �� ��, ��� ����� ������� ��������?|��|���|���|���|2";
                default:
                    Debug.Log("Unable to generate test position!");
                    break;
            }
            return "8/k4p/8/7K/8/8/8/8 w KQkq - 0 1|���� �� ����� �����?|��|���|���|���|2";
        }

        string GenTest5Position(int Question) //������
        {
            switch (Question)
            {
                case 1:
                    return "6k1/3q2p1/p2bp2p/3p1r2/1p1Pp3/3bQ1PP/PP1B1rB1/1N2R1RK w KQkq - 0 1|��� �������, �������� �� ��� �������� ����������|��|���|���|���|1";
                case 2:
                    return "8/8/8/3p5/2kPK3/8/8/8 w KQkq - 0 1|��� �������� ��������? ����� �����.|��|���|���|���|1";
                case 3:
                    return "5NbK/5p1P/8/7k/6p1/8/8/8 w KQkq - 0 1|����� �� ��� ������� ���, �� ������� ������?|��|���|���|���|2";
                default:
                    Debug.Log("Unable to generate test position!");
                    break;
            }
            return "8/8/8/3p5/2kPK3/8/8/8 w KQkq - 0 1|��� �������� ��������? ����� �����.|��|���|���|���|1";
        }
    }
}

