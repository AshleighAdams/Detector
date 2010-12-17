/*
 * ---------------------------------------------------
 *      Copyright 2010 Mitchel Collins - XiaTek
 *                http://XiaTek.org
 *     Released Open Source under the BSD License
 *     Please report any bugs to c0bra@xiatek.org
 * ---------------------------------------------------
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using Detector.Motion;
using Detector.Tracking;

namespace Detector.Motion
{
    /// <summary>
    /// Data of a target
    /// </summary>
    public class Target
    {
        /// <summary>
        /// Set the infomation for the target
        /// </summary>
        /// <param name="x">Postion X the target starts</param>
        /// <param name="y">Postion Y the target starts</param>
        /// <param name="size_x">The width of the target</param>
        /// <param name="size_y">The length of the target</param>
        public Target(int x, int y, int size_x, int size_y)
        {
            this._x = x;
            this._y = y;
            this._size_x = size_x;
            this._size_y = size_y;
        }
        public byte[,] Shape;
        public void ShapeDetection()
        {
        }


        #region Properties
        public int X
        {
            get
            {
                return this._x;
            }
            set
            {
                this._x = value;
            }
        }
        public int Y
        {
            get
            {
                return this._y;
            }
            set
            {
                this._y = value;
            }
        }
        public int SizeX
        {
            get
            {
                return this._size_x;
            }
            set
            {
                this._size_x = value;
            }
        }
        public int SizeY
        {
            get
            {
                return this._size_y;
            }
            set
            {
                this._size_y = value;
            }
        }
        #endregion
        private int _x = 0;
        private int _y = 0;
        private int _size_x = 0;
        private int _size_y = 0;
    }
}
