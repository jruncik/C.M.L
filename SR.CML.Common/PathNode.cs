/* ------------------------------------------------------------------------- *
 * Copyright (C) 2008-2009 Jaroslav Runcik
 *
 * Jaroslav Runcik <J [dot] Runcik [at] seznam [dot] cz>
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * ------------------------------------------------------------------------- */

using System;
using System.Text;
using System.Runtime.InteropServices;

using System.Diagnostics;

namespace SR.CML.Common
{
	[StructLayout(LayoutKind.Sequential)]
	public struct PathNode
	{
		public Int32 _centreX;
		public Int32 _centreY;
		public Int32 _centreZ;

		public Single _directionX;
		public Single _directionY;
		public Single _directionZ;

		public Single _limitLeft;
		public Single _limitRight;

		public Single _driveLeft;
		public Single _driveRight;

		public PathNode(Byte[] byteBuffer)
		{
			Debug.Assert(byteBuffer.Length==40);

			Int32		length	= byteBuffer.Length;
			IntPtr		ptr		= Marshal.AllocHGlobal(length);
			GCHandle	handle	= GCHandle.Alloc(ptr, GCHandleType.Pinned);

			Marshal.Copy(byteBuffer, 0, ptr, length);
			this = (PathNode)Marshal.PtrToStructure(ptr, typeof(PathNode));

			handle.Free();

#if DEBUG
			CheckBufferRead(byteBuffer);
#endif
		}

		[Conditional("DEBUG")]
		private void CheckBufferRead(Byte[] byteBuffer)
		{
			Debug.Assert(_centreX == BitConverter.ToInt32(byteBuffer, 0));
			Debug.Assert(_centreY == BitConverter.ToInt32(byteBuffer, 4));
			Debug.Assert(_centreZ == BitConverter.ToInt32(byteBuffer, 8));

			Debug.Assert(_directionX == BitConverter.ToSingle(byteBuffer, 12));
			Debug.Assert(_directionY == BitConverter.ToSingle(byteBuffer, 16));
			Debug.Assert(_directionZ == BitConverter.ToSingle(byteBuffer, 20));

			Debug.Assert(_limitLeft		== BitConverter.ToSingle(byteBuffer, 24));
			Debug.Assert(_limitRight	== BitConverter.ToSingle(byteBuffer, 28));

			Debug.Assert(_driveLeft		== BitConverter.ToSingle(byteBuffer, 32));
			Debug.Assert(_driveRight	== BitConverter.ToSingle(byteBuffer, 36));
		}

		public override string ToString()
		{
			StringBuilder pathNodeAsString = new StringBuilder(128);

			pathNodeAsString.Append(_centreX);
			pathNodeAsString.Append('	');
			pathNodeAsString.Append(_centreY);
			pathNodeAsString.Append('	');
			pathNodeAsString.Append(_centreZ);
			pathNodeAsString.Append('	');
			pathNodeAsString.Append(_directionX);
			pathNodeAsString.Append('	');
			pathNodeAsString.Append(_directionY);
			pathNodeAsString.Append('	');
			pathNodeAsString.Append(_directionZ);
			pathNodeAsString.Append('	');
			pathNodeAsString.Append(_limitLeft);
			pathNodeAsString.Append('	');
			pathNodeAsString.Append(_limitRight);
			pathNodeAsString.Append('	');
			pathNodeAsString.Append(_driveLeft);
			pathNodeAsString.Append('	');
			pathNodeAsString.Append(_driveRight);
			pathNodeAsString.Append('	');
			return pathNodeAsString.ToString();
		}
	}
}
