using System;
using System.Collections.Generic;

namespace Chess.Game
{
    public class TestManager
    {
        List<string> GenTest = new List<string>(); //инициализируем список тестовых заданий
        TestGenerator TestGenerator = new TestGenerator(); //запускаем генератор тестов

        Tuple<string, string, string, string, string, string, string> QuestionGiver(int Question) //метод для формирования вопроса
        {
            Tuple<string, string, string, string, string, string, string> TestQuestion; //кортеж с текстовыми данными вопроса
            string[] TestSplitter; //массив строк для разделения сырой строки вопроса

            TestSplitter = GenTest[Question].Split('|'); //разделяем сырую строку
            TestQuestion = Tuple.Create(TestSplitter[0], TestSplitter[1], TestSplitter[2], TestSplitter[3], TestSplitter[4], TestSplitter[5], TestSplitter[6]); //формируем кортеж
            
            return TestQuestion; //возвращаем получившийся набор данных
        }

        public List<Tuple<string, string, string, string, string, string, string>> StartTest(int TestIndex) //инициализатор теста
        {
            GenTest = TestGenerator.GenTest(TestIndex); //генерируем сырые вопросы теста

            List<Tuple<string, string, string, string, string, string, string>> Test = new List<Tuple<string, string, string, string, string, string, string>>(); //формируем список кортежей

            Test.Add(QuestionGiver(0)); //заполняем его вопросами
            Test.Add(QuestionGiver(1));
            Test.Add(QuestionGiver(2));


            return Test; //возвращаем готовый тест
        }

    }
}
    
