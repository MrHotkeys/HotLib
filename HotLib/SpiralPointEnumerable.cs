using System;
using System.Collections;
using System.Collections.Generic;

namespace HotLib
{
    public struct SpiralPointEnumerable : IEnumerable<(int x, int y)>
    {
        public enum Heading
        {
            Up,
            Down,
            Left,
            Right,
        }

        public int Width { get; }

        public int Height { get; }

        private sbyte AxesX { get; }

        private sbyte AxesY { get; }

        public SpiralPointEnumerable(int radius, sbyte axesX = 1, sbyte axesY = -1)
            : this(radius, radius, axesX, axesY)
        { }

        public SpiralPointEnumerable(int width, int height, sbyte axesX = 1, sbyte axesY = -1)
        {
            if (width <= 0)
                throw new ArgumentException("Must be positive!", nameof(width));
            Width = width;

            if (height <= 0)
                throw new ArgumentException("Must be positive!", nameof(height));
            Height = height;

            if (Math.Abs(axesX) != 1)
                throw new ArgumentException("Axis values must be 1 or -1 to indicate behavior as we move right!", nameof(axesX));
            AxesX = axesX;

            if (Math.Abs(axesY) != 1)
                throw new ArgumentException("Axis values must be 1 or -1 to indicate behavior as we move up!", nameof(axesY));
            AxesY = axesY;
        }

        public IEnumerator<(int x, int y)> GetEnumerator()
        {
            var heading = Heading.Down;
            var distance = 1;
            var turns = 0;
            var currentX = 0;
            var currentY = 0;

            var centerX = AxesX > 0 ?
                          (Width - 1) / 2 :
                          Width / 2;
            var centerY = AxesY > 0 ?
                          Height / 2 :
                          (Height - 1) / 2;

            var count = Width * Height;

            while (count > 0) // TODO
            {
                var positionX = currentX + centerX;
                var positionY = currentY + centerY;
                if (positionX >= 0 && positionX < Width &&
                    positionY >= 0 && positionY < Height)
                {
                    yield return (positionX, positionY);
                }

                switch (heading)
                {
                    case Heading.Up:
                        currentY += AxesY;
                        break;
                    case Heading.Down:
                        currentY -= AxesY;
                        break;
                    case Heading.Left:
                        currentX -= AxesX;
                        break;
                    case Heading.Right:
                        currentX += AxesX;
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                distance--;
                if (distance <= 0)
                {
                    switch (heading)
                    {
                        case Heading.Up:
                            heading = Heading.Left;
                            break;
                        case Heading.Down:
                            heading = Heading.Right;
                            break;
                        case Heading.Left:
                            heading = Heading.Down;
                            break;
                        case Heading.Right:
                            heading = Heading.Up;
                            break;
                        default:
                            throw new InvalidOperationException();
                    }

                    turns++;

                    distance = (turns / 2) + 1;
                }

                count--;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
