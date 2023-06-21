using UnityEngine;

namespace Chess
{
    public class Task
	{
		public static int randIndex;

		public static int GenerateIndex(int startingPosition, int Condition, bool SecondLesson, bool Horizontal, int Row_Col)
        {
			if (Condition == 1)
            {
				randIndex = Random.Range(0, 6);
				return randIndex = startingPosition + 8 * randIndex;
            }

			if (Condition == 2)
            {
				if (startingPosition % 2 == 0)
				{
					while (!(randIndex % 2 == 0)) randIndex = Random.Range(0, 63);
					if (7 - (randIndex / 8) % 2 == 0) randIndex -= 1;
					Debug.Log(randIndex);
					return randIndex;
				}
				else
                {
					while (randIndex % 2 == 0) randIndex = Random.Range(0, 63);
					if (7 - (randIndex / 8) % 2 != 0) randIndex -= 1;
					//Debug.Log(randIndex);
					return randIndex;
                }
			}

			/*if (SecondLesson)
            {
				if (Horizontal)
                {

                }
            }*/
			return randIndex = Random.Range(0, 63);
		}

		public static string GenerateTask(int randIndex)
		{
			return BoardRepresentation.SquareNameFromIndex(randIndex);
		}
	}
}