using System;

namespace SheetCodesEditor
{
    public class SheetCell
    {
        public object startingData;
        public object data;

        public bool dirty
        {
            get
            {
                if (startingData == null)
                    return data != null;

                bool startingDataIsArray = startingData is Array;
                bool dataIsArray = data is Array;

                if (startingDataIsArray != dataIsArray)
                    return true;

                if (!startingDataIsArray)
                    return !startingData.Equals(data);

                Array dataArray = data as Array;
                Array startingDataArray = startingData as Array;

                if (dataArray.Length != startingDataArray.Length)
                    return true;

                for (int i = 0; i < dataArray.Length; i++)
                {
                    object dataArrayItem = dataArray.GetValue(i);
                    object startingArrayItem = startingDataArray.GetValue(i);

                    if (startingArrayItem == null)
                    {
                        if (dataArrayItem == null)
                            continue;

                        return true;
                    }
                    else
                    {
                        if (startingArrayItem.Equals(dataArrayItem))
                            continue;

                        return true;
                    }
                }

                return false;
            }
        }

        public SheetCell(object data)
        {
            startingData = data;
            if (data is Array)
            {
                Array dataArray = data as Array;
                Array copyArray = Array.CreateInstance(dataArray.GetType().GetElementType(), dataArray.Length);
                Array.Copy(dataArray, copyArray, dataArray.Length);
                this.data = copyArray;
            }
            else
                this.data = data;
        }
    }
}