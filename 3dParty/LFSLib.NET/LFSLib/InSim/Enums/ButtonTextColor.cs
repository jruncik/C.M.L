using System;
using System.Collections.Generic;
using System.Text;

namespace FullMotion.LiveForSpeed.InSim.Enums
{
  /// <summary>
  /// Colors for text on <see cref="LfsButton"/> instances. These colors refer to settings
  /// in LFS, i.e. they do define actual colors, but rather color styles. To control explicit
  /// colors, use the color control sequences in the button text
  /// </summary>
  public enum ButtonTextColor
  {
    /// <summary>
    /// not user editable (Default)
    /// </summary>
    LightGrey = 0,
    /// <summary>
    /// LFS default: yellow
    /// </summary>
    TitleColor = 1,
    /// <summary>
    /// LFS default: black
    /// </summary>
    UnselectedText = 2,
    /// <summary>
    /// LFS default: white
    /// </summary>
    SelectedText = 3,
    /// <summary>
    /// LFS default: green
    /// </summary>
    Ok = 4,
    /// <summary>
    /// LFS default: red
    /// </summary>
    Cancel = 5,
    /// <summary>
    /// LFS default: pale blue
    /// </summary>
    TextString = 6,
    /// <summary>
    /// LFS default: grey
    /// </summary>
    Unavailable = 7
  }
}
