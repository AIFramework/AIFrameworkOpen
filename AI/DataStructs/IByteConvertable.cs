namespace AI.DataStructs
{
    /// <summary>
    /// Object with binary serialization support
    /// </summary>
    public interface IByteConvertable
    {
        /// <summary>
        /// Writing an object to a byte array
        /// </summary>
        byte[] GetBytes();
    }

    internal static class KeyWords
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
