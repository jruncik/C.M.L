using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using FullMotion.LiveForSpeed.InSim;
using FullMotion.LiveForSpeed.InSim.Enums;

namespace FullMotion.LiveForSpeed
{
  public enum FmrButton
  {
    SimulatorWarningButton,
    SimulatorMessageButton,
    RaceStatRangeStart = 2,
    RaceStatRangeEnd = RaceStatRangeStart + 27,
  }

  public class RaceStatsDisplay
  {
    #region log4net -------------------------------------------------------------------------------
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    #endregion

    public const byte LEFT = 0;
    public const byte LABEL_WIDTH = 16;
    public const byte SPLIT_WIDTH = 12;
    public const byte TIME_WIDTH = 15;
    public const byte ROW_HEIGHT = 7;

    public const ButtonTextColor BEST_COLOR = ButtonTextColor.Ok;
    public const ButtonTextColor DELTA_PLUS_COLOR = ButtonTextColor.Cancel;
    public const ButtonTextColor DELTA_MINUS_COLOR = ButtonTextColor.Ok;
    public const ButtonTextColor DEFAULT_COLOR = ButtonTextColor.SelectedText;


    const byte TOP = 20;
    const int LAPS = 3;
    const int SPLITS = 3;

    InSimHandler handler;
    byte connectionId;
    StatsDisplayLapRow[] lapRows;
    StatsDisplayLapDeltaRow currentLapRow;
    StatsDisplaySplitDeltaRow[] splitRows;
    LfsButton headerRow;
    LfsButton currentLapHeaderRow;
    LfsButton currentLapHeaderRowLabel;
    LfsButton firstLabel;
    LfsButton bestLabel;

    public RaceStatsDisplay(InSimHandler handler, byte connectionId)
    {
      this.handler = handler;
      this.connectionId = connectionId;
      InitButtons();
    }

    private void InitButtons()
    {
      byte top = TOP;
      byte startId = (byte)FmrButton.RaceStatRangeStart;

      headerRow = new LfsButton(startId++);
      headerRow.Left = LEFT;
      headerRow.Top = top;
      headerRow.Height = ROW_HEIGHT;
      headerRow.Width = (byte)(LABEL_WIDTH + TIME_WIDTH);
      headerRow.Color = ButtonColor.Dark;
      headerRow.TextAlignment = ButtonTextAlignment.Center;
      headerRow.TextColor = DEFAULT_COLOR;
      headerRow.Text = "Racing Statistics";
      top += headerRow.Height;

      currentLapHeaderRow = new LfsButton(startId++);
      currentLapHeaderRow.Left = LEFT;
      currentLapHeaderRow.Width = headerRow.Width;
      currentLapHeaderRow.Height = ROW_HEIGHT;
      currentLapHeaderRow.Color = headerRow.Color;
      currentLapHeaderRow.TextAlignment = ButtonTextAlignment.Left;
      currentLapHeaderRow.TextColor = DEFAULT_COLOR;

      currentLapHeaderRowLabel = new LfsButton(startId++);
      currentLapHeaderRowLabel.Left = LEFT;
      currentLapHeaderRowLabel.Width = LABEL_WIDTH;
      currentLapHeaderRowLabel.Height = ROW_HEIGHT;
      currentLapHeaderRowLabel.TextAlignment = ButtonTextAlignment.Right;
      currentLapHeaderRowLabel.TextColor = DEFAULT_COLOR;

      firstLabel = new LfsButton(startId++);
      firstLabel.Left = LEFT;
      firstLabel.Top = top;
      firstLabel.Width = LABEL_WIDTH;
      firstLabel.Height = ROW_HEIGHT;
      firstLabel.Text = "Lap";
      firstLabel.TextAlignment = FullMotion.LiveForSpeed.InSim.Enums.ButtonTextAlignment.Left;
      firstLabel.Color = FullMotion.LiveForSpeed.InSim.Enums.ButtonColor.Transparent;
      firstLabel.TextColor = DEFAULT_COLOR;

      bestLabel = new LfsButton(startId++);
      bestLabel.Left = LEFT;
      bestLabel.Width = LABEL_WIDTH;
      bestLabel.Height = ROW_HEIGHT;
      bestLabel.Text = "Best";
      bestLabel.TextAlignment = FullMotion.LiveForSpeed.InSim.Enums.ButtonTextAlignment.Left;
      bestLabel.Color = FullMotion.LiveForSpeed.InSim.Enums.ButtonColor.Transparent;
      bestLabel.TextColor = BEST_COLOR;

      lapRows = new StatsDisplayLapRow[LAPS];
      for (int i = 0; i < LAPS; i++)
      {
        lapRows[i] = new StatsDisplayLapRow(handler, connectionId, startId, top);
        startId += lapRows[i].ButtonCount;
        top += lapRows[i].Height;
      }

      splitRows = new StatsDisplaySplitDeltaRow[SPLITS];
      for (int i = 0; i < SPLITS; i++)
      {
        splitRows[i] = new StatsDisplaySplitDeltaRow(handler, connectionId, startId);
        startId += splitRows[i].ButtonCount;
      }

      currentLapRow = new StatsDisplayLapDeltaRow(handler, connectionId, startId);
      startId += currentLapRow.ButtonCount;

      log.DebugFormat("next startId is {0} and range endId is {1}", startId, (byte)FmrButton.RaceStatRangeEnd);
    }

