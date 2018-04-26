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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Diagnostics;

namespace SR.CML.TrackEdit
{
	public partial class FormTrackEditor : Form
	{
		private Track		_track;
		private Point[]		_centerPointsForDrawing	= null;
		private Point[]		_leftPointsForDrawing	= null;
		private Point[]		_rightPointsForDrawing	= null;
		private Point[]		_leftPointsForLimits	= null;
		private Point[]		_rightPointsForLimits	= null;
		private Point[]		_profileForDrawing		= null;

		public FormTrackEditor()
		{
			InitializeComponent();
			_track = new Track();
		}

		private void pictureBox_Paint(object sender, PaintEventArgs e)
		{
			if (_track.Border.IsEmpty) {
				return;
			}

			if (_centerPointsForDrawing==null) {
				List<Point> centerPoints;
				List<Point> leftPointsDrive;
				List<Point> rightPointsDive;
				List<Point> leftPointsLimits;
				List<Point> rightPointsLimits;
				List<Point> profilePoints;
				GeneratePath(out centerPoints, out leftPointsDrive, out rightPointsDive, out leftPointsLimits, out rightPointsLimits, out profilePoints);

				_centerPointsForDrawing	= centerPoints.ToArray();
				_profileForDrawing		= profilePoints.ToArray();

				_leftPointsForDrawing	= leftPointsDrive.ToArray();
				_rightPointsForDrawing	= rightPointsDive.ToArray();

				_leftPointsForLimits	= leftPointsLimits.ToArray();
				_rightPointsForLimits	= rightPointsLimits.ToArray();
			}

			e.Graphics.DrawPolygon(Pens.Red, _centerPointsForDrawing);

			for (Int32 i = 0; i<_leftPointsForDrawing.Length; ++i) {
				e.Graphics.DrawLine(Pens.Blue, _leftPointsForDrawing[i], _rightPointsForDrawing[i]);
			}

			e.Graphics.DrawPolygon(Pens.Black,	_leftPointsForDrawing);
			e.Graphics.DrawPolygon(Pens.Black,	_rightPointsForDrawing);
		}

		private void GeneratePath(out List<Point> centerPoints, out List<Point> leftPointsDrive, out List<Point> rightPointsDive, out List<Point> leftPointsLimits, out List<Point> rightPointsLimits, out List<Point> profilePoints) {
			Int32 left		= _track.Border.Left / UInt16.MaxValue;
			Int32 top		= _track.Border.Top / UInt16.MaxValue;
			Int32 width		= _track.Border.Width / UInt16.MaxValue;
			Int32 height	= _track.Border.Height / UInt16.MaxValue;
			Int32 minHeight	= _track.Profile.Width / UInt16.MaxValue;
			Int32 maxHeight	= _track.Profile.Height / UInt16.MaxValue;

			Int32 profileHeight = maxHeight - minHeight;

			IList<PathNode> path = _track.Path;

			double zoom			= 0.7;
			double profileZoom	= 2;

			Point centerPoint	= Point.Empty;
			centerPoints		= new List<Point>(path.Count);
			leftPointsDrive		= new List<Point>(path.Count);
			rightPointsDive		= new List<Point>(path.Count);
			leftPointsLimits	= new List<Point>(path.Count);
			rightPointsLimits	= new List<Point>(path.Count);

			profilePoints = new List<Point>(path.Count);

			double trackX = 0.0;
			double trackY = 0.0;

			Int32 index = 0;
			double profileY = 0.0;

			double sinAlpha = 0.0;
			double cosAlpha = 0.0;
			double trackZoomed = 0.0;

			foreach (PathNode node in path) {
				trackX = ((node._centreX / (double)UInt16.MaxValue) + width - left) * zoom;
				trackY = ((-node._centreY / (double)UInt16.MaxValue) + height + 20) * zoom;

				profileY = (node._centreZ / (double)UInt16.MaxValue) * profileZoom;

				centerPoint = new Point((Int32)trackX, (Int32)trackY);
				centerPoints.Add(centerPoint);
				profilePoints.Add(new Point(index + 50, (Int32)profileY + height + 100 + (Int32)(profileHeight * profileZoom)));

				sinAlpha = node._directionY;
				cosAlpha = node._directionX;

				trackZoomed = node._driveLeft * zoom;
				trackX = centerPoint.X + (sinAlpha * trackZoomed);
				trackY = centerPoint.Y + (cosAlpha * trackZoomed);
				leftPointsDrive.Add(new Point((Int32)trackX, (Int32)trackY));

				trackZoomed = node._driveRight * zoom;
				trackX = centerPoint.X + (sinAlpha * trackZoomed);
				trackY = centerPoint.Y + (cosAlpha * trackZoomed);
				rightPointsDive.Add(new Point((Int32)trackX, (Int32)trackY));

				sinAlpha = Math.Abs(node._directionY);
				cosAlpha = Math.Abs(node._directionX);

				trackZoomed = node._driveLeft * zoom;
				trackX = centerPoint.X + (sinAlpha * trackZoomed);
				trackY = centerPoint.Y + (cosAlpha * trackZoomed);
				leftPointsLimits.Add(new Point((Int32)trackX, (Int32)trackY));

				trackZoomed = node._driveRight * zoom;
				trackX = centerPoint.X + (sinAlpha * trackZoomed);
				trackY = centerPoint.Y + (cosAlpha * trackZoomed);
				rightPointsLimits.Add(new Point((Int32)trackX, (Int32)trackY));
				++index;
			}
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();

			if (openFileDialog.ShowDialog() == DialogResult.OK) {
				if (_track.LoadPath(openFileDialog.FileName)) {
					_centerPointsForDrawing = null;
					pictureBox.Invalidate();
				}
			}
		}
	}
}
