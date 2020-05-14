using UnityEngine;

namespace SheetCodesEditor
{
    public static class WindowSettings
    {
        public const int MINWIDTH_COLLECTIONEDIT = 242 + BORDER_SIZE * 2;
        public const int MINWIDTH_DATADISPLAY = 750;
        public const int MINHEIGTH_WINDOW = 300;
        public const int WIDTH_SELECTIONEDIT = 300;
        public const int MAXWIDTH_INITALIZATION = 300;
        public const int MAXHEIGHT_INITALIZATION = 300;
        public const int WIDTH_PADDING = 5;
        public const int HEIGHT_PADDING = 5;
        public const int ONE_LINE_HEIGHT = 13;
        public const int MINWIDTH = 50;
        public const int MAXWIDTH_MAINCONTENT = 800;
        public const int MAXWIDTH_LOCKEDCONTENT = 300;
        public const int SCROLLBAR_SIZE = 10;
        public const int SCROLLBAR_REALSIZE = 16;
        public const int SCROLLBAR_HALFSIZE = SCROLLBAR_SIZE / 2;
        public const int MINHEIGHT = ONE_LINE_HEIGHT + HEIGHT_PADDING;
        public const int MAXHEIGHT_MAINCONTENT = 1200;
        public const int MAXHEIGHT_LOCKEDCONTENT = MINHEIGHT * 3;
        public const int BORDER_SIZE = 4;
        public const int COMPONENT_ADDED_WIDTH = 50;
        public const int TASKBAR_SEPARATOR_HEIGHT = 8;
        public const int EMPTY_ROW_HEIGHT = MINHEIGHT + BORDER_SIZE;
        public const int EMPTY_COLUMN_WIDTH = 100;
        public const int TASKBAR_HEIGHT = TASKBAR_SEPARATOR_HEIGHT + BUTTONROW_HEIGHT;
        public const int BUTTONROW_HEIGHT = ONE_LINE_HEIGHT + HEIGHT_PADDING;
        public const int LABELWIDTH_COLLECTIONEDIT = 35;
        public const int SCROLLSPEED_COLLECTIONEDIT = MINHEIGHT * 3;
        public const string EDITORPREFS_KEY_SELECTIONEDIT = "Closed While Selection Edit Open";
        public const string EDITORPREFS_KEY_HEIGHTDIMENSIONS = "Sheet {0} HeightDimensions";
        public const string EDITORPREFS_KEY_WIDTHDIMENSIONS = "Sheet {0} WidthDimensions";
        public const string EDITORPREFS_KEY_COLLECTIONEDIT_WIDTH = "Collection Edit Width";
        public const string EDITORPREFS_KEY_LAST_OPENED_SHEET = "Last Opened Sheet";
        public const int OVERFLOW_WIDTH_MARGIN = 5;
        public const int OVERFLOW_HEIGHT_MARGIN = 5;
        public const int OVERFLOW_ICON_SIZE = 10;
        public const int OVERFLOW_ICON_HALFSIZE = OVERFLOW_ICON_SIZE / 2;
        public static GUIStyle GUI_STYLE { get { return GUI.skin.label; } }
    }
}