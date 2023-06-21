using System;
using System.Collections.Generic;

namespace Chess.Game
{
    public class TestManager
    {
        List<string> GenTest = new List<string>(); //�������������� ������ �������� �������
        TestGenerator TestGenerator = new TestGenerator(); //��������� ��������� ������

        Tuple<string, string, string, string, string, string, string> QuestionGiver(int Question) //����� ��� ������������ �������
        {
            Tuple<string, string, string, string, string, string, string> TestQuestion; //������ � ���������� ������� �������
            string[] TestSplitter; //������ ����� ��� ���������� ����� ������ �������

            TestSplitter = GenTest[Question].Split('|'); //��������� ����� ������
            TestQuestion = Tuple.Create(TestSplitter[0], TestSplitter[1], TestSplitter[2], TestSplitter[3], TestSplitter[4], TestSplitter[5], TestSplitter[6]); //��������� ������
            
            return TestQuestion; //���������� ������������ ����� ������
        }

        public List<Tuple<string, string, string, string, string, string, string>> StartTest(int TestIndex) //������������� �����
        {
            GenTest = TestGenerator.GenTest(TestIndex); //���������� ����� ������� �����

            List<Tuple<string, string, string, string, string, string, string>> Test = new List<Tuple<string, string, string, string, string, string, string>>(); //��������� ������ ��������

            Test.Add(QuestionGiver(0)); //��������� ��� ���������
            Test.Add(QuestionGiver(1));
            Test.Add(QuestionGiver(2));


            return Test; //���������� ������� ����
        }

    }
}
    
