namespace KT3_Algorithms
{
    internal class Program
    {
        // Задача 1
        // Для каждого дня нужно найти ближайший день с большей ценой.
        // Выбрал монотонный стек: храним дни,
        // для которых ответ ещё не найден.
        public static int[] FindNextGreater(int[] prices)
        {
            int[] result = new int[prices.Length];
            Stack<int> stack = new Stack<int>();

            for (int i = 0; i < prices.Length; i++)
            {
                while (stack.Count > 0 && prices[i] > prices[stack.Peek()])
                {
                    int index = stack.Pop();
                    result[index] = i - index;
                }

                stack.Push(i);
            }

            return result;
        }


        // Задача 2
        // Нужно найти кратчайший путь по клеткам матрицы.
        // Выбрал BFS: все переходы имеют одинаковую стоимость 1,
        // поэтому BFS находит минимальное количество ходов.
        public static int FindShortestPath(char[][] map)
        {
            int n = map.Length;
            int m = map[0].Length;

            int startRow = 0;
            int startCol = 0;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    if (map[i][j] == 'S')
                    {
                        startRow = i;
                        startCol = j;
                    }
                }
            }

            int[,] distance = new int[n, m];

            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    distance[i, j] = -1;

            Queue<(int row, int col)> queue = new Queue<(int row, int col)>();

            queue.Enqueue((startRow, startCol));
            distance[startRow, startCol] = 0;

