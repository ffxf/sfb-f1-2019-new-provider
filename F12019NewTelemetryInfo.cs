// Builds the telemetry data exposed in SimFeedback

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
            // The game has no traction loss telemetry key. So trying to derive a key from wheel slip rate of the
            // rears and the rotation acceleration. If both are high we likely are losing traction and are spinning
            // (or are starting to)
            // The value of rotation acceleration is quite erratic, unfortunately. Also it is not, as one might expect
            // measured in g (not sure what it is because the values go into the tens of thousands). So trying to
            // get a reasonable value by a lot of low pass filtering and by using log(10).
            LowpassFilter lp = (LowpassFilter)session.Get("TractionLossLowPass", new LowpassFilter());
            LowpassFilter lp2 = (LowpassFilter)session.Get("TractionLossLowPass2", new LowpassFilter());
            float turn = (float)lp.thirdOrder_lowpassFilter(telemetryData.angularAccelerationY, 0.3);
            // 'turn' gives us the rotation direction but we are going to pass it through abs and log and would loose the info
            // so storing it here
            int sign = 1;
            if (turn < 0)
                sign = -1;
            float data = sign * (float) Math.Log(Math.Abs(turn), 10) * Math.Abs(telemetryData.wheelSlipRR + telemetryData.wheelSlipRL);
            return new F12019NewTelemetryValue("TractionLoss", (float)lp2.thirdOrder_lowpassFilter(data, 0.3));
        }

        private F12019NewTelemetryValue BumpLeft()
        {
            // Attempting to derive a telemetry key for going over curbs, gravel or grass one sided.
            // The game gives use suspension acceleration per wheel. So using that here with some
            // low pass filtering because the value change quite erratically
            LowpassFilter lp = (LowpassFilter)session.Get("BumpLeftLowPass", new LowpassFilter());
            float data = (float) Math.Log(Math.Abs(telemetryData.suspAccelFL));
            data = Math.Abs((float)lp.firstOrder_lowpassFilter(data, 0.3));
            return new F12019NewTelemetryValue("BumpLeft", -data);
        }

        private F12019NewTelemetryValue BumpRight()
        {
            // Same as BumpLeft just using the other tyre (and needing a separate low pass filter)
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