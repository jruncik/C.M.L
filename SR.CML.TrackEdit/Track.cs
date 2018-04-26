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
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using System.Diagnostics;

namespace SR.CML.TrackEdit
{
	public class Track : IDisposable
	{
		private bool	_disposed	= false;
		private Int32	_finishLine	= 0;

		private Rectangle _border = Rectangle.Empty;
		public Rectangle Border
		{
			get { return _border; }
		}

		private Size _size = Size.Empty;
		public Size Profile
		{
			get { return _size; }
		}

		private List<PathNode> _path = null;
		public IList<PathNode> Path
		{
			get {
				if (_path==null) {
					return null;
				}

				return _path.AsReadOnly();
			}
		}


		~Track()
		{
			Debug.Assert(_disposed, "Track, call dispose before the application is closed!");
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!_disposed) {
				if (disposing) {
					if (_path!=null) {
						_path.Clear();
						_path = null;
					}
				}
				_disposed = true;
			}
		}

		public bool LoadPath(String fileName)
		{
			if (String.IsNullOrEmpty(fileName)) {
				return false;
			}

			try {
				using (FileStream fs = new FileStream(fileName, FileMode.Open)) {
					byte[] trackInfo = new byte[16];
					fs.Read(trackInfo, 0, trackInfo.Length);

					if (trackInfo[6]!=0x00 || trackInfo[7]!=0x00) {
						Debug.Assert(false, "Wrong path file format");
						return false;
					}

					Int32 numberNodes = BitConverter.ToInt32(trackInfo, 8);
					_finishLine = BitConverter.ToInt32(trackInfo, 12);

					if (_path==null) {
						_path = new List<PathNode>(numberNodes);
					} else {
						_path.Clear();
						_path.Capacity = numberNodes;
					}

					Int32		minHeight	= Int32.MaxValue;
					Int32		maxHeight	= Int32.MinValue;
					Int32		left		= Int32.MaxValue;
					Int32		right		= Int32.MinValue;
					Int32		top			= Int32.MaxValue;
					Int32		bottom		= Int32.MinValue;
					PathNode	node;
					byte[]		nodeBlock	= new byte[40];

					for (Int32 i=0; i<numberNodes; ++i) {

						fs.Read(nodeBlock, 0, 40);
						node = new PathNode(nodeBlock);
						_path.Add(node);

						if (node._centreX>right) {
							right = node._centreX;
						}

						if (node._centreX<left) {
							left = node._centreX;
						}

						if (node._centreY>bottom) {
							bottom = node._centreY;
						}

						if (node._centreY<top) {
							top = node._centreY;
						}

						if (node._centreZ>maxHeight) {
							maxHeight = node._centreZ;
						}

						if (node._centreZ<minHeight) {
							minHeight = node._centreZ;
						}

					}

					_border	= new Rectangle(left, top, right, bottom);
					_size	= new Size(minHeight, maxHeight);
				}

				return true;
			} catch (Exception ex) {
				Debug.Assert(false, ex.Message);
			}
			return false;
		}

	}
}