            int[] dr = { -1, 1, 0, 0 };
            int[] dc = { 0, 0, -1, 1 };

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                for (int i = 0; i < 4; i++)
                {
                    int nextRow = current.row + dr[i];
                    int nextCol = current.col + dc[i];

                    if (nextRow < 0 || nextRow >= n || nextCol < 0 || nextCol >= m)
                        continue;

                    if (map[nextRow][nextCol] == '#')
                        continue;

                    if (distance[nextRow, nextCol] != -1)
                        continue;

                    distance[nextRow, nextCol] = distance[current.row, current.col] + 1;

                    if (map[nextRow][nextCol] == 'T')
                        return distance[nextRow, nextCol];

                    queue.Enqueue((nextRow, nextCol));
                }
            }

            return -1;
        }


        // Задача 3
        // Каждый узел указывает максимум на один следующий узел.
        // Это можно рассматривать как связный список.
        // Выбрал алгоритм Флойда: два указателя позволяют
        // найти цикл без дополнительной памяти.
        public static bool HasCycle(int[] next, int start)
        {
            int slow = start;
            int fast = start;

            while (fast != -1 && next[fast - 1] != -1)
            {
                slow = next[slow - 1];
                fast = next[next[fast - 1] - 1];

                if (slow == fast)
                    return true;
            }

            return false;
        }


        // Задача 4
        // Ищем максимальный прямоугольник из единиц.
        // Каждую строку превращаем в гистограмму высот,
        // а максимальный прямоугольник в гистограмме
        // ищем монотонным стеком.
        public static int MaximalRectangle(char[][] matrix)
        {
            if (matrix.Length == 0)
                return 0;

            int n = matrix.Length;
            int m = matrix[0].Length;

            int[] heights = new int[m];
            int result = 0;

            for (int row = 0; row < n; row++)
            {
                for (int col = 0; col < m; col++)
                {
                    if (matrix[row][col] == '1')
                        heights[col]++;
                    else
                        heights[col] = 0;
                }

                result = Math.Max(result, LargestRectangle(heights));
            }

            return result;
        }

        private static int LargestRectangle(int[] heights)
        {
            Stack<int> stack = new Stack<int>();
            int result = 0;

            for (int i = 0; i <= heights.Length; i++)
            {
                int currentHeight = i == heights.Length ? 0 : heights[i];

                while (stack.Count > 0 && currentHeight < heights[stack.Peek()])
                {
                    int height = heights[stack.Pop()];

                    int left = stack.Count == 0 ? -1 : stack.Peek();

                    int width = i - left - 1;

                    result = Math.Max(result, height * width);
                }

                stack.Push(i);
            }

            return result;
        }


        // Задача 5
        // Состояние зависит не только от клетки,
        // но и от набора уже собранных ключей.
        // Поэтому BFS идёт по состояниям (строка, столбец, ключи).
        public static int FindShortestPathWithKeys(char[][] map)
        {
            int n = map.Length;
            int m = map[0].Length;

            int startRow = 0;
            int startCol = 0;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    if (map[i][j] == 'S')
                    {
                        startRow = i;
                        startCol = j;
                    }
                }
            }

            int maxMasks = 1024;

            bool[,,] visited = new bool[n, m, maxMasks];

            var queue = new Queue<(int row, int col, int keys, int distance)>();

            queue.Enqueue((startRow, startCol, 0, 0));
            visited[startRow, startCol, 0] = true;

            int[] dr = { -1, 1, 0, 0 };
            int[] dc = { 0, 0, -1, 1 };

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                for (int i = 0; i < 4; i++)
                {
                    int nextRow = current.row + dr[i];
                    int nextCol = current.col + dc[i];

                    if (nextRow < 0 || nextRow >= n ||
                        nextCol < 0 || nextCol >= m)
                        continue;

                    char cell = map[nextRow][nextCol];

                    if (cell == '#')
                        continue;

                    int keys = current.keys;

                    if (cell >= 'A' && cell <= 'J')
                    {
                        int key = cell - 'A';

                        if ((keys & (1 << key)) == 0)
                            continue;
                    }

                    if (cell >= 'a' && cell <= 'j')
                    {
                        int key = cell - 'a';
                        keys |= 1 << key;
                    }

                    if (visited[nextRow, nextCol, keys])
                        continue;

                    if (cell == 'T')
                        return current.distance + 1;

                    visited[nextRow, nextCol, keys] = true;

                    queue.Enqueue((nextRow, nextCol, keys, current.distance + 1));
                }
            }

            return -1;
        }


        // Задача 6
        // Массив можно рассматривать как связный список:
        // значение nums[i] указывает на следующий индекс.
        // Повторяющееся число образует цикл.
        // Выбрал алгоритм Флойда, чтобы использовать O(1) памяти.
        public static int FindDuplicate(int[] nums)
        {
            int slow = nums[0];
            int fast = nums[0];

            do
            {
                slow = nums[slow];
                fast = nums[nums[fast]];
            }
            while (slow != fast);

            slow = nums[0];

            while (slow != fast)
            {
                slow = nums[slow];
                fast = nums[fast];
            }

            return slow;
        }


        static void Main(string[] args)
        {
            // Задача 1
            int[] prices = { 73, 74, 75, 71, 69, 72, 76, 73 };
            Console.WriteLine(string.Join(" ", FindNextGreater(prices)));

            // Задача 2
            char[][] map =
            {
                "S..#".ToCharArray(),
                ".#..".ToCharArray(),
                "...T".ToCharArray()
            };

            Console.WriteLine(FindShortestPath(map));

            // Задача 3
            int[] next = { 2, 3, 4, 2 };

            Console.WriteLine(HasCycle(next, 1));

            // Задача 4
            char[][] matrix =
            {
                "10100".ToCharArray(),
                "10111".ToCharArray(),
                "11111".ToCharArray(),
                "10010".ToCharArray()
            };

            Console.WriteLine(MaximalRectangle(matrix));
            

            // Задача 5
            char[][] keyMap =
            {
                "S.a".ToCharArray(),
                ".#A".ToCharArray(),
                "..T".ToCharArray()
            };

            Console.WriteLine(FindShortestPathWithKeys(keyMap));

            // Задача 6
            int[] nums = { 1, 3, 4, 2, 2 };

            Console.WriteLine(FindDuplicate(nums));
        }
    }
}