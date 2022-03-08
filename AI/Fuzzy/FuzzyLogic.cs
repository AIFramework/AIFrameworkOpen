using System;

namespace AI.Fuzzy
{
    /// <summary>
    /// Fuzzy Logic Variable(FLV)
    /// </summary>
    [Serializable]
    public class FLV
    {
        private double _flv;

        /// <summary>
        /// Нечеткая логическая переменная
        /// </summary>
        public double Flv
        {
            get => _flv;
            set
            {
                if (value > 1)
                {
                    _flv = 1;
                }
                else if (value < 0)
                {
                    _flv = 0;
                }
                else
                {
                    _flv = value;
                }
            }

        }
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public FLV()
        {
            _flv = 0;
        }

        /// <summary>
        /// Инициализация double
        /// </summary>
        public static implicit operator FLV(double data) => new FLV(data);
        /// <summary>
        /// Инициализация float
        /// </summary>
        public static implicit operator FLV(float data) => new FLV(data);
        /// <summary>
        /// Инициализация int
        /// </summary>
        public static implicit operator FLV(int data) => new FLV(data);
        /// <summary>
        /// Инициализация double
        /// </summary>
        public static implicit operator double(FLV data) => data._flv;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="flv">численное значение нечеткой переменной</param>
        public FLV(double flv)
        {
            _flv = flv;
        }
        ///Не 
        public static FLV operator !(FLV a)
        {
            return new FLV(1.0 - a._flv);
        }
        ///И
        public static FLV operator &(FLV a, FLV b)
        {
            return new FLV(a._flv * b._flv);
        }
        /// <summary>
        /// Или
        /// </summary>
        public static FLV operator |(FLV a, FLV b)
        {
            return new FLV(a._flv + b._flv - a._flv * b._flv);
        }

        /// <summary>
        /// Импликация Клини
        /// </summary>
        public static FLV KDImplication(FLV @if, FLV then) 
        {
            return Math.Max(1-@if, then);
        }
        
        /// <summary>
        /// Импликация Гогена
        /// </summary>
        public static FLV GImplication(FLV @if, FLV then) 
        {
            return @if > 0 ? Math.Min(1, then / @if) : 1;
        }
    }
}