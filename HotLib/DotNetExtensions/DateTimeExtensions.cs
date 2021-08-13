using System;

namespace HotLib.DotNetExtensions
{
    /// <summary>
    /// Contains extension methods for <see cref="DateTime"/>.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Gets the next midnight on the given day of the week.
        /// </summary>
        /// <param name="from">The current <see cref="DateTime"/> to offset.</param>
        /// <param name="dayOfWeek">The day of the week to seek.</param>
        /// <returns>A new <see cref="DateTime"/> set to the next midnight on the given day of the week.</returns>
        public static DateTime GetNext(this DateTime from, DayOfWeek dayOfWeek) =>
            GetNext(from, dayOfWeek, TimeSpan.Zero);

        /// <summary>
        /// Gets the next occurence of the given time of day on the given day of the week.
        /// </summary>
        /// <param name="from">The current <see cref="DateTime"/> to offset.</param>
        /// <param name="dayOfWeek">The day of the week to seek.</param>
        /// <param name="timeOfDay">The time of day to seek.</param>
        /// <returns>A new <see cref="DateTime"/> set to the next occurrence of the given time of day on the given day of the week.</returns>
        public static DateTime GetNext(this DateTime from, DayOfWeek dayOfWeek, TimeSpan timeOfDay)
        {
            var weekStart = from
                .Date
                .AddDays(-(double)from.DayOfWeek);

            var next = weekStart
                .AddDays((double)dayOfWeek)
                .Add(timeOfDay);

            return next > from ?
                next :
                next.AddDays(7);
        }
    }
}
