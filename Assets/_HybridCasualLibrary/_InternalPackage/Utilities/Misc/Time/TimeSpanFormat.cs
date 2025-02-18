using System;

[Serializable]
public class TimeSpanFormat
{
    public string hourSuffix = "h ";
    public string minSuffix = "m ";
    public string secSuffix = "s ";

    public virtual string Convert(TimeSpan timeSpan)
    {
        if (timeSpan.Hours > 0)
        {
            if (timeSpan.Minutes > 0)
            {
                var stringBuilder = new System.Text.StringBuilder();
                stringBuilder.Append("{0:0}");
                stringBuilder.Append(hourSuffix);
                stringBuilder.Append("{1:0}");
                stringBuilder.Append(minSuffix);
                return string.Format(stringBuilder.ToString(), timeSpan.Hours, timeSpan.Minutes);
            }
            else
            {
                var stringBuilder = new System.Text.StringBuilder();
                stringBuilder.Append("{0:0}");
                stringBuilder.Append(hourSuffix);
                return string.Format(stringBuilder.ToString(), timeSpan.Hours);
            }
        }
        else
        {
            if (timeSpan.Minutes > 0)
            {
                if (timeSpan.Seconds > 0)
                {
                    var stringBuilder = new System.Text.StringBuilder();
                    stringBuilder.Append("{0:0}");
                    stringBuilder.Append(minSuffix);
                    stringBuilder.Append("{1:0}");
                    stringBuilder.Append(secSuffix);
                    return string.Format(stringBuilder.ToString(), timeSpan.Minutes, timeSpan.Seconds);
                }
                else
                {
                    var stringBuilder = new System.Text.StringBuilder();
                    stringBuilder.Append("{0:0}");
                    stringBuilder.Append(minSuffix);
                    return string.Format(stringBuilder.ToString(), timeSpan.Minutes);
                }
            }
            else
            {
                var stringBuilder = new System.Text.StringBuilder();
                stringBuilder.Append("{0:0}");
                stringBuilder.Append(secSuffix);
                return string.Format(stringBuilder.ToString(), timeSpan.Seconds);
            }
        }
    }
}
