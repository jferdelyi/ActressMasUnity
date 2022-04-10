using System;

namespace ActressMasWrapper {

    public class Tools {
        // Number of created agents
        protected static long msTotalCount = 0;

        // Global random generator
        public static Random Rand { get; protected set; } = new();

        // Get random position
        public static int[] RandomPermutation(int pNumber) {
            // Fisher-Yates shuffle
            int[] lNumbers = new int[pNumber];
            for (int i = 0; i < pNumber; i++) {
                lNumbers[i] = i;
            }
            while (pNumber > 1) {
                int k = Rand.Next(pNumber--);
                int temp = lNumbers[pNumber]; lNumbers[pNumber] = lNumbers[k]; lNumbers[k] = temp;
            }
            return lNumbers;
        }

        // Create agent name
        public static string CreateName() {
            return "Agent[" + ++msTotalCount + "]";
        }
    }
}
