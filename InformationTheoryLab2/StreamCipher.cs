using System;
using System.Text;

namespace InformationTheoryLab2
{
    public class StreamCipher
    {
        private bool[] register = new bool[38];

        private readonly int[] feedbackTaps = { 0, 32, 33, 37 };

        public void SetInitialState(string initialState)
        {
            for (int i = 0; i < 38; i++)
            {
                char c = initialState[i];
                register[i] = (c == '1');
            }
        }

        public (string result, string keyStream) Process(string inputBits)
        {
            if (string.IsNullOrEmpty(inputBits))
            {
                return ("", "");
            }

            StringBuilder resultBuilder = new StringBuilder(inputBits.Length);
            StringBuilder keyBuilder = new StringBuilder(inputBits.Length);

            for (int i = 0; i < inputBits.Length; i++)
            {
                bool keyBit = register[0];
                keyBuilder.Append(keyBit ? '1' : '0');

                bool inputBit = inputBits[i] == '1';
                bool outputBit = inputBit ^ keyBit;
                resultBuilder.Append(outputBit ? '1' : '0');

                bool feedback = register[feedbackTaps[0]];
                for (int j = 1; j < feedbackTaps.Length; j++)
                {
                    feedback ^= register[feedbackTaps[j]];
                }

                for (int j = 0; j < 37; j++)
                {
                    register[j] = register[j + 1];
                }

                register[37] = feedback;
            }

            return (resultBuilder.ToString(), keyBuilder.ToString());
        }
    }
}