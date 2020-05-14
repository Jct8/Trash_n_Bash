using System.Collections.Generic;
using UnityEngine;

namespace SheetCodesEditor
{
    public class CellDrawData
    {
        public readonly Rect cellRect;
        public readonly Rect rightBorderRect;
        public readonly Rect bottomBorderRect;
        public readonly Rect iconRect;

        public readonly bool hasRightBorder;
        public readonly bool hasBottomBorder;
        public readonly int maxAutoWidth;

        public CellDrawData(Rect cellRect, Rect rightBorderRect, Rect bottomBorderRect, Rect iconRect, bool hasRightBorder, bool hasBottomBorder, int maxAutoWidth)
        {
            this.cellRect = cellRect;
            this.rightBorderRect = rightBorderRect;
            this.bottomBorderRect = bottomBorderRect;
            this.iconRect = iconRect;
            this.hasRightBorder = hasRightBorder;
            this.hasBottomBorder = hasBottomBorder;
            this.maxAutoWidth = maxAutoWidth;
        }

        public static CellDrawData[,] GetCellDrawData(LoadedPageData pageData, DrawRange drawRange, int visibleWidth, int visibleHeight)
        {
            List<int> widthDimensions = pageData.widthDimensions;
            List<int> heightDimensions = pageData.heightDimensions;

            int drawRangeX = drawRange.xMax - drawRange.xMin;
            int drawRangeY = drawRange.yMax - drawRange.yMin;

            if (drawRangeX < 0)
                drawRangeX = 0;

            if (drawRangeY < 0)
                drawRangeY = 0;
            CellDrawData[,] cellDrawData = new CellDrawData[drawRangeX + 1, drawRangeY + 1];

            int[] widths = new int[cellDrawData.GetLength(0)];
            int[] heights = new int[cellDrawData.GetLength(1)];

            int lastWidthCutoff = 0;
            int lastHeightCutoff = 0;

            heights[0] = heightDimensions[0] + WindowSettings.BORDER_SIZE;
            widths[0] = widthDimensions[0] + WindowSettings.BORDER_SIZE;

            int visibleWidthRemaining = visibleWidth;
            for (int i = 1; i < cellDrawData.GetLength(0); i++)
            {
                int cellIndex = i + drawRange.xMin;
                int cellWidth = widthDimensions[cellIndex] + WindowSettings.BORDER_SIZE;
                int minCellWidth = Mathf.Min(cellWidth, visibleWidthRemaining);
                widths[i] = minCellWidth;
                visibleWidthRemaining -= minCellWidth;

                if (i == cellDrawData.GetLength(0) - 1 && cellWidth > minCellWidth)
                    lastWidthCutoff = cellWidth - minCellWidth;
            }

            int visibleHeightRemaining = visibleHeight;
            for (int i = 1; i < cellDrawData.GetLength(1); i++)
            {
                int cellIndex = i + drawRange.yMin;
                int cellHeight = heightDimensions[cellIndex] + WindowSettings.BORDER_SIZE;
                int minCellHeight = Mathf.Min(cellHeight, visibleHeightRemaining);
                heights[i] = minCellHeight;
                visibleHeightRemaining -= minCellHeight;

                if (i == cellDrawData.GetLength(1) - 1 && cellHeight > minCellHeight)
                    lastHeightCutoff = cellHeight - minCellHeight;
            }
            int maxWidthStart = visibleWidth + widthDimensions[0];
            int currentX = 0;
            for (int i = 0; i < widths.Length; i++)
            {
                int currentY = WindowSettings.TASKBAR_HEIGHT;
                for (int j = 0; j < heights.Length; j++)
                {
                    int rightBorderSize = WindowSettings.BORDER_SIZE;
                    int bottomBorderSize = WindowSettings.BORDER_SIZE;

                    if (i == widths.Length - 1)
                    {
                        rightBorderSize -= lastWidthCutoff;
                        if (rightBorderSize < 0)
                            rightBorderSize = 0;
                    }

                    if (j == heights.Length - 1)
                    {
                        bottomBorderSize -= lastHeightCutoff;
                        if (bottomBorderSize < 0)
                            bottomBorderSize = 0;
                    }

                    bool hasRightBorder = rightBorderSize > 0;
                    bool hasBottomBorder = bottomBorderSize > 0;

                    Rect cellRect = new Rect();
                    cellRect.x = currentX;
                    cellRect.y = currentY;
                    cellRect.width = widths[i] - rightBorderSize;
                    cellRect.height = heights[j] - bottomBorderSize;

                    Rect iconRect = new Rect();
                    iconRect.x = currentX + widths[i] - rightBorderSize - 14;
                    iconRect.y = currentY + heights[j] - bottomBorderSize - 5;
                    iconRect.width = 12;
                    iconRect.height = 3;

                    Rect rightBorderRect = new Rect();
                    if (hasRightBorder)
                    {
                        rightBorderRect.x = currentX + widths[i] - WindowSettings.BORDER_SIZE;
                        rightBorderRect.y = currentY;
                        rightBorderRect.width = rightBorderSize;
                        rightBorderRect.height = heights[j];
                    }

                    Rect bottomBorderRect = new Rect();
                    if (hasBottomBorder)
                    {
                        bottomBorderRect.x = currentX;
                        bottomBorderRect.y = currentY + heights[j] - WindowSettings.BORDER_SIZE;
                        bottomBorderRect.width = widths[i] - rightBorderSize;
                        bottomBorderRect.height = bottomBorderSize;
                    }
                    int maxAutoWidth = maxWidthStart - currentX;
                    cellDrawData[i, j] = new CellDrawData(cellRect, rightBorderRect, bottomBorderRect, iconRect, hasRightBorder, hasBottomBorder, maxAutoWidth);
                    currentY += heights[j];
                }
                currentX += widths[i];
            }

            return cellDrawData;
        }
    }
}
