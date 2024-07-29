namespace MatchThreeEngine
{
    /// <summary>
    /// Представляет ход в игре "три в ряд", указывая начальную и конечную позиции.
    /// </summary>
    public sealed class MoveController
    {
        /// <summary>
        /// Получает координату X начальной позиции.
        /// </summary>
        public readonly int X1;

        /// <summary>
        /// Получает координату Y начальной позиции.
        /// </summary>
        public readonly int Y1;

        /// <summary>
        /// Получает координату X конечной позиции.
        /// </summary>
        public readonly int X2;

        /// <summary>
        /// Получает координату Y конечной позиции.
        /// </summary>
        public readonly int Y2;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Move"/>.
        /// </summary>
        /// <param name="x1">Координата X начальной позиции.</param>
        /// <param name="y1">Координата Y начальной позиции.</param>
        /// <param name="x2">Координата X конечной позиции.</param>
        /// <param name="y2">Координата Y конечной позиции.</param>
        public MoveController(int x1 = 0, int y1 = 0, int x2 = 0, int y2 = 0)
        {
            // Опционально можно проверить координаты, если есть определенные ограничения
            // Например, если координаты должны быть неотрицательными или в пределах определенных границ
            // if (x1 < 0 || y1 < 0 || x2 < 0 || y2 < 0)
            // {
            //     throw new ArgumentOutOfRangeException("Координаты должны быть неотрицательными.");
            // }

            X1 = x1;
            Y1 = y1;

            X2 = x2;
            Y2 = y2;
        }
    }
}