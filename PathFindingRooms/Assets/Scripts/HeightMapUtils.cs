using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class HeightMapUtils {
    private struct IntPair {
        public int a;
        public int b;

        public IntPair(int a, int b) {
            this.a = a;
            this.b = b;
        }
    }

    public class HeightMapNodeChoice {
        public int row;
        public int col;
        public float cost;

        public HeightMapNodeChoice(int row, int col, float cost) {
            this.row = row;
            this.col = col;
            this.cost = cost;
        }
    }

    public static List<HeightMapNodeChoice> getUnexploredNeighbors(float[,] heightMap, bool[,] explored, int row, int col) {

        List<HeightMapNodeChoice> neighbors = new List<HeightMapNodeChoice>();

        if (row >= 0 && col >= 0 && row < heightMap.GetLength(0) && col < heightMap.GetLength(1)) {
            foreach (IntPair p in getNeighborPositions(row, col)) {
                int r = p.a;
                int c = p.b;
                if (r >= 0 && c >= 0 && r < heightMap.GetLength(0) && c < heightMap.GetLength(1)) {
                    if (!explored[r, c]) {
                        neighbors.Add(new HeightMapNodeChoice(r, c, 0));
                    }
                }
            }

            explored[row, col] = true;
        }

        return neighbors;
    }

    private static IEnumerable<IntPair> getNeighborPositions(int row, int col) {
        yield return new IntPair(row + 1, col);
        yield return new IntPair(row - 1, col);
        yield return new IntPair(row, col + 1);
        yield return new IntPair(row, col + 1);
    }
}
