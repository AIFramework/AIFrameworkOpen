using AI.DataStructs.Algebraic;

namespace AI.Controls.ComplexObjectControl.Base
{
    /// <summary>
    /// Сложный объект (API для доступа к физ. объекту/диф. модель)
    /// </summary>
    public abstract class ComplexObjectBase
    {
        /// <summary>
        /// Возвращает вектор состояния объекта
        /// </summary>
        /// <param name="action">Вектор управляющего воздействия</param>
        /// <returns></returns>
        public abstract Vector GetState(Vector action);
    }
}