    public void Render(List<LapData> lapData, int bestLapIdx)
    {
      log.Debug("starting render");
      log.Debug("LapData: " + lapData.Count);
      handler.DeleteButton(bestLabel, connectionId);
      handler.DeleteButton(firstLabel, connectionId);
      for (int i = 0; i < SPLITS; i++)
      {
        splitRows[i].Hide();
      }
      currentLapRow.Hide();

      handler.ShowButton(headerRow, connectionId);

      byte top = (byte)(headerRow.Top + headerRow.Height);
      byte firstTop = lapRows[0].Top;
      byte bestTop = 0;

      if (lapData.Count > 1)
      {
        log.Debug("checking on block data");
        int blockIdx = 0;
        int lapDataIdx = 0;
        int lastLapDataIdx = lapData.Count - 2; //1 for count to index, 1 since the last it never in the block
        bool hideFirst = false;


        if (!lapData[lapData.Count - 1].LapTime.HasValue)
        {
          log.Debug("current lap in progress, so another lap is taken from the block");
          lastLapDataIdx--;
        }
        if (lastLapDataIdx < 0)
        {
          log.Debug("previous lap is handled outside block and no laps are left for the block");
        }
        else
        {
          log.Debug("got block data");
          lapDataIdx = (lastLapDataIdx + 1) - LAPS;
          lapDataIdx = (lapDataIdx < 0) ? 0 : lapDataIdx;

          if (bestLapIdx < lapDataIdx)
          {
            // best is our first entry before laps
            log.Debug("best is our first entry before regular lap block");
            lapRows[0].Show(lapData[bestLapIdx].Lap, lapData[bestLapIdx].LapTime.Value, true);
            bestTop = firstTop;
            top = (byte)(lapRows[0].Top + lapRows[0].Height);
            firstTop = top;
            lapDataIdx++;
            blockIdx++;
          }
          else
          {
            // best is part of our standard block
            log.Debug("best may be part of our lap block");
          }

          log.Debug("blockIdx: " + blockIdx);
          log.DebugFormat("lapdata starts at {0} and ends at {1}", lapDataIdx, lastLapDataIdx);
          for (; lapDataIdx <= lastLapDataIdx; lapDataIdx++, blockIdx++)
          {
            bool best = false;
            if (lapDataIdx == bestLapIdx)
            {
              best = true;
              bestTop = lapRows[blockIdx].Top;
              if (blockIdx == 0)
              {
                hideFirst = true;
              }
            }
            lapRows[blockIdx].Show(lapData[lapDataIdx].Lap, lapData[lapDataIdx].LapTime.Value, best);
            top = (byte)(lapRows[blockIdx].Top + lapRows[blockIdx].Height);
          }

          // show first label
          if (!hideFirst)
          {
            log.Debug("showing firstLabel @ " + firstTop);
            firstLabel.Top = firstTop;
            handler.ShowButton(firstLabel, connectionId);
          }
        }
      }

      // Show split block
      log.Debug("split block logic");
      LapData current = lapData[lapData.Count - 1];
      LapData previous = null;
      if (lapData.Count > 1)
      {
        previous = lapData[lapData.Count - 2];
      }

      if (!current.LapTime.HasValue && previous != null)
      {
          log.Debug("handling previous lap data");
          LapData prePrevious = null;
          if (lapData.Count > 2)
          {
            prePrevious = lapData[lapData.Count - 3];
          }
          TimeSpan delta = new TimeSpan(0);
          if (prePrevious != null)
          {
            delta = previous.LapTime.Value - prePrevious.LapTime.Value;
          }
          bool best = (lapData.Count - 2 == bestLapIdx);
          currentLapRow.Show(previous.Lap, previous.LapTime.Value, delta, best, top);
          if (best)
          {
            bestTop = top;
          }
          top += currentLapRow.Height;
      }

      if (previous != null)
      {
        currentLapHeaderRow.Top = top;
        currentLapHeaderRowLabel.Top = top;
        currentLapHeaderRowLabel.Text = current.Lap.ToString();
        handler.ShowButton(currentLapHeaderRow, connectionId);
        handler.ShowButton(currentLapHeaderRowLabel, connectionId);
        top += currentLapHeaderRow.Height;
      }

      for (int i = 0; i < SPLITS; i++)
      {
        if (current.Splits[i].HasValue)
        {
          log.Debug("showing split " + (i + 1));
          TimeSpan delta = new TimeSpan(0);
          if (previous != null)
          {
            delta = current.Splits[i].Value - previous.Splits[i].Value;
          }
          splitRows[i].Show(i + 1, current.Splits[i].Value, delta, top);
          top += splitRows[i].Height;
        }
      }
      bool currentIsBest = false;
      if (current.LapTime.HasValue)
      {
        log.Debug("showing lap total");
        TimeSpan delta = new TimeSpan(0);
        if (previous != null)
        {
          delta = current.LapTime.Value - previous.LapTime.Value;
        }
        currentIsBest = (lapData.Count - 1 == bestLapIdx);
        currentLapRow.Show(0, current.LapTime.Value, delta, currentIsBest, top);
        top += currentLapRow.Height;
      }
      if (!currentIsBest && lapData.Count > 1)
      {
        // show best label
        log.Debug("showing bestLabel @ " + bestTop);
        bestLabel.Top = bestTop;
        handler.ShowButton(bestLabel, connectionId);
      }
    }
  }
}
