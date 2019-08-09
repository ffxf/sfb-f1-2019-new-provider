using SimFeedback.log;
using SimFeedback.telemetry;
using System;
using System.Reflection;

namespace F12019New
{
    class F12019NewTelemetryInfo : TelemetryInfo
    {
        private F12019NewTelemetryData telemetryData;
        private Session session;

        public F12019NewTelemetryInfo(F12019NewTelemetryData data, Session session)
        {
            telemetryData = data;
            this.session = session;
        }

        
        private F12019NewTelemetryValue TractionLoss()
        {
            LowpassFilter lp = (LowpassFilter)session.Get("TractionLossLowPass", new LowpassFilter());
            LowpassFilter lp2 = (LowpassFilter)session.Get("TractionLossLowPass2", new LowpassFilter());
            float turn = (float)lp.thirdOrder_lowpassFilter(telemetryData.angularAccelerationY, 0.3);
            int sign = 1;
            if (turn < 0)
                sign = -1;
            float data = sign * (float) Math.Log(Math.Abs(turn), 10) * Math.Abs(telemetryData.wheelSlipRR + telemetryData.wheelSlipRL);
            data = (float)lp2.thirdOrder_lowpassFilter(data, 0.3);
            return new F12019NewTelemetryValue("TractionLoss", data);
        }

        private F12019NewTelemetryValue BumpLeft()
        {
            LowpassFilter lp = (LowpassFilter)session.Get("BumpLeftLowPass", new LowpassFilter());
            float data = (float) Math.Log(Math.Abs(telemetryData.suspAccelFL));
            data = Math.Abs((float)lp.firstOrder_lowpassFilter(data, 0.3));
            return new F12019NewTelemetryValue("BumpLeft", -data);
        }

        private F12019NewTelemetryValue BumpRight()
        {
            LowpassFilter lp = (LowpassFilter)session.Get("BumpRightLowPass", new LowpassFilter());
            float data = (float)Math.Log(Math.Abs(telemetryData.suspAccelFR));
            data = Math.Abs((float)lp.firstOrder_lowpassFilter(data, 0.3));
            return new F12019NewTelemetryValue("BumpRight", data);
        }

        public TelemetryValue TelemetryValueByName(string name)
        {
            F12019NewTelemetryValue tv;

            switch (name)
            {
                case "TractionLoss":
                    tv = TractionLoss();
                    break;
                case "BumpRight":
                    tv = BumpRight();
                    break;
                case "BumpLeft":
                    tv = BumpLeft();
                    break;
                
                default:
                    object data;
                    Type eleDataType = typeof(F12019NewTelemetryData);
                    PropertyInfo propertyInfo;
                    FieldInfo fieldInfo = eleDataType.GetField(name);
                    if (fieldInfo != null)
                    {
                        data = fieldInfo.GetValue(telemetryData);
                    }
                    else if ((propertyInfo = eleDataType.GetProperty(name)) != null)
                    {
                        data = propertyInfo.GetValue(telemetryData, null);
                    }
                    else
                    {
                        throw new UnknownTelemetryValueException(name);
                    }
                    tv = new F12019NewTelemetryValue(name, data);
                    object value = tv.Value;
                    if (value == null)
                    {
                        throw new UnknownTelemetryValueException(name);
                    }
                    break;
            }

            return tv;
        }

    }
}