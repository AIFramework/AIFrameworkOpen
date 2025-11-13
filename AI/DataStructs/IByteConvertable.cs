namespace AI.DataStructs
{
    /// <summary>
    /// Объект с поддержкой бинарной сериализации
    /// </summary>
    public interface IByteConvertable
    {
        /// <summary>
        /// Преобразование объекта в массив байт
        /// </summary>
        byte[] GetBytes();
    }

    public static class KeyWords
    {
        public const string Vector = "vect";
        public const string Matrix = "matr";
        public const string Tensor = "tens";
        public const string NDTensor = "ndt";
        public const string NNValue = "nnval";
        public const string ComplexVector = "complvect";
        public const string ComplexMatrix = "complmatr";
    }
}
